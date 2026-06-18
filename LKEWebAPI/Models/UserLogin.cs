using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LKEWebAPI.Models
{
    public class UserLogin
    {

        public int ID { get; set; }
        public string WebUserName { get; set; }
        public string WebUserPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<bool> Inactive { get; set; }
        public string IsLocked { get; set; }
        public string LastLogin { get; set; }
        public string RetryCount { get; set; }
        public Nullable<System.DateTime> PasswordUpdatedOn { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public string Email { get; set; }
        public Nullable<long> Phone { get; set; }
        public string User_ID { get; set; }

    }
}