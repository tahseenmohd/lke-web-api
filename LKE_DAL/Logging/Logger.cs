using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportal_DAL.Logging
{
    public static class Logger
    {
        private static log4net.ILog Log { get; set; }
        private static readonly ILog m_log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void Write(Exception E)
        {
            m_log.Debug(E);

        }
        public static void WriteDetails(Exception E, string MethodName, string MethodParams, int? UserID)
        {
            string value = MethodName + ' ' + MethodParams + ' ' + UserID.ToString() + ' ' + DateTime.Now;
            m_log.Debug(value, E);

        }
    }
}
