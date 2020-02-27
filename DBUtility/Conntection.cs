using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtility
{
    public class Conntection
    {
        public static readonly string strConnection = System.Configuration.ConfigurationManager.AppSettings["SQLConnString"];
        public static readonly string strConnection1 = System.Configuration.ConfigurationManager.AppSettings["SQLConnString1"];
    }
}
