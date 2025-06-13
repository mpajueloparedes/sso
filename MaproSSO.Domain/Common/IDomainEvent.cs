using MediatR;

namespace MaproSSO.Domain.Common
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}