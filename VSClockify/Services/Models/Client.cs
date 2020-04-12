using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSClockify.Services.Models
{
    public class Client : ClocifyResource
    {
        public string workspaceId { get; set; }
    }
}
