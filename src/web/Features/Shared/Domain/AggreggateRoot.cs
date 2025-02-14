namespace web.Features.Shared.domain;

public abstract class AggreggateRoot
{
        private readonly List<IDomainEvent> _domainEvents = [];
        public IEnumerable<IDomainEvent> DomainEvents => _domainEvents;

        public void RaiseDomainEvent(IDomainEvent domainEvent)
        {
                _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
                _domainEvents.Clear();
        }
}
