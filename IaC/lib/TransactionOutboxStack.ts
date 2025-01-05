import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import * as ec2 from "aws-cdk-lib/aws-ec2";
import * as lambda from "aws-cdk-lib/aws-lambda";
import * as iam from "aws-cdk-lib/aws-iam";

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

    const lambdaFn = new lambda.Function(this, "Api", {
      functionName: "bp-tx-ob-api-lambda",
      code: lambda.Code.fromAsset("../publish/api"),
      handler: "TransactionalOutboxPatternApp",
      runtime: lambda.Runtime.DOTNET_8,
      architecture: lambda.Architecture.ARM_64,
      memorySize: 512,
      role,
      vpc,
      securityGroups: [sg],
    });

    const fnUrl = lambdaFn.addFunctionUrl({
      authType: lambda.FunctionUrlAuthType.NONE,
    });

    new cdk.CfnOutput(this, "TxOutboxFnUrl", { value: fnUrl.url });
  }
}
