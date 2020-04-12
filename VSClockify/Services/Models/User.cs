using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSClockify.Services.Models
{
    public class User
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string defaultWorkspace { get; set; }
        public string activeWorkspace { get; set; }
        public string profilePicture { get; set; }
    }
}
