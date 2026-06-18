using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportal_DAL
{
    public partial class ChartDetail
    {
        [NotMapped]
        public string  AccountNoAndDescp { get; set; }
    }
}
