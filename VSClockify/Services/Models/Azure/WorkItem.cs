using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSClockify.Services.Models.Azure
{
    public class WorkItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Workitemtype { get; set; }
        public string Assignedto { get; set; }
        public string State { get; set; }
        public string Url { get; set; }
        public string Color { get; set; }
    }
    
}
