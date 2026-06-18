using Exportal_DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LKE_DAL.Models;
using System.Data.SqlClient;
using LKE_DAL.Utilities;
using System.Data;

namespace LKE_DAL.Repositories
{
    public class RAssetMaintance : RepositoryBase
    {
        private IConnectionFactory dbFactory;
        public RAssetMaintance(IConnectionFactory factory)
        {
            dbFactory = factory;
        }
        public List<AssetMaintananceModel> getAssetMaintananceList(string AssetID, string SaleStartDate, string SaleEndDate, string AcqStartDate, string AcqEndDate)
        {
            List<AssetMaintananceModel> makeAssetMainList = new List<AssetMaintananceModel>();

            try
            {
                var storedProcName = "getIS_1200_AssetMaintenance";
                var parameters = new List<SqlParameter>();

                if (AssetID == "null")
                {
                    AssetID = "";
                }
                if (SaleStartDate == "null")
                {
                    SaleStartDate = "";
                }
                if (SaleEndDate == "null")
                {
                    SaleEndDate = "";
                }
                if (AcqStartDate == "null")
                {
                    AcqStartDate = "";
                }
                if (AcqEndDate == "null")
                {
                    AcqEndDate = "";

                }

                parameters.Add(new SqlParameter("@AssetID", AssetID));
                parameters.Add(new SqlParameter("@SaleStartDate", SaleStartDate));
                parameters.Add(new SqlParameter("@SaleEndDate", SaleEndDate));
                parameters.Add(new SqlParameter("@AcqStartDate", AcqStartDate));
                parameters.Add(new SqlParameter("@AcqEndDate", AcqEndDate));

                DataSet dsResponse = null;
                // SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                // conn.Open();
                //base.ReturnDataSP(conn, storedProcName, parameters);

                //   dsResponse = base.ReturnDataSet(dbFactory, "PEXP_GET_ENTITY_LIST", parameters);
                dsResponse = base.ReturnDataSet(dbFactory, storedProcName, parameters);
                // conn.Close();
                if (dsResponse != null || dsResponse.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in dsResponse.Tables[0].Rows)
                    {
                        makeAssetMainList.Add(new AssetMaintananceModel(dr));
                    }
                }
                else
                {

                }

            }
            catch (Exception ex)
            {

            }
            return makeAssetMainList;

        }

        public string updateAssetMaintanance(AssetMaintananceModelPost amp)
        {
            string aId = string.Empty;
            try
            {
                var storedProcName = "updIS_1200_AssetMaintenance";
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@AssetID", amp.AssetID));
                parameters.Add(new SqlParameter("@LKEEligible", amp.LKEEligible));
                parameters.Add(new SqlParameter("@LKEEligible_Purchase", amp.LKEEligible_Purchase));
                parameters.Add(new SqlParameter("@LiabilityRelieved", amp.LiabilityRelieved));
                parameters.Add(new SqlParameter("@LiabilityAssumed", amp.LiabilityAssumed));
                parameters.Add(new SqlParameter("@BootReceived", amp.BootReceived));
                parameters.Add(new SqlParameter("@RelatedPartySale", amp.RelatedPartySale));
                parameters.Add(new SqlParameter("@RelatedPartyPurchase", amp.RelatedPartyPurchase));
                parameters.Add(new SqlParameter("@Dep_Type", amp.Dep_Type));
                DataSet dsResponse = null;
                dsResponse = base.ReturnDataSet(dbFactory, storedProcName, parameters);

                if (dsResponse == null || dsResponse.Tables.Count == 0)
                {

                }
                else
                {
                    if (dsResponse.Tables[0].Rows.Count > 0)
                        aId = dsResponse.Tables[0].Rows[0]["AssetID"].ToString();

                }
            }
            catch (Exception ex)
            {

            }
            return aId;
        }
    }
}