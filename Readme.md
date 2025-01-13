# Demo: Transactional Outbox using Lambda to Process SQS Messages

## Motivation

1. Experiment with how to implement the transactional outbox pattern in a serverless way that doesn't require constant compute in order to poll for messages to process.
2. Build out some potentially reusable code around abstracting SQS interactions and routing messages to various registered handlers. I wanted to do this without the need for reflection.

This is more of a plumbing proof-of-concept. The sample domain is very weak.

## Build/Deploy

This is deployed to AWS using the AWS CDK.

### Prerequisites

No included as part of this stack is the VPC, RDS, and Security Groups needed. I've started to attempt to share that infrastructure in a separate stack/repo: https://github.com/brendonparker/bp-aws-rds-serverless

### Script

From within the IaC folder:
```
dotnet publish -c Release -o ../publish/api --runtime linux-arm64 ../BP.TransactionalOutboxDemo
cdk deploy
```