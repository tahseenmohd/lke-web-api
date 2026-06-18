using Exportal_DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LKE_DAL.Models
{
    public class PendingIdentifications
    {
        public string AssetID { get; set; }
        public string AssetDescr { get; set; }
        public DateTime? SaleDate { get; set; }
        public decimal SalesPrice { get; set; }
        public string AssetCategoryMain { get; set; }
        public DateTime? Day45 { get; set; }
        public int DayRemaining { get; set; }
        public int ID { get; set; }
        public string MakeModel { get; set; }
        public int LEID { get; set; }
        public int CaseID { get; set; }
        public int CaseID2 { get; set; }
        public double PotentialGain { get; set; }
        public DateTime PISDate { get; set; }

        public string AssetIdentified {get;set;}

        public decimal FMV { get; set; }

        public int Quantity { get; set; }

        public int IDLink { get; set; }
        public string  AssetCategory { get; set; }

        public Double PurPriceEst { get; set; }
        //public PendingIdentifications() { }

        //public PendingIdentifications(DataRow dr)
        //{

        //    if (dr.Table.Columns.Contains("AssetID"))
        //    {
        //        this.AssetID = DataTypesUtilities.StringNZ(dr["AssetID"]);
        //    }
        //    if (dr.Table.Columns.Contains("AssetCategory"))
        //    {
        //        this.AssetCategory = DataTypesUtilities.StringNZ(dr["AssetCategory"]);
        //    }
        //    if (dr.Table.Columns.Contains("AssetDescr"))
        //    {
        //        this.AssetDescr = DataTypesUtilities.StringNZ(dr["AssetDescr"]);
        //    }
        
        //    if (dr.Table.Columns.Contains("SalesPrice"))
        //    {
        //        this.SalesPrice = DataTypesUtilities.DecimalNZ(dr["LiabilityRelieved"], 0);
        //    }

        //    if (dr.Table.Columns.Contains("CaseID"))
        //    {
        //        this.CaseID = DataTypesUtilities.IntNZ(dr["CaseID"], 0);
        //    }
        //    if (dr.Table.Columns.Contains("DayRemaining"))
        //    {
        //        this.DayRemaining = DataTypesUtilities.IntNZ(dr["DayRemaining"], 0);
        //    }
        //    if (dr.Table.Columns.Contains("CaseID2"))
        //    {
        //        this.CaseID2 = DataTypesUtilities.IntNZ(dr["CaseID2"], 0);
        //    }

        //    if (dr.Table.Columns.Contains("LEID"))
        //    {
        //        this.LEID = DataTypesUtilities.IntNZ(dr["LEID"], 0);
        //    }
        //    //if (dr.Table.Columns.Contains("ID"))
        //    //{
        //    //    this.ID = DataTypesUtilities.IntNZ(dr["ID"], 0);
        //    //}
        //    //if (dr.Table.Columns.Contains("ID"))
        //    //{
        //    //    this.ID = DataTypesUtilities.IntNZ(dr["ID"], 0);
        //    //}
        //    if (dr.Table.Columns.Contains("SaleDate"))
        //    {
        //        if (!dr.IsNull("SaleDate"))
        //        {
        //            DateTime SaleDate = (DateTime)Convert.ToDateTime(dr["SaleDate"]);
        //            this.SaleDate = SaleDate;
        //        }
        //        else
        //        {
        //            this.SaleDate = null;
        //        }
        //    }

        //    if (dr.Table.Columns.Contains("Day45"))
        //    {
        //        if (!dr.IsNull("Day45"))
        //        {
        //            DateTime Day45 = (DateTime)Convert.ToDateTime(dr["Day45"]);
        //            this.Day45 = Day45;

        //        }
        //        else
        //        {
        //            this.Day45 = null;
        //        }
        //    }
           
        //    if (dr.Table.Columns.Contains("PISDate"))
        //    {
        //        if (!dr.IsNull("PISDate"))
        //        {
        //            DateTime PISDate = (DateTime)Convert.ToDateTime(dr["PISDate"]);
        //            this.PISDate = PISDate;
        //        }
        //        else
        //        {
        //            this.SaleDate = null;
        //        }
        //    }

           

        //    if (dr.Table.Columns.Contains("AssetDescr"))
        //    {
        //        this.AssetDescr = DataTypesUtilities.StringNZ(dr["AssetDescr"]);
        //    }
        //    if (dr.Table.Columns.Contains("AssetCategory"))
        //    {
        //        this.AssetCategory = DataTypesUtilities.StringNZ(dr["AssetCategory"]);
        //    }

        //    if (dr.Table.Columns.Contains("AssetIdentified"))
        //    {
        //        this.AssetIdentified = DataTypesUtilities.StringNZ(dr["AssetIdentified"]);
        //    }

        //    if (dr.Table.Columns.Contains("Quantity"))
        //    {
        //        this.Quantity = DataTypesUtilities.IntNZ(dr["Quantity"], 0);
        //    }

        //    if (dr.Table.Columns.Contains("FMV"))
        //    {
        //        this.FMV = DataTypesUtilities.DecimalNZ(dr["FMV"], 0);
        //    }


        //    if (dr.Table.Columns.Contains("ID"))
        //    {
        //        this.ID = DataTypesUtilities.IntNZ(dr["ID"], 0);
        //    }

        //    if (dr.Table.Columns.Contains("CaseID"))
        //    {
        //        this.CaseID = DataTypesUtilities.IntNZ(dr["CaseID"], 0);
        //    }

        //    if (dr.Table.Columns.Contains("CaseID2"))
        //    {
        //        this.CaseID2 = DataTypesUtilities.IntNZ(dr["CaseID2"], 0);
        //    }

        //    if (dr.Table.Columns.Contains("LEID"))
        //    {
        //        this.LEID = DataTypesUtilities.IntNZ(dr["LEID"], 0);
        //    }
        //    if (dr.Table.Columns.Contains("IDLink"))
        //    {
        //        this.IDLink = DataTypesUtilities.IntNZ(dr["IDLink"],0);
        //    }

        //    if (dr.Table.Columns.Contains("AssetCategoryMain"))
        //    {
        //        this.AssetCategoryMain = DataTypesUtilities.StringNZ(dr["AssetCategoryMain"]);
        //    }
           
        //    if (dr.Table.Columns.Contains("PurPriceEst"))
        //    {
        //        this.PurPriceEst = DataTypesUtilities.FloatNZ(dr["PurPriceEst"], 0);
        //    }
        //    if (dr.Table.Columns.Contains("PotentialGain"))
        //    {
        //        this.PotentialGain = DataTypesUtilities.FloatNZ(dr["PotentialGain"], 0);
        //    }

        //}
    }

    public class MainData
    {
        public string AssetID { get; set; }
        public DateTime? SaleDate { get; set; }
        public decimal SalesPrice { get; set; }
        public string AssetCategory { get; set; }
        public DateTime? Day45 { get; set; }
        public int DayRemaining { get; set; }
        public int ID { get; set; }

        public int LEID { get; set; }
        public int CaseID { get; set; }
        public int CaseID2 { get; set; }
        public double PotentialGain { get; set; }
        public DateTime PISDate { get; set; }

        public List<SubData> subdata { get; set; }


    }
    public class SubData
    {

        public string AssetIdentified { get; set; }

        public decimal FMV { get; set; }

        public int Quantity { get; set; }

        public int IDLink { get; set; }
        public string AssetCategory { get; set; }

        public Double PurPriceEst { get; set; }

    }
}