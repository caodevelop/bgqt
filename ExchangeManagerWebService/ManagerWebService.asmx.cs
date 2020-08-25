using Common;
using Entity;
using Provider.ExchangeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Services;

namespace ExchangeManagerWebService
{
    /// <summary>
    /// ManagerWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://Betternext.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ManagerWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public bool NewExchangeUser(Guid transactionid,ref byte[] info, out string strError)
        {
            bool result = true;
            strError = string.Empty;
            string paramstr = string.Empty;
            UserInfo user = (UserInfo)JsonHelper.DeserializeObject(info);
            paramstr += $"||LastName:{user.LastName}";
            paramstr += $"||FirstName:{user.FirstName}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||Password:{user.Password}";
            paramstr += $"||ParentDistinguishedName:{user.ParentDistinguishedName}";
            paramstr += $"||SAMAccountName:{user.SAMAccountName}";
            paramstr += $"||AliasName:{user.AliasName}";
            paramstr += $"||NextLoginChangePassword:{user.NextLoginChangePassword}";
            paramstr += $"||DisplayName:{user.DisplayName}";
            paramstr += $"||Database:{user.UserExchange.Database}";

            if (!ExchangeProvider.AddMailbox(ref user, ref strError))
            {
                Log4netHelper.Error("NewExchangeUser异常", paramstr, strError, transactionid);
                result = false;
            }
            if (!ExchangeProvider.EnableMailboxArchive(user.UserAccount, out strError))
            {
                Log4netHelper.Error("EnableMailboxArchive异常", paramstr, strError, transactionid);
                result = false;
            }
            if (!ExchangeProvider.SetMailboxRegionalConfiguration(user.UserAccount, out strError))
            {
                Log4netHelper.Error("SetMailboxRegionalConfiguration异常", paramstr, strError, transactionid);
                result = false;
            }

            info = JsonHelper.SerializeObject(user);
            return result;
        }

        [WebMethod]
        public bool UpdateExchangeUser(Guid transactionid, string userAccount, string displayName,
            string alias,
           byte[] info,
            out string strError)
        {
            bool result = true;
            strError = string.Empty;
            string paramstr = string.Empty;
            ExchangeInfo userExchange = (ExchangeInfo)JsonHelper.DeserializeObject(info);
            paramstr += $"||userAccount:{userAccount}";
            paramstr += $"||displayName:{displayName}";
            paramstr += $"||ExchangeStatus:{userExchange.ExchangeStatus}";
            paramstr += $"||MailSize:{userExchange.MailSize}";

            if (!ExchangeProvider.UpdateExchangeUser(userAccount, displayName, alias, userExchange, out strError))
            {
                Log4netHelper.Error("UpdateExchangeUser异常", paramstr, strError, transactionid);
                result = false;
            }
            return result;
        }

        [WebMethod]
        public bool UpdateUserAlias(Guid transactionid, string userAccount, string alias, out string strError)
        {
            bool result = true;
            strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userAccount:{userAccount}";
            paramstr += $"||alias:{alias}";


            if (!ExchangeProvider.UpdateUserAlias(userAccount, alias, out strError))
            {
                Log4netHelper.Error("UpdateUserAlias异常", paramstr, strError, transactionid);
                result = false;
            }
            return result;
        }

        [WebMethod]
        public bool UpdateUserExchange(Guid transactionid, string userAccount, string displayName, byte[] info, out string strError)
        {
            bool result = true;
            strError = string.Empty;
            string paramstr = string.Empty;
            ExchangeUserInfo userExchange = (ExchangeUserInfo)JsonHelper.DeserializeObject(info);
            paramstr += $"||userAccount:{userAccount}";
            paramstr += $"||displayName:{displayName}";
            paramstr += $"||MailSize:{userExchange.mailSize}";

            if (!ExchangeProvider.UpdateUserExchange(userAccount, displayName, userExchange, out strError))
            {
                Log4netHelper.Error("UpdateUserExchange异常", paramstr, strError, transactionid);
                result = false;
            }
            return result;
        }

        [WebMethod]
        public bool GetUserExchange(Guid transactionid, string userAccount,
          out byte[] info,
            out string strError)
        {
            bool result = true;
            strError = string.Empty;
            ExchangeUserInfo userExchange = new ExchangeUserInfo();

            string paramstr = string.Empty;
            paramstr += $"||userAccount:{userAccount}";
            if (!ExchangeProvider.GetUserExchange(userAccount, out userExchange, out strError))
            {
                //Log4netHelper.Error("GetUserExchange异常", paramstr, strError, transactionid);
                LoggerHelper.Error("GetUserExchange", paramstr, strError, transactionid);
                result = false;
            }

            info = JsonHelper.SerializeObject(userExchange);
            return result;
        }

        [WebMethod]
        public bool GetUser(Guid transactionid, string userID,
          out byte[] info,
            out string strError)
        {
            bool result = true;
            strError = string.Empty;
            UserInfo user = new UserInfo();

            string paramstr = string.Empty;
            paramstr += $"||userID:{userID}";
            if (!ExchangeProvider.GetUser(userID, out user, out strError))
            {
                Log4netHelper.Error("GetUser异常", paramstr, strError, transactionid);
                result = false;
            }
            info = JsonHelper.SerializeObject(user);
            return result;
        }

        [WebMethod]
        public bool DisableMailbox(Guid transactionid, string userID, out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||userID:{userID}";

            //if (!ExchangeProvider.DisableMailbox(userID, out strError))
            //{
            //    Log4netHelper.Error("DisableMailbox异常", paramstr, strError, transactionid);
            //    result = false;
            //}

            return result;
        }

        [WebMethod]
        public bool EnableMailbox(Guid transactionid, string userID, string alias, out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||userID:{userID}";

            if (!ExchangeProvider.EnableMailbox(userID, alias, out strError))
            {
                Log4netHelper.Error("EnableMailbox异常", paramstr, strError, transactionid);
                result = false;
            }

            if (!ExchangeProvider.EnableMailboxArchive(userID, out strError))
            {
                Log4netHelper.Error("EnableMailboxArchive异常", paramstr, strError, transactionid);
                return false;
            }
            if (!ExchangeProvider.SetMailboxRegionalConfiguration(userID, out strError))
            {
                Log4netHelper.Error("SetMailboxRegionalConfiguration异常", paramstr, strError, transactionid);
                result = false;
            }
            return result;
        }

        [WebMethod]
        public bool EnableMailboxAndMailboxDataBase(Guid transactionid, string userID, string alias, string mailboxdb, out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||userID:{userID}";
            paramstr += $"||mailboxdb:{mailboxdb}";

            if (!ExchangeProvider.EnableMailbox(userID, alias, mailboxdb, out strError))
            {
                Log4netHelper.Error("EnableMailbox异常", paramstr, strError, transactionid);
                result = false;
            }

            if (!ExchangeProvider.EnableMailboxArchive(userID, out strError))
            {
                Log4netHelper.Error("EnableMailboxArchive异常", paramstr, strError, transactionid);
                return false;
            }
            if (!ExchangeProvider.SetMailboxRegionalConfiguration(userID, out strError))
            {
                Log4netHelper.Error("SetMailboxRegionalConfiguration异常", paramstr, strError, transactionid);
                result = false;
            }
            return result;
        }

        [WebMethod]
        public bool NewDistributionGroup(Guid transactionid, ref byte[] info, out string strError)
        {
            bool result = true;
            strError = string.Empty;
            GroupInfo group = (GroupInfo)JsonHelper.DeserializeObject(info);
            string paramstr = string.Empty;
            paramstr += $"||displayName:{group.DisplayName}";
            paramstr += $"||account:{group.Account}";
            paramstr += $"||ParentDistinguishedName:{group.ParentDistinguishedName}";

            if (!ExchangeProvider.NewDisGroup(ref group, out strError))
            {
                Log4netHelper.Error("NewDistributionGroup异常", paramstr, strError, transactionid);
                result = false;
            }
            info = JsonHelper.SerializeObject(group);
            return result;
        }

        [WebMethod]
        public bool GetDistributionGroup(Guid transactionid, string groupID, out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||groupID:{groupID}";

            if (!ExchangeProvider.GetDistributionGroup(groupID, out strError))
            {
                Log4netHelper.Error("GetDistributionGroup异常", paramstr, strError, transactionid);
                result = false;
            }

            return result;
        }

        [WebMethod]
        public bool EnableDisGroup(Guid transactionid, string displayName, string account, out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||displayName:{displayName}";
            paramstr += $"||account:{account}";

            if (!ExchangeProvider.EnableDisGroup(displayName, account, out strError))
            {
                Log4netHelper.Error("EnableDisGroup异常", paramstr, strError, transactionid);
                result = false;
            }

            return result;
        }

        [WebMethod]
        public bool SetDistributionGroup(Guid transactionid, string groupID, string displayName, string account, out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||groupID:{groupID}";
            paramstr += $"||displayName:{displayName}";
            paramstr += $"||account:{account}";

            if (!ExchangeProvider.SetDistributionGroup(groupID, displayName, account, out strError))
            {
                Log4netHelper.Error("SetDistributionGroup异常", paramstr, strError, transactionid);
                result = false;
            }

            return result;
        }

        [WebMethod]
        public bool SetDistributionGroupModeratedBy(Guid transactionid, string groupID,
            bool moderationEnabled,
            List<Guid> users,
            out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||groupID:{groupID}";
            paramstr += $"||ModerationEnabled:{moderationEnabled}";


            if (!ExchangeProvider.SetDistributionGroup(groupID, moderationEnabled, users, ref strError))
            {
                Log4netHelper.Error("SetDistributionGroupModeratedBy异常", paramstr, strError, transactionid);
                result = false;
            }

            return result;
        }

        [WebMethod]
        public bool GetDistributionGroupModeratedBy(Guid transactionid, string groupID,
           out List<string> users,
            out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||groupID:{groupID}";

            if (!ExchangeProvider.GetDistributionGroupModeratedBy(groupID, out users, out strError))
            {
                Log4netHelper.Error("GetDistributionGroupModeratedBy异常", paramstr, strError, transactionid);
                result = false;
            }

            return result;
        }

        [WebMethod]
        public bool GetDatabaseList(Guid transactionid, out byte[] info, out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            List<MailDataBaseInfo> list = new List<MailDataBaseInfo>();
            if (!ExchangeProvider.GetDatabaseList(out list, out strError))
            {
                Log4netHelper.Error("GetDatabaseList异常", paramstr, strError, transactionid);
                result = false;
            }
            info = JsonHelper.SerializeObject(list);
            return result;
        }

        [WebMethod]
        public bool RemoveSensitiveMail(Guid transactionid, string userMail, string keywords, DateTime startTime, DateTime endTime, out string resultmessage, out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;

            if (!ExchangeProvider.RemoveSensitiveMail(userMail, keywords, startTime, endTime, out resultmessage, out strError))
            {
                Log4netHelper.Error("RemoveSensitiveMail异常", paramstr, strError, transactionid);
                result = false;
            }

            return result;
        }

        [WebMethod]
        public bool CreateMailContact(Guid transactionid,
            string name,
            string displayname,
            string mail,
            string organizationalUnit,
            out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||name:{name}";
            paramstr += $"||displayName:{displayname}";
            paramstr += $"||mail:{mail}";
            paramstr += $"||organizationalUnit:{organizationalUnit}";

            if (!ExchangeProvider.AddMailContact(name, mail, displayname, organizationalUnit, string.Empty, ref strError))
            {
                Log4netHelper.Error("CreateMailContact异常", paramstr, strError, transactionid);
                result = false;
            }

            return result;
        }

        [WebMethod]
        public bool UpdateMailContact(Guid transactionid,
            string name,
            string displayname,
            string mail,
            string organizationalUnit,
            out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||name:{name}";
            paramstr += $"||displayName:{displayname}";
            paramstr += $"||mail:{mail}";
            paramstr += $"||organizationalUnit:{organizationalUnit}";

            if (!ExchangeProvider.SetMailContact(name, organizationalUnit, displayname,new List<string>(),false, mail,  ref strError))
            {
                Log4netHelper.Error("UpdateMailContact异常", paramstr, strError, transactionid);
                result = false;
            }

            return result;
        }

        [WebMethod]
        public bool RemoveContact(Guid transactionid,
           string name,
           string organizationalUnit,
           out string strError)
        {
            bool result = true;
            strError = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"||name:{name}";
            paramstr += $"||organizationalUnit:{organizationalUnit}";

            if (!ExchangeProvider.RemoveContact(name, out strError))
            {
                Log4netHelper.Error("RemoveContact异常", paramstr, strError, transactionid);
                result = false;
            }

            return result;
        }

        #region 邮件数量
        [WebMethod(Description = "获取系统邮件数量")]
        public bool GetServerMailCount(DateTime starTime, DateTime endTime, out int sendCount, out int receiveCount, out string strError)
        {
            strError = string.Empty;
            sendCount = 0;
            receiveCount = 0;

            if (!ExchangeProvider.GetServerMailCount(starTime, endTime, out sendCount, out receiveCount, out strError))
            {
                return false;
            }
            return true;
        }

        [WebMethod(Description = "获取账户邮件数量")]
        public bool GetUserMailCount(DateTime starTime, DateTime endTime, string mail, out int sendCount, out int receiveCount, out string strError)
        {
            strError = string.Empty;
            sendCount = 0;
            receiveCount = 0;

            if (!ExchangeProvider.GetUserMailCount(starTime, endTime, mail, out sendCount, out receiveCount, out strError))
            {
                return false;
            }
            return true;
        }

        #endregion

    }
}
