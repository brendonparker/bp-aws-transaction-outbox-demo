#!/usr/bin/env node
import * as cdk from "aws-cdk-lib";
import { TransactionOutboxStack } from "./lib/TransactionOutboxStack";

const app = new cdk.App();
new TransactionOutboxStack(app, "TransactionOutboxStack", {
  env: {
    account: process.env.CDK_DEFAULT_ACCOUNT,
    region: process.env.CDK_DEFAULT_REGION,
  },
});
