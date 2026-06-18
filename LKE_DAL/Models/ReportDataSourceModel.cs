using Exportal_DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Models
{
     public  class ReportDataSourceModel
    {
        public int ID { get; set; }
        public string DatasourceType { get; set; }
        public string DatasourceName { get; set; }
        public int ReportID { get; set; }
        public int DisplayOrder { get; set; }
        public string SheetAppend { get; set; }
        public string CellAppend { get; set; }
        public string ReportClassName { get; set; }
        //TableName
        public string TableName { get; set; }
        public ICollection<ReportDataSourceParams> ReportDatasourceParams { get; set; }

        //public ReportDataSourceModel(DataRow dr)
        //{
        //    if (dr.Table.Columns.Contains("ID"))
        //    {
        //        this.ID = DataTypesUtilities.IntNZ(dr["ID"]);
        //    }
        //    if (dr.Table.Columns.Contains("DatasourceType"))
        //    {
        //        this.DatasourceType = DataTypesUtilities.StringNZ(dr["DatasourceType"]);
        //    }
        //    if (dr.Table.Columns.Contains("DatasourceName"))
        //    {
        //        this.DatasourceName = DataTypesUtilities.StringNZ(dr["DatasourceName"]);
        //    }
        //    if (dr.Table.Columns.Contains("ReportID"))
        //    {
        //        this.ReportID = DataTypesUtilities.IntNZ(dr["ReportID"]);
        //    }
        //    if (dr.Table.Columns.Contains("DisplayOrder"))
        //    {
        //        this.DisplayOrder = DataTypesUtilities.IntNZ(dr["DisplayOrder"]);
        //    }
        //    if (dr.Table.Columns.Contains("SheetAppend"))
        //    {
        //        this.SheetAppend = DataTypesUtilities.StringNZ(dr["SheetAppend"]);
        //    }
        //    if (dr.Table.Columns.Contains("CellAppend"))
        //    {
        //        this.CellAppend = DataTypesUtilities.StringNZ(dr["CellAppend"]);
        //    }
        //    if (dr.Table.Columns.Contains("ReportClassName"))
        //    {
        //        this.ReportClassName = DataTypesUtilities.StringNZ(dr["ReportClassName"]);
        //    }
        //}
    }

    public class ReportDataSourceParams
    {
        public int ID { get; set; }
        public int ParamID { get; set; }
        public string ParamName { get; set; }
        public string ParamType { get; set; }
        public int VarcharLen { get; set; }
        public int ReportDatasourceID { get; set; }
        public string ParamValue { get; set; }
        public string SourceType { get; set; }
        public string SourceName { get; set; }

        //public ReportDataSourceParams(DataRow dr)
        //{
        //    if (dr.Table.Columns.Contains("ID"))
        //    {
        //        this.ID = DataTypesUtilities.IntNZ(dr["ID"]);
        //    }
        //    if (dr.Table.Columns.Contains("ParamName"))
        //    {
        //        this.ParamName = DataTypesUtilities.StringNZ(dr["ParamName"]);
        //    }
        //    if (dr.Table.Columns.Contains("ParamType"))
        //    {
        //        this.ParamType = DataTypesUtilities.StringNZ(dr["ParamType"]);
        //    }
        //    if (dr.Table.Columns.Contains("VarcharLen"))
        //    {
        //        this.VarcharLen = DataTypesUtilities.IntNZ(dr["VarcharLen"]);
        //    }
        //    if (dr.Table.Columns.Contains("ReportDatasourceID"))
        //    {
        //        this.ReportDatasourceID = DataTypesUtilities.IntNZ(dr["ReportDatasourceID"]);
        //    }
        //    if (dr.Table.Columns.Contains("ParamValue"))
        //    {
        //        this.ParamValue = DataTypesUtilities.StringNZ(dr["ParamValue"]);
        //    }
        //    if (dr.Table.Columns.Contains("SourceType"))
        //    {
        //        this.SourceType = DataTypesUtilities.StringNZ(dr["SourceType"]);
        //    }
        //    if (dr.Table.Columns.Contains("SourceName"))
        //    {
        //        this.SourceName = DataTypesUtilities.StringNZ(dr["SourceName"]);
        //    }
        //}
    }
}
