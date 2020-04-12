using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSClockify.Services.Models;

namespace VSClockify.Services
{
    public class ClockyifyService
    {
        private KeyValuePair<string,string> AuthHeader
        {
            get
            {
                return new KeyValuePair<string, string>("X-Api-Key", ServiceUtility.ClockifyApiKey);
            }
        }
        public User GetUser()
        {
            var status = 0;
            var result = ServiceUtility.WebAPIRequest(out status, ServiceUtility.ClockifyApiUrl, Enums.WebMethod.GET, "/user", null,null,headers:new List<KeyValuePair<string, string>>() { AuthHeader });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<User>(result);

        }
        public List<Project> GetProjects(string workspaceId)
        {
            var status = 0;
            var result = ServiceUtility.WebAPIRequest(out status, ServiceUtility.ClockifyApiUrl, Enums.WebMethod.GET, "/workspaces/"+workspaceId+"/projects", null, null, headers: new List<KeyValuePair<string, string>>() { AuthHeader });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Project>>(result);

        }
        public TimeEntryResponse PostTimeEntry(string workspaceId, TimeEntryRequest data)
        {
            var status = 0;
            var result = ServiceUtility.WebAPIRequest(out status, ServiceUtility.ClockifyApiUrl, Enums.WebMethod.POST, "/workspaces/" + workspaceId + "/time-entries", null, data, headers: new List<KeyValuePair<string, string>>() { AuthHeader });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TimeEntryResponse>(result);

        }
        public TimeEntryResponse StopTimeEntry(string workspaceId,string userId,StopTimeEntryRequest data)
        {
            var status = 0;
            var result = ServiceUtility.WebAPIRequest(out status, ServiceUtility.ClockifyApiUrl, Enums.WebMethod.PATCH, "/workspaces/" + workspaceId + "/user/"+userId+"/time-entries", null, data, headers: new List<KeyValuePair<string, string>>() { AuthHeader });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TimeEntryResponse>(result);

        }
    }
}
