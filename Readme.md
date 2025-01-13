# Demo: Transactional Outbox using Lambda to Process SQS Messages

## Motivation

1. Experiment with how to implement the transactional outbox pattern in a serverless way that doesn't require constant compute in order to poll for messages to process.
2. Build out some potentially reusable code around abstracting SQS interactions and routing messages to various registered handlers. I wanted to do this without the need for reflection.

This is more of a plumbing proof-of-concept. The sample domain is very weak.

## Overview

The transactional outbox pattern is used when you want to update a database and send messages to a message broker, but want to avoid the scenario where something goes wrong between updating the database and sending the events, causing only one or the other to happen.

This is done by using a database transaction to both update the database and insert a message into a table/queue. Ensuring a single atomic operation.

Traditionally, there is then another process polling this table looking for messages and dispatching them accordingly.

In order to run this in a serverless environment, where we don't have constant compute running to poll for messages, I've adapted the pattern a bit to do the single atomic transaction with both the updates and the message insertion, but then subsequently queue a message to indicate that there are new messages in the queue, effectively "priming the pump" of SQS.
Yes, there is a chance this message fails, but _eventually_ all the messages should be picked up and should be recoverable.

I'm relying on a single FIFO queue with message grouping to ensure that there isn't concurrency when dealing with processing the messages off of the queue.

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