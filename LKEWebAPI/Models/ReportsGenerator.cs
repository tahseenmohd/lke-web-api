using Exportal_DAL.Utilities;
using Ionic.Zip;
using LKE_DAL;
using LKE_DAL.Models;
using LKE_DAL.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using LKEWebAPI.Utilities;
using System.Threading;

namespace LKEWebAPI.Models
{
    public class ReportsGenerator
    {
        ArrayList categoryOneList = new ArrayList();
        ArrayList categoryTwoList = new ArrayList();
        ArrayList categoryThreeList = new ArrayList();
        public object PX { get; private set; }

        public ReportsGenerator()
        {

            categoryOneList.Add(12);
            categoryOneList.Add(1);
            categoryOneList.Add(8);
            categoryOneList.Add(2);
            categoryOneList.Add(3);

            categoryTwoList.Add(17);
            categoryTwoList.Add(7);
            categoryTwoList.Add(6);

            categoryThreeList.Add(10);
            categoryThreeList.Add(4);
        }

        public Task createMultipleReports(ReportModel reportObj, int requestID, string pdfsPath, string excelDirectoryPath)
        {
            return Task.Run(async () =>
            {
                SqlConnectionFactory ConnectionFactory = null;
                RTransImports transImports = null;
                string connectionString = string.Empty;
                var ServerURL = System.Configuration.ConfigurationManager.AppSettings["ServerURL"];
                string outputDirectoryName = null;
                bool shouldSendAnEmail = false;
                int[] emailSendingReportIds = { 6, 7, 17, 4, 10 };
                try
                {
                    connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                    connectionString = connectionString.Replace("XXXXXX", reportObj.DB_Name);
                    ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                    transImports = new RTransImports(ConnectionFactory);
                    string timestamp = DateTime.Now.ToString("MMddyyyy-HHmmss-fff");
                    outputDirectoryName = "ReportSet-" + timestamp;
                    foreach (Report report in reportObj.ReportList)
                    {
                        if (emailSendingReportIds.Contains(report.ReportID))
                        {
                            shouldSendAnEmail = true;
                            break;
                        }

                    }


                    foreach (Report report in reportObj.ReportList)
                    {

                        //getting spnames and their parameters to execute and populate 
                        List<ReportDataSourceModel> reportDataSourceList = new List<ReportDataSourceModel>();
                        reportDataSourceList = transImports.getReportDataSourcesWithParams(report.ReportID);

                        Dictionary<string, object> reportAvailableObj = isReportAvailableForSelectionCrieteria(reportObj, report, reportDataSourceList);
                        if ((bool)reportAvailableObj["isReportPresent"])
                        {

                            string directoryPathToCreateFile = pdfsPath + outputDirectoryName;
                            string taxbookName = "";
                            try
                            {
                                taxbookName = report.reportParams[0];
                            }
                            catch (Exception ex)
                            {

                            }

                            if (!Directory.Exists(pdfsPath))
                            {
                                Directory.CreateDirectory(pdfsPath);
                            }

                            if (!Directory.Exists(directoryPathToCreateFile))
                            {
                                Directory.CreateDirectory(directoryPathToCreateFile);
                            }

                            //if report available, copy that report to the outputDirectoryName and proceed
                            string reportContainer = reportAvailableObj["container"].ToString();
                            string reportBlobName = reportAvailableObj["blobName"].ToString();

                            byte[] d = await transImports.DownloadFileFromBlob(reportBlobName, reportContainer);
                    
                        File.WriteAllBytes(pdfsPath + outputDirectoryName + "\\" + report.ReportShortName + "-" + taxbookName + "-" + timestamp + ".xlsx", d);
                        }
                        else
                        {
                            //add parameters if required

                            //reportParameters = reportObj.reportParamsList.Find(delegate (ReportParams c) { return c.ReportId == report.ReportID; });
                            try
                            {
                                string createdFilePath = ""; //transImports.createReport(excelDirectoryPath + report.templateUrl + ".xlsx", pdfsPath, reportDataSourceList, outputDirectoryName, report.ReportShortName, report.reportParams, timestamp, reportObj.entityName);

                                //----------

                                try
                                {
                                    createdFilePath = transImports.createReport(excelDirectoryPath + report.templateUrl + ".xlsx", pdfsPath, reportDataSourceList, outputDirectoryName, report.ReportShortName, report.reportParams, timestamp, reportObj.entityName, false, "");

                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message.ToLower().Contains("datetime") || ex.Message.ToLower().Contains("date and/or time"))
                                    {
                                        try
                                        {
                                            createdFilePath = transImports.createReport(excelDirectoryPath + report.templateUrl + ".xlsx", pdfsPath, reportDataSourceList, outputDirectoryName, report.ReportShortName, report.reportParams, timestamp, reportObj.entityName, false, "first");

                                        }
                                        catch (Exception exc)
                                        {
                                            if (exc.Message.ToLower().Contains("datetime") || ex.Message.ToLower().Contains("date and/or time"))
                                            {
                                                createdFilePath = transImports.createReport(excelDirectoryPath + report.templateUrl + ".xlsx", pdfsPath, reportDataSourceList, outputDirectoryName, report.ReportShortName, report.reportParams, timestamp, reportObj.entityName, false, "second");

                                            }
                                        }

                                    }
                                }
                                ///--------------

                                SaveFileInAzureBlobStorage(createdFilePath, reportObj, requestID, report,reportObj.username);
                            }
                            catch (Exception ex)
                            {

                            }

                        }

                    }
                    //save zipfile
                    ZipFile zipFile = new ZipFile();
                    zipFile.AddDirectory(pdfsPath + outputDirectoryName);
                    zipFile.Save(pdfsPath + outputDirectoryName + ".zip");
                    string filepath = string.Empty;
                    SaveZipFileInAzureBlobStorage(pdfsPath + outputDirectoryName + ".zip", reportObj.DB_Name,ref filepath);
                    //delete folder which we created
                    using (LKE_LoginEntities context = new LKE_LoginEntities())
                    {
                        UserActionTrack userActionObj = context.UserActionTracks.Where(i => i.Id == requestID).FirstOrDefault();
                        userActionObj.IsCompleted = true;
                        //ServerURL
                        //@22112017 --venky @store zip file path in database from azure(blob storage) not web server path
                        //userActionObj.CreatedFilePath = ServerURL + "//PDFS//" + outputDirectoryName + ".zip";
                        userActionObj.CreatedFilePath = filepath;

                        context.Entry(userActionObj).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }

                    //sending email to user that reports are generated
                    if (shouldSendAnEmail)
                    {
                        //@29112017 --venky @sending email's from azure(blob storage) not in web server path
                        //sendEmailToUser(reportObj.userEmailID, ServerURL + "//PDFS//" + outputDirectoryName + ".zip", reportObj);
                        sendEmailToUser(reportObj.userEmailID, filepath, reportObj);
                    }
                    try
                    {
                        DirectoryInfo directory = new DirectoryInfo(pdfsPath + outputDirectoryName);
                        directory.Delete(true);
                        //@29112017 --venky @Deleting .zip file from webserver after generating the reports
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        File.Delete(pdfsPath + outputDirectoryName + ".zip");

                    }
                    catch (Exception ex)
                    {

                    }
                }
                catch (Exception ex)
                {

                }

                return;
            });
        }

        private void SaveZipFileInAzureBlobStorage(string createdFilePath,string DB_Name ,ref string filepath)
        {
            string ContainerName = DB_Name + "-Reports";
            ContainerName = ContainerName.ToLower();
            ContainerName = ContainerName.Replace('_', '-');
            ContainerName = ContainerName.Replace(' ', '-');
            MemoryStream ms = new MemoryStream();
            string fileType = "application/zip";

            string fileName = System.IO.Path.GetFileName(createdFilePath);
            FileInfo file = new FileInfo(createdFilePath);
            file.Open(FileMode.Open).CopyTo(ms);
            try
            {
                // Get Blob Container
                CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference(ContainerName);
                // Get reference to blob (binary content)
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                SetPublicContainerPermissions(container);
                blockBlob.Properties.ContentType = fileType;
                blockBlob.Metadata["filename"] = fileName;
                filepath= container.Uri+"/"+ fileName.ToString().Trim();

                Stream stream = new MemoryStream(ms.ToArray());

                AsyncCallback UploadCompleted = new AsyncCallback(OnUploadCompleted);
                blockBlob.BeginUploadFromStream(stream, UploadCompleted, blockBlob);
            }
            catch (Exception ex)
            {
                throw ex;
            }
                      
        }

        private void OnUploadCompleted(IAsyncResult result)
        {
            CloudBlockBlob blob = (CloudBlockBlob)result.AsyncState;
            blob.SetMetadata();
            blob.EndUploadFromStream(result);

        }
        public static void SetPublicContainerPermissions(CloudBlobContainer container)
        {
            BlobContainerPermissions permissions = container.GetPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            container.SetPermissions(permissions);
        }

        //Creating reports with refreshed data
        public bool createReports(ReportModel reportObj, int requestID, string pdfsPath, string excelDirectoryPath,string username)
        {

            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            var ServerURL = System.Configuration.ConfigurationManager.AppSettings["ServerURL"];
            string outputDirectoryName = null;
            string createdFilePath = "";
            bool Output = false;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", reportObj.DB_Name);
                ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);
                string timestamp = DateTime.Now.ToString("MMddyyyy-HHmmss-fff");

                outputDirectoryName = "ReportSet-" + timestamp;

                foreach (Report report in reportObj.ReportList)
                {

                    //getting spnames and their parameters to execute and populate 
                    List<ReportDataSourceModel> reportDataSourceList = new List<ReportDataSourceModel>();
                    reportDataSourceList = transImports.getReportDataSourcesWithParams(report.ReportID);

                    //Recreating files on the server 

                    //@venky,01022018: we have passed parameter reportObj.DB_Name -> reportObj.entityname
                    try
                    {
                        createdFilePath = transImports.createReport(excelDirectoryPath + report.templateUrl, pdfsPath, reportDataSourceList, outputDirectoryName, report.ReportShortName, report.reportParams, timestamp, reportObj.entityName, false, "");

                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToLower().Contains("datetime") || ex.Message.ToLower().Contains("date and/or time"))
                        {
                            try
                            {
                                createdFilePath = transImports.createReport(excelDirectoryPath + report.templateUrl, pdfsPath, reportDataSourceList, outputDirectoryName, report.ReportShortName, report.reportParams, timestamp, reportObj.entityName, false, "first");

                            }
                            catch (Exception exc)
                            {
                                if (exc.Message.ToLower().Contains("datetime") || ex.Message.ToLower().Contains("date and/or time"))
                                {
                                    createdFilePath = transImports.createReport(excelDirectoryPath + report.templateUrl, pdfsPath, reportDataSourceList, outputDirectoryName, report.ReportShortName, report.reportParams, timestamp, reportObj.entityName, false, "second");
                                }
                            }

                        }
                    }
                    if (createdFilePath != "")
                    {

                        SaveFileInAzureBlobStorage(createdFilePath, reportObj, requestID, report, username);
                        Output = true;
                    }

                }

                //save zipfile
                //delete folder which we created
                using (LKE_LoginEntities context = new LKE_LoginEntities())
                {
                    UserActionTrack userActionObj = context.UserActionTracks.Where(i => i.Id == requestID).FirstOrDefault();
                    userActionObj.IsCompleted = true;
                    //ServerURL
                    //userActionObj.CreatedFilePath = ServerURL + "//PDFS//" + outputDirectoryName + ".zip";
                    context.Entry(userActionObj).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }


                try
                {
                    DirectoryInfo directory = new DirectoryInfo(pdfsPath + outputDirectoryName);
                    directory.Delete(true);
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

            return Output;

        }



        private void SaveFileInAzureBlobStorage(string createdFilePath, ReportModel reportObj, int requestID, Report report,string username)
        {
            string StartDate = "", EndDate = "", Dep_Type = "", CaseID = "", CaseID2 = "", LEID = "";

            switch (report.ReportID)
            {
                case 1:
                    StartDate = report.reportParams[9]; EndDate = report.reportParams[10];
                    CaseID = report.reportParams[11]; CaseID2 = report.reportParams[12]; LEID = report.reportParams[14]; break;


                case 2:
                    StartDate = report.reportParams[22]; EndDate = report.reportParams[23];
                    CaseID = report.reportParams[24]; CaseID2 = report.reportParams[25]; LEID = report.reportParams[26]; break;

                case 3:
                    StartDate = report.reportParams[79]; EndDate = report.reportParams[80];
                    CaseID = report.reportParams[76]; CaseID2 = report.reportParams[77]; LEID = report.reportParams[78]; break;

                case 4:
                    StartDate = report.reportParams[31]; EndDate = report.reportParams[32]; Dep_Type = report.reportParams[36];
                    CaseID = report.reportParams[33]; CaseID2 = report.reportParams[34]; LEID = report.reportParams[37]; break;


                case 6:
                    StartDate = report.reportParams[63]; EndDate = report.reportParams[64]; Dep_Type = report.reportParams[68];
                    CaseID = report.reportParams[65]; CaseID2 = report.reportParams[66]; LEID = report.reportParams[67]; break;


                case 7:
                    StartDate = report.reportParams[63]; EndDate = report.reportParams[64]; Dep_Type = report.reportParams[86];
                    CaseID = report.reportParams[82]; CaseID2 = report.reportParams[83]; LEID = report.reportParams[84]; break;

                case 8:
                    StartDate = report.reportParams[103]; EndDate = report.reportParams[104];
                    CaseID = report.reportParams[100]; CaseID2 = report.reportParams[101]; LEID = report.reportParams[102]; break;

                case 10:
                    StartDate = report.reportParams[69]; EndDate = report.reportParams[70]; Dep_Type = report.reportParams[75];
                    CaseID = report.reportParams[71]; CaseID2 = report.reportParams[73]; LEID = report.reportParams[74]; break;

                case 17:
                    StartDate = report.reportParams[63]; EndDate = report.reportParams[64]; Dep_Type = report.reportParams[6];
                    CaseID = report.reportParams[3]; CaseID2 = report.reportParams[4]; LEID = report.reportParams[5]; break;

                case 12:
                    //@venky,01022018: for StartDate,endDate required
                    //StartDate = null; EndDate = null;
                    StartDate = report.reportParams[427];EndDate= report.reportParams[460];
                    Dep_Type = null; CaseID = null; CaseID2 = null; LEID = null;
                    break;

            }




            string ContainerName = reportObj.DB_Name + "-Reports";
            ContainerName = ContainerName.ToLower();
            ContainerName = ContainerName.Replace('_', '-');
            ContainerName = ContainerName.Replace(' ', '-');
            string BlobName = ""; // report.ReportID + "_" + StartDate.Replace("/","-") + "_" + EndDate.Replace("/", "-") + "_" + Dep_Type + ".xlsx";

            BlobName = System.IO.Path.GetFileName(createdFilePath);


            RTransImports transImports = new RTransImports(null);
            transImports.uploadFileToBlobServer(createdFilePath, ContainerName, BlobName);


            //archive the report to ReportsArchive table
            using (LKE_LoginEntities context = new LKE_LoginEntities())
            {

                //containerName = userid-dbname
                //blobName = reportshortname_startdate_enddate_deptype.xlsx
                ReportsArchive reportArchiveObj = new ReportsArchive();
                reportArchiveObj.UserActionID = requestID;
                reportArchiveObj.ReportID = report.ReportID;
                reportArchiveObj.ReportName = report.ReportShortName;
                reportArchiveObj.ContainerName = ContainerName;
                reportArchiveObj.BlobName = BlobName;
                reportArchiveObj.Username = username;

                try
                {
                    reportArchiveObj.StartDate = DateTime.ParseExact(StartDate, @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    reportArchiveObj.EndDate = DateTime.ParseExact(EndDate, @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception ex)
                {
                    reportArchiveObj.StartDate = Convert.ToDateTime(StartDate);
                    reportArchiveObj.EndDate = Convert.ToDateTime(EndDate);
                }


                reportArchiveObj.Dep_Type = Dep_Type;

                reportArchiveObj.CaseID = Convert.ToInt32(CaseID);
                reportArchiveObj.CaseID2 = Convert.ToInt32(CaseID2);
                reportArchiveObj.LEID = Convert.ToInt32(LEID);

                //reportArchiveObj.ReportArchivePath = pdfsPath + outputDirectoryName + "\\" + report.ReportShortName + ".xlsx";
                reportArchiveObj.Username = reportObj.username;
                //code to add request criteria
                context.Entry(reportArchiveObj).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        private Dictionary<string, object> isReportAvailableForSelectionCrieteria(ReportModel reportObj, Report report, List<ReportDataSourceModel> reportDataSourceList)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("isReportPresent", false);
            result.Add("container", "");
            result.Add("blobName", "");
            SqlConnectionFactory ConnectionFactoryLL = null;
            RTransImports transImportsLL = null;
            string connectionStringLL = string.Empty;


            try
            {
                connectionStringLL = ConfigurationManager.ConnectionStrings["LKE_LoginConnectionString"].ConnectionString;
                ConnectionFactoryLL = new SqlConnectionFactory("LKE_LoginConnectionString", connectionStringLL);
                transImportsLL = new RTransImports(ConnectionFactoryLL);
                //check for the report with same criteria and return back the record.
                ReportsArchive archiveObj = transImportsLL.getReportArchiveIfAvailable(reportObj, report, reportDataSourceList, categoryOneList, categoryTwoList, categoryThreeList);
                if (archiveObj != null)
                {
                    result["isReportPresent"] = true;
                    result["container"] = archiveObj.ContainerName;
                    result["blobName"] = archiveObj.BlobName;
                }
                else
                {
                    result["isReportPresent"] = false;
                    result["container"] = "";
                    result["blobName"] = "";
                }
            }
            catch (Exception ex)
            {
                result["isReportPresent"] = false;
                result["container"] = "";
                result["blobName"] = "";
            }

            return result;
        }

        public void sendEmailToUser(string emailId, string link, ReportModel reportObj)

        {
            try
            {
                string body = string.Empty;
                string smtpClientHost = "ipaddress";
                MailMessage message = new MailMessage();

                //Mail From: WTP Admin Id
                string from_EmailId = System.Configuration.ConfigurationSettings.AppSettings["adminEmail"].ToString();
                message.From = new MailAddress(from_EmailId);

                //Mail To: Current user Email id
                message.To.Add(new MailAddress(emailId));
                /*    message.To.Add(new MailAddress("suraj.k@xtreamit.com"));*///message.To.Add(new MailAddress(emailId));

                //Mail Subject 
                message.Subject = "Requested reports available for download";

                StringBuilder sbBody = new StringBuilder();


                sbBody.Append("<table width='100%', border='0' bordercolor='#CFD8E6' cellpadding='4' cellspacing='0'>");

                sbBody.Append("<tr>"); //sbBody.Append("<td>");
                sbBody.Append("<td style = 'font -size: 11pt;font-family: Times New Roman, Times, serif; text-align left' >");
                sbBody.Append("Hi " + reportObj.fullName + ", </td></tr>");

                //URL for downloading the link
                sbBody.Append("<tr><td>The following report(s) are ready and can be downloaded by </td></tr><tr><td> <a href='" + link + "'>clicking here:</a>  <br /> </td></tr>");

                //List of reports user requested

                for (int i = 0; i < reportObj.ReportList.Count; i++)
                {
                    Report report = reportObj.ReportList[i];
                    sbBody.Append("<tr><td>" + (i + 1) + ") " + report.ReportName + " <br /></td></tr>");
                }

                sbBody.Append("<tr><td></td></tr><tr><td>Thank you,</td></tr><tr><td> Software Administrator</td></tr>");
                //sbBody.Append("</td>");
                //sbBody.Append("</tr>");
                sbBody.Append("</table>");
                body = sbBody.ToString();
                message.Body = body;
                message.IsBodyHtml = true;


                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.office365.com";
                smtp.EnableSsl = true;
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = System.Configuration.ConfigurationSettings.AppSettings["adminEmail"].ToString();
                NetworkCred.Password = System.Configuration.ConfigurationSettings.AppSettings["adminPassword"].ToString();
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(message);
            }

            catch (Exception e)
            {
                //Unable to send mail to user
            }


        }




        public void sendEmailToAdmin(Dictionary<string, List<string>> EmailResult, string Module = "RefreshReport")
        {
            try
            {
                string body = string.Empty;
                string smtpClientHost = "ipaddress";
                MailMessage message = new MailMessage();

                //Mail From: WTP Admin Id
                string from_EmailId = System.Configuration.ConfigurationSettings.AppSettings["adminEmail"].ToString();
                message.From = new MailAddress(from_EmailId);

                //Mail To: Current user Email id
                //message.To.Add(new MailAddress("jim.fyhrie@wtpadvisors.com"));//message.To.Add(new MailAddress(emailId));

                //@ need to change email while staging

                //message.To.Add(new MailAddress("azhar.m@xtreamitsolutions.com"));
                //MailAddress copy = new MailAddress("mohammedazharuddin100@gmail.com");
                message.To.Add(new MailAddress("Stephanie.Bean@wtpadvisors.com"));
                message.To.Add(new MailAddress("ron.hodgeman@wtpadvisors.com"));
                MailAddress copy = new MailAddress("jim.fyhrie@wtpadvisors.com");


                message.CC.Add(copy);

                StringBuilder sbBody = new StringBuilder();

                sbBody.Append("<table width='100%', border='0' bordercolor='#CFD8E6' cellpadding='4' cellspacing='0'>");

                sbBody.Append("<tr>"); //sbBody.Append("<td>");
                sbBody.Append("<td style = 'font -size: 11pt;font-family: Times New Roman, Times, serif; text-align left' >");
                sbBody.Append("Hi,</td></tr>");


                int i = 0;
                try
                {
                    StringBuilder msg = new StringBuilder();


                    foreach (KeyValuePair<string, List<string>> dicItem in EmailResult)
                    {

                        foreach (string entry in dicItem.Value)
                        {
                            if (Module == "RefreshReport")
                            {
                                msg.Append("<tr><td><br> Database : ");
                                msg.Append(dicItem.Key.ToString() + " <br /></td></tr>");
                            }

                            //iterating the list of string(Report List) for the current dictionary entry.
                            msg.Append("<tr><td>" + (i + 1) + "." + entry.ToString() + " <br /></td></tr>");
                            i++;
                        }
                    }





                    if (Module == "RefreshReport")
                    {
                        message.Subject = "Refreshing of reports completed";


                        if (i > 0)
                        {
                            sbBody.Append("<tr><td><br>All the reports have been refreshed successfully in the databases that you had selected except the following: </ td ></ tr > ");
                            sbBody.Append(msg);
                        }
                        else
                        {
                            sbBody.Append("<tr><td><br>All the reports have been refreshed successfully in the databases that you had selected.</ td ></ tr >");
                        }
                    }
                    else
                    {
                        message.Subject = "Client Data Refresh Reports";

                        sbBody.Append("<tr><td>The routine is executed successfully for refreshing the Client data, Kindly, find the result as mentioned below.</td></tr>");
                        sbBody.Append(msg);
                    }



                }
                catch (Exception ex)
                { }



                sbBody.Append("<tr><td></td></tr><tr><td><br>Thank you,</td></tr><tr><td> Software Administrator</td></tr>");
                sbBody.Append("</table>");
                body = sbBody.ToString();
                message.Body = body;
                message.IsBodyHtml = true;


                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.office365.com";
                smtp.EnableSsl = true;
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = System.Configuration.ConfigurationSettings.AppSettings["adminEmail"].ToString();
                NetworkCred.Password = System.Configuration.ConfigurationSettings.AppSettings["adminPassword"].ToString();
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(message);
            }

            catch (Exception e)
            {
                //Unable to send mail to user
            }


        }

        public string DeleteFileFromAzure(string dbname, int? ReportID, DateTime StartDate, DateTime EndDate)
        {

            string Result = "";
            try
            {
                using (LKE_LoginEntities dbContext = new LKE_LoginEntities())
                {

                    //-------------1Delete the files from Azure Blob Storage ---------------
                    var Recentfield = (from ep in dbContext.UserActionTracks
                                       join e in dbContext.ReportsArchives on ep.Id equals e.UserActionID
                                       where (ep.Database == dbname) &
                                       (e.StartDate >= StartDate & e.StartDate <= EndDate | (e.StartDate == null & e.EndDate == null))
                                       & (e.ReportID == ReportID)
                                       orderby e.ID descending
                                       select new
                                       {
                                           e.ID,
                                           e.BlobName,
                                           e.ContainerName,
                                           e.UserActionID //@venky


                                       }).ToList();//.Take(1);

                    if (Recentfield != null)
                    {
                        int j = 0;

                        foreach (var item in Recentfield)
                        {
                            try
                            {
                                if (j != 0)
                                {

                                    ReportsArchive obj = dbContext.ReportsArchives.Where(c => c.ID == item.ID).ToList<ReportsArchive>().FirstOrDefault();
                                    dbContext.ReportsArchives.Remove(obj);
                                    dbContext.SaveChanges();

                                    //@venky,31012018: for delete alredy refreshed record
                                    var userobj = dbContext.UserActionTracks.Where(c => c.Id == item.UserActionID && c.CreatedFilePath==null).ToList();
                                    
                                    if (userobj.Count > 0)
                                    {
                                        var objmy = dbContext.UserActionTracks.Find(userobj[0].Id);
                                        dbContext.UserActionTracks.Remove(objmy);
                                        dbContext.SaveChanges();
                                    }
                                    //@venky,31012018: for delete alredy refreshed record
                                    // Get Blob Container
                                    CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference(item.ContainerName);

                                    // Get reference to blob (binary content)
                                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(item.BlobName);

                                    //delete blob from container    
                                    blockBlob.Delete();

                                  

                                }
                                j++;

                                Result += ReportID.ToString() + ",";
                            }



                            catch (Exception ex)
                            { }
                        }//end of for each

                    }//end of if


                }//end of using block                

            }//End of try
            catch (Exception ex)
            { }

            return Result;

        }



    }



}

