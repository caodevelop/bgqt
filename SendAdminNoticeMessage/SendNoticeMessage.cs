using Common;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendAdminNoticeMessage
{
    public class SendNoticeMessage
    {
        public bool SendMessage(Guid transactionid)
        {
            bool result = true;
            List<UserInfo> nlist = new List<UserInfo>();
            List<UserInfo> llist = new List<UserInfo>();

            string error = string.Empty;
            string paramstr = string.Empty;

            string funname = "SendNoticeMessage";

            try
            {
                do
                {
                    DBProvider bProvider = new DBProvider();
                    if (!bProvider.GetSendMessageUsers(transactionid, out nlist, out llist, out error))
                    {
                        LoggerHelper.Info("SendNoticeMessage", funname, paramstr, error, false, transactionid);
                        result = false;
                        break;
                    }

                    string subject = ConfigHelper.ConfigInstance["NoticeSubject"];
                    string sender = ConfigHelper.ConfigInstance["NoticeSender"];
                    string receiver = ConfigHelper.ConfigInstance["NoticeReceiver"];
                    string body = DateTime.Now.ToShortDateString() + " 入职员工数量为：" + nlist.Count + "，离职且AD中未禁用员工数量为：" + llist.Count;
                    string mailbody = string.Empty;
                    BuildingEntryAndLeaveSystemMailBody(body, nlist, llist, out mailbody);
                    SendNoticeMail(transactionid, subject, sender, receiver, mailbody);

                    LoggerHelper.Info("SendNoticeMessage", funname, paramstr, "发送员工变更通知邮件成功", true, transactionid);
                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Info("SendNoticeMessage", funname, paramstr, ex.ToString(), false, transactionid);
                result = false;
            }
            return result;
        }

        private void BuildingEntryAndLeaveSystemMailBody(string body, List<UserInfo> nlist, List<UserInfo> llist, out string mailbody)
        {
            mailbody = string.Empty;
            mailbody = "<span>" + body + "</span><br>";
            if (nlist.Count > 0)
            {
                mailbody += "<span>入职员工如下：</span>";
                mailbody += "<table><tr><td style='width:100px;'>员工号</td><td style='width:120px;'>姓名</td><td style='width:120px;'>手机号码</td><td style='width:200px;'>职位</td><td style='width:200px;'>部门</td><td style='width:200px;'>公司</td><td style='width:200px;'>入职时间</td></tr>";
                foreach (var m in nlist)
                {
                    mailbody += "<tr><td>" + m.EMPLID + "</td><td>" + m.NAME + "</td><td>" + m.PHONE1 + "</td><td>" + m.POSN_DESCR + "</td><td>" + m.DEPT_DESCR + "</td><td>" + m.HPS_WORK_COMP_DESC + "</td><td>" + m.Entry_DT + "</td></tr>";
                }
                mailbody += "</table><br>";
            }
            if (llist.Count > 0)
            {
                mailbody += "<span>离职且AD中未禁用员工如下：</span>";
                mailbody += "<table><tr><td style='width:100px;'>员工号</td><td style='width:120px;'>姓名</td><td style='width:120px;'>手机号码</td><td style='width:200px;'>职位</td><td style='width:200px;'>部门</td><td style='width:200px;'>公司</td><td style='width:200px;'>离职时间</td></tr>";
                foreach (var m in llist)
                {
                    mailbody += "<tr><td>" + m.EMPLID + "</td><td>" + m.NAME + "</td><td>" + m.PHONE1 + "</td><td>" + m.POSN_DESCR + "</td><td>" + m.DEPT_DESCR + "</td><td>" + m.HPS_WORK_COMP_DESC + "</td><td>" + m.TERMINATION_DT + "</td></tr>";
                }
                mailbody += "</table>";
            }
        }

        public bool SendNoticeMail(Guid transactionid, string subject, string sender, string receiver, string mailbody)
        {
            bool result = true;

            string paramstr = string.Empty;
            paramstr = $"Sender:{sender}";
            paramstr += $"||Subject:{subject}";
            paramstr += $"||Body:{mailbody}";
            paramstr += $"||Receiver:{receiver}";

            string funname = "SendNoticeMail";

            try
            {
                do
                {
                    ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                    service.Credentials = new WebCredentials(ConfigHelper.ConfigInstance["AD_Admin"], ConfigHelper.ConfigInstance["AD_Password"]);
                    service.Url = new Uri(ConfigHelper.ConfigInstance["EWS_Uri"]);
                    service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, sender);

                    EmailMessage mail = new EmailMessage(service);
                    // Set the properties on the meeting object to create the meeting.
                    mail.Subject = subject;
                    mail.Body = new MessageBody();
                    mail.Body.Text = mailbody;
                    mail.Body.BodyType = BodyType.HTML;

                    mail.From = sender;
                    List<string> receives = receiver.Split(';').ToList<string>();
                    if (receives != null && receives.Count > 0)
                    {
                        foreach (var m in receives)
                        {
                            if (!string.IsNullOrWhiteSpace(m))
                            {
                                EmailAddress email = new EmailAddress(m, m);
                                mail.ToRecipients.Add(email);
                            }
                        }
                    }
                    mail.Send();
                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"{funname}异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }
            return result;
        }
    }
}
