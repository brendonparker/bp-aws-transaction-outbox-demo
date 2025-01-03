namespace TransactionalOutboxPatternApp.Domain;

public interface IIntegrationEvent
{
    string ToJson();
}