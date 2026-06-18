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

namespace LKEWebAPI.Controllers
{
    public class ReportsController : ApiController
    {


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
                    else if(reportObj.ReportList.Count == 1)
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

                reportGenerator.createMultipleReports(reportObj,requestID,pdfsPath,excelDirectoryPath);
                result.Add("status", true);
                result.Add("requestID", requestID);
            }
            catch (Exception Exp)
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
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString",connectionString);
                transImports = new RTransImports(ConnectionFactory);

                List<ReportDetail> list= transImports.getReportsList();


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
            r1.ReportName ="LKE Management Report";
            Report r2 = new Report();
            r2.ReportName = "Depreciation Report";

            repObj.ReportList.Add(r1);
            repObj.ReportList.Add(r2);

            rg.sendEmailToUser("suraj.k@xtreamit.com", "http://www.google.com",repObj);
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
                    List<UserActionTrack> list = context.UserActionTracks.Where(x=>x.IsCompleted==true && x.Username.Equals(username) && x.Database.Equals(dbName)).OrderByDescending(m => m.CreatedDate).Take(10).ToList();

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
        public IHttpActionResult removeNotifications( string username)
        {
            //reports/removeNotifications?username=
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                using (LKE_LoginEntities context = new LKE_LoginEntities())
                {
                    List<UserActionTrack> list = context.UserActionTracks.Where(x => x.IsCompleted == true && x.Username.Equals(username)).ToList();
                    foreach(UserActionTrack uat in list)
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

                List<_45DayIDDeadlineModel> list = transImports.get45DayIDDeadlines(entityID,caseID,caseID2);
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
        public IHttpActionResult SalePurchaseActivityList(string dbName,int entityID,int caseID,int caseID2)
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

                List<SalePurchaseActivity> list = transImports.getSalePurchaseActivityList(entityID,caseID,caseID2);
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
                catch(Exception ex)
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



        [HttpPost]
        public IHttpActionResult RefreshReportData(RefreshReportData objrefreshdata)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {

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

            try
            {
                using (LKE_LoginEntities dbContext = new LKE_LoginEntities())
                {
                    if(objArchive.dblist != null)
                    { 
                    //For each database in the list finding the reports exists or not
                    for(int i= 0; i<= objArchive.dblist.Length-1; i++)
                    {
                            //Checking the existance of the files
                            var dbname = objArchive.dblist[i].ToString();

                    
                            var recordlist = (from ep in dbContext.UserActionTracks
                                              join e in dbContext.ReportsArchives on ep.Id equals e.UserActionID
                                              where ep.Database == dbname
                                              select new
                                              {
                                                  ep.Id,
                                                  ep.Username
                                                  //,
                                                  //ReportId =ep.ReportId                                                     
                                                  //,IsStarted=ep.IsStarted
                                                  //,IsCompleted =ep.IsCompleted
                                                  //,CreatedFilePath =ep.CreatedFilePath
                                                  //,Database    =ep.Database
                                                  //,CreatedDate =ep.CreatedDate
                                                  //,ReportIDShortNames  =ep.ReportIDShortNames
                                                  //,NotificationText   = ep.NotificationText
                                                  //,IsRequestRecreation =ep.IsRequestRecreation

                                              }).ToList();




                            bool FileExists = dbContext.UserActionTracks.Any(rep => rep.Database == dbname); ///////////////// add the cond for creation date
                            





                            if (recordlist != null)
                            {
                                try {

                                    foreach (var item in recordlist)
                                    { }

                                }
                                catch(Exception ex) { }

                            }

                            //File exists
                            if (FileExists)
                        {
                            //Getting the Files list
                            List<UserActionTrack> FilesList = dbContext.UserActionTracks.Where(rep => rep.Database == dbname).ToList();

                                //Recreating each file in the database
                                foreach (var file in FilesList)
                                {
                                    try {
                                    //update the flag in the database as Recreation=True
                                    file.IsRequestRecreation = true;

                                    var objReportsArchives = dbContext.ReportsArchives.Where(rep => rep.UserActionID == file.Id).FirstOrDefault();

                                        if(objReportsArchives != null)
                                        { 
                                    //-------------1Delete the files from Azure Blob Storage ---------------

                                    // Get Blob Container
                                    CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference(objReportsArchives.ContainerName);

                                    // Get reference to blob (binary content)
                                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(objReportsArchives.BlobName);

                                            //delete blob from container    
                                            //blockBlob.Delete();



                                            //Recreating files                                            
                                            ReportModel objReportModel = new ReportModel();
                                            objReportModel.DB_Name = dbname;                                                          
                                                                                        
                                            objReportModel.ReportList = new List<Report>();
                                            Report objReport = new Report();

                                            objReport.ReportID = Convert.ToInt32(objReportsArchives.ReportID);
                                            objReport.ReportName = objReportsArchives.ReportName;
                                            objReport.ReportShortName = file.ReportIDShortNames;
                                            

                                            int? caseID = objReportsArchives.CaseID;
                                            int? caseID2 = objReportsArchives.CaseID2;
                                            int? entityID = objReportsArchives.LEID;
                                            string deptype = objReportsArchives.Dep_Type;                          
                                            
                                                          
                                            Dictionary<int?, string> reportParams = new Dictionary<int?, string>();

                                            switch (file.ReportId)
                                            {
                                                case 1:

                                                    reportParams.Add(9, objArchive.startDate);
                                                    reportParams.Add(11,caseID.ToString());
                                                    reportParams.Add(10, objArchive.endDate); //$scope.endTaxYearLMR,//end date 
                                                    reportParams.Add(11, caseID.ToString()); //case id
                                                    reportParams.Add(12, caseID2.ToString()); //case id 2
                                                    reportParams.Add(14, entityID.ToString()); //leid
                                                    reportParams.Add(15, deptype);//dep type
                                                    reportParams.Add(0, deptype);                                                    
                                                    break;

                                                case 2:

                                                    reportParams.Add(22, objArchive.startDate);
                                                    reportParams.Add(24, caseID.ToString());
                                                    reportParams.Add(23, objArchive.endDate); //end date 
                                                    reportParams.Add(24, caseID.ToString()); //case id
                                                    reportParams.Add(25, caseID2.ToString()); //case id 2
                                                    reportParams.Add(26, entityID.ToString()); //leid
                                                    reportParams.Add(27, deptype); //dep type
                                                    reportParams.Add(0, deptype);
                                                    break;

                                                case 3:

                                                    reportParams.Add(79, objArchive.startDate);
                                                    reportParams.Add(76, caseID.ToString());
                                                    reportParams.Add(80, objArchive.endDate); //end date 
                                                    reportParams.Add(76, caseID.ToString()); //case id
                                                    reportParams.Add(77, caseID2.ToString()); //case id 2
                                                    reportParams.Add(78, entityID.ToString()); //leid
                                                    reportParams.Add(80, deptype); //dep type
                                                    reportParams.Add(0, deptype);
                                                    break;

                                                case 4:

                                                    reportParams.Add(31, objArchive.startDate);
                                                    reportParams.Add(33, caseID.ToString());
                                                    reportParams.Add(32, objArchive.endDate); //end date 
                                                    reportParams.Add(33, caseID.ToString()); //case id
                                                    reportParams.Add(34, caseID2.ToString()); //case id 2
                                                    reportParams.Add(37, entityID.ToString()); //leid
                                                    reportParams.Add(36, deptype); //dep type
                                                    reportParams.Add(0, deptype);
                                                    break;

                                                case 6:

                                                    reportParams.Add(63, objArchive.startDate);
                                                    reportParams.Add(65, caseID.ToString());
                                                    reportParams.Add(64, objArchive.endDate); //end date 
                                                    reportParams.Add(65, caseID.ToString()); //case id
                                                    reportParams.Add(66, caseID2.ToString()); //case id 2
                                                    reportParams.Add(67, entityID.ToString()); //leid
                                                    reportParams.Add(68, deptype); //dep type
                                                    reportParams.Add(0, deptype);
                                                    break;

                                                case 7:

                                                    reportParams.Add(87, objArchive.taxyearId);
                                                    reportParams.Add(63, objArchive.startDate);
                                                    reportParams.Add(82, caseID.ToString());
                                                    reportParams.Add(64, objArchive.endDate); //end date 
                                                    reportParams.Add(82, caseID.ToString()); //case id
                                                    reportParams.Add(83, caseID2.ToString()); //case id 2
                                                    reportParams.Add(84, entityID.ToString()); //leid
                                                    reportParams.Add(86, deptype); //dep type
                                                    reportParams.Add(0, deptype);
                                                    break;



                                                case 8:


                                                    reportParams.Add(103, objArchive.startDate);
                                                    reportParams.Add(100, caseID.ToString());
                                                    reportParams.Add(104, objArchive.endDate); //end date 
                                                    reportParams.Add(100, caseID.ToString()); //case id
                                                    reportParams.Add(101, caseID2.ToString()); //case id 2
                                                    reportParams.Add(102, entityID.ToString()); //leid
                                                    reportParams.Add(0, deptype);
                                                    break;

                                                case 10:


                                                    reportParams.Add(69, objArchive.startDate);
                                                    reportParams.Add(71, caseID.ToString());
                                                    reportParams.Add(70, objArchive.endDate); //end date 
                                                    reportParams.Add(71, caseID.ToString()); //case id
                                                    reportParams.Add(73, caseID2.ToString()); //case id 2
                                                    reportParams.Add(74, entityID.ToString()); //leid
                                                    reportParams.Add(75, deptype);
                                                    reportParams.Add(0, deptype);
                                                    break;

                                                case 12:

                                                    reportParams.Add(0, deptype);
                                                    break;



                                            }
                                            
                                            objReport.reportParams = reportParams;


                                            //add paramtere based on report id                                            
                                            objReportModel.ReportList.Add(objReport);
                                            

                                            string outputDirectoryName = Path.GetFileName(file.CreatedFilePath);
                                            string pdfsPath = HttpContext.Current.Server.MapPath(Path.Combine("~/PDFS/"));
                                            string excelDirectoryPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Resources/"));


                                            ReportsGenerator reportGenerator = new ReportsGenerator();                                                        
                                            reportGenerator.RecreateReports(objReportModel, file.Id, pdfsPath, excelDirectoryPath, outputDirectoryName, objReportsArchives.ContainerName , objReportsArchives.BlobName);
                                            

                                        }
                                    }
                                    catch (Exception ex)
                                    { }

                                }
                                result.Add("status", true);
                                
        }

                    }
                    }

                }
                result.Add("status", true );
            }
            catch (Exception ex)
            {
                result.Add("status", false);
                result.Add("Exception: ", ex.Message.ToString());
            }
            return Ok(result);

        }




        


    }
}
