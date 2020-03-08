using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendAdminNoticeMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            Guid transactionid = Guid.NewGuid();
            string error = string.Empty;
            try
            {
                SendNoticeMessage manager = new SendNoticeMessage();
                manager.SendMessage(transactionid);

            }
            catch (Exception ex)
            {
                LoggerHelper.Error("SendMessage", "", ex.ToString(), transactionid);
            }
        }
    }
}
