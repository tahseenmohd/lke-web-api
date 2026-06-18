using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportal_DAL.Utilities
{
    public interface IConnectionFactory
    {
        IDbConnection GetConnection();
    }
}
