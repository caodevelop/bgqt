using Common;
using Entity;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Provider.ExchangeProvider
{
    public class EwsProvider
    {
        public EwsProvider()
        {
            try
            {
                //安全验证证书
                ServicePointManager.ServerCertificateValidationCallback =
                       delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                       {
                           // Replace this line with code to validate server certificate.
                           return true;
                       };
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("ServerCertificateValidationCallback Exception： " + ex.ToString());
            }
        }
        
        public bool SendMeeting(Guid transactionid, MeetingInfo meeting, out ErrorCodeInfo error)
        {
            bool result = true;
            error = new ErrorCodeInfo();

            string paramstr = string.Empty;
            paramstr = $"Sender:{meeting.Sender}";
            paramstr += $"||Subject:{meeting.Subject}";
            paramstr += $"||Body:{meeting.Body}";
            paramstr += $"||Start:{meeting.Start}";
            paramstr += $"||End:{meeting.End}";
            paramstr += $"||Attendees:";
            foreach (string Attendee in meeting.Attendees)
            {
                paramstr += Attendee + "，";
            }
            string funname = "SendMeeting";

            try
            {
                do
                {
                    ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                    service.Credentials = new WebCredentials(ConfigHelper.ConfigInstance["AD_Admin"], ConfigHelper.ConfigInstance["AD_Password"]);
                    service.Url = new Uri(ConfigHelper.ConfigInstance["EWS_Uri"]);
                    service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, meeting.Sender);

                    Appointment appointment = new Appointment(service);
                    // Set the properties on the meeting object to create the meeting.
                    appointment.Subject = meeting.Subject;
                    appointment.Body = meeting.Body;
                    appointment.Start = meeting.Start;
                    appointment.End = meeting.End;
                    appointment.Location = meeting.Location;
                    foreach (string Attendee in meeting.Attendees)
                    {
                        appointment.RequiredAttendees.Add(Attendee);
                    }
                    appointment.ReminderMinutesBeforeStart = 15;
                    appointment.Save(SendInvitationsMode.SendToAllAndSaveCopy);
                    // Verify that the meeting was created.
                    Item item = Item.Bind(service, appointment.Id, new PropertySet(ItemSchema.Subject));

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SendMeeting异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }
            return result;
        }
    }
}
