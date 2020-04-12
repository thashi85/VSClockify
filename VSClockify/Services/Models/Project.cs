using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSClockify.Services.Models
{
    public class Project:ClocifyResource
    {        
        public bool archived { get; set; }
        public string clientId { get; set; }
        public string color { get; set; }        
        public string workspaceId { get; set; }
        public Client client { get; set; }
        public List<Task> tasks { get; set; }
    }

}
