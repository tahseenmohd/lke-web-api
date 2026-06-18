using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Models
{
    public class MakeModel
    {
        public string Make{ get; set; }
        public string Model{ get; set; }
        public int ID{ get; set; }
        private string makeModelValue;
        public string AssetCategory{ get; set; }
        public int ExcludeFromDisplay{ get; set; }
        public int MakeModelID_Display{ get; set; }
        public int MakeModelID_Match{ get; set; }

        public string MakeModelValue
        {
            get
            {
                return Make+"/"+Model;
            }

            set
            {
                makeModelValue = value;
            }
        }
    }
}
