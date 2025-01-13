namespace BP.TransactionalOutboxDemo.Domain;

public abstract class AggregateRoot
{
    public required long Id { get; set; } = 0;

    private readonly IList<IIntegrationEvent> _integrationEvents = [];

    protected void AddIntegrationEvent(IIntegrationEvent integrationEvent) =>
        _integrationEvents.Add(integrationEvent);

    public IReadOnlyList<IIntegrationEvent> GetIntegrationEvents() =>
        _integrationEvents.AsReadOnly();
}