using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Models
{
    public class _45DayIDDeadlineModel
    {
        public string AssetID { get; set; }
        public string AssetDescr { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalesPrice { get; set; }
        public string AssetCategory { get; set; }
        public DateTime Day45 { get; set; }
        public int DayRemaining { get; set; }
        public int ID { get; set; }
        public string MakeModel { get; set; }
        public int LEID { get; set; }
        public int CaseID { get; set; }
        public int CaseID2 { get; set; }
        public double PotentialGain { get; set; }
        public string IdentifyingNumber { get; set; }
        public int AssetType { get; set; }
    }

    public class _45DayIDRecentlySubmitted
    {
        public string AssetID { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime Day45 { get; set; }
        public int DayRemaining { get; set; }
        public int CaseID { get; set; }
        public int CaseID2 { get; set; }
        public int LEID { get; set; }
        public int ID { get; set; }
        public DateTime FortyFiveDate { get; set; }
    }

    public class SalePurchaseActivity
    {
        public string AssetCategory { get; set; }
        public int CaseID { get; set; }
        public int CaseID2 { get; set; }
        public double Sales { get; set; }
        public double Purchases { get; set; }
        public string IdentifyingNumber { get; set; }
    }
}
