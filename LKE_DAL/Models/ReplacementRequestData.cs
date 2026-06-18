using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Models
{
    public class ReplacementRequestData
    {
        public string dbName;
        public int userID;
        public string username;
        public string signature;
        public string description;
        public List<MakeIDData> makeModelList;
    }
    public class MakeIDData
    {
        public int rowID;
        public int quantity;
        public float purchasePrice;
        public int makeModID;
    }

    public class RevokeIDReq
    {
        public string dbName;
        public int userID;
        public string username;
        public string signature;
        public string description;
        public List<int> revokeIDList;
    }
}
