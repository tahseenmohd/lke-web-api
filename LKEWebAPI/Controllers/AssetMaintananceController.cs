using Exportal_DAL.Utilities;
using LKE_DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LKE_DAL.Models;
namespace LKEWebAPI.Controllers
{
    public class AssetMaintananceController : ApiController
    {
        [Route("assetMaintanance/getAssetMaintananceList")]
        [HttpGet]
        public IHttpActionResult getAssetMaintananceList(string dbName, string AssetID, string SaleStartDate, string SaleEndDate, string AcqStartDate, string AcqEndDate)
        {

            //http://localhost:60789/AMID/getAssetMaintananceList?dbName=USA_Truck
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RAssetMaintance assetMaintanance = null;
            string connectionString = string.Empty;


            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["AssetMaintanance_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", dbName);
                ConnectionFactory = new SqlConnectionFactory("AssetMaintanance_ConnectionString", connectionString);
                assetMaintanance = new RAssetMaintance(ConnectionFactory);

                List<AssetMaintananceModel> list = assetMaintanance.getAssetMaintananceList(AssetID, SaleStartDate, SaleEndDate, AcqStartDate, AcqEndDate);
                result.Add("data", list);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }
        [HttpPost]
        [Route("assetMaintanance/updateAssetMaintananceList")]
        public IHttpActionResult updateAssetMaintananceList(AssetMaintananceModelPost amp)
        {
            //http://localhost:60789/assetMaintanance/updateAssetMaintananceList

            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RAssetMaintance assetMaintanance = null;
            string connectionString = string.Empty;
            string aID = string.Empty;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["AssetMaintanance_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", amp.dbname);
                ConnectionFactory = new SqlConnectionFactory("AssetMaintanance_ConnectionString", connectionString);
                assetMaintanance = new RAssetMaintance(ConnectionFactory);

                if (amp.AssetID != "" || amp.AssetID == string.Empty)
                {
                    aID = assetMaintanance.updateAssetMaintanance(amp);
                    result.Add("AssetID", aID);
                    result.Add("status", true);
                }
                else
                {
                    result.Add("AssetID", 0);
                    result.Add("status", false);
                }

            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);

        }
    }
}
