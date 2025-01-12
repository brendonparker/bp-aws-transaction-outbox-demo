## Demo: Transaction Outbox using Lambda to Process SQS Messages

### Build/Deploy

From within the IaC folder:
```
dotnet publish -c Release -o ../publish/api --runtime linux-arm64 ../TransactionOutboxPatternApp
cdk deploy
```