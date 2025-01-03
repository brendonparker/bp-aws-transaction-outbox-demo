namespace TransactionalOutboxPatternApp.Domain;

public abstract class AggregateRoot
{
    private readonly IList<IIntegrationEvent> _integrationEvents = [];

    protected void AddIntegrationEvent(IIntegrationEvent integrationEvent) =>
        _integrationEvents.Add(integrationEvent);

    public IReadOnlyList<IIntegrationEvent> GetIntegrationEvents() =>
        _integrationEvents.AsReadOnly();
}