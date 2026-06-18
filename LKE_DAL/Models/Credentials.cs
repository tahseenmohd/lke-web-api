using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Models
{
   public class Credentials 
    {
      public  string username { get; set; }

        public string oldPassword { get; set; }

        public string newPassword { get; set; }
    }
}
