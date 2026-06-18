using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;



namespace Exportal_DAL.Utilities
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        private string _connectionName;
        private string _connectionString;

        public SqlConnectionFactory(string connectionName)
        {
            _connectionName = connectionName;
            _connectionString = ConfigurationManager.ConnectionStrings["United_RentalsLKE_LoginEntities1"].ConnectionString;

        }
        public SqlConnectionFactory(string connectionName,string connectionString)
        {
            _connectionName = connectionName;
            _connectionString = connectionString;

        }
        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
