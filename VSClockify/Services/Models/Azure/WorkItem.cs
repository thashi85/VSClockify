using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSClockify.Services.Models.Azure
{
    public class WorkItem: AzureWorkItem
    {
        
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Workitemtype { get; set; }
        public string Assignedto { get; set; }
        public string State { get; set; }
        public string Url { get; set; }
        public string Color { get; set; }

        public double Estimate { get; set; }
        public double Remaining { get; set; }
        public double Completed { get; set; }
    }

    public class AzureWorkItem
    {
        public string Id { get; set; }
    }
}
