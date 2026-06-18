using Microsoft.WindowsAzure.Storage.Blob;
using Exportal_DAL.Utilities;
using LKE_DAL;
using LKE_DAL.Models;
using LKE_DAL.Repositories;
using LKEWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LKEWebAPI.Utilities;
using System.Threading.Tasks;
using System.Threading;

namespace LKEWebAPI.Controllers
{
    public class ReportsController : ApiController
    {
        string ApplicationUrl = System.Configuration.ConfigurationManager.AppSettings["AppURL"];

        //[Route("reports/getReport")]
        //[HttpPost]
        //public IHttpActionResult getData(ReportsReq req)
        //{
        //    string connection = ConfigurationManager.ConnectionStrings["United_RentalsLKE_LoginEntities1"].ConnectionString;
        //    var ConnectionFactory = new SqlConnectionFactory(connection);
        //    var transImports = new RTransImports(ConnectionFactory);
        //    string excelDirectoryPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Resources/"));
        //    excelDirectoryPath += "DepreciationReport_Template.xlsx";
        //    string pdfsPath = HttpContext.Current.Server.MapPath(Path.Combine("~/PDFS/"));
        //    string timeStamp = DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss-fff");
        //    using (United_RentalsLKE_LoginEntities1EF context = new United_RentalsLKE_LoginEntities1EF())
        //    {
        //        //res = context.ReportLists.ToList();
        //        string tableName = null;
        //        switch(req.ReportId)
        //        {
        //            case 16:
        //                tableName = "IS_Reports_0012_AssetDepreciationReport_WithLayers_ForTemplate";
        //                break;
        //            case 17:
        //                tableName = "IS_Reports_0012_AssetDepreciationReport_LayerRollup_ForTemplate";
        //                break;
        //            case 18:
        //                tableName = "IS_Reports_0012_AssetDepreciationReport_LayerRollup_ForTemplate";
        //                break;
        //            default:
        //                tableName = "IS_Reports_0012_AssetDepreciationReport_LayerRollup_Attach_Addition_ForTemplate";
        //                break;
        //        }

        //        try
        //        {
        //            ReportDatasource reportDs = context.ReportDatasources.Where(s => s.ReportID == 16).ToList<ReportDatasource>().FirstOrDefault();
        //            reportDs.ReportDatasourceParams = context.ReportDatasourceParams.Where(s => s.ReportDatasourceID == reportDs.ID).ToList();
        //            string filePath = transImports.createReport(excelDirectoryPath, pdfsPath, reportDs,req,tableName);
        //            return Ok(filePath);
        //        }
        //        catch(Exception ex)
        //        {

        //        }
        //    }
        //    return Ok();
        //}


        [Route("reports/getMultipleReports")]
        [HttpPost]
        public IHttpActionResult GetMultipleReports(ReportModel reportObj)
        {

            string pdfsPath = string.Empty;
            string excelDirectoryPath = string.Empty;
            Dictionary<string, object> result = new Dictionary<string, object>();
            ReportsGenerator reportGenerator = new ReportsGenerator();
            try
            {
                int requestID = -1;
                using (LKE_LoginEntities context = new LKE_LoginEntities())
                {
                    UserActionTrack userReq = new UserActionTrack();
                    userReq.Username = reportObj.username;
                    userReq.Database = reportObj.DB_Name;
                    userReq.Type = "Report";
                    userReq.IsStarted = true;
                    var timeUtc = DateTime.UtcNow;
                    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
                    userReq.CreatedDate = easternTime;
                    userReq.ReportIDShortNames = string.Join("|", reportObj.ReportList.Select(x => x.ReportShortName));
                    if (reportObj.ReportList.Count > 1)
                    {
                        userReq.NotificationText = reportObj.ReportList.Count + " reports are ready";
                    }
                    else if (reportObj.ReportList.Count == 1)
                    {
                        userReq.NotificationText = reportObj.ReportList[0].ReportName + " is ready";
                    }
                    context.UserActionTracks.Add(userReq);
                    context.Entry(userReq).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    requestID = userReq.Id;
                }
                pdfsPath = HttpContext.Current.Server.MapPath(Path.Combine("~/PDFS/"));
                excelDirectoryPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Resources/"));

                reportGenerator.createMultipleReports(reportObj, requestID, pdfsPath, excelDirectoryPath);
                result.Add("status", true);
                result.Add("requestID", requestID);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }
            return Ok(result);
        }


        [Route("reports/getUserReports")]
        [HttpGet]
        public IHttpActionResult GetUserReports(string dbName)
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

                List<ReportDetail> list = transImports.getReportsList();


                result.Add("data", list);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }


        [Route("reports/sendSampleEmail")]
        [HttpGet]
        public IHttpActionResult SendEmail()
        {
            ReportsGenerator rg = new ReportsGenerator();
            ReportModel repObj = new ReportModel();
            repObj.userEmailID = "suraj.k@xtreamit.com";
            repObj.ReportList = new List<Report>();
            Report r1 = new Report();
            r1.ReportName = "LKE Management Report";
            Report r2 = new Report();
            r2.ReportName = "Depreciation Report";

            repObj.ReportList.Add(r1);
            repObj.ReportList.Add(r2);

            rg.sendEmailToUser("suraj.k@xtreamit.com", "http://www.google.com", repObj);
            return Ok();
        }


        [Route("reports/getDepTypes")]
        [HttpGet]
        public IHttpActionResult GetDepreciationTypes(string dbName)
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

                List<NameValue> list = transImports.getDepreciationTypes();


                result.Add("data", list);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }


        [Route("reports/getYears")]
        [HttpGet]
        public IHttpActionResult GetYearsList(string dbName)
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

                List<YearModel> list = transImports.getYearsList();


                result.Add("data", list);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }


        [Route("reports/getNotifications")]
        [HttpGet]
        public IHttpActionResult GetNotificationsList(string dbName, string username)
        {
            //reports/getNotifications?dbName=&username=
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                using (LKE_LoginEntities context = new LKE_LoginEntities())
                {
                    List<UserActionTrack> list = context.UserActionTracks.Where(x => x.IsCompleted == true && x.Username.Equals(username) && x.Database.Equals(dbName)).OrderByDescending(m => m.CreatedDate).Take(10).ToList();

                    result.Add("data", list);
                    result.Add("status", true);
                }
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }


        [Route("reports/removeNotifications")]
        [HttpGet]
        public IHttpActionResult removeNotifications(string username)
        {
            //reports/removeNotifications?username=
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                using (LKE_LoginEntities context = new LKE_LoginEntities())
                {
                    List<UserActionTrack> list = context.UserActionTracks.Where(x => x.IsCompleted == true && x.Username.Equals(username)).ToList();
                    foreach (UserActionTrack uat in list)
                    {
                        context.UserActionTracks.Remove(uat);
                    }
                    context.SaveChanges();
                    result.Add("data", list);
                    result.Add("status", true);
                }
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }

        //reports/get45DayIDDeadlines?dbName=
        [Route("reports/get45DayIDDeadlines")]
        [HttpGet]
        public IHttpActionResult get45DayIDDeadlines(string dbName, int entityID, int caseID, int caseID2)
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

                List<_45DayIDDeadlineModel> list = transImports.get45DayIDDeadlines(entityID, caseID, caseID2);
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

        //reports/salePurchaseActivityList?dbName=
        [Route("reports/salePurchaseActivityList")]
        [HttpGet]
        public IHttpActionResult SalePurchaseActivityList(string dbName, int entityID, int caseID, int caseID2)
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

                List<SalePurchaseActivity> list = transImports.getSalePurchaseActivityList(entityID, caseID, caseID2);
                //list.Add(new SalePurchaseActivity());

                result.Add("data", list);
                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);
        }


        [Route("reports/getReportStatus")]
        [HttpGet]
        public IHttpActionResult getReportGenerationStatus(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            using (LKE_LoginEntities context = new LKE_LoginEntities())
            {
                try
                {
                    UserActionTrack requestAction = context.UserActionTracks.Where(x => x.Id == id).FirstOrDefault();
                    result.Add("status", true);
                    result.Add("data", requestAction);
                }
                catch (Exception ex)
                {
                    result.Add("status", false);
                }
            }
            return Ok(result);
        }


        [Route("reports/getRequestsByUser")]
        [HttpGet]
        public IHttpActionResult getRequestsMadeByUser(string username)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            using (LKE_LoginEntities context = new LKE_LoginEntities())
            {
                try
                {
                    List<UserActionTrack> requestActions = context.UserActionTracks.Where(x => x.Username == username).ToList();
                    result.Add("status", true);
                    result.Add("data", requestActions);
                }
                catch (Exception ex)
                {
                    result.Add("status", false);
                }
            }
            return Ok(result);
        }



        public IHttpActionResult RefreshData(string deptype, string yearid, string Yearid1, string RunYearand1, string RunMatch, string ManualMatches, string AlternateMatch, string dbName)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;
            Dictionary<string, List<string>> EmailResult = new Dictionary<string, List<string>>();
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", dbName);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);
                //var list=transImports.RefreshData(objrefreshdata);
                //HttpContext.Server.ScriptTimeout = 300;
                List<string> list = transImports.RefreshData(deptype, yearid, Yearid1, RunYearand1, RunMatch, ManualMatches, AlternateMatch);

                if (list.Count != 0)
                {
                    EmailResult.Add("Result", list);
                    ReportsGenerator report = new ReportsGenerator();
                    report.sendEmailToAdmin(EmailResult, "RefreshData");
                }


                result.Add("data", list);
                result.Add("status", true);
            }

            catch (Exception ex)
            {
                result.Add("status", false);
                result.Add("Exception: ", ex.Message.ToString());
            }


            return Ok(result);

        }


        /*
         * Description: Update archiving
         * Author:
         * Created Date
         * Updated on:
         * Updated By:
         * Parameters:
         */


        [HttpPost]

        public IHttpActionResult UpdateArchive(ArchiveDTO objArchive)
        {
            //url: http://localhost:60789/api/Reports/UpdateArchive/dblist=united_rentals

            Dictionary<string, object> result = new Dictionary<string, object>();

            string pdfsPath = HttpContext.Current.Server.MapPath(Path.Combine("~/PDFS/"));
            string excelDirectoryPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Resources/"));

            //ReportsGenerator rep = new ReportsGenerator();
            //rep.UpdateArchive(objArchive);
            UpdateArchives(objArchive, pdfsPath, excelDirectoryPath);
            result.Add("status", true);
            return Ok(result);

        }


        [HttpPost]
        //public Task UpdateArchives(ArchiveDTO objArchive)
        public Task UpdateArchives(ArchiveDTO objArchive, string pdfsPath, string excelDirectoryPath)
        {
            //url: http://localhost:60789/api/Reports/UpdateArchive/dblist=united_rentals
            return Task.Run(async () =>
            {

                //url: http://localhost:60789/api/Reports/UpdateArchive/dblist=united_rentals
                Dictionary<string, object> result = new Dictionary<string, object>();
                Dictionary<string, List<string>> EmailResult = new Dictionary<string, List<string>>();
                DateTime StartDate;
                DateTime EndDate;
                int RefreshrequestID = -1;
                try
                {
                    StartDate = DateTime.ParseExact(objArchive.startDate, @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                     EndDate = DateTime.ParseExact(objArchive.endDate, @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                     StartDate = Convert.ToDateTime(objArchive.startDate);
                     EndDate = Convert.ToDateTime(objArchive.endDate);
                }

                ReportsGenerator reportGenerator = new ReportsGenerator();


                try
                {
                    using (LKE_LoginEntities dbContext = new LKE_LoginEntities())
                    {
                        if (objArchive.dblist != null)
                        {
                            //For each database in the list finding the reports exists or not
                            for (int i = 0; i <= objArchive.dblist.Length - 1; i++)
                            {
                                //Checking the existance of the files
                                var dbname = objArchive.dblist[i].ToString();
                                List<string> reportlist = new List<string>();

                                //getting all the records for database and the start and end date
                                var recordlist = (from ep in dbContext.UserActionTracks
                                                  join e in dbContext.ReportsArchives on ep.Id equals e.UserActionID
                                                  where (ep.Database == dbname) &
                                                  (e.StartDate >= StartDate & e.StartDate <= EndDate |( e.StartDate==null & e.EndDate==null) )

                                                  select new
                                                  {
                                                      ep.Id,
                                                      ep.Username,
                                                      e.ContainerName,
                                                      e.BlobName,
                                                      e.ReportID,
                                                      e.ReportName,
                                                      ep.ReportIDShortNames,
                                                      e.CaseID,
                                                      e.CaseID2,
                                                      e.LEID,
                                                      e.Dep_Type,
                                                      ep.CreatedFilePath

                                                  }).ToList();




                                if (recordlist != null)
                                {
                                    //@29112017 venky @ for insert new reqId while refreshing report

                                    using (LKE_LoginEntities context = new LKE_LoginEntities())
                                    {
                                        UserActionTrack userReq = new UserActionTrack();
                                        //userReq.Username ="xtreamadmin" ;//@ TO DO
                                        userReq.Username = objArchive.username;
                                        userReq.Database = dbname;
                                        userReq.Type = "Report";
                                        userReq.IsStarted = true;
                                        var timeUtc = DateTime.UtcNow;
                                        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                                        DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
                                        userReq.CreatedDate = easternTime;
                                        userReq.ReportIDShortNames = "";//@ TO DO
                                        userReq.NotificationText = "Refreshed reports are ready in archive";
                                        context.UserActionTracks.Add(userReq);
                                        context.Entry(userReq).State = System.Data.Entity.EntityState.Added;
                                        context.SaveChanges();
                                        RefreshrequestID = userReq.Id;
                                    }
                                    //RefreshrequestID= GenarateReqId(dbname);
                                    //@29112017 venky @ for insert new reqId while refreshing report

                                    //Getting the Files list


                                    //Recreating each file in the database
                                    foreach (var file in recordlist)
                                    {
                                        try
                                        {
                                            int? caseID = file.CaseID;
                                            int? caseID2 = file.CaseID2;
                                            int? entityID = file.LEID;
                                            string deptype = file.Dep_Type;
                                            if (deptype == "") { deptype = "US_Tax"; }
                                            //Thread.Sleep(1000);
                                            //-----------------------------------------------------------
                                            string delresult = reportGenerator.DeleteFileFromAzure(dbname, file.ReportID, StartDate, EndDate);
                                            //--------------------Recreating files-------------
                                            //Thread.Sleep(1000);

                                            ReportModel objReportModel = new ReportModel();
                                            objReportModel.ReportList = new List<Report>();
                                            objReportModel.DB_Name = dbname;
                                            objReportModel.entityName = objArchive.entityName;
                                            objReportModel.userID = dbContext.WebUsers.Where(x => x.WebUserName.Equals(file.Username)).ToList()[0].ID.ToString();
                                            objReportModel.username = file.Username;


                                            Report objReport = new Report();
                                            objReport.reportParams = new Dictionary<int?, string>();
                                            objReport.ReportID = Convert.ToInt32(file.ReportID);
                                            objReport.ReportName = file.ReportName;
                                            objReport.ReportShortName = file.ReportName;

                                            //Setting the parameters for each report

                                            switch (file.ReportID)
                                            {
                                                case 1://done

                                                    objReport.reportParams.Add(9, objArchive.startDate);
                                                    objReport.reportParams.Add(10, objArchive.endDate); //$scope.endTaxYearLMR,//end date 
                                                    objReport.reportParams.Add(11, caseID.ToString()); //case id
                                                    objReport.reportParams.Add(12, caseID2.ToString()); //case id 2
                                                    objReport.reportParams.Add(14, entityID.ToString()); //leid                                                
                                                    objReport.reportParams.Add(15, deptype);//dep type
                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.templateUrl = "SalesPurchasesActivityReport_Template.xlsx";
                                                    break;

                                                case 2: //Done

                                                    objReport.reportParams.Add(22, objArchive.startDate);
                                                    objReport.reportParams.Add(23, objArchive.endDate); //end date 
                                                    objReport.reportParams.Add(24, caseID.ToString()); //case id
                                                    objReport.reportParams.Add(25, caseID2.ToString()); //case id 2
                                                    objReport.reportParams.Add(26, entityID.ToString()); //leid
                                                    objReport.reportParams.Add(27, deptype); //dep type
                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.templateUrl = "GainRecognizedReport_Template.xlsx";
                                                    break;

                                                case 3:

                                                    objReport.reportParams.Add(76, caseID.ToString()); //case id
                                                    objReport.reportParams.Add(77, caseID2.ToString()); //case id 2
                                                    objReport.reportParams.Add(78, entityID.ToString()); //leid
                                                    objReport.reportParams.Add(79, objArchive.startDate);
                                                    objReport.reportParams.Add(80, objArchive.endDate); //end date 
                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.templateUrl = "Upcoming45DayIDs_Template.xlsx";
                                                    break;

                                                case 4:

                                                    objReport.reportParams.Add(31, objArchive.startDate);  //Start date                                              
                                                    objReport.reportParams.Add(32, objArchive.endDate); //end date 
                                                    objReport.reportParams.Add(33, caseID.ToString()); //case id
                                                    objReport.reportParams.Add(34, caseID2.ToString()); //case id 2                                                
                                                    objReport.reportParams.Add(36, deptype);            //dep type
                                                    objReport.reportParams.Add(37, entityID.ToString()); //leid
                                                    //@venky: for US_Tax => US Tax 
                                                    //objReport.reportParams.Add(0, deptype);
                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.templateUrl = "SalesTransactionReport_Template.xlsx";

                                                    break;

                                                case 6:

                                                    objReport.reportParams.Add(63, objArchive.startDate);
                                                    objReport.reportParams.Add(64, objArchive.endDate); //end date                                              
                                                    objReport.reportParams.Add(65, caseID.ToString()); //case id
                                                    objReport.reportParams.Add(66, caseID2.ToString()); //case id 2
                                                    objReport.reportParams.Add(67, entityID.ToString()); //leid
                                                    objReport.reportParams.Add(68, deptype); //dep type
                                                    //@venky: for US_Tax => US Tax 
                                                    //objReport.reportParams.Add(0, deptype);
                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.templateUrl = "SalesofBusinessAssets_Template.xlsx";
                                                    break;

                                                case 7:
                                                    objReport.reportParams.Add(63, objArchive.startDate);
                                                    objReport.reportParams.Add(64, objArchive.endDate); //end date                                                                                                 
                                                    objReport.reportParams.Add(82, caseID.ToString()); //case id
                                                    objReport.reportParams.Add(83, caseID2.ToString()); //case id 2
                                                    objReport.reportParams.Add(84, entityID.ToString()); //leid
                                                    objReport.reportParams.Add(86, deptype); //dep type
                                                    objReport.reportParams.Add(87, objArchive.taxyearId);

                                                    //@venky: for US_Tax => US Tax 
                                                    //objReport.reportParams.Add(0, deptype);
                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.templateUrl = "LKEExchanges_Template.xlsx";
                                                    break;



                                                case 8:


                                                    objReport.reportParams.Add(100, caseID.ToString()); //case id
                                                    objReport.reportParams.Add(101, caseID2.ToString()); //case id 2
                                                    objReport.reportParams.Add(102, entityID.ToString()); //leid
                                                    objReport.reportParams.Add(103, objArchive.startDate);
                                                    objReport.reportParams.Add(104, objArchive.endDate); //end date 

                                                    //@venky: for US_Tax => US Tax 
                                                    //objReport.reportParams.Add(0, deptype);
                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.templateUrl = "45DayID_RecentlySubmitted_Template.xlsx";
                                                    break;

                                                case 10:


                                                    objReport.reportParams.Add(69, objArchive.startDate);
                                                    objReport.reportParams.Add(70, objArchive.endDate); //end date 
                                                    objReport.reportParams.Add(71, caseID.ToString()); //case id
                                                    objReport.reportParams.Add(73, caseID2.ToString()); //case id 2
                                                    objReport.reportParams.Add(74, entityID.ToString()); //leid
                                                    objReport.reportParams.Add(75, deptype);
                                                    //@venky: for US_Tax => US Tax 
                                                    //objReport.reportParams.Add(0, deptype);
                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.templateUrl = "PurchaseTransactionReport_Template.xlsx";
                                                    break;

                                                case 12:

                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.reportParams.Add(427, objArchive.startDate); //start date 
                                                    objReport.reportParams.Add(460, objArchive.endDate);
                                                    objReport.templateUrl = "MakeModelReport_Template.xlsx";
                                                    break;

                                                case 17:


                                                    objReport.reportParams.Add(2, objArchive.taxyearId);
                                                    objReport.reportParams.Add(3, caseID.ToString()); //case id
                                                    objReport.reportParams.Add(4, caseID2.ToString()); //case id 2
                                                    objReport.reportParams.Add(5, entityID.ToString()); //leid
                                                    objReport.reportParams.Add(6, deptype); //depType
                                                    objReport.reportParams.Add(63, objArchive.startDate); //start date 
                                                    objReport.reportParams.Add(64, objArchive.endDate);
                                                    //@venky for US_Tax => US Tax 
                                                    //objReport.reportParams.Add(0, deptype);
                                                    objReport.reportParams.Add(0, "US Tax");
                                                    objReport.templateUrl = "DepreciationReport_Template.xlsx";
                                                    break;



                                            }

                                            //add paramtere based on report id                                            
                                            objReportModel.ReportList.Add(objReport);
                                            string outputDirectoryName = Path.GetFileName(file.CreatedFilePath);

                                        //@29112017 @venky file.id -> RefreshrequestID 
                                        //bool output = reportGenerator.createReports(objReportModel, file.Id, pdfsPath, excelDirectoryPath);
                                        bool output = reportGenerator.createReports(objReportModel, RefreshrequestID, pdfsPath, excelDirectoryPath,objArchive.username);
                                        if (!output)
                                        {
                                            //reportlist.Add(objReport.ReportName);
                                            reportlist.Add(file.BlobName);
                                        }

                                        }
                                        catch (Exception ex)
                                        {
                                            reportlist.Add(file.BlobName);
                                        }

                                    }//end of for each

                                EmailResult.Add(dbname, reportlist);
                               }

                            }//end of for loop

                            //Sending Email with the result
                            reportGenerator.sendEmailToAdmin(EmailResult, "RefreshReport");

                        }//end of if 
                    }//end of using


                }//end of try
                catch (Exception ex)
                {
                    result.Add("status", false);
                    result.Add("Exception: ", ex.Message.ToString());
                }

                return;
            });
        }


    }
}
