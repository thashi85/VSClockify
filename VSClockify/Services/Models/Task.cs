using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSClockify.Services.Models
{
    public class Task:ClocifyResource
    {
        public List<string> assigneeIds { get; set; }
        public string projectId { get; set; }
    }
}
