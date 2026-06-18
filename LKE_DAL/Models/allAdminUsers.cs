using Exportal_DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Models
{
    public class AllAdminUsers
    {
        public int Clients { get; set; }
        public string ClientName { get; set; }
        public int ID { get; set; }
        public string WebUserName { get; set; }
        public string WebUserPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? Inactive { get; set; }
        public string IsLocked { get; set; }
        public string LastLogin { get; set; }
        public string RetryCount { get; set; }
        //public Nullable<System.DateTime> PasswordUpdatedOn { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string User_ID { get; set; }

        //public AllAdminUsers(DataRow dr)
        //{
        //    if (dr.Table.Columns.Contains("ID"))
        //    {
        //        this.ID = DataTypesUtilities.IntNZ(dr["ID"]);
        //    }

        //}

    }
}
