using Exportal_DAL.Utilities;
using LKE_DAL;
using LKE_DAL.Models;
using LKE_DAL.Repositories;
using LKEWebAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Configuration;
using LKE_DAL.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace LKEWebAPI.Controllers
{
    public class AdministrationController : ApiController
    {
        string ApplicationUrl = System.Configuration.ConfigurationManager.AppSettings["AppURL"];
        LKE_LoginEntities context = new LKE_LoginEntities();
        Simple3Des crypt = new Simple3Des("cryptval");
        private string followMessage;
        private string copyMessage;

        [HttpPost]

        public IHttpActionResult AddEditUser(WebUserModel wu)
        {
            //url http://localhost:60789/api/Administration/AddEditUser

            var backup = wu;
            LKE_LoginEntities context = new LKE_LoginEntities();

            var response = new Dictionary<string, object>();

            try
            {

                if (wu.WebUser.ID != 0)
                {
                    context.Entry(wu.WebUser).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

                }

                else
                {
                    wu.WebUser.WebUserPassword = crypt.EncryptData(wu.WebUser.WebUserPassword);
                    context.Entry(wu.WebUser).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    //SendEmail(backup.WebUser, ApplicationUrl + "/login",
                    //          "Your account has been locked as you entered an incorrect password five consecutive times.", 
                    //" LKE Software – Welcome New User", "In order to log into the LKE software, please copy and paste the following link into Google Chrome:",
                    //          "After entering your username (provided by your client service manager in a separate email) and the temporary password shown below, you will be prompted to change your password."
                    //           + "<br/><br/>" + " Temporary password = " + " <b>" + crypt.DecryptData(wu.WebUser.WebUserPassword) + "</b>"
                    //          + "<br/><br/>" + "When setting your new password, the requirements are as follows:- "
                    //          + "<br/><br/>" + "§  At least one upper case letter;" + "<br/><br/>" + "§  At least one lower case letter;;"
                    //          + "<br/><br/>" + "§  At least one number; and" + "<br/><br/>" + "§  At least one special character." + "<br/><br/>"
                    //          + "Please contact your client service manager if you need any assistance." + "<br/><br/>" + "");

                    SendEmail(backup.WebUser, ApplicationUrl + "/login",
                              "",
                    " LKE Software – Welcome New User", "In order to log into the LKE software, please copy and paste the following link into Google Chrome:",
                              "After entering your username (provided by your client service manager in a separate email) and the temporary password shown below, you will be prompted to change your password."
                               + "<br/><br/>" + " Temporary password = " + " <b>" + crypt.DecryptData(wu.WebUser.WebUserPassword) + "</b>"
                              + "<br/><br/>" + "When setting your new password, the requirements are as follows:- "
                              + "<br/>" + " <ul>" 
                              + " <li>" + "At least one upper case letter;"+ "</li> " 
                              + ""  +" <li>" +"At least one lower case letter;" + "</li> "
                              + "" + " <li>" + "At least one number; and" + "</li> "
                              + "" + " <li>" + "At least one of the following special characters: @, !, #, $, or %."  +"</li> " 
                              +"" + " </ul>"
                              + "Please contact your client service manager if you need any assistance." + "" + "");

                }

                foreach (WebDbaseAssignment item in wu.Clients)
                {

                    item.UserID = wu.WebUser.ID;
                    var found = context.WebDbaseAssignments.Where(x => x.ID == item.ID).FirstOrDefault();
                    if (found == null)
                    {
                        context.Entry(item).State = System.Data.Entity.EntityState.Added;
                        context.SaveChanges();
                    }


                }

                foreach (WebDbaseAssignment item in wu.Delclients)
                {

                    item.UserID = wu.WebUser.ID;
                    context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();

                }

                return Ok(response);

            }
            catch (Exception exp)
            {
                return Ok(response);
            }

        }

        [HttpGet]
        public IHttpActionResult GetDatabasesForUser(int userid)
        {
            //url http://localhost:60789/api/Administration/GetDatabasesForUser?userid=
            LKE_LoginEntities context = new LKE_LoginEntities();

            var response = new Dictionary<string, object>();

            try
            {
                List<WebDbaseAssignment> databaseList = context.WebDbaseAssignments.Where(x => x.UserID == userid).ToList();

                //WebRole webRole = context.WebRoles.Where(y => y.WebUserID == userid).First();

                //WebRoleDesc webRoleDesc= context.WebRoleDescs.Where(z =>z.Role.ToUpper()==webRole.Role.ToUpper()).First();


                //for(int i = 0; i < databaseList.Count; i++)
                //{
                //    databaseList[i].roleDesc= webRoleDesc.ID;
                //}


                response.Add("databaseList", databaseList);
                response.Add("success", true);
                return Ok(response);

            }
            catch (Exception exp)
            {
                response.Add("databaseList", null);
                response.Add("success", false);
                return Ok(response);

            }

        }

        private void SendEmail(AllAdminUsers wu)
        {
            throw new NotImplementedException();
        }

        //public void SendEmail(WebUser wu, string LinkUrl, string Message, string Subject, string followMessage, string copyMessage)
        //{
        //    try
        //    {
        //        // string url = string.Empty;
        //        MailMessage mail = new MailMessage();

        //        string link = string.Empty;

        //        link = LinkUrl;
        //        mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["adminEmail"].ToString());
        //        mail.To.Add(wu.Email);
        //        mail.Subject = Subject;
        //        mail.Body = String.Format("Hi " + wu.FirstName + "," + " <br/> <br/>" + Message + ".<br/>Your Username is: " + wu.WebUserName + " and Password is: " + crypt.DecryptData(wu.WebUserPassword) + " <br/> <br/>" + followMessage + " <br/> <br/>" + link + " <br/> <br/>" + copyMessage + "<br/><br/>Thanks,<br/>Administrator");
        //        mail.IsBodyHtml = true;
        //        SmtpClient smtpclient = new SmtpClient();
        //        smtpclient.Host = "smtp.office365.com";
        //        smtpclient.EnableSsl = true;
        //        NetworkCredential NetworkCred = new NetworkCredential();

        //        NetworkCred.UserName = System.Configuration.ConfigurationManager.AppSettings["adminEmail"].ToString();
        //        NetworkCred.Password = System.Configuration.ConfigurationManager.AppSettings["adminPassword"].ToString();
        //        smtpclient.UseDefaultCredentials = true;
        //        smtpclient.Credentials = NetworkCred;
        //        smtpclient.Port = 587;
        //        smtpclient.Send(mail);
        //    }
        //    catch (Exception exp)
        //    {

        //    }

        //}
        public void SendEmail(WebUser wu, string LinkUrl, string Message, string Subject, string followMessage, string copyMessage)
        {
            try
            {
                // string url = string.Empty;
                MailMessage mail = new MailMessage();

                string link = string.Empty;

                link = LinkUrl;
                mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["adminEmail"].ToString());
                mail.To.Add(wu.Email);
                mail.Subject = Subject;
                mail.Body = String.Format("" + wu.FirstName + "," + "" + Message + " <br/> <br/>" + followMessage + " <br/> <br/>" + link + " <br/> <br/>" + copyMessage + "<br/><br/>Thank you,<br/>Software Administrator");
                mail.IsBodyHtml = true;
                SmtpClient smtpclient = new SmtpClient();
                smtpclient.Host = "smtp.office365.com";
                smtpclient.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential();

                NetworkCred.UserName = System.Configuration.ConfigurationManager.AppSettings["adminEmail"].ToString();
                NetworkCred.Password = System.Configuration.ConfigurationManager.AppSettings["adminPassword"].ToString();
                smtpclient.UseDefaultCredentials = true;
                smtpclient.Credentials = NetworkCred;
                smtpclient.Port = 587;
                smtpclient.Send(mail);
            }
            catch (Exception exp)
            {

            }

        }

       

        [HttpGet]
        public IHttpActionResult GetAdminAllUsers()
        {
            //url http://localhost:60789/api/Administration/GetAdminAllUsers
            Dictionary<string, object> result = new Dictionary<string, object>();

            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;

            try
            {

                connectionString = ConfigurationManager.ConnectionStrings["Reports_ConnectionString"].ConnectionString;
                connectionString = connectionString.Replace("XXXXXX", "LKE_LOGIN");
                //ConnectionFactory = new SqlConnectionFactory("Reports_ConnectionString", connectionString);
                //transImports = new RTransImports(ConnectionFactory);

                //List<AllAdminUsers> list = transImports.adminAllUsers();

                List<AllAdminUsers> list1 = new List<AllAdminUsers>();

                SqlConnection wetf = new SqlConnection(connectionString);
                wetf.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.GetAdminAllUsers", wetf))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DB_CommandTimeOut"].ToString());
                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(cmd);
                    bool wetgfe;
                    adapter.Fill(ds);
                    var evfDataList = ds.Tables[0].AsEnumerable().Select(x => new
                    {
                        Id = (x.Field<int>("Id")),
                        WebUserName = (x.Field<string>("WebUserName")),
                        WebUserPassword = (x.Field<string>("WebUserPassword")),
                        FirstName = (x.Field<string>("FirstName")),
                        LastName = (x.Field<string>("LastName")),
                        Email = (x.Field<string>("Email")),
                        Phone = (x.Field<string>("Phone")),
                        Inactive = (x.Field<bool?>("Inactive")),
                        IsLocked = (x.Field<string>("IsLocked")),
                        SecurityQuestion = (x.Field<string>("SecurityQuestion")),
                        SecurityAnswer = (x.Field<string>("SecurityAnswer")),
                        Clients = (x.Field<int?>("Clients")),
                        ClientName = (x.Field<string>("ClientName")),
                        RoleDesc = (x.Field<int>("RoleID"))
                    });

                    wetf.Close();
                    result.Add("data", evfDataList);
                    result.Add("status", true);
                }
            }

            catch (Exception ex)
            {
                result.Add("status", false);
            }

            return Ok(result);

        }

        [HttpGet]
        public IHttpActionResult LockUnLockUsers(int wuid, string IsLocked)
        {

            //url http://localhost:60789/api/Administration/LockUnLockUsers

            var response = new Dictionary<string, object>();
            try
            {
                var found = context.WebUsers.Where(x => x.ID == wuid).FirstOrDefault();
                found.IsLocked = IsLocked;
                if (Convert.ToBoolean(IsLocked) == false)
                {
                    found.Inactive = false;
                    found.IsLocked = "false";
                    found.PasswordUpdatedOn = null;
                    //SendEmail(found, ApplicationUrl + "/login", "Your account has been unlocked by the administrator", "Password Recovery", "You can reset your password using this link:", "Please copy the entire link into the browser.");
                }
                context.Entry(found).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                response.Add("status", 200);
                response.Add("result", true);

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Add("status", 400);
                response.Add("result", ex);
                return Ok(response);
            }
        }

        [HttpGet]
        public IHttpActionResult ActiveInactiveUsers(int wuid, bool Inactive)
        {

            //url http://localhost:60789/api/Administration/ActiveInactiveUsers

            var response = new Dictionary<string, object>();
            try
            {

                var found = context.WebUsers.Where(x => x.ID == wuid).FirstOrDefault();
                found.Inactive = Inactive;
                context.Entry(found).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                response.Add("status", 200);
                response.Add("result", true);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Add("status", 400);
                response.Add("result", ex);
                return Ok(response);
            }
        }


        [HttpGet]

        // url: http://localhost:60789/api/Administration/GetAdminRolesLookup
        public IHttpActionResult GetAdminRolesLookup()
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            try
            {
                LKE_LoginEntities context = new LKE_LoginEntities();
                List<WebRoleDesc> webRolesData = context.WebRoleDescs.ToList();
                response.Add("status", 200);
                response.Add("result", webRolesData);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Add("status", 400);
                response.Add("result", ex);
                return Ok(response);
            }
        }
        [HttpGet]
        // url: http://localhost:60789/api/Administration/usernameExist?username=arshad
        public IHttpActionResult usernameExist(string username)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            try
            {
                var UsernameCount = context.WebUsers.Where(x => x.WebUserName == username).Count();
                if (UsernameCount == 0)
                {
                    response.Add("status", 200);
                    response.Add("result", false);
                    return Ok(response);
                }
                else
                {
                    response.Add("status", 200);
                    response.Add("result", true);
                    return Ok(response);

                }
            }
            catch (Exception exp)
            {
                response.Add("status", 400);
                response.Add("result", exp);
                return Ok(response);

            }
        }

        [HttpGet]
        public IHttpActionResult GetAllDatabasesList()
        {
            //url http://localhost:60789/api/Administration/GetAllDatabasesList
            LKE_LoginEntities context = new LKE_LoginEntities();

            var response = new Dictionary<string, object>();

            try
            {
                List<WebDatabas> AllDatabasesList = context.WebDatabases.ToList();

                //WebRole webRole = context.WebRoles.Where(y => y.WebUserID == userid).First();

                //WebRoleDesc webRoleDesc= context.WebRoleDescs.Where(z =>z.Role.ToUpper()==webRole.Role.ToUpper()).First();


                //for(int i = 0; i < databaseList.Count; i++)
                //{
                //    databaseList[i].roleDesc= webRoleDesc.ID;
                //}


                response.Add("databaseList", AllDatabasesList);
                response.Add("success", true);
                return Ok(response);

            }
            catch (Exception exp)
            {
                response.Add("databaseList", null);
                response.Add("success", false);
                return Ok(response);

            }

        }
    }
}