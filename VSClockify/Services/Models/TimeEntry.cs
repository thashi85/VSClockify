using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSClockify.Services.Models.Azure;

namespace VSClockify.Services.Models
{
    public class TimeEntry
    {       
        public bool billable { get; set; }
        public string description { get; set; }
        public string projectId { get; set; }
        public string taskId { get; set; }
        public List<string> tagIds { get; set; }
    }
    public class TimeEntryRequest : TimeEntry
    {
        public DateTime start { get; set; }
        public DateTime? end { get; set; }
    }

    public class TimeEntryResponse : TimeEntry
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string workspaceId { get; set; }
        public TimeInterval timeInterval { get; set; }

        public double durationD { get; set; }

    }
    public class TimeEntryResult : TimeEntryResponse
    {
        public string color { get; set; }
        public bool selected { get; set; }
        public double estimate { get; set; }
        public string remaining { get; set; }
        public string completed { get; set; }
        public string state { get; set; }

        public WorkItem azureItem { get; set; }

    }


    public class TimeInterval
    {
        public DateTime start { get; set; }
        public DateTime? end { get; set; }
        public string duration { get; set; } // (Example: PT1H30M15S - 1 hour 30 minutes 15 seconds)
        
    }
    public class StopTimeEntryRequest
    {
        public DateTime end { get; set; }
    }
}
