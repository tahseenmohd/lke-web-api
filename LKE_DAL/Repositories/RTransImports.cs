using Exportal_DAL.Logging;
using Exportal_DAL.Utilities;
using Ionic.Zip;
using LKE_DAL.Models;
using LKE_DAL.Utilities;
using LKEWebAPI.Utilities;
using Microsoft.WindowsAzure.Storage.Blob;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Repositories
{
    public class RTransImports : RepositoryBase
    {
        private IConnectionFactory dbFactory;
        private List<SqlParameter> parameters;

        public RTransImports(IConnectionFactory factory)
        {
            dbFactory = factory;
        }
        
        public string createReport(string templateFilePath, string pdfsDirectoryPath, List<ReportDataSourceModel> rdsList, string DirectoryToCreateFile, string reportName, Dictionary<int?, string> reportParams, string timestamp, string dbName, bool IsRecreation = false, string Istypechange = "first")
        {
            string directoryPathToCreateFile = pdfsDirectoryPath + DirectoryToCreateFile;
            string taxbookName = "";

            if (!Directory.Exists(pdfsDirectoryPath))
            {
                Directory.CreateDirectory(pdfsDirectoryPath);
            }

            if (!Directory.Exists(directoryPathToCreateFile))
            {
                Directory.CreateDirectory(directoryPathToCreateFile);
            }

            FileInfo templateFile = new FileInfo(templateFilePath);
            if (rdsList.Count > 0)
            {
                taxbookName = reportParams[0];
            }

            string newfilePath = "";

            if (IsRecreation)
            {
                newfilePath = directoryPathToCreateFile + "\\" + templateFilePath;
            }
            else
            {
                newfilePath = directoryPathToCreateFile + "\\" + reportName + "-" + taxbookName + "-" + timestamp + ".xlsx";
            }

            FileInfo newFile = new FileInfo(newfilePath);




            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                //executing the stored procedures
                foreach (ReportDataSourceModel rds in rdsList)
                {

                    //cost or price columns
                    int[] priceColumnsList = null;
                    int[] dateColumnsList = null;
                    int[] booleanColumnsList = null;

                    switch (rds.ReportID)
                    {
                        case 1:
                            priceColumnsList = new int[2] { 2, 3 };
                            dateColumnsList = new int[0] { };
                            booleanColumnsList = new int[0] { };
                            break;
                        case 2:
                            priceColumnsList = new int[1] { 5 };
                            dateColumnsList = new int[1] { 4 };
                            booleanColumnsList = new int[0] { };
                            break;
                        case 3:
                            priceColumnsList = new int[2] { 5, 6 };
                            dateColumnsList = new int[2] { 4, 7 };
                            booleanColumnsList = new int[0] { };
                            break;
                        case 4:
                            priceColumnsList = new int[7] { 9, 10, 12, 13, 14, 15, 16 };
                            dateColumnsList = new int[3] { 7, 8, 11 };
                            booleanColumnsList = new int[1] { 5 };
                            break;
                        case 6:
                            priceColumnsList = new int[9] { 7, 8, 10, 11, 12, 13, 14, 15, 16 };
                            dateColumnsList = new int[3] { 5, 6, 9 };
                            booleanColumnsList = new int[0] { };
                            break;
                        case 7:
                            priceColumnsList = new int[0] { };
                            dateColumnsList = new int[0] { };
                            booleanColumnsList = new int[0] { };
                            break;
                        case 8:
                            priceColumnsList = new int[2] { 4, 5 };
                            dateColumnsList = new int[4] { 6, 7, 8, 9 };
                            booleanColumnsList = new int[0] { };
                            break;
                        case 10:
                            priceColumnsList = new int[2] { 8, 9 };
                            dateColumnsList = new int[2] { 6, 7 };
                            booleanColumnsList = new int[1] { 10 };
                            break;
                        case 12:
                            priceColumnsList = new int[0] { };
                            dateColumnsList = new int[0] { };
                            booleanColumnsList = new int[0] { };
                            break;
                        case 17:
                            priceColumnsList = new int[6] { 5, 9, 14, 15, 16, 17 };
                            dateColumnsList = new int[3] { 6, 7, 10 };
                            booleanColumnsList = new int[1] { 8 };
                            break;

                    }



                    //parameter values for the specific report
                    switch (rds.ReportID)
                    {
                        case 17:
                            reportParams.Add(8, rds.TableName);
                            break;

                        case 3:
                            reportParams.Add(81, rds.TableName);
                            break;
                    }

                    string spName = rds.DatasourceName;
                    var parameterSP = new List<SqlParameter>();
                    foreach (ReportDataSourceParams parameter in rds.ReportDatasourceParams)
                    {

                        if ((parameter.ParamName == "@StartDate" || parameter.ParamName == "@EndDate") && Istypechange == "first")// && (parameter.ParamType!="DateTime"))
                        {
                            SqlParameter param = new SqlParameter(parameter.ParamName, SqlDbType.DateTime);
                            param.Value = DateTime.ParseExact(reportParams[parameter.ParamID], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            parameterSP.Add(param);
                        }
                        else if ((parameter.ParamName == "@StartDate" || parameter.ParamName == "@EndDate") && Istypechange == "second")// && (parameter.ParamType!="DateTime"))
                        {
                            SqlParameter param = new SqlParameter(parameter.ParamName, SqlDbType.DateTime);
                            param.Value = Convert.ToDateTime(reportParams[parameter.ParamID]);
                            parameterSP.Add(param);
                        }
                        else
                        {
                            parameterSP.Add(new SqlParameter(parameter.ParamName, reportParams[parameter.ParamID]));
                        }


                        //parameterSP.Add(new SqlParameter(parameter.ParamName, reportParams[parameter.ParamID]));



                    }
                    DataSet dsResponseTemp = null;

                    dsResponseTemp = base.ReturnDataSet(dbFactory, spName, parameterSP);
                    // dsResponseTemp = base.ReturnDataSetTenmp(spName, parameterSP);
                    //this is the response on executing each stored procedure.
                    //add this response to excel file sheet

                    // Openning first Worksheet of the template file i.e. 'Sample1.xlsx'
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[rds.SheetAppend];

                    if (rds.ReportID == 7)
                    {
                        //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("LKEExchanges");
                        int excelRowStart = 10;
                        int excelColumnStart = 1;

                        char[] alphabets = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                        'U','V','W','X','Y','Z'};

                        int[] relinquishedTablePriceCols = { 6, 8, 10, 11, 12, 13, 14, 15 };
                        int[] relinquishedTableDateCols = { 3, 4, 5 };
                        int[] relinquishedTablePercentCols = { 7, 9 };

                        int[] replaceTablePriceCols = { 6, 8, 10, 11, 12 };
                        int[] replaceTableDateCols = { 3 };
                        int[] replaceTablePercentCols = { 7 };
                        int[] replaceTableTotalLabelCols = { 5 };
                        int[] replaceTableBoolCols = { 4 };
                        //arrange tables data in key value pair
                        if (dsResponseTemp.Tables.Count == 2)
                        {
                            DataTable relinquishDT = dsResponseTemp.Tables[0].Clone();

                            DataTable replacementDT = dsResponseTemp.Tables[1].Clone();
                            worksheet.Cells["A1"].Value = dbName;
                            worksheet.Cells["A3"].Value = "Like Kind Exchanges Report(" + taxbookName + ")";
                            worksheet.Cells["A6"].Value = "Tax Year Ending :" + reportParams[64];


                            DataTable transacIDDT = dsResponseTemp.Tables[0].DefaultView.ToTable(true, "LKETransID");



                            int count = 1;
                            // Dictionary<int, List<DataTable>> transactionDic = new Dictionary<int, List<DataTable>>();
                            foreach (DataRow row in transacIDDT.Rows)
                            {

                                relinquishDT.Clear();
                                replacementDT.Clear();

                                int transID = int.Parse(row["LKETransID"].ToString());
                                // relinquishDT.ImportRow(row);
                                relinquishDT = (from myRow in dsResponseTemp.Tables[0].AsEnumerable()
                                                where myRow.Field<int>("LKETransID") == transID
                                                select myRow).CopyToDataTable();
                                //removing unwanted columns
                                relinquishDT.Columns.Remove("LKETransID");



                                DataView dvRDT = relinquishDT.DefaultView;
                                dvRDT.Sort = "Acquisition Date asc";
                                relinquishDT = dvRDT.ToTable();
                                replacementDT = (from myRow in dsResponseTemp.Tables[1].AsEnumerable()
                                                 where myRow.Field<int>("LKETransID") == transID
                                                 select myRow).CopyToDataTable();

                                //removing unwanted columns
                                replacementDT.Columns.Remove("LKETransID");
                                replacementDT.Columns["BlankValue"].ColumnName = " ";

                                replacementDT.Columns["BlankValue1"].ColumnName = "  ";

                                worksheet.Cells[excelRowStart, excelColumnStart].Value = "Exchange Transaction: " + count;
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart].Style.Font.SetFromFont(new Font("Tahoma", 11));
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart].Style.Font.Bold = true;
                                excelRowStart++;

                                worksheet.Cells[excelRowStart, excelColumnStart].Value = "Relinquished Assets";
                                //applying font size
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart].Style.Font.SetFromFont(new Font("Tahoma", 11));
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart].Style.Font.Bold = true;
                                //applying color
                                worksheet.Cells[excelRowStart, 1, excelRowStart + 1, relinquishDT.Columns.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                worksheet.Cells[excelRowStart, 1, excelRowStart + 1, relinquishDT.Columns.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                                worksheet.Cells[excelRowStart, 1, excelRowStart + 1, relinquishDT.Columns.Count].Style.Font.Color.SetColor(Color.White);

                                for (int i = 0; i < relinquishedTablePriceCols.Length; i++)
                                {
                                    worksheet.Cells[excelRowStart + 2, relinquishedTablePriceCols[i], excelRowStart + 1 + relinquishDT.Rows.Count + 1, relinquishedTablePriceCols[i]].Style.Numberformat.Format = "$#,##0.00;($#,##0.00)";
                                    worksheet.Cells[excelRowStart + 2, relinquishedTablePriceCols[i], excelRowStart + 1 + relinquishDT.Rows.Count + 1, relinquishedTablePriceCols[i]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                }
                                //[$-en-US,101]#,##0.00;-#,##0.00
                                for (int i = 0; i < relinquishedTablePercentCols.Length; i++)
                                {
                                    worksheet.Cells[excelRowStart + 2, relinquishedTablePercentCols[i], excelRowStart + 1 + relinquishDT.Rows.Count + 1, relinquishedTablePercentCols[i]].Style.Numberformat.Format = "#,##0.00;-#,##0.00";
                                    worksheet.Cells[excelRowStart + 2, relinquishedTablePercentCols[i], excelRowStart + 1 + relinquishDT.Rows.Count + 1, relinquishedTablePercentCols[i]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                }

                                for (int i = 0; i < relinquishedTableDateCols.Length; i++)
                                {
                                    worksheet.Cells[excelRowStart + 2, relinquishedTableDateCols[i], excelRowStart + 1 + relinquishDT.Rows.Count + 1, relinquishedTableDateCols[i]].Style.Numberformat.Format = "m/d/yyyy";
                                    worksheet.Cells[excelRowStart + 2, relinquishedTableDateCols[i], excelRowStart + 1 + relinquishDT.Rows.Count + 1, relinquishedTableDateCols[i]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                }

                                excelRowStart++;

                                worksheet.Cells[excelRowStart, excelColumnStart].LoadFromDataTable(relinquishDT, true);
                                worksheet.Cells[excelRowStart, 9].Value = "%";
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart + relinquishDT.Columns.Count].Style.Font.SetFromFont(new Font("Tahoma", 8));
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart + relinquishDT.Columns.Count].Style.Font.Bold = true;
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart + relinquishDT.Columns.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[excelRowStart + 1, excelColumnStart, excelRowStart + 1 + relinquishDT.Rows.Count, excelColumnStart + relinquishDT.Columns.Count].Style.Font.SetFromFont(new Font("Tahoma", 6));
                                worksheet.Cells[excelRowStart + 1 + relinquishDT.Rows.Count, excelColumnStart, excelRowStart + 1 + relinquishDT.Rows.Count, excelColumnStart + relinquishDT.Columns.Count].Style.Font.Bold = true;

                                bool firstColumn = true;
                                foreach (int columnPriceIndex in relinquishedTablePriceCols)
                                {
                                    int rowIDStartIndex = excelRowStart + 1;
                                    int rowIDEndIndex = excelRowStart + relinquishDT.Rows.Count;
                                    int cellIndex = excelRowStart + 1 + relinquishDT.Rows.Count;
                                    char columnChar = alphabets[columnPriceIndex - 1];
                                    if (firstColumn)
                                    {
                                        worksheet.Cells[cellIndex, columnPriceIndex - 1].Value = "Total";
                                        firstColumn = false;
                                    }
                                    worksheet.Cells[cellIndex, columnPriceIndex].Formula = "=SUM(" + columnChar + rowIDStartIndex + ":" + columnChar + rowIDEndIndex + ")";
                                }
                                foreach (int colPercentIndex in relinquishedTablePercentCols)
                                {
                                    int rowIDStartIndex = excelRowStart + 1;
                                    int rowIDEndIndex = excelRowStart + relinquishDT.Rows.Count;
                                    int cellIndex = excelRowStart + 1 + relinquishDT.Rows.Count;
                                    char columnChar = alphabets[colPercentIndex - 1];
                                    worksheet.Cells[cellIndex, colPercentIndex].Formula = "=SUM(" + columnChar + rowIDStartIndex + ":" + columnChar + rowIDEndIndex + ")";

                                }

                                //inducing some space after one table
                                excelRowStart = excelRowStart + relinquishDT.Rows.Count + 4;

                                worksheet.Cells[excelRowStart, excelColumnStart].Value = "Replacement Assets";
                                //applying font size
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart].Style.Font.SetFromFont(new Font("Tahoma", 11));
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart].Style.Font.Bold = true;
                                //applying color
                                worksheet.Cells[excelRowStart, 1, excelRowStart + 1, replacementDT.Columns.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                worksheet.Cells[excelRowStart, 1, excelRowStart + 1, replacementDT.Columns.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                                worksheet.Cells[excelRowStart, 1, excelRowStart + 1, replacementDT.Columns.Count].Style.Font.Color.SetColor(Color.White);


                                for (int i = 0; i < replaceTablePriceCols.Length; i++)
                                {
                                    worksheet.Cells[excelRowStart + 2, replaceTablePriceCols[i], excelRowStart + 1 + replacementDT.Rows.Count + 1, replaceTablePriceCols[i]].Style.Numberformat.Format = "$#,##0.00;($#,##0.00)";
                                    worksheet.Cells[excelRowStart + 2, replaceTablePriceCols[i], excelRowStart + 1 + replacementDT.Rows.Count + 1, replaceTablePriceCols[i]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                }

                                for (int i = 0; i < replaceTablePercentCols.Length; i++)
                                {
                                    worksheet.Cells[excelRowStart + 2, replaceTablePercentCols[i], excelRowStart + 1 + replacementDT.Rows.Count + 1, replaceTablePercentCols[i]].Style.Numberformat.Format = "#,##0.00;-#,##0.00";
                                    worksheet.Cells[excelRowStart + 2, replaceTablePercentCols[i], excelRowStart + 1 + replacementDT.Rows.Count + 1, replaceTablePercentCols[i]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                }

                                for (int i = 0; i < replaceTableDateCols.Length; i++)
                                {
                                    worksheet.Cells[excelRowStart + 2, replaceTableDateCols[i], excelRowStart + 1 + replacementDT.Rows.Count + 1, replaceTableDateCols[i]].Style.Numberformat.Format = "m/d/yyyy";
                                    worksheet.Cells[excelRowStart + 2, replaceTableDateCols[i], excelRowStart + 1 + replacementDT.Rows.Count + 1, replaceTableDateCols[i]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                }

                                for (int i = 0; i < replaceTableBoolCols.Length; i++)
                                {
                                    worksheet.Cells[excelRowStart + 2, replaceTableBoolCols[i], excelRowStart + 1 + replacementDT.Rows.Count + 1, replaceTableBoolCols[i]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                }

                                for (int i = 0; i < replaceTableTotalLabelCols.Length; i++)
                                {
                                    worksheet.Cells[excelRowStart + 2, replaceTableTotalLabelCols[i], excelRowStart + 1 + replacementDT.Rows.Count + 1, replaceTableTotalLabelCols[i]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                }

                                excelRowStart++;

                                worksheet.Cells[excelRowStart, excelColumnStart].LoadFromDataTable(replacementDT, true);

                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart + replacementDT.Columns.Count].Style.Font.SetFromFont(new Font("Tahoma", 8));
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart + replacementDT.Columns.Count].Style.Font.Bold = true;
                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, excelColumnStart + replacementDT.Columns.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[excelRowStart + 1, excelColumnStart, excelRowStart + 1 + replacementDT.Rows.Count, excelColumnStart + replacementDT.Columns.Count].Style.Font.SetFromFont(new Font("Tahoma", 6));
                                worksheet.Cells[excelRowStart + 1 + replacementDT.Rows.Count, excelColumnStart, excelRowStart + 1 + replacementDT.Rows.Count, excelColumnStart + replacementDT.Columns.Count].Style.Font.Bold = true;

                                firstColumn = true;
                                foreach (int columnPriceIndex in replaceTablePriceCols)
                                {
                                    int rowIDStartIndex = excelRowStart + 1;
                                    int rowIDEndIndex = excelRowStart + replacementDT.Rows.Count;
                                    int cellIndex = excelRowStart + 1 + replacementDT.Rows.Count;
                                    char columnChar = alphabets[columnPriceIndex - 1];
                                    if (firstColumn)
                                    {
                                        worksheet.Cells[cellIndex, columnPriceIndex - 1].Value = "Total";
                                        firstColumn = false;
                                    }
                                    worksheet.Cells[cellIndex, columnPriceIndex].Formula = "=SUM(" + columnChar + rowIDStartIndex + ":" + columnChar + rowIDEndIndex + ")";
                                }
                                foreach (int percentColIndex in replaceTablePercentCols)
                                {
                                    int rowIDStartIndex = excelRowStart + 1;
                                    int rowIDEndIndex = excelRowStart + replacementDT.Rows.Count;
                                    int cellIndex = excelRowStart + 1 + replacementDT.Rows.Count;
                                    char columnChar = alphabets[percentColIndex - 1];

                                    worksheet.Cells[cellIndex, percentColIndex].Formula = "=SUM(" + columnChar + rowIDStartIndex + ":" + columnChar + rowIDEndIndex + ")";
                                }
                                excelRowStart = excelRowStart + replacementDT.Rows.Count + 2;
                                count++;




                            }
                        }
                        worksheet.Cells.AutoFitColumns();
                        worksheet.Column(3).Width = 11.14;
                        worksheet.Column(4).Width = 11.14;
                        worksheet.Column(5).Width = 11.14;
                        worksheet.Column(10).Width = 14;
                        worksheet.Column(10).Width = 14;
                        worksheet.Column(11).Width = 14;
                        worksheet.Column(12).Width = 14;
                        worksheet.Column(13).Width = 14;
                        worksheet.Column(14).Width = 14;
                        worksheet.Column(15).Width = 14;
                        worksheet.Column(7).Width = 6.14;
                        worksheet.Column(9).Width = 6.14;
                        worksheet.Column(3).Style.WrapText = true;
                        worksheet.Column(4).Style.WrapText = true;
                        worksheet.Column(5).Style.WrapText = true;
                        worksheet.Column(10).Style.WrapText = true;
                        worksheet.Column(10).Style.WrapText = true;
                        worksheet.Column(11).Style.WrapText = true;
                        worksheet.Column(12).Style.WrapText = true;
                        worksheet.Column(13).Style.WrapText = true;
                        worksheet.Column(14).Style.WrapText = true;
                        worksheet.Column(15).Style.WrapText = true;
                    }
                    else if (rds.ReportID == 8)
                    {
                        // ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Completed45DayIDReport");
                        int excelRowStart = 7;
                        int excelColumnStart = 1;

                        //arrange tables data in key value pair
                        if (dsResponseTemp.Tables.Count == 2)
                        {
                            DataTable parentDT = dsResponseTemp.Tables[0].Clone();

                            DataTable childDT = dsResponseTemp.Tables[1].Clone();

                            worksheet.Cells["A1"].Value = dbName;
                            worksheet.Cells["A2"].Value = "Completed 45 Day ID Report (" + taxbookName + ")";
                            worksheet.Cells["A4"].Value = "For the Period: " + reportParams[104];

                            //worksheet.Cells[2, 1].Value = "WTP EXCHANGE";
                            //worksheet.Cells[2, 1, 2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 20));
                            //worksheet.Cells[excelRowStart, 1, excelRowStart + 1, parentDT.Columns.Count].Style.Font.Color.SetColor(Color.DarkBlue);

                            //worksheet.Cells[3, 1].Value = "Quantitative Excellence";
                            //worksheet.Cells[3, 1, 3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                            //worksheet.Cells[3, 1, 3, 1].Style.Font.Color.SetColor(Color.LightBlue);

                            //worksheet.Cells[5, 1].Value = "Completed 45 Day ID Report";
                            //worksheet.Cells[5, 1, 5, 1].Style.Font.SetFromFont(new Font("Times New Roman", 17));
                            //worksheet.Cells[5, 1, 5, 1].Style.Font.Color.SetColor(Color.DarkBlue);
                            int parentColumnCount = dsResponseTemp.Tables[0].Columns.Count + 1;
                            int childColCount = dsResponseTemp.Tables[1].Columns.Count;
                            worksheet.Cells[7, 1, 7, parentColumnCount + childColCount].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[7, 1, 7, parentColumnCount + childColCount].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                            worksheet.Cells[7, 1, 7, parentColumnCount + childColCount].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[7, 1, 7, parentColumnCount + childColCount].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                            worksheet.Cells[7, 1, 7, parentColumnCount + childColCount].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[7, 1, 7, parentColumnCount + childColCount].Style.Font.SetFromFont(new Font("Tahoma", 8));
                            worksheet.Cells[7, 1, 7, parentColumnCount + childColCount].Style.Font.Bold = true;

                            int count = 1;

                            // Dictionary<int, List<DataTable>> transactionDic = new Dictionary<int, List<DataTable>>();
                            foreach (DataRow row in dsResponseTemp.Tables[0].Rows)
                            {
                                try
                                {
                                    parentDT.Clear();
                                    childDT.Clear();

                                    string assetID = row["Asset ID"].ToString();
                                    parentDT.ImportRow(row);
                                    childDT = (from myRow in dsResponseTemp.Tables[1].AsEnumerable()
                                               where myRow.Field<string>("Asset ID") == assetID
                                               select myRow).CopyToDataTable();

                                    childDT.Columns.Remove("Asset ID");
                                    if (count == 1)
                                    {
                                        if (parentDT.Rows.Count > 0)
                                        {
                                            worksheet.Cells[excelRowStart, excelColumnStart].LoadFromDataTable(parentDT, true);
                                            if (childDT.Rows.Count > 0)
                                            {
                                                worksheet.Cells[excelRowStart, parentColumnCount].LoadFromDataTable(childDT, true);
                                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, parentColumnCount + childDT.Columns.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, parentColumnCount + childDT.Columns.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                                                worksheet.Cells[excelRowStart, excelColumnStart, excelRowStart, parentColumnCount + childDT.Columns.Count].Style.Font.Color.SetColor(Color.White);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (parentDT.Rows.Count > 0)
                                        {
                                            worksheet.Cells[excelRowStart, excelColumnStart].LoadFromDataTable(parentDT, false);
                                            if (childDT.Rows.Count > 0)
                                            {
                                                worksheet.Cells[excelRowStart, parentColumnCount].LoadFromDataTable(childDT, false);

                                            }
                                        }
                                    }
                                    if (childDT.Rows.Count > 0)
                                    {
                                        excelRowStart += (childDT.Rows.Count);
                                    }
                                    else
                                    {
                                        excelRowStart += 1;
                                    }
                                    count++;

                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            worksheet.Cells[8, 1, 8 + dsResponseTemp.Tables[0].Rows.Count + dsResponseTemp.Tables[1].Rows.Count, parentColumnCount + childColCount].Style.Font.SetFromFont(new Font("Tahoma", 8));
                        }
                        worksheet.Cells.AutoFitColumns();
                        //APPLY formatting to columns of price
                        if (priceColumnsList != null)
                        {
                            for (int i = 0; i < priceColumnsList.Length; i++)
                            {
                                worksheet.Column(priceColumnsList[i]).Style.Numberformat.Format = "$#,##0.00;($#,##0.00)";
                                worksheet.Column(priceColumnsList[i]).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            }
                        }

                        //APPLY formatting to columns of date
                        if (dateColumnsList != null)
                        {
                            for (int i = 0; i < dateColumnsList.Length; i++)
                            {
                                worksheet.Column(dateColumnsList[i]).Style.Numberformat.Format = "m/d/yyyy";
                                worksheet.Column(dateColumnsList[i]).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            }
                        }
                    }
                    else
                    {
                        /*  1	Sale/Purchase Activity Summary Report      2
                           2	Gain Recognized Report                     4
                           3	45-Day ID Deadline Report                  10
                           4	Sale Transaction Report                    5
                           6	Sales of Business Assets                   7
                           7	Like Kind Exchanges Report                 13
                           8	Completed 45-Day ID Deadline Report        11
                           10	Purchase Transaction Report                8
                           12	Make / Model Report                        9
                           17	Tax Depreciation Report                    1                                            
                           */

                        //placing dbname and taxbook and tax year in excel file

                        //if(taxbookName == "US_Tax")
                        //{
                        //    taxbookName = "US TAX";
                        //}


                        switch (rds.ReportID)
                        {
                            case 1: //2
                                worksheet.Cells["A1"].Value = dbName;
                                worksheet.Cells["A3"].Value = "For the Period: " + reportParams[9] + " - " + reportParams[10];
                                break;
                            case 2: //4
                                worksheet.Cells["A1"].Value = dbName;
                                worksheet.Cells["A2"].Value = "Gain Recognized Report  (" + taxbookName + ")";
                                worksheet.Cells["A3"].Value = "For the Period: " + reportParams[22] + " - " + reportParams[23];
                                break;
                            case 3:   //10
                                worksheet.Cells["A1"].Value = dbName;
                                worksheet.Cells["A3"].Value = "For the Period: " + reportParams[79] + " - " + reportParams[80];
                                break;
                            case 4:  //5
                                worksheet.Cells["A1"].Value = dbName;
                                worksheet.Cells["A2"].Value = "Sale Transactions Report  (" + taxbookName + ")";
                                worksheet.Cells["A3"].Value = "For the Period: " + reportParams[31] + " - " + reportParams[32];
                                break;
                            case 6:  //7
                                worksheet.Cells["A1"].Value = dbName;
                                worksheet.Cells["A2"].Value = "Sales of Business Assets Report  (" + taxbookName + ")";
                                worksheet.Cells["A3"].Value = "For the Period: " + reportParams[63] + " - " + reportParams[64];
                                break;
                            case 10: //8
                                worksheet.Cells["A1"].Value = dbName;
                                worksheet.Cells["A2"].Value = "Purchase Transactions Report  (" + taxbookName + ")";
                                worksheet.Cells["A3"].Value = "For the Period: " + reportParams[69] + " - " + reportParams[70];
                                break;
                            case 12:  //9
                                worksheet.Cells["A1"].Value = dbName;
                                //worksheet.Cells["A3"].Value = "For the Period Ended";
                                break;
                            case 17:   ///1
                                worksheet.Cells["A1"].Value = dbName;
                                worksheet.Cells["A2"].Value = "Depreciation Report: " + taxbookName;
                                worksheet.Cells["A3"].Value = "For the Period Ended: " + reportParams[64];
                                break;
                        }


                        // Get locations of column names inside excel:
                        DataTable selectedDataTable = dsResponseTemp.Tables[0];


                        //if (rds.ReportID == 17)
                        //{
                        //    //applying order by
                        //    DataView viewFI = new DataView(selectedDataTable);
                        //    viewFI.Sort = "[Parent Asset ID], [Asset ID], [Asset Type], [Date in Service]"; //rds.SortVariables
                        //    selectedDataTable = null;
                        //    selectedDataTable = viewFI.ToTable();
                        //}
                        ExcelRangeBase range = null;
                        if (rds.ReportID == 17 || rds.ReportID == 6 || rds.ReportID == 4 || rds.ReportID == 10)
                        {
                            range = worksheet.Cells[rds.CellAppend].LoadFromDataTable(selectedDataTable, true);
                        }
                        else
                        {
                            range = worksheet.Cells[rds.CellAppend].LoadFromDataTable(selectedDataTable, false);
                        }

                        if (range != null)
                        {
                            //range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Font.SetFromFont(new Font("Tahoma", 8));
                        }

                        //APPLY formatting to columns of price
                        if (priceColumnsList != null)
                        {
                            for (int i = 0; i < priceColumnsList.Length; i++)
                            {
                                worksheet.Column(priceColumnsList[i]).Style.Numberformat.Format = "$#,##0.00;($#,##0.00)";
                                worksheet.Column(priceColumnsList[i]).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            }
                        }

                        //APPLY formatting to columns of price
                        if (booleanColumnsList != null)
                        {
                            for (int i = 0; i < booleanColumnsList.Length; i++)
                            {
                                //worksheet.Column(booleanColumnsList[i]).Style.Numberformat.Format = "$#,##0.00;($#,##0.00)";
                                worksheet.Column(booleanColumnsList[i]).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            }
                        }

                        //APPLY formatting to columns of date
                        if (dateColumnsList != null)
                        {
                            for (int i = 0; i < dateColumnsList.Length; i++)
                            {
                                worksheet.Column(dateColumnsList[i]).Style.Numberformat.Format = "m/d/yyyy";
                                worksheet.Column(dateColumnsList[i]).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            }
                        }

                        if (rds.ReportID == 17)
                        {
                            //changing header style
                            //applying color
                            worksheet.Cells[10, 1, 10, selectedDataTable.Columns.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[10, 1, 10, selectedDataTable.Columns.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                            worksheet.Cells[10, 1, 10, selectedDataTable.Columns.Count].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[10, 1, 10, selectedDataTable.Columns.Count].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

                            worksheet.Cells[10, 1, 10, selectedDataTable.Columns.Count].Style.Font.SetFromFont(new Font("Tahoma", 8));
                            worksheet.Cells[10, 1, 10, selectedDataTable.Columns.Count].Style.Font.Bold = true;
                        }
                        else if (rds.ReportID == 6 || rds.ReportID == 4 || rds.ReportID == 10)
                        {
                            //changing header style
                            //applying color
                            worksheet.Cells[7, 1, 7, selectedDataTable.Columns.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[7, 1, 7, selectedDataTable.Columns.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                            worksheet.Cells[7, 1, 7, selectedDataTable.Columns.Count].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[7, 1, 7, selectedDataTable.Columns.Count].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                            worksheet.Cells[7, 1, 7, selectedDataTable.Columns.Count].Style.Font.SetFromFont(new Font("Tahoma", 8));
                            worksheet.Cells[7, 1, 7, selectedDataTable.Columns.Count].Style.Font.Bold = true;
                        }

                        //worksheet.Cells.AutoFitColumns();
                    }


                    /*     1	Sale/Purchase Activity Summary Report      2
                           2	Gain Recognized Report                     4
                           3	45-Day ID Deadline Report                  10
                           4	Sale Transaction Report                    5
                           6	Sales of Business Assets                   7
                           7	Like Kind Exchanges Report                 13
                           8	Completed 45-Day ID Deadline Report        11
                           10	Purchase Transaction Report                8
                           12	Make / Model Report                        9
                           17	Tax Depreciation Report                    1                                            
                           */
                    //10 4 17 7
                    switch (rds.ReportID)
                    {
                        case 1:
                            worksheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            break;
                        case 2:
                            worksheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            break;
                        case 3:
                            worksheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            break;
                        case 4:
                            worksheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            break;
                        case 6:
                            worksheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            break;
                        case 8:
                            worksheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            break;
                        case 10:
                            worksheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            break;
                        case 12:
                            worksheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            break;
                        case 17:
                            worksheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            break;
                    }



                    //string cell = rds.CellAppend;
                    //int startIndex = cell.IndexOfAny("0123456789".ToCharArray());
                    //string column = cell.Substring(0, startIndex);
                    //int rowStart = Int32.Parse(cell.Substring(startIndex));
                    //int columnStart = GetColumnNumber(column);

                    //for (int i = rowStart, l = 0; l < 12152; i++, l++)
                    //{
                    //    for (int j = columnStart, m = 0; m < dsResponseTemp.Tables[0].Columns.Count; j++, m++)
                    //    {
                    //        worksheet.Cells[i, j].Value = dsResponseTemp.Tables[0].Rows[l][m];
                    //        worksheet.Cells[i, j].Style.HorizontalAlignment = worksheet.Cells[i - 1, j].Style.HorizontalAlignment;
                    //        worksheet.Cells[i, j].Style.VerticalAlignment = worksheet.Cells[i - 1, j].Style.VerticalAlignment;

                    //        worksheet.Cells[i, j].Style.Font.Size = worksheet.Cells[i - 1, j].Style.Font.Size;
                    //        worksheet.Cells[i, j].Style.Font.Family = worksheet.Cells[i - 1, j].Style.Font.Family;
                    //        worksheet.Cells[i, j].Style.Font.Name = worksheet.Cells[i - 1, j].Style.Font.Name;
                    //    }
                    //}
                }

                package.Save();

            }
            return newfilePath;
        }

        public void callIS_1127b_45DayID_spConfirmWTPID_User(string username, int rowID)
        {
            try
            {

                string storedProcName = "dbo.IS_1127b_45DayID_spConfirmWTPID_User";
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@ROWID", rowID));
                parameters.Add(new SqlParameter("@UserName", username));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                base.ReturnDataSP(conn, storedProcName, parameters);
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void callIS_1127a_45DayID_spMakeID_WTP(int rowID)
        {
            try
            {

                string storedProcName = "dbo.IS_1127a_45DayID_spMakeID_WTP";
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@ROWID", rowID));

                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                base.ReturnDataSP(conn, storedProcName, parameters);
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public List<AllAdminUsers> lockUnlockUsers()
        {
            throw new NotImplementedException();
        }

        public ReportsArchive getReportArchiveIfAvailable(ReportModel reportObj, Report report, List<ReportDataSourceModel> reportDataSourceList, ArrayList categoryOneList, ArrayList categoryTwoList, ArrayList categoryThreeList)
        {
            ReportsArchive archiveObj = null;

            string whereCondition = "";

            String StartDate = "", EndDate = "", Dep_Type = "";
            switch (report.ReportID)
            {
                case 1:
                    StartDate = report.reportParams[9]; EndDate = report.reportParams[10];
                    break;
                case 8:
                    StartDate = report.reportParams[103]; EndDate = report.reportParams[104];
                    break;
                case 2:
                    StartDate = report.reportParams[22]; EndDate = report.reportParams[23];
                    break;
                case 3:
                    StartDate = report.reportParams[79]; EndDate = report.reportParams[80];
                    break;
                case 17:
                    StartDate = report.reportParams[63]; EndDate = report.reportParams[64]; Dep_Type = report.reportParams[6];
                    break;
                case 7:
                    StartDate = report.reportParams[63]; EndDate = report.reportParams[64]; Dep_Type = report.reportParams[86];
                    break;
                case 6:
                    StartDate = report.reportParams[63]; EndDate = report.reportParams[64]; Dep_Type = report.reportParams[68];
                    break;
                case 10:
                    StartDate = report.reportParams[69]; EndDate = report.reportParams[70]; Dep_Type = report.reportParams[75];
                    break;
                case 4:
                    StartDate = report.reportParams[31]; EndDate = report.reportParams[32]; Dep_Type = report.reportParams[36];
                    break;
            }
            if (categoryOneList.Contains(report.ReportID))
            {

                if (report.ReportID == 12)
                {
                    whereCondition = "";
                }
                else
                {
                    whereCondition = " ra.StartDate = '" + StartDate + "' AND ra.EndDate = '" + EndDate + "'";
                }
            }
            else if (categoryTwoList.Contains(report.ReportID))
            {
                whereCondition = " ra.StartDate ='" + StartDate + "' AND ra.EndDate = '" + EndDate + "' AND ra.Dep_Type = '" + Dep_Type + "'";
            }
            else if (categoryThreeList.Contains(report.ReportID))
            {
                whereCondition = " ra.StartDate = '" + StartDate + "' AND ra.EndDate = '" + EndDate + "' AND ra.Dep_Type = '" + Dep_Type + "'";
            }
            if (whereCondition.Trim().Length > 0)
                whereCondition += " AND ra.ReportID = " + report.ReportID + /*" AND ra.Username = '" + reportObj.username+ */" AND uat.[Database] = '" + reportObj.DB_Name + "'";
            else
                whereCondition += " ra.ReportID = " + report.ReportID + /*" AND ra.Username = '" + reportObj.username+ */" AND uat.[Database] = '" + reportObj.DB_Name + "'";

            try
            {

                string selectReportDSQuery = "select top(1) ra.*,uat.* from [dbo].[ReportsArchive] ra JOIN [dbo].[UserActionTrack] uat on ra.UserActionID = uat.Id where " + whereCondition + " order by ra.Id desc";
                var parameters = new List<SqlParameter>();
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                archiveObj = DBExtension.SingleOrDefault<ReportsArchive>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));
                conn.Close();
            }
            catch (Exception ex)
            {

            }
            //DateTime startDate = null;
            //DateTime endDate = null;

            //report.ReportID;
            //reportObj.username;
            //reportObj.DB_Name;
            //start date
            //end date
            //dep type


            //context.ReportsArchives.Where



            return archiveObj;
        }

        public _45DayIDReplacementAsset getMakeModeIDData(int makeModeID)
        {
            _45DayIDReplacementAsset makeModObj = null;
            try
            {

                string selectReportDSQuery = "select * from [dbo].[IS_1125_45DayID_ReplacementAssets] where MakeModID=" + makeModeID;
                var parameters = new List<SqlParameter>();
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                makeModObj = DBExtension.SingleOrDefault<_45DayIDReplacementAsset>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));
                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return makeModObj;
        }

        public List<MakeModel> getMakeModelList(string assetCategory)
        {
            List<MakeModel> makeModelList = new List<MakeModel>();

            try
            {
                string selectReportDSQuery = "select * from Assets_MakeModels where ExcludeFromDisplay=0 and AssetCategory='" + assetCategory + "'";
                var parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("@tableName", tableName));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                makeModelList = DBExtension.ToList<MakeModel>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));

                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return makeModelList;
        }



        public List<string> RefreshData(string deptype, string yearid, string Yearid1, string RunYearand1, string RunMatch, string ManualMatches, string AlternateMatch)
        {
            List<string> RoutineOutputList = new List<string>();
            try
            {

                var spname = "sp_IS_Reports_RefreshReportData";
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@depType", deptype));
                parameters.Add(new SqlParameter("@YeariD", yearid));
                parameters.Add(new SqlParameter("@Yearid1", Yearid1));
                parameters.Add(new SqlParameter("@RunMatch", RunMatch));
                parameters.Add(new SqlParameter("@ManualMatches", ManualMatches));
                parameters.Add(new SqlParameter("@RunYearAnd1", RunYearand1));
                parameters.Add(new SqlParameter("@alternatematch", AlternateMatch));
                DataSet dsresponse = null;

                dsresponse = base.ReturnDataSet(dbFactory, spname, parameters);
                if (dsresponse != null || dsresponse.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in dsresponse.Tables[0].Rows)
                    {
                        RoutineOutputList.Add(dr[1].ToString());
                    }
                }

            }
            catch (Exception e)
            {

            }
            return RoutineOutputList;

        }



        //getYearsList
        public List<YearModel> getYearsList()
        {
            List<YearModel> yearsList = new List<YearModel>();

            try
            {
                string selectReportDSQuery = "SELECT Year as display,ID as value,StartDate,EndDate from Year where DisplayDD=1 order by Year desc";
                var parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("@tableName", tableName));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                yearsList = DBExtension.ToList<YearModel>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));

                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return yearsList;
        }


        public List<NameValue> getDepreciationTypes()
        {
            List<NameValue> depTypes = new List<NameValue>();

            try
            {

                string selectReportDSQuery = "SELECT DispName as display,DeprDescAbbrev as value from Depreciation_Methods where (Active = 1) and DispName IS NOT NULL order by ID";
                var parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("@tableName", tableName));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                depTypes = DBExtension.ToList<NameValue>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));

                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return depTypes;
        }


        public List<ReportDetail> getReportsList()
        {
            List<ReportDetail> reportList = new List<ReportDetail>();

            try
            {
                string selectReportDSQuery = "SELECT a.*,b.DisplayText as CategoryDisplayText,b.DisplayOrder as CategoryDisplayOrder,b.Description as CategoryDesc from ReportList a JOIN ReportCategory b on a.ReportCategoryID = b.ReportCategoryID";
                var parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("@tableName", tableName));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                reportList = DBExtension.ToList<ReportDetail>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));

                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return reportList;
        }

        public static int GetColumnNumber(string name)
        {
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }

            return number;
        }


        public List<ReportDataSourceModel> getReportDataSourcesWithParams(int reportID)
        {
            List<ReportDataSourceModel> reportDataSourceList = new List<ReportDataSourceModel>();
            string tableName = null;
            // DataSet dsResponseTemp = null;

            //SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
            //conn.Open();
            //reportList = DBExtension.ToList<ReportDetail>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));

            //conn.Close();

            try
            {
                tableName = "ReportDatasource";
                string selectReportDSQuery = "select * from " + tableName + " where ReportID=" + reportID;
                var parameters = new List<SqlParameter>();

                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                reportDataSourceList = DBExtension.ToList<ReportDataSourceModel>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));
                conn.Close();
                foreach (ReportDataSourceModel rds in reportDataSourceList)
                {
                    rds.ReportDatasourceParams = getParamsForDataSource(rds.ID);
                }

            }
            catch (Exception ex)
            {

            }
            return reportDataSourceList;
        }






        public EntityInformation getEntityData()
        {
            EntityInformation entityData = new EntityInformation();
            string tableName = null;
            //DataSet dsResponseTemp = null;

            try
            {
                tableName = "ReportDatasourceParams";
                string selectReportDSQuery = "SELECT a.*, e.IdentifyingNumber as EntityName from (SELECT * FROM dbo.EntityAnnualInformation WITH(NOLOCK) WHERE YearID IN(SELECT ID FROM dbo.YEAR WITH(NOLOCK) WHERE LockStatus <> 1)) a inner join Entity e on a.EntityID=e.ID";
                var parameters = new List<SqlParameter>();
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                entityData = DBExtension.SingleOrDefault<EntityInformation>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));
                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return entityData;
        }

        public List<_45DayIDDeadlineModel> get45DayIDDeadlines(int LEID, int caseID1, int caseID2)
        {
            List<_45DayIDDeadlineModel> paramsList = new List<_45DayIDDeadlineModel>();
            string tableName = null;
            //DataSet dsResponseTemp = null;

            try
            {

                string selectReportDSQuery = "select * from [dbo].[IS_1100_HomePage_Upcoming45DaysIDs]  where LEID=" + LEID + " AND CaseID=" + caseID1 + " AND CaseID2=" + caseID2 + " Order By  AssetID";
                var parameters = new List<SqlParameter>();
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                paramsList = DBExtension.ToList<_45DayIDDeadlineModel>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));
                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return paramsList;
        }

        public List<_45DayIDRecentlySubmitted> getRecentlySubmitted45DayID(int LEID, int caseID1, int caseID2)
        {
            List<_45DayIDRecentlySubmitted> paramsList = new List<_45DayIDRecentlySubmitted>();
            try
            {

                string selectReportDSQuery = "select * from [dbo].[IS_1120_45DayID_RecentlySubmitted]  where LEID=" + LEID + " AND CaseID=" + caseID1 + " AND CaseID2=" + caseID2 + " Order By  AssetID";
                var parameters = new List<SqlParameter>();
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                paramsList = DBExtension.ToList<_45DayIDRecentlySubmitted>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));
                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return paramsList;
        }

        public List<_45DayIDReplacementAsset> getReplacementAssets()
        {
            List<_45DayIDReplacementAsset> paramsList = new List<_45DayIDReplacementAsset>();
            try
            {

                string selectReportDSQuery = "select * from [dbo].[IS_1125_45DayID_ReplacementAssets]";
                var parameters = new List<SqlParameter>();
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                paramsList = DBExtension.ToList<_45DayIDReplacementAsset>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));
                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return paramsList;
        }

        private ICollection<ReportDataSourceParams> getParamsForDataSource(int? reportDSId)
        {
            List<ReportDataSourceParams> paramsList = new List<ReportDataSourceParams>();
            string tableName = null;
            //DataSet dsResponseTemp = null;

            try
            {
                tableName = "ReportDatasourceParams";
                string selectReportDSQuery = "select * from " + tableName + " where ReportDatasourceID=" + reportDSId;
                var parameters = new List<SqlParameter>();
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                paramsList = DBExtension.ToList<ReportDataSourceParams>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));
                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return paramsList;
        }

        public List<SalePurchaseActivity> getSalePurchaseActivityList(int LEID, int caseID1, int caseID2)
        {
            List<SalePurchaseActivity> paramsList = new List<SalePurchaseActivity>();
            string tableName = null;
            //DataSet dsResponseTemp = null;

            try
            {

                string selectReportDSQuery = "select * from [dbo].[IS_1105_HomePage_SalesPurchases180DaysPivot] where LEID=" + LEID + " AND CaseID=" + caseID1 + " AND CaseID2=" + caseID2 + " order by AssetCategory";
                var parameters = new List<SqlParameter>();
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                paramsList = DBExtension.ToList<SalePurchaseActivity>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));
                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return paramsList;
        }

        public void callIS_1128_45DayID_spMakeID_ApdAssignments(int quantity, int rowID, float purchasePrice, int makeModID)
        {
            try
            {

                string storedProcName = "dbo.IS_1128_45DayID_spMakeID_ApdAssignments";
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@ROWID", rowID));
                parameters.Add(new SqlParameter("@Quantity", quantity));
                parameters.Add(new SqlParameter("@PurchasePrice", purchasePrice));
                parameters.Add(new SqlParameter("@MakeModID", makeModID));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                base.ReturnDataSP(conn, storedProcName, parameters);
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void callIS_1127_45DayID_spMakeID(string username, int rowID)
        {
            try
            {

                string storedProcName = "dbo.IS_1127_45DayID_spMakeID";
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@ROWID", rowID));
                parameters.Add(new SqlParameter("@UserName", username));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                base.ReturnDataSP(conn, storedProcName, parameters);
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void callIS_1126_45DayID_spRevokeID(int rowID)
        {
            try
            {

                string storedProcName = "dbo.IS_1126_45DayID_spRevokeID";
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@ROWID", rowID));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                base.ReturnDataSP(conn, storedProcName, parameters);
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }


        public void saveSignature(int userID, string username, string signature, string description)
        {
            try
            {

                string storedProcName = "dbo.pWTPLKELogSignature";
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@UserID", userID));
                parameters.Add(new SqlParameter("@UserName", username));
                parameters.Add(new SqlParameter("@Signature", signature));
                parameters.Add(new SqlParameter("@Description", description));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                base.ReturnDataSP(conn, storedProcName, parameters);
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void SetLoginHistory(int userID, int webDBId)
        {
            try
            {

                string storedProcName = "dbo.pLKESecurityLogInHistoryAdd";
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@p_webuserID", userID));
                parameters.Add(new SqlParameter("@p_webuserDbaseID", webDBId));
                //parameters.Add(new SqlParameter("@Signature", signature));
                //parameters.ad("@NewId", SqlDbType.Int).Direction = ParameterDirection.Output;
               // parameters.Add(new SqlParameter("@Description", description));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                base.ReturnDataSP(conn, storedProcName, parameters);
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public List<AllAdminUsers> adminAllUsers()
        {
            List<AllAdminUsers> paramsList = new List<AllAdminUsers>();
            try
            {

                string storedProcName = "dbo.GetAdminAllUsers";
                var parameters = new List<SqlParameter>();

                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                paramsList = DBExtension.ToList<AllAdminUsers>(base.ReturnDataSP(conn, storedProcName, parameters));
                conn.Close();

            }
            catch (Exception ex)
            {

            }
            return paramsList;
        }

        public void uploadFileToBlobServer(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                MemoryStream ms = new MemoryStream();
                fs.CopyTo(ms);
                // Get Blob Container

                CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference("documents");
                container.CreateIfNotExists();

                // Get reference to blob (binary content)
                CloudBlockBlob blockBlob = container.GetBlockBlobReference("template.xlsx");

                // set its properties
                blockBlob.Properties.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                blockBlob.Metadata["filename"] = fs.Name;
                blockBlob.Metadata["filemime"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // Get stream from file bytes
                Stream stream = new MemoryStream(ms.ToArray());


                // Async upload of stream to Storage
                AsyncCallback UploadCompleted = new AsyncCallback(OnUploadCompleted);
                blockBlob.BeginUploadFromStream(stream, UploadCompleted, blockBlob);
            }
        }
        public void uploadFileToBlobServer(string filePath, string containerName, string fileName)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                MemoryStream ms = new MemoryStream();
                fs.CopyTo(ms);
                // Get Blob Container

                CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference(containerName);
                container.CreateIfNotExists();

                // Get reference to blob (binary content)
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                // set its properties
                blockBlob.Properties.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                blockBlob.Metadata["filename"] = fs.Name;
                blockBlob.Metadata["filemime"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // Get stream from file bytes
                Stream stream = new MemoryStream(ms.ToArray());


                // Async upload of stream to Storage
                AsyncCallback UploadCompleted = new AsyncCallback(OnUploadCompleted);
                blockBlob.BeginUploadFromStream(stream, UploadCompleted, blockBlob);
            }
        }
        private void OnUploadCompleted(IAsyncResult result)
        {
            CloudBlockBlob blob = (CloudBlockBlob)result.AsyncState;
            blob.SetMetadata();
            blob.EndUploadFromStream(result);
        }

        public async Task<byte[]> DownloadFileFromBlob(string FileName)
        {
            // Get Blob Container
            CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference("documents");

            // Get reference to blob (binary content)
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileName);

            // Read content
            using (MemoryStream ms = new MemoryStream())
            {
                blockBlob.DownloadToStream(ms);
                return ms.ToArray();
            }
        }

        public async Task<byte[]> DownloadFileFromBlob(string blobName, string containerName)
        {
            // Get Blob Container
            CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference(containerName);

            // Get reference to blob (binary content)
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            // Read content
            using (MemoryStream ms = new MemoryStream())
            {
                blockBlob.DownloadToStream(ms);
                return ms.ToArray();
            }
        }

        // PENDING IDENTIFICATIONS

        public List<PendingIdentifications> getPendingIdentificationsMain()
        {

            List<PendingIdentifications> PendingIdentificationsList = new List<PendingIdentifications>();

            try
            {
                string selectReportDSQuery = "select * from [dbo].[IS_1127_45DayID_WTPIDentifiedDetail_Main]";
                var parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("@tableName", tableName));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                PendingIdentificationsList = DBExtension.ToList<PendingIdentifications>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));

                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return PendingIdentificationsList;
        }
        // SUB GRID
        public List<PendingIdentifications> getPendingIdentificationsSub()
        {

            List<PendingIdentifications> PendingIdentificationsList = new List<PendingIdentifications>();

            try
            {
                string selectReportDSQuery = "select [Asset Identified] as AssetIdentified,[Quantity],[FMV],[IDLink],[AssetCategory],[PurPriceEst] from [dbo].[IS_1128_45DayID_WTPIdentifiedDetail_Sub]";
                var parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("@tableName", tableName));
                SqlConnection conn = (SqlConnection)dbFactory.GetConnection();
                conn.Open();
                PendingIdentificationsList = DBExtension.ToList<PendingIdentifications>(base.ReturnDataSetQuery(conn, selectReportDSQuery, parameters));

                conn.Close();
            }
            catch (Exception ex)
            {

            }
            return PendingIdentificationsList;
        }

    }


}