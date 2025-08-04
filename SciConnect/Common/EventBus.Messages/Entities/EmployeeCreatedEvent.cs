using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Entities
{
    public class EmployeeCreatedEvent
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string InstitutionName { get; set; } = default!;
    }
}
