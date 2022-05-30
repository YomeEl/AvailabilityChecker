using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvailabilityChecker.Email
{
    public record Sender(string Name, string Address, string Password, string Host, int Port);
}
