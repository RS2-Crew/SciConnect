using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Application.Common;

public interface ICurrentUserService
{
    string? Username { get; }
    string? Email { get; }
    string? Role { get; }
}

