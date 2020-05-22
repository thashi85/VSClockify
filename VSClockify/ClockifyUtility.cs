using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VSClockify.Services;
using VSClockify.Services.Models;

namespace VSClockify
{
    public class ClockifyUtility
    {
        private static readonly Lazy<ClockyifyService> lazyClockifyService = new Lazy<ClockyifyService>(() => new ClockyifyService());
        private static readonly Lazy<AzureAPIService> lazyAzureAPIService = new Lazy<AzureAPIService>(() => new AzureAPIService());
        private static ClockyifyService _clockifyService
        {
          get { return lazyClockifyService.Value; }
        }
        private static AzureAPIService _azureService
        {
            get { return lazyAzureAPIService.Value; }
        }
        public static User GetUser()
        {
            return _clockifyService.GetUser();
        }
        public static List<User> GetUsers(string workspace)
        {
            return _clockifyService.GetUsers(workspace);
        }
        public static string GetWorkspace(User user)
        {
            if (user != null)
                return string.IsNullOrEmpty(user.activeWorkspace) ? user.defaultWorkspace : user.activeWorkspace;
            return "";
        }

        public static List<Project> GetProjects(string workspaceId)
        {
            return _clockifyService.GetProjects(workspaceId);
        }
        public static DateTime GetWeekStart()
        {
            DateTime baseDate = DateTime.Today;
            var noOfDt = (int)baseDate.DayOfWeek;
            if (noOfDt == 0)
                noOfDt = 7;
            return baseDate.AddDays(-(noOfDt - 1));
           
        }
        public static DateTime GetWeekEnd()
        {
            DateTime baseDate = DateTime.Today;
            var noOfDt = (int)baseDate.DayOfWeek;
            if (noOfDt == 0)
                noOfDt = 7;
            
            return baseDate.AddDays(-(noOfDt - 1)).AddDays(6);
        }
        public static List<Services.Models.Azure.WorkItem> GetWorkItems(string txt,string name,string email)
        {
            return _azureService.GetWorkItems(txt, name, email);
        }
        public static bool PatchWorkItems(string taskId, List<Services.Models.Azure.WorkItemChange> changes)
        {
            return _azureService.PatchWorkItems(taskId, changes);
        }
        public static TimeEntryResponse StartClockify(string workspaceId, TimeEntryRequest req)
        {
            return _clockifyService.PostTimeEntry(workspaceId, req); 
        }
        public static TimeEntryResponse StopClockify(string workspaceId,string userId, StopTimeEntryRequest req)
        {
            return _clockifyService.StopTimeEntry(workspaceId, userId,req);
        }
        public static List<TimeEntryResult>  GetTimeEntries(string workspaceId,string userId,DateTime start,DateTime end)
        {
            var res = new List<TimeEntryResult>();
            var _timeEntries = _clockifyService.GetTimeEntries(workspaceId, userId, start, end.AddHours(23).AddMinutes(59));
            if (_timeEntries != null)
            {
                _timeEntries.ForEach(i =>
                {
                    i.taskId = "";
                    if (!string.IsNullOrEmpty(i.description) && i.timeInterval.duration != null)
                    {
                        var reg = @"#\d*";
                        var matches = Regex.Match(i.description, reg);
                        i.taskId = (matches != null && matches.Length > 0) ? matches.Value.Replace("#", "") : "";
                        var d = i.timeInterval.duration.Replace("PT", "");
                        var arr = d.Split('H');
                        var h = 0.00;
                        var m = 0.00;
                        var s = 0.00;
                        if (arr.Length > 0)
                        {
                            h = arr.Length > 1 ? Int32.Parse(arr[0]) : 0;
                            arr = arr.Length > 1 ? arr[1].Split('M') : arr[0].Split('M');
                            m = arr.Length > 1 ? Int32.Parse(arr[0]) : 0;
                            arr = arr.Length > 1 ? arr[1].Split('S') : arr[0].Split('S');
                            s = arr.Length > 1 ? Int32.Parse(arr[0]) : 0;

                        }
                        i.durationD = Math.Round((h + (m / 60.00) + (s / 3600.00)), 2);
                    }
                });
                var workItems = _azureService.GetWorkItemDetail(_timeEntries.Where(s => !string.IsNullOrEmpty(s.taskId)).Select(s => Int32.Parse(s.taskId)).Distinct().ToList());
                if (workItems != null)
                {
                   
                    _timeEntries.GroupBy(t => t.taskId).ToList().ForEach((s) =>
                    {
                        var _entries = _timeEntries.Where(t => t.taskId == s.Key).ToList();
                        var _completed = Math.Round(_entries.Select(t => t.durationD).Sum(), 2);
                        if (!string.IsNullOrEmpty(s.Key))
                        {
                            var _itm = workItems.Where(w => w.Id == s.Key).FirstOrDefault();
                            if (_itm != null)
                            {
                                res.Add(new TimeEntryResult()
                                {
                                    taskId = s.Key,
                                    description = _entries.Select(t => t.description).Max(),
                                    selected = !string.IsNullOrEmpty(s.Key) && _completed > 0 && _completed != _itm.Completed,
                                    color = _itm.Color,

                                    durationD = _completed,
                                    completed = _itm.Completed.ToString(),
                                    estimate = _itm.Estimate,
                                    state = _itm.State,
                                    azureItem = _itm,
                                    remaining = (_completed != _itm.Completed && _itm.Estimate > 0) ? Math.Round((_itm.Estimate - _completed), 2).ToString() : _itm.Remaining.ToString(),

                                });
                            }
                        }
                        else
                        {
                            _entries.Where(t => t.taskId == s.Key).ToList().ForEach(tsk =>
                            {
                                res.Add(new TimeEntryResult()
                                {
                                    taskId = tsk.taskId,
                                    description = tsk.description,
                                    // selected = !string.IsNullOrEmpty(s.Key) && _completed > 0 && _completed != _itm.Completed,
                                    // color = _itm.Color,

                                    durationD = Math.Round(tsk.durationD, 2),
                                    //completed = _itm.Completed.ToString(),
                                    //estimate = _itm.Estimate,
                                    //remaining = Math.Round((_itm.Estimate - _completed), 2).ToString(),

                                });
                            });

                        }
                    });
                    
                }
            }
            return res;
        }
    }
}
