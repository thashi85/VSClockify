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

            var result = ServiceUtility.WebAPIRequest(out status, ServiceUtility.AzureSearchAPIEndPoint, Enums.WebMethod.POST, "",  null, Newtonsoft.Json.JsonConvert.DeserializeObject(postData), headers: new List<KeyValuePair<string, string>>() { AuthHeader });
            dynamic objs = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(result);
            if (objs!=null && objs["count"] >0  && objs["results"]!=null) 
            {
                return ((Newtonsoft.Json.Linq.JArray)objs["results"]).Select(d => (dynamic)d).ToList().Where(d => d["fields"]["system.workitemtype"].ToString()=="Task" || d["fields"]["system.workitemtype"].ToString() == "Bug").Select(s => new WorkItem()
                    {
                        Id = s["fields"]["system.id"].ToString(),
                        Url = s["url"].ToString(),
                        Workitemtype= s["fields"]["system.workitemtype"].ToString(),
                        Color = s["fields"]["system.workitemtype"].ToString()=="Task" ? "Green":"Purple",
                        Desc ="#" + s["fields"]["system.id"].ToString() + " :" + s["fields"]["system.title"].ToString(),
                        Title = "#" + s["fields"]["system.id"].ToString()+" :"+s["fields"]["system.title"].ToString()
                    
                }).ToList();
            }
            return null;
        }
        public bool PatchWorkItems(string workItemId,List<WorkItemChange> changes)
        {            
            var status = 0;            
            var result = ServiceUtility.WebAPIRequest(out status, ServiceUtility.AzureAPIEndPoint, Enums.WebMethod.PATCH, 
                                                    "wit/workitems/"+ workItemId +"?api-version=5.1", null, changes, 
                                                    headers: new List<KeyValuePair<string, string>>() { AuthHeader,
                                                                     new KeyValuePair<string, string>("Content-Type","application/json-patch+json") });
            var objs = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureWorkItem>(result);
            if (objs != null && !string.IsNullOrEmpty(objs.Id))
            {
                return true;
            }
            return false;
        }

        public List<WorkItem> GetWorkItemDetail(List<int> workItems)
        {
            var status = 0;
            string postData = File.ReadAllText(ServiceUtility.AppFolderPath + "//Resources//azure//workItemDetails.txt");
            postData = postData.Replace("#WorkItems#", string.Join(",",workItems));
        
            var result = ServiceUtility.WebAPIRequest(out status, ServiceUtility.AzureAPIEndPoint, Enums.WebMethod.POST, "/wit/workitemsbatch?api-version=5.1", null, Newtonsoft.Json.JsonConvert.DeserializeObject(postData), headers: new List<KeyValuePair<string, string>>() { AuthHeader });
            dynamic objs = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(result);
            if (objs != null && objs["count"] > 0 && objs["value"] != null)
            {
                //.Where(d => d["fields"]["system.workitemtype"].ToString() == "Task" || d["fields"]["system.workitemtype"].ToString() == "Bug")
                return ((Newtonsoft.Json.Linq.JArray)objs["value"]).Select(d => (dynamic)d).ToList().Select(s => new WorkItem()
                {
                    Id = s["fields"]["System.Id"].ToString(),
                   // Url = s["url"].ToString(),
                    Workitemtype = s["fields"]["System.WorkItemType"].ToString(),
                    Color = s["fields"]["System.WorkItemType"].ToString() == "Task" ? "Green" : "Purple",
                    Desc = "#" + s["fields"]["System.Id"].ToString() + " :" + s["fields"]["System.Title"].ToString(),
                    Title = "#" + s["fields"]["System.Id"].ToString() + " :" + s["fields"]["System.Title"].ToString(),
                    Estimate = Convert.ToDouble((s["fields"]["Microsoft.VSTS.Scheduling.OriginalEstimate"]?? "0").ToString()),
                    Remaining = Convert.ToDouble((s["fields"]["Microsoft.VSTS.Scheduling.RemainingWork"]?? "0").ToString()),
                    Completed = Convert.ToDouble((s["fields"]["Microsoft.VSTS.Scheduling.CompletedWork"]?? "0").ToString())

                }).ToList();
            }
            return null;
        }
    }
}
