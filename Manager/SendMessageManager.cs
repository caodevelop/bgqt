using Common;
using Entity;
using Newtonsoft.Json;
using Provider.ExchangeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class SendMessageManager
    {
        private string _clientip = string.Empty;
        public SendMessageManager(string ip)
        {
            _clientip = ip;
        }

        public bool SendMeeting(Guid transactionid, AdminInfo admin, MeetingInfo meeting, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            string Attendees = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"Sender:{meeting.Sender}";
            paramstr += $"||Subject:{meeting.Subject}";
            paramstr += $"||Body:{meeting.Body}";
            paramstr += $"||Start:{meeting.Start}";
            paramstr += $"||End:{meeting.End}";
            paramstr += $"||Attendees:";
            foreach (string Attendee in meeting.Attendees)
            {
                paramstr += Attendee + "，";
                Attendees += Attendee + "，";
            }
            string funname = "SendMeeting";

            try
            {
                do
                {
                    error = meeting.SendCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    EwsProvider provider = new EwsProvider();
                    if (!provider.SendMeeting(transactionid, meeting, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "发送会议";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now} 发送会议。" +
                        $"发件人 ：{meeting.Sender}，" +
                        $"主题 ：{meeting.Subject}，" +
                        $"地点 ：{meeting.Location}，" +
                        $"开始时间 ：{meeting.Start}，" +
                        $"结束时间 ：{meeting.End}，" +
                        $"参与人：{Attendees} " +
                        $"内容：{meeting.Body}";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("SendMessageManager调用SendMeeting异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
    }
}
