namespace BP.TransactionalOutboxDemo.Domain;

public interface IIntegrationEvent;

public interface IMessageGroupId
{
    public string MessageGroupId { get; }
}