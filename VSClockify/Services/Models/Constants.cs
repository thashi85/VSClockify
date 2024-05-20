using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSClockify.Services.Models
{
    public class Constants
    {
        public static class ActionType
        {
            public const string ADD = "add";
            public const string REPLACE = "replace";
        }
        public static class Attributes
        {
           // public const string CompletedHours = "/fields/Microsoft.VSTS.Scheduling.CompletedWork";
            public const string CompletedHours = "/fields/Custom.ActualEffort";
            public const string RemainingHours = "/fields/Microsoft.VSTS.Scheduling.RemainingWork";
            public const string State = "/fields/System.State";
        }
    }
}
