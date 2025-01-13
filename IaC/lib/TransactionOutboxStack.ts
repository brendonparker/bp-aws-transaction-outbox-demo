import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import * as ec2 from "aws-cdk-lib/aws-ec2";
import * as lambda from "aws-cdk-lib/aws-lambda";
import * as iam from "aws-cdk-lib/aws-iam";
import * as sqs from "aws-cdk-lib/aws-sqs";
import * as eventsource from "aws-cdk-lib/aws-lambda-event-sources";

export class TransactionOutboxStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    const vpc = ec2.Vpc.fromLookup(this, "Vpc", {
      vpcName: "bp-vpc",
    });

    const sg = ec2.SecurityGroup.fromLookupByName(
      this,
      "LambdaSecurityGroup",
      "bp-app-sg",
      vpc
    );

    const dlq = new sqs.Queue(this, "DLQ", {
      queueName: "bp-tx-ob-dlq.fifo",
      contentBasedDeduplication: true,
    });

    const queue = new sqs.Queue(this, "Queue", {
      queueName: "bp-tx-ob.fifo",
      deadLetterQueue: {
        queue: dlq,
        maxReceiveCount: 1,
      },
      contentBasedDeduplication: true,
    });

    const role = new iam.Role(this, "LambdaRole", {
      roleName: "bp-tx-ob-lambda-role",
      assumedBy: new iam.ServicePrincipal("lambda.amazonaws.com"),
      managedPolicies: [
        iam.ManagedPolicy.fromAwsManagedPolicyName(
          "service-role/AWSLambdaVPCAccessExecutionRole"
        ),
        iam.ManagedPolicy.fromAwsManagedPolicyName(
          "service-role/AWSLambdaBasicExecutionRole"
        ),
      ],
    });

    queue.grantSendMessages(role);
    queue.grantConsumeMessages(role);

    role.addToPolicy(
      new iam.PolicyStatement({
        resources: [
          `arn:aws:ssm:${this.region}:${this.account}:parameter/aws/reference/secretsmanager/bp-db-secret`,
        ],
        actions: ["ssm:GetParameter"],
      })
    );
    role.addToPolicy(
      new iam.PolicyStatement({
        resources: [
          `arn:aws:secretsmanager:${this.region}:${this.account}:secret:bp-db-secret-*`,
        ],
        actions: ["secretsmanager:GetSecretValue"],
      })
    );

    const lambdaDefaults = {
      code: lambda.Code.fromAsset("../publish/api"),
      runtime: lambda.Runtime.DOTNET_8,
      architecture: lambda.Architecture.ARM_64,
      memorySize: 512,
      role,
      vpc,
      securityGroups: [sg],
      environment: {
        AWS_ACCOUNT_ID: this.account,
      },
      timeout: cdk.Duration.seconds(15),
    };

    const lambdaFn = new lambda.Function(this, "Api", {
      ...lambdaDefaults,
      functionName: "bp-tx-ob-api-lambda",
      handler: "BP.TransactionalOutboxDemo",
    });

    const fnUrl = lambdaFn.addFunctionUrl({
      authType: lambda.FunctionUrlAuthType.NONE,
    });

    const lambdaQueueProcessor = new lambda.Function(this, "QueueProcessor", {
      ...lambdaDefaults,
      functionName: "bp-tx-ob-queue-lambda",
      handler:
        "BP.TransactionalOutboxDemo::BP.TransactionalOutboxDemo.QueueLambda::Handler",
    });

    lambdaQueueProcessor.addEventSource(
      new eventsource.SqsEventSource(queue, {
        batchSize: 1,
      })
    );

    new cdk.CfnOutput(this, "TxOutboxFnUrl", { value: fnUrl.url });
  }
}
