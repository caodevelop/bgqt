using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncADControl
{
    class Program
    {
        static void Main(string[] args)
        {
            string error = string.Empty;
            try
            {
                SyncADManager sync = new SyncADManager();
                sync.Load(out error);
                //test0228
                if (string.IsNullOrEmpty(error))
                {
                    Log4netHelper.Info("Sync AD Control Result Succeeded" );
                }
                else
                {
                    Log4netHelper.Error("Sync AD Control Result：" + error);
                }
               
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("Sync AD Control Exception：" + ex.ToString());
                Console.WriteLine(error);
            }
        }
    }
}
