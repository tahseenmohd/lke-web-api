using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Exportal_DAL.Utilities;
using LKE_DAL.Utilities;
using LKE_DAL.Models;

namespace LKE_DAL.Repositories
{

    /// <summary>
    /// This class handles making calls to the database. It should be inherited by all repository classes
    ////// </summary>
    public abstract class RepositoryBase
    {
        /// <summary>
        /// This method returns a dataset from executing the command with the parameters given
        /// </summary>
        /// <param name="dbFactory"></param>
        /// <param name="commandName">Stored procedure name</param>
        /// <param name="parameters">Stored procedure parameters</param>
        /// <returns>Data Set with the results from calling the stored procedure</returns>
        /// 
        public DataSet ReturnDataSet(IConnectionFactory dbFactory, string commandName, List<SqlParameter> parameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = (SqlConnection)dbFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(commandName, conn))
                {
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DB_CommandTimeOut"].ToString());

                    foreach (SqlParameter sp in parameters)
                        cmd.Parameters.Add(sp);
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);
                    conn.Close();
                }
            }
            return ds;
        }

        //public DataSet ReturnDataSetTenmp(string commandName, List<SqlParameter> parameters)
        //{
        //    DataSet ds = new DataSet();
        //    using (SqlConnection conn = new SqlConnection("Server=tcp:xfti85q52d.database.windows.net;Database=WyomingCat;Trusted_Connection=no;User Id=adminwtp;Password=7531WTPtax;encrypt=True"))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(commandName, conn))
        //        {
        //            conn.Open();
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DB_CommandTimeOut"].ToString());

        //            foreach (SqlParameter sp in parameters)
        //                cmd.Parameters.Add(sp);
        //            SqlDataAdapter da = new SqlDataAdapter();
        //            da.SelectCommand = cmd;

        //            da.Fill(ds);
        //            conn.Close();
        //        }
        //    }
        //    return ds;
        //}

        /// <summary>
        /// Executes a stored procedure
        /// </summary>
        /// <param name="dbFactory"></param>
        /// <param name="commandName">Stored procedure name</param>
        /// <param name="parameters">Stored procedure parameters</param>
        public void ExecuteStoredProcedure(IConnectionFactory dbFactory, string commandName, List<SqlParameter> parameters)
        {
            using (SqlConnection conn = (SqlConnection)dbFactory.GetConnection())
            {

                using (SqlCommand cmd = new SqlCommand(commandName, conn))
                {
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DB_CommandTimeOut"].ToString());
                    foreach (SqlParameter sp in parameters)
                        cmd.Parameters.Add(sp);

                    cmd.ExecuteReader();
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// Executes a stored procedure
        /// </summary>
        /// <param name="dbFactory"></param>
        /// <param name="commandName">Stored procedure name</param>
        /// <param name="parameters">Stored procedure parameters</param>
        public int ExecuteNonQuery(IConnectionFactory dbFactory, string commandName, List<SqlParameter> parameters)
        {
            using (SqlConnection conn = (SqlConnection)dbFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(commandName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DB_CommandTimeOut"].ToString());
                    foreach (SqlParameter sp in parameters)
                        cmd.Parameters.Add(sp);


                    //return Convert.ToInt32(cmd.Parameters["@Result"].Value);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return Convert.ToInt32(cmd.Parameters["@Result"].Value);
                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }
            }
        }

        public IDataReader ReturnDataSP(SqlConnection conn, string commandName, List<SqlParameter> parameters)
        {
            SqlDataReader reader = null;

            using (SqlCommand cmd = new SqlCommand(commandName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DB_CommandTimeOut"].ToString());

                foreach (SqlParameter sp in parameters)
                    cmd.Parameters.Add(sp);
                reader = cmd.ExecuteReader();


            }
            
            return reader;
        }


        public IDataReader ReturnDataSetQuery(SqlConnection conn, string queryString, List<SqlParameter> parameters)
        {
            SqlDataReader reader = null;
           
                using (SqlCommand cmd = new SqlCommand(queryString, conn))
                {
                    cmd.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DB_CommandTimeOut"].ToString());

                    foreach (SqlParameter sp in parameters)
                        cmd.Parameters.Add(sp);
                    reader = cmd.ExecuteReader();
                   
                }
            
            return reader;
        }


        public DataSet ReturnDataSetQueryOld(IConnectionFactory dbFactory, string queryString, List<SqlParameter> parameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = (SqlConnection)dbFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(queryString, conn))
                {
                    conn.Open();
                    cmd.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DB_CommandTimeOut"].ToString());

                    foreach (SqlParameter sp in parameters)
                        cmd.Parameters.Add(sp);
                    //reader = cmd.ExecuteReader();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);
                    conn.Close();
                }
            }
            return ds;
        }

        internal IDataReader ReturnDataSP(SqlConnection conn, string storedProcName, object parameters)
        {
            throw new NotImplementedException();
        }
    }
}
