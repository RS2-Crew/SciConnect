using Autocomplete.API.Domain.Models;
using Autocomplete.API.Infrastructure.Persistence;
using EventBus.Messages.Events;
using MassTransit;

namespace Autocomplete.API.Consumer;

public class EntityChangedConsumer : IConsumer<AutocompleteEntityChanged>
{
    private readonly AutoDbContext _db;
    public EntityChangedConsumer(AutoDbContext db) => _db = db;

    public async Task Consume(ConsumeContext<AutocompleteEntityChanged> c)
    {
        var type = c.Message.Type.ToString().ToLowerInvariant();

        if (c.Message.Kind is ChangeKind.Created or ChangeKind.Updated)
        {
            var name = (c.Message.Name ?? "").Trim();
            var norm = name.ToLowerInvariant();

            var row = await _db.Entries.FindAsync(c.Message.Id);
            if (row is null)
            {
                _db.Entries.Add(new AutocompleteEntry
                {
                    Id = c.Message.Id,
                    Type = type,
                    Name = name,
                    NameNorm = norm,
                    UpdatedAt = c.Message.OccurredAt
                });
            }
            else
            {
                row.Type = type; row.Name = name; row.NameNorm = norm; row.UpdatedAt = c.Message.OccurredAt;
            }
        }
        else if (c.Message.Kind == ChangeKind.Deleted)
        {
            var row = await _db.Entries.FindAsync(c.Message.Id);
            if (row is not null) _db.Entries.Remove(row);
        }

        await _db.SaveChangesAsync();
    }
}
