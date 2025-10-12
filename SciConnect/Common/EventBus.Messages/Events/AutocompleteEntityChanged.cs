using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events;

public enum EntityType
{
    Institution,
    Instrument,
    Analysis,
    Microorganism,
    Keyword,
    Employee
}

public enum ChangeKind
{
    Created,
    Updated,
    Deleted
}

public abstract record class IntegrationEvent
{
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string? CorrelationId { get; init; }
}

public sealed record class AutocompleteEntityChanged : IntegrationEvent
{
    public EntityType Type { get; init; }
    public ChangeKind Kind { get; init; }
    public int Id { get; init; }
    public string? Name { get; init; } // may be null for Deleted
}
