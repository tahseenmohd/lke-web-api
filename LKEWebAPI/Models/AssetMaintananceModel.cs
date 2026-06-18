using Exportal_DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Models
{
    public class AssetMaintananceModel
    {

        public string AssetID { get; set; }

        public string Dep_Type { get; set; }
        public string ParentAssetID { get; set; }
        public bool LKEEligible { get; set; }
        public bool LKEEligible_Purchase { get; set; }
        public float LiabilityRelieved { get; set; }
        public float LiabilityAssumed { get; set; }
        public bool RelatedPartySale { get; set; }
        public bool RelatedPartyPurchase { get; set; }
        public float ActConsReceipt { get; set; }
        public DateTime? AcqDate { get; set; }
        public DateTime? ParentAcqDate { get; set; }
        public float AcqCost { get; set; }
        public DateTime? SaleDate { get; set; }
        public decimal SalesPrice { get; set; } // money
        public string AssetDescr { get; set; }  //varchar(255)
        public string AssetCategory { get; set; } //varchar(100)
        public string MakeModel { get; set; } //varchar(303)

        public int AssetMakeModelID { get; set; }

        public float BootReceived { get; set; }

        public int ID { get; set; }

        public int CaseID { get; set; }

        public int CaseID2 { get; set; }

        public int LEID { get; set; }

       //varchar(20)

        public int CoreDataID { get; set; }

        public int MakeModelsID { get; set; }


        public AssetMaintananceModel() { }

        public AssetMaintananceModel(DataRow dr)
        {

            if (dr.Table.Columns.Contains("AssetID"))
            {
                this.AssetID = DataTypesUtilities.StringNZ(dr["AssetID"]);
            }
            if (dr.Table.Columns.Contains("ParentAssetID"))
            {
                this.ParentAssetID = DataTypesUtilities.StringNZ(dr["ParentAssetID"]);
            }
            if (dr.Table.Columns.Contains("LKEEligible"))
            {
                this.LKEEligible = DataTypesUtilities.BoolNZ(dr["LKEEligible"], false);
            }
            if (dr.Table.Columns.Contains("LKEEligible_Purchase"))
            {
                this.LKEEligible_Purchase = DataTypesUtilities.BoolNZ(dr["LKEEligible_Purchase"], false);
            }

            if (dr.Table.Columns.Contains("LiabilityRelieved"))
            {
                this.LiabilityRelieved = DataTypesUtilities.FloatNZ(dr["LiabilityRelieved"], 0);
            }

            if (dr.Table.Columns.Contains("LiabilityAssumed"))
            {
                this.LiabilityAssumed = DataTypesUtilities.FloatNZ(dr["LiabilityAssumed"], 0);
            }
            if (dr.Table.Columns.Contains("RelatedPartySale"))
            {
                this.RelatedPartySale = DataTypesUtilities.BoolNZ(dr["RelatedPartySale"], false);
            }
            if (dr.Table.Columns.Contains("RelatedPartyPurchase"))
            {
                this.RelatedPartyPurchase = DataTypesUtilities.BoolNZ(dr["RelatedPartyPurchase"], false);
            }

            if (dr.Table.Columns.Contains("ActConsReceipt"))
            {
                this.ActConsReceipt = DataTypesUtilities.FloatNZ(dr["ActConsReceipt"], 0);
            }


            if (dr.Table.Columns.Contains("AcqDate"))
            {
                if (!dr.IsNull("AcqDate"))
                {
                    DateTime dateTimeAcqDate = (DateTime)Convert.ToDateTime(dr["AcqDate"]);
                    this.AcqDate = dateTimeAcqDate;
                }
                else
                {
                    this.AcqDate = null;
                }
            }

            if (dr.Table.Columns.Contains("ParentAcqDate"))
            {
                if (!dr.IsNull("ParentAcqDate"))
                {
                    DateTime dateTimeParentAcqDate = (DateTime)Convert.ToDateTime(dr["ParentAcqDate"]);
                    this.ParentAcqDate = dateTimeParentAcqDate;

                }
                else
                {
                    this.ParentAcqDate = null;
                }
            }
            if (dr.Table.Columns.Contains("AcqCost"))
            {
                this.AcqCost = DataTypesUtilities.FloatNZ(dr["AcqCost"], 0);
            }
            if (dr.Table.Columns.Contains("SaleDate"))
            {
                if (!dr.IsNull("SaleDate"))
                {
                    DateTime dateTimeSaleDate = (DateTime)Convert.ToDateTime(dr["SaleDate"]);
                    this.SaleDate = dateTimeSaleDate;
                }
                else
                {
                    this.SaleDate = null;
                }
            }

            if (dr.Table.Columns.Contains("AssetDescr"))
            {
                this.AssetDescr = DataTypesUtilities.StringNZ(dr["AssetDescr"]);
            }
            if (dr.Table.Columns.Contains("AssetCategory"))
            {
                this.AssetCategory = DataTypesUtilities.StringNZ(dr["AssetCategory"]);
            }

            if (dr.Table.Columns.Contains("MakeModel"))
            {
                this.MakeModel = DataTypesUtilities.StringNZ(dr["MakeModel"]);
            }

            if (dr.Table.Columns.Contains("AssetMakeModelID"))
            {
                this.AssetMakeModelID = DataTypesUtilities.IntNZ(dr["AssetMakeModelID"], 0);
            }

            if (dr.Table.Columns.Contains("BootReceived"))
            {
                this.BootReceived = DataTypesUtilities.FloatNZ(dr["BootReceived"], 0);
            }


            if (dr.Table.Columns.Contains("ID"))
            {
                this.ID = DataTypesUtilities.IntNZ(dr["ID"], 0);
            }

            if (dr.Table.Columns.Contains("CaseID"))
            {
                this.CaseID = DataTypesUtilities.IntNZ(dr["CaseID"], 0);
            }

            if (dr.Table.Columns.Contains("CaseID2"))
            {
                this.CaseID2 = DataTypesUtilities.IntNZ(dr["CaseID2"], 0);
            }

            if (dr.Table.Columns.Contains("LEID"))
            {
                this.LEID = DataTypesUtilities.IntNZ(dr["LEID"], 0);
            }
            if (dr.Table.Columns.Contains("Dep_Type"))
            {
                this.Dep_Type = DataTypesUtilities.StringNZ(dr["Dep_Type"]);
            }

            if (dr.Table.Columns.Contains("CoreDataID"))
            {
                this.CoreDataID = DataTypesUtilities.IntNZ(dr["CoreDataID"], 0);
            }
            if (dr.Table.Columns.Contains("MakeModelsID"))
            {
                this.MakeModelsID = DataTypesUtilities.IntNZ(dr["MakeModelsID"], 0);
            }
            if (dr.Table.Columns.Contains("SalesPrice"))
            {
                this.SalesPrice = DataTypesUtilities.DecimalNZ(dr["SalesPrice"], 0);
            }

            //if (dr.Table.Columns.Contains("Dep_Type"))
            //{
            //    this.Dep_Type = DataTypesUtilities.StringNZ(dr["Dep_Type"]);
            //}
        }


    }


    public class AssetMaintananceModelPost
    {
        public string dbname { get; set; }
        public string AssetID { get; set; }

        public bool LKEEligible { get; set; }
        public bool LKEEligible_Purchase { get; set; }
        public float LiabilityRelieved{ get; set; }
        public float LiabilityAssumed { get; set; }
        public float BootReceived { get; set; }
        public bool RelatedPartySale { get; set; }
        public bool RelatedPartyPurchase { get; set; }

        public string Dep_Type { get; set; }

    }

}
