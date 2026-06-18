using Exportal_DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace LKE_DAL.Models
{
    public class ReportsReq
    {
        public int ReportId { get; set; }
        public int YearId { get; set; }
        public int CaseId { get; set; }
        public int LeId { get; set; }
        public string Dep_type { get; set; }
    }

    public class ReportModel
    {
        public List<Report> ReportList { get; set; }
        public string DB_Name { get; set; }
        public string username { get; set; }
        public string entityName { get; set; }
        public string userEmailID { get; set; }
        public string fullName { get; set; }

        public string userID { get; set; }
    }

    public class Report
    {
        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public string ReportShortName { get; set; }
        public Dictionary<int?,string> reportParams { get; set; }
        public string templateUrl { get; set; }
    }


    public class ArchiveDTO
    {
        public string[] dblist { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string taxyearId { get; set; }
        public string username { get; set; }
        public string entityName { get; set; }
    }

    public class RefreshReportData
    {
        public string deptype { get; set; }
        public string yearid { get; set; }
        public string Yearid1 { get; set; }
        public string RunYearand1 { get; set; }
        public string RunMatch { get; set; }
        public string ManualMatches { get; set; }
        public string AlternateMatch { get; set; }
        public string dbName { get; set; }
        public RefreshReportData() { }

        public RefreshReportData(DataRow dr)
        {
            if (dr.Table.Columns.Contains("AlternateMatch"))
            {
                this.AlternateMatch = DataTypesUtilities.StringNZ(dr["AlternateMatch"]);
            }
        }
    }

    /// <summary>
    /// ReportParams
    /// </summary>
    //public class ReportParams
    //{
    //    public int ReportId { get; set; }
    //    public int? taxYear { get; set; }
    //    public DateTime? taxYearFrom { get; set; }
    //    public DateTime? taxYearTo { get; set; }
    //    public string depreciationType { get; set; }
    //    public string preference { get; set; }
    //    public string templateUrl { get; set; }
    //}

}




