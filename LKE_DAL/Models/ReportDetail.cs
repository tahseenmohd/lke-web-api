using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Models
{
    public class ReportDetail
    {
        public int ReportListID { get; set; }
        public string LinkText{ get; set; }
        public string URL{ get; set; }
        public string Description{ get; set; }
        public int ReportCategoryID{ get; set; }
        public int DisplayOrder{ get; set; }
        public int DocumentType{ get; set; }
        public int DepType { get; set; }
        public int Archive { get; set; }
        public string ExcelTemplateName{ get; set; }
        public int ExcelTemplateID{ get; set; }
        public int CustomSql { get; set; }
        public int Active{ get; set; }
        public int ProcessID{ get; set; }
        public int ViewOnScreen { get; set; }
        public string OutputName { get; set; }
        public string CategoryDisplayText { get; set; }
        public int CategoryDisplayOrder { get; set; }
        public string CategoryDesc { get; set; }
        public int Display { get; set; }
    }
}
