using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Entities
{
    public class SimpleEntityCreatedEvent
    {
        public string EntityType { get; set; } = default!; // e.g., "Keyword", "Instrument", etc.
        public string Name { get; set; } = default!;
    }
}
