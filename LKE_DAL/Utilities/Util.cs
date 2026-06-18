using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Exportal_DAL.Utilities
{
   
    public class Util
    {
        public static Dictionary<string, object> RowToDictionary<T>(T source)
        {
            var dict = new Dictionary<string, object>();

            var dr = source as DataRow;
            if (dr != null)
            {
                var cols = dr.Table.Columns;
                foreach (DataColumn col in cols)
                {
                    var name = col.ColumnName;
                    var value = dr[name];
                    if (value != null &&
                        !Convert.IsDBNull(value))
                    {
                        dict.Add(name, value);
                    }
                }
            }
            return dict;
        }

        public static List<Dictionary<string, object>> TableToList<T>(T source)
        {
            var list = new List<Dictionary<string, object>>();
            var dt = source as DataTable;
            if (dt != null)
            {
                var cols = dt.Columns;
                foreach (DataRow dr in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in cols)
                    {
                        var name = col.ColumnName;
                        var value = dr[name];
                        if (value != null &&
                            !Convert.IsDBNull(value))
                        {
                            dict.Add(name, value);
                        }
                        else
                        {
                            dict.Add(name, "NULL");
                        }
                    }
                    list.Add(dict);
                }
            }

            return list;
        }

    }
}
    