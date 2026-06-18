using Exportal_DAL.Utilities;
using LKE_DAL.Models;
using LKE_DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LKEWebAPI.Controllers
{
    public class _45DayIDController : ApiController
    {
        [Route("45DayID/getRecentlySubmitted")]
        [HttpGet]
        public IHttpActionResult get45DayRecentlySubmitted(string dbName, int entityID, int caseID, int caseID2)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);

                List<_45DayIDRecentlySubmitted> list = transImports.getRecentlySubmitted45DayID(entityID, caseID, caseID2);
                //list.Add(new _45DayIDDeadlineModel());

                result.Add("data", list);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }

        [Route("uploadFile")]
        [HttpGet]
        public async Task<IHttpActionResult> testUploadFile()
        {
            RTransImports transImports = new RTransImports(null);
            transImports.uploadFileToBlobServer(HttpContext.Current.Server.MapPath(Path.Combine("~/Resources/")) + "45DayID_RecentlySubmitted_Template.xlsx");

            string destPath = HttpContext.Current.Server.MapPath(Path.Combine("~/PDFS/"));
            byte[] d = await transImports.DownloadFileFromBlob("template.xlsx");

            File.WriteAllBytes(destPath + "template.xlsx", d);

            return Ok();
        }


        [Route("45DayID/getReplacementAssets")]
        [HttpGet]
        public IHttpActionResult getReplacementAssets(string dbName)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);

                List<_45DayIDReplacementAsset> list = transImports.getReplacementAssets();
                result.Add("data", list);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }


        [Route("45DayID/getMakeModels")]
        [HttpGet]
        public IHttpActionResult getMakeModelList(string dbName, string assetCategory)
        {
            //http://localhost:60789/45DayID/getMakeModels?dbName=Power_Motive&assetCategory=333120
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);

                List<MakeModel> list = transImports.getMakeModelList(assetCategory);
                result.Add("data", list);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }




        [Route("45DayID/saveReplacementData")]
        [HttpPost]
        public IHttpActionResult saveReplacementData(ReplacementRequestData reqData)
        {
            //http://localhost:60789/45DayID/getMakeModels?dbName=Power_Motive&assetCategory=333120
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", reqData.dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);
                //log signature
                //transImports.saveSignature(reqData.userID, reqData.username, reqData.signature, reqData.description);

                foreach (MakeIDData data in reqData.makeModelList)
                {
                    //update Assets_DepreciationAttributes
                    //transImports.callIS_1127_45DayID_spMakeID(reqData.username, data.rowID);
                    transImports.callIS_1127a_45DayID_spMakeID_WTP(data.rowID);
                    //insert into Assets_LKE_Sales_MakeMod_Options
                    transImports.callIS_1128_45DayID_spMakeID_ApdAssignments(data.quantity, data.rowID, data.purchasePrice, data.makeModID);

                }
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }

        [Route("45DayID/revokeIDs")]
        [HttpPost]
        public IHttpActionResult revokeIDs(RevokeIDReq reqData)
        {
            //http://localhost:60789/45DayID/getMakeModels?dbName=Power_Motive&assetCategory=333120
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", reqData.dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);
                //log signature
                transImports.saveSignature(reqData.userID, reqData.username, reqData.signature, reqData.description);

                foreach (int rowId in reqData.revokeIDList)
                {
                    //update Assets_DepreciationAttributes
                    transImports.callIS_1126_45DayID_spRevokeID(rowId);
                }
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }



        [Route("45DayID/getMakeModeIDData")]
        [HttpGet]
        public IHttpActionResult getMakeModeIDData(string dbName, int makeModeID)
        {
            //http://localhost:60789/45DayID/getMakeModeIDData?dbName=Power_Motive&makeModeID=333120
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);

                _45DayIDReplacementAsset makeModObj = transImports.getMakeModeIDData(makeModeID);
                result.Add("data", makeModObj);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }


        // PENDING IDENTIFICATIONS

        //Arshad Start

        [Route("45DayID/getPendingIdentificationGridData")]
        [HttpGet]
        public IHttpActionResult getPendingIdentificationGridData(string dbName)
        {
            //http://localhost:60789/45DayID/getPendingIdentificationData?dbName=Power_Motive
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            //string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);

                List<PendingIdentifications> pendingidentificationslistMain = transImports.getPendingIdentificationsMain();
                List<PendingIdentifications> pendingidentificationslistSub = transImports.getPendingIdentificationsSub();
                List<MainData> maindata = new List<MainData>();


                foreach (var p in pendingidentificationslistMain)
                {
                    MainData main = new MainData();
                    List<SubData> subdata = new List<SubData>();
                    main.AssetID = p.AssetID;
                    main.AssetCategory = p.AssetCategory;
                    main.SaleDate = p.SaleDate;
                    main.SalesPrice = p.SalesPrice;
                    main.Day45 = p.Day45;
                    main.DayRemaining = p.DayRemaining;
                    main.PISDate = p.PISDate;
                    main.CaseID = p.CaseID;
                    main.CaseID2 = p.CaseID2;
                    main.LEID = p.LEID;
                    main.ID = p.ID;
                    main.PotentialGain = p.PotentialGain;
                    foreach (var m in pendingidentificationslistSub)
                    {

                        if (p.ID == m.IDLink)
                        {
                            SubData sub = new SubData();
                            sub.AssetCategory = m.AssetCategory;
                            sub.AssetIdentified = m.AssetIdentified;
                            sub.FMV = m.FMV;
                            sub.Quantity = m.Quantity;
                            sub.PurPriceEst = m.PurPriceEst;
                            sub.IDLink = m.IDLink;
                            subdata.Add(sub);

                        }
                        else
                        {

                        }

                    }
                    main.subdata = subdata;
                    maindata.Add(main);
                }

                result.Add("data", maindata);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }


        [Route("45DayID/getPendingIdentificationSubData")]
        [HttpGet]
        public IHttpActionResult getPendingIdentificationSubData(string dbName)
        {
            //http://localhost:60789/45DayID/getPendingIdentificationData?dbName=Power_Motive
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            //string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);

                List<PendingIdentifications> pendingidentificationslistSub = transImports.getPendingIdentificationsSub();
                result.Add("data", pendingidentificationslistSub);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }

        [Route("45DayID/submitSignatureData")]
        [HttpPost]
        public IHttpActionResult submitSignatureData(ReplacementRequestData reqData)
        {
            //http://localhost:60789/45DayID/getMakeModels?dbName=Power_Motive&assetCategory=333120
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", reqData.dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);
                //log signature
                transImports.saveSignature(reqData.userID, reqData.username, reqData.signature, reqData.description);

                foreach (MakeIDData data in reqData.makeModelList)
                {
                    //update Assets_DepreciationAttributes
                    //transImports.callIS_1127_45DayID_spMakeID(reqData.username, data.rowID);
                    //Arshad
                    transImports.callIS_1127b_45DayID_spConfirmWTPID_User(reqData.username, data.rowID);
                    //insert into Assets_LKE_Sales_MakeMod_Options
                    //transImports.callIS_1128_45DayID_spMakeID_ApdAssignments(data.quantity, data.rowID, data.purchasePrice, data.makeModID);

                }
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }
        //Arshad End
    }
}
