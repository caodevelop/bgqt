using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SensitiveMailService
{
    partial class RemoveSensitiveMailService : ServiceBase
    {
        private Thread main_Thread = null;
        string ip = string.Empty;
        public RemoveSensitiveMailService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            SensitiveMailManager serviceManager = new SensitiveMailManager(ip);
            //Thread.Sleep(12000);
            Log4netHelper.Info($"Thread Start: {DateTime.Now}");
            main_Thread = new Thread(new ThreadStart(serviceManager.RemoveSensitiveMailQueue));
            main_Thread.Start();
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }
    }
}
