using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Models
{
    public class WebUserModel
    {
        public List<WebDbaseAssignment> Clients { get; set; }

        public List<WebDbaseAssignment> Delclients { get; set; }
        public WebUser WebUser { get; set; }
    }
}
