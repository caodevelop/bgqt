using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SensitiveMailService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RemoveSensitiveMailService()
            };
            ServiceBase.Run(ServicesToRun);
            //string ip = string.Empty;
            //SensitiveMailManager serviceManager = new SensitiveMailManager(ip);
            //serviceManager.RemoveSensitiveMailQueue();
        }
    }
}
