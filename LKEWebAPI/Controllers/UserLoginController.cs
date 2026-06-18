using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data;
using LKE_DAL;
using LKE_DAL.Utilities;
using LKEWebAPI.Models;
using Exportal_DAL.Utilities;
using System.Configuration;
using LKE_DAL.Repositories;
using LKE_DAL.Models;
using System.Net.Mail;
using System.Net;

namespace LKEWebAPI.Controllers
{
    public class UserLoginController : ApiController
    {
        public string followMessage;
        public string copyMessage;
        string ApplicationUrl = System.Configuration.ConfigurationManager.AppSettings["AppURL"];
        LKE_LoginEntities dbContext = new LKE_LoginEntities();

        public string IsLocked { get; private set; }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("UserLogin/AuthenticateUser")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult AuthenticateUser(LoginCredentials creds)
        {

            IsLocked = "False";
            using ( LKE_LoginEntities dbContext = new LKE_LoginEntities());
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("status", false);
            Simple3Des crypt = new Simple3Des("cryptval");
            
            try
            {
                //Using the LKE_LoginLKE_LoginEntities1 to get the data related to the provided user
                using (LKE_LoginEntities dbContext = new LKE_LoginEntities())
                {

                    string Encryption_For_Authentication = System.Configuration.ConfigurationManager.AppSettings["Encryption_For_Authentication"];

                    //System.Configuration.KeyValueConfigurationElement Encryption_For_Authentication =
                    //rootWebConfig.AppSettings.Settings["Encryption_For_Authentication"];

                    if (Encryption_For_Authentication != null)
                    {
                        //Encrypt the password else pass the password as simple text
                        if (Encryption_For_Authentication == "True")
                        {
                            creds.Password = crypt.EncryptData(creds.Password);
                            string password = crypt.DecryptData("RPuQjqPLNiBhtSmXvWaosV/S9R+pupio3FH0OMSfjRY=");
                        }
                    }

                    //Checking the user validity based on the Username, Password, and also whether user is active or not and if it is not loackout due to invalid login attempts.

                    bool userValid = dbContext.WebUsers.Any(user => user.WebUserName == creds.Username && user.WebUserPassword == creds.Password);
                    WebUser CurrentUser = dbContext.WebUsers.Where(user => user.WebUserName == creds.Username && user.WebUserPassword == creds.Password).ToList<WebUser>().FirstOrDefault();


                    // User found in the database
                    if (userValid)
                    {
                       

                        //Checking whether the user is active or not
                        if (CurrentUser.Inactive == true)
                        {
                            result["status"] = false;
                            result.Add("displayMessage", "The user " + creds.Username.ToString() + " is Inactive user");
                        }

                        //Checking whether the user is Lockedout or not
                        else if (CurrentUser.IsLocked == "True" && CurrentUser.PasswordUpdatedOn != null)
                        {
                            result["status"] = false;
                            result.Add("displayMessage", "The account for user " + creds.Username.ToString() + " is locked");
                        }

                        //Selecting the List of database for the user to be used by the Reporting Model
                        else
                        {
                            //Getting the DB Name assigned to user 
                            //List<string> DBList = GetAssignedDataBaseByUserId(CurrentUser.ID);

                            CurrentUser.RetryCount = "0";
                            CurrentUser.Inactive = false;
                            CurrentUser.IsLocked = "False";
                            dbContext.Entry(CurrentUser).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();


                            var DB_List = (from dba in dbContext.WebDbaseAssignments
                                           join db in dbContext.WebDatabases on dba.DbaseID equals db.ID
                                           join usr in dbContext.WebUsers on dba.UserID equals usr.ID
                                           where usr.ID == CurrentUser.ID && db.Active == 1
                                           orderby db.ActualDbaseName ascending
                                           select new
                                           {
                                               db.ActualDbaseName,
                                               db.ID
                                           }
                                          ).ToList();



                            //var role = (from r in dbContext.WebRoles
                            //            join c in dbContext.WebUsers on r.ID equals c.ID
                            //            where r.ID == c.ID
                            //            select r.Role).ToString();
                            // .SingleOrDefault();

                            var role = dbContext.WebRoles.Where(r => r.WebUserID == CurrentUser.ID).Select(r => r.Role).FirstOrDefault();

                            /*var ID = dbContext.WebUsers.Where(i => i.ID == CurrentUser.ID).Select(i => i.ID).FirstOrDefaul();*/

                            result["status"] = true;
                            result.Add("displayMessage", "Successfully Logged In");
                            result.Add("DBList", DB_List);//return DBList;
                            result.Add("FullName", CurrentUser.FirstName + " " + CurrentUser.LastName);
                            result.Add("UserEmailID", CurrentUser.Email);
                            result.Add("username", creds.Username);
                            result.Add("userID", CurrentUser.ID);
                            result.Add("role", role);
                            result.Add("passwordUpdatedOn", CurrentUser.PasswordUpdatedOn);
                            //result.Add("ID", ID);
                        }

                    }
                    //User is a invalid user
                    else
                    {
                        WebUser Current = dbContext.WebUsers.Where(user => user.WebUserName == creds.Username).First();

                        if (Convert.ToInt32(Current.RetryCount)<4)
                        {
                            Current.RetryCount = (Convert.ToInt32(Current.RetryCount) + 1).ToString();
                            Current.Inactive = false;
                            Current.IsLocked = "False";
                          
                            result["status"] = false;
                            result.Add("displayMessage", "The user name or password provided is incorrect. Number of remaining attempts: "  + (5-Convert.ToInt32(Current.RetryCount)));
                            
                        }
                        else
                        {
                            Current.RetryCount = (5).ToString();
                            Current.Inactive = false;
                            Current.IsLocked = "True";
                            Current.PasswordUpdatedOn = null;
                            result["status"] = false;
                          
                            result.Add("displayMessage", "Your account is locked. Please contact the administrator.");
                            SendEmail(Current, ApplicationUrl + "/login",
                                 "Your account has been locked as you entered an incorrect password five consecutive times.",
                                "LKE Software", "To reset your password, please copy and paste the following link into Google Chrome:", 
                                "When prompted to enter your temporary password, please enter the following: " + "<b>" + crypt.DecryptData(Current.WebUserPassword) + "</b>"
                                + "<br/><br/>" + "When setting your new password, the requirements are as follows:- "
                                + "<br/>" + " <ul>"
                              + " <li>" + "At least one upper case letter;" + "</li> "
                              + "" + " <li>" + "At least one lower case letter;" + "</li> "
                              + "" + " <li>" + "At least one number; and" + "</li> "
                              + "" + " <li>" + "At least one of the following special characters: @, !, #, $, or %." + "</li> "
                              + "" + " </ul>"
                              + "Please contact your client service manager if you need any assistance." + "" + "");
                        }

                       

                        dbContext.Entry(Current).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                      



                    }

                }

            }// End of Try Block

            catch (Exception ex)
            {
                result["status"] = false;
                result.Add("ExceptionMessage", ex.ToString());
                result.Add("displayMessage", "The user name or password provided is incorrect.");
            }

            return Ok(result);
        }

        
     

        [System.Web.Http.AcceptVerbs("GET")]
        [Route("UserLogin/GetEntityData")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult GetEntityData(string dbName)
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

                EntityInformation entityData = transImports.getEntityData();


                result.Add("data", entityData);

                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }
            return Ok(result);
        }

        [System.Web.Http.AcceptVerbs("GET")]
        [Route("UserLogin/LoginHistory")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult SetLoginHistory(int userId,int webDbId)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            SqlConnectionFactory ConnectionFactory = null;
            RTransImports transImports = null;
            string connectionString = string.Empty;
            string excelDirectoryPath = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["LKE_LoginConnectionString"].ConnectionString;
                //connectionString = connectionString.Replace("XXXXXX", dbName);
                ConnectionFactory = new SqlConnectionFactory("LKE_LoginConnectionString", connectionString);
                transImports = new RTransImports(ConnectionFactory);

                transImports.SetLoginHistory(userId, webDbId);


                result.Add("data", "success");

                result.Add("status", true);
            }
            catch (Exception ex)
            {
                result.Add("status", false);
            }
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ChangePassword(Credentials creds)
        {
            //url http://localhost:60789/api/UserLogin/ChangePassword

            Simple3Des crypt = new Simple3Des("cryptval");
            var response = new Dictionary<string, object>();
            var found = dbContext.WebUsers.Where(x => x.WebUserName == creds.username).FirstOrDefault();
            if(found!=null)
            {
                if (found.WebUserPassword == crypt.EncryptData(creds.oldPassword))
                {
                    found.WebUserPassword = crypt.EncryptData(creds.newPassword);

                    found.PasswordUpdatedOn = DateTime.Now;
                    found.IsLocked = "False";
                    dbContext.Entry(found).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    response.Add("status", 200);
                             response.Add("result", true);
                           
                }

            }
            else
            {
                response.Add("status", 400);
                      
                     
            }
            //    var response = new Dictionary<string, object>();
            //Simple3Des crypt = new Simple3Des("cryptval");
            //    try
            //    {
            //            string Encryption_For_Authentication = System.Configuration.ConfigurationManager.AppSettings["Encryption_For_Authentication"];

            //        //System.Configuration.KeyValueConfigurationElement Encryption_For_Authentication =
            //        //rootWebConfig.AppSettings.Settings["Encryption_For_Authentication"];

            //        if (Encryption_For_Authentication != null)
            //        {
            //            //Encrypt the password else pass the password as simple text
            //            if (Encryption_For_Authentication == "True")
            //            {
            //                creds.oldPassword = crypt.EncryptData(creds.Password);
            //            }
            //        }

            //        //Checking the user validity based on the Username, Password, and also whether user is active or not and if it is not loackout due to invalid login attempts.

            //        bool userValid = dbContext.WebUsers.Any(user => user.WebUserName == creds.Username && user.WebUserPassword == creds.Password);
            //        WebUser CurrentUser = dbContext.WebUsers.Where(user => user.WebUserName == creds.Username && user.WebUserPassword == creds.Password).ToList<WebUser>().FirstOrDefault();

            //        // User found in the database
            //        if (userValid)
            //        {
            //            var found = dbContext.WebUsers.Where(x => x.ID == wu.ID).FirstOrDefault();
            //            found.WebUserPassword = wu.WebUserPassword;
            //            found.PasswordUpdatedOn = DateTime.Now;
            //            dbContext.Entry(found).State = System.Data.Entity.EntityState.Modified;
            //            dbContext.SaveChanges();
            //            response.Add("status", 200);
            //            response.Add("result", true);
            //            return Ok(response);
            //        }
            //        else
            //    {
            //        response.Add("status", 400);
            //        response.Add("result", ex);
            //        return Ok(response);
            //    }
            //}
            //    catch (Exception ex)
            //    {
            //        response.Add("status", 400);
            //        response.Add("result", ex);
            //        return Ok(response);
            //    }

            return Ok(response);
        }

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
                mail.Body = String.Format("Hi " + wu.FirstName + "," + " <br/> <br/>" + Message + " <br/> <br/>" + followMessage + " <br/> <br/>" + link + " <br/> <br/>" + copyMessage + "<br/><br/>Thank you,<br/>Software Administrator");
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
    }
    
}
