using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSClockify.Services.Models;
using VSClockify.Services.Models.Azure;

namespace VSClockify.Services
{
    public class AzureAPIService
    {
        private KeyValuePair<string, string> AuthHeader
        {
            get
            {
                return new KeyValuePair<string, string>("Authorization", "Basic "+Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", ServiceUtility.AzurePAT, ServiceUtility.AzurePAT))));
            }
        }
        public List<WorkItem> GetWorkItems(string txt,string username,string email)
        {
            var status = 0;
            string postData = File.ReadAllText(ServiceUtility.AppFolderPath+ "//Resources//azure//workItemSearch.txt");
            postData=postData.Replace("#Text#", txt);
            postData = postData.Replace("#User#", username+" <"+email+">");

            var result = ServiceUtility.WebAPIRequest(out status, ServiceUtility.AzureAPIEndPoint, Enums.WebMethod.POST, "",  null, Newtonsoft.Json.JsonConvert.DeserializeObject(postData), headers: new List<KeyValuePair<string, string>>() { AuthHeader });
            dynamic objs = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(result);
            if (objs!=null && objs["count"] >0  && objs["results"]!=null) 
            {
                return ((Newtonsoft.Json.Linq.JArray)objs["results"]).Select(d => (dynamic)d).ToList().Select(s => new WorkItem()
                    {
                        Id = s["fields"]["system.id"].ToString(),
                        Url = s["url"].ToString(),
                        Desc = s["fields"]["system.workitemtype"].ToString() + s["fields"]["system.id"].ToString() + ":" + s["fields"]["system.title"].ToString(),
                        Title = "#" + s["fields"]["system.id"].ToString()+":"+s["fields"]["system.title"].ToString()
                    
                }).ToList();
            }
            return null;
        }
    }
}
