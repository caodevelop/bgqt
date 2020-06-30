using Entity;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provider.DBProvider;
using Provider.ADProvider;
using Newtonsoft.Json;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using System.Threading;

namespace Manager
{
    public class UserManager
    {
        private string _clientip = string.Empty;
        public UserManager(string ip)
        {
            _clientip = ip;
        }

        public bool UserLogin(Guid transactionid, string account, string password, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += "useraccount:" + account;
            paramstr += "||password:" + password;
            string funname = "UserLogin";

            try
            {
                do
                {
                    AdminInfo info = new AdminInfo();
                    UserProvider adProvider = new UserProvider();
                    if (!adProvider.GetLoginUser(transactionid, account, password, out info, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    RoleDBProvider Provider = new RoleDBProvider();
                    if (!Provider.GetUserRole(transactionid, info.UserID, ref info, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!info.ParamList.ContainsValue(RoleParamCode.Login))
                    {
                        error.Code = ErrorCode.UserNotLoginRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    info.Token = TokenManager.GenToken(info.UserID);
                    string json = JsonConvert.SerializeObject(info);
                    LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = info.UserID;
                    operateLog.AdminAccount = info.UserAccount;
                    operateLog.RoleID = info.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "管理员登录";
                    operateLog.OperateLog = info.UserAccount + "于" + DateTime.Now + "登录平台";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用UserLogin异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ChangeUserPassword(Guid transactionid, string account, string oldpassword, string newpassword, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += "useraccount:" + account;
            paramstr += "||oldpassword:" + oldpassword;
            paramstr += "||newpassword:" + newpassword;
            string funname = "ChangeUserPassword";

            try
            {
                do
                {
                    if (!MatchPassword(newpassword))
                    {
                        error.Code = ErrorCode.PasswordIllegal;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserInfo user = new UserInfo();
                    UserProvider adProvider = new UserProvider();
                    if (!adProvider.GetUserByAccountPassword(transactionid, account, oldpassword, out user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!adProvider.ChangeUserPassword(transactionid, user, oldpassword, newpassword, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = Guid.Empty;
                    operateLog.AdminAccount = account;
                    operateLog.RoleID = Guid.Empty;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "修改密码";
                    operateLog.OperateLog = $"{account}于{DateTime.Now}修改密码。";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(account, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用ChangeUserPassword异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetValCode(Guid transactionid, out string code, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            code = string.Empty;

            string funname = "GetValCode";
            try
            {
                do
                {
                    CheckCodeHelper cch = new CheckCodeHelper();
                    code = cch.RndNum(4);
                    result = true;
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                //LoggerHelper.Info(orgID, operaterID, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用GetValCode异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetValImg(Guid transactionid, string code, out byte[] byteArr, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            byteArr = null;

            string funname = "GetValImg";
            try
            {
                do
                {
                    CheckCodeHelper cch = new CheckCodeHelper();
                    cch.CreateCheckCodeImage(code, 78, 30, 12, out byteArr);
                    result = true;
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                //LoggerHelper.Info(orgID, operaterID, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用GetValImg异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool validateToken(Guid transactionid, string userAccount, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||userAccount:{userAccount}";

            string funname = "validateToken";

            try
            {
                do
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    if (string.IsNullOrEmpty(userAccount))
                    {
                        dictionary.Add("error", "账户为空");
                        dictionary.Add("data", false);
                        strJsonResult = JsonConvert.SerializeObject(dictionary);
                        result = false;
                        break;
                    }

                    CommonProvider commonProvider = new CommonProvider();
                    List<DirectoryEntry> items = new List<DirectoryEntry>();

                    if (!commonProvider.SearchEntryData(ConfigADProvider.GetRootADPath(), userAccount, SearchType.MailUserByMailAddress, out items, out message))
                    {
                        dictionary.Add("error", message);
                        dictionary.Add("data", false);
                        strJsonResult = JsonConvert.SerializeObject(dictionary);
                        LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    if (items.Count > 0)
                    {
                        dictionary.Add("error", "");
                        dictionary.Add("data", true);
                        strJsonResult = JsonConvert.SerializeObject(dictionary);
                        LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                    else
                    {
                        dictionary.Add("error", "账户不存在。");
                        dictionary.Add("data", false);
                        strJsonResult = JsonConvert.SerializeObject(dictionary);
                        LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用validateToken异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool searchADObject(Guid transactionid, string searchkey, string DistinguishedName, int pagesize, int pageindex, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||DistinguishedName:{DistinguishedName}";
            string funname = "searchADObject";
            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    DirectoryEntry groupEntry = new DirectoryEntry();
                    HabListInfo list = new HabListInfo();
                    if (string.IsNullOrEmpty(DistinguishedName))
                    {
                        List<DirectoryEntry> items = new List<DirectoryEntry>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), searchkey, SearchType.MailUser, out items, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        list.pageIndex = pageindex;
                        list.pageSize = pagesize;
                        list.pageCount = (items.Count + pagesize - 1) / pagesize;
                        List<HabAddressUserInfo> users = new List<HabAddressUserInfo>();
                        foreach (DirectoryEntry entry in items)
                        {
                            HabAddressUserInfo info = new HabAddressUserInfo();
                            info.Title = Convert.ToString(entry.Properties["Title"].Value);
                            info.Manager = Convert.ToString(entry.Properties["Manager"].Value);
                            info.Department = Convert.ToString(entry.Properties["Department"].Value);
                            info.SubMembers = null;
                            info.TelePhoneNumber = Convert.ToString(entry.Properties["TelePhoneNumber"].Value);
                            info.Fax = Convert.ToString(entry.Properties["facsimileTelephoneNumber"].Value);
                            info.Mobile = Convert.ToString(entry.Properties["Mobile"].Value);
                            info.UserPrincipalName = Convert.ToString(entry.Properties["UserPrincipalName"].Value);
                            info.UserAccountControl = UserProvider.IsAccountActive(Convert.ToInt32(entry.Properties["userAccountControl"][0]));
                            info.OfficeName = Convert.ToString(entry.Properties["physicalDeliveryOfficeName"].Value);
                            info.SN = Convert.ToString(entry.Properties["SN"].Value);
                            info.GivenName = Convert.ToString(entry.Properties["GivenName"].Value);
                            info.Mail = Convert.ToString(entry.Properties["Mail"].Value);
                            info.MailNickname = Convert.ToString(entry.Properties["MailNickname"].Value);
                            info.MSExchArchiveQuota = 0; //Convert.ToInt32(entry.Properties["MSExchArchiveQuota"].Value);
                            info.MSExchArchiveWarnQuota = 0;// Convert.ToInt32(entry.Properties["MSExchArchiveWarnQuota"].Value);
                            info.MSExchCalendarLoggingQuota = 0;// Convert.ToInt32(entry.Properties["MSExchCalendarLoggingQuota"].Value);
                            info.MSExchDumpsterQuota = 0; //Convert.ToInt32(entry.Properties["MSExchDumpsterQuota"].Value);
                            info.MSExchDumpsterWarningQuota = 0; //Convert.ToInt32(entry.Properties["MSExchDumpsterWarningQuota"].Value);
                            info.HomePage = Convert.ToString(entry.Properties["wWWHomePage"].Value);
                            info.l = Convert.ToString(entry.Properties["l"].Value);
                            info.St = Convert.ToString(entry.Properties["st"].Value);
                            info.StreetAddress = Convert.ToString(entry.Properties["streetAddress"].Value);
                            info.ObjectGUID = entry.Guid.ToString();
                            info.Name = Convert.ToString(entry.Properties["Name"].Value);
                            info.DisplayName = Convert.ToString(entry.Properties["DisplayName"].Value);
                            info.DistinguishedName = Convert.ToString(entry.Properties["DistinguishedName"].Value);
                            info.Company = Convert.ToString(entry.Properties["Company"].Value);
                            info.CN = Convert.ToString(entry.Properties["CN"].Value);
                            info.ObjectClass = "user";
                            info.SAMAccountName = Convert.ToString(entry.Properties["SAMAccountName"].Value);
                            for (int i = 0; i < entry.Properties["proxyAddresses"].Count; i++)
                            {
                                info.ProxyAddress.Add(Convert.ToString(entry.Properties["proxyAddresses"][i]));
                            }
                            info.SmtpEmail = Convert.ToString(entry.Properties["Mail"].Value);
                            info.SipEmail = null;
                            info.MSExchHideFromAddressLists = false;
                            info.HABSeniorityIndex = entry.Properties["HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(entry.Properties["HABSeniorityIndex"].Value);
                            info.PostalCode = Convert.ToString(entry.Properties["postalCode"].Value);
                            info.AdminCount = 0;
                            info.Description = Convert.ToString(entry.Properties["Description"].Value);
                            info.LegacyExchangeDN = Convert.ToString(entry.Properties["LegacyExchangeDN"].Value);
                            for (int i = 0; i < entry.Properties["Memberof"].Count; i++)
                            {
                                info.Memberof.Add(Convert.ToString(entry.Properties["Memberof"][i]));
                            }
                            info.WhenChanged = Convert.ToDateTime(entry.Properties["WhenChanged"].Value);
                            info.WhenCreated = Convert.ToDateTime(entry.Properties["WhenCreated"].Value);
                            info.HasThumbnailPhoto = false;
                            info.ColorIndex = 0;
                            info.PhotoDisplayText = null;
                            info.MsExchRecipientTypeDetails = 1;
                            info.ExtensionAttribute1 = Convert.ToString(entry.Properties["ExtensionAttribute1"].Value);
                            info.ExtensionAttribute2 = Convert.ToString(entry.Properties["ExtensionAttribute2"].Value);
                            info.ExtensionAttribute3 = Convert.ToString(entry.Properties["ExtensionAttribute3"].Value);
                            info.ExtensionAttribute4 = Convert.ToString(entry.Properties["ExtensionAttribute4"].Value);
                            info.ExtensionAttribute5 = Convert.ToString(entry.Properties["ExtensionAttribute5"].Value);
                            info.ExtensionAttribute6 = Convert.ToString(entry.Properties["ExtensionAttribute6"].Value);
                            info.ExtensionAttribute7 = Convert.ToString(entry.Properties["ExtensionAttribute7"].Value);
                            info.CanonicalName = new string[0];
                            info.IpPhone = string.Empty;
                            info.HomePhone = string.Empty;
                            users.Add(info);
                        }

                        list.data = users.Skip((pageindex - 1) * pagesize).Take(pagesize).OrderByDescending(x => x.HABSeniorityIndex).ThenBy(x => x.DisplayName).ToList<object>();
                        strJsonResult = JsonConvert.SerializeObject(list);
                        LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                    else
                    {
                        if (!commonProvider.GetADEntryByPath(DistinguishedName, out groupEntry, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        list.pageIndex = pageindex;
                        list.pageSize = pagesize;
                        list.pageCount = (groupEntry.Properties["member"].Count + pagesize - 1) / pagesize;
                        List<HabAddressUserInfo> users = new List<HabAddressUserInfo>();
                        DirectoryEntry entry = new DirectoryEntry();
                        for (int i = 0; i < groupEntry.Properties["member"].Count; i++)
                        {
                            HabAddressUserInfo info = new HabAddressUserInfo();
                            string memberDn = Convert.ToString(groupEntry.Properties["member"][i]);
                            if (!commonProvider.GetADEntryByPath(memberDn, out entry, out message))
                            {
                                continue;
                            }
                            info.ObjectClass = entry.SchemaClassName;
                           
                            if (info.ObjectClass == "user")
                            {
                                info.Title = Convert.ToString(entry.Properties["Title"].Value);
                                info.Manager = Convert.ToString(entry.Properties["Manager"].Value);
                                info.Department = Convert.ToString(entry.Properties["Department"].Value);
                                info.SubMembers = null;
                                info.TelePhoneNumber = Convert.ToString(entry.Properties["TelePhoneNumber"].Value);
                                info.Fax = Convert.ToString(entry.Properties["facsimileTelephoneNumber"].Value);
                                info.Mobile = Convert.ToString(entry.Properties["Mobile"].Value);
                                info.UserPrincipalName = Convert.ToString(entry.Properties["UserPrincipalName"].Value);
                                info.UserAccountControl = UserProvider.IsAccountActive(Convert.ToInt32(entry.Properties["userAccountControl"][0]));
                                info.OfficeName = Convert.ToString(entry.Properties["physicalDeliveryOfficeName"].Value);
                                info.SN = Convert.ToString(entry.Properties["SN"].Value);
                                info.GivenName = Convert.ToString(entry.Properties["GivenName"].Value);
                                info.Mail = Convert.ToString(entry.Properties["Mail"].Value);
                                info.MailNickname = Convert.ToString(entry.Properties["MailNickname"].Value);
                                info.MSExchArchiveQuota = 0; //Convert.ToInt32(entry.Properties["MSExchArchiveQuota"].Value);
                                info.MSExchArchiveWarnQuota = 0;// Convert.ToInt32(entry.Properties["MSExchArchiveWarnQuota"].Value);
                                info.MSExchCalendarLoggingQuota = 0;// Convert.ToInt32(entry.Properties["MSExchCalendarLoggingQuota"].Value);
                                info.MSExchDumpsterQuota = 0; //Convert.ToInt32(entry.Properties["MSExchDumpsterQuota"].Value);
                                info.MSExchDumpsterWarningQuota = 0; //Convert.ToInt32(entry.Properties["MSExchDumpsterWarningQuota"].Value);
                                info.HomePage = Convert.ToString(entry.Properties["wWWHomePage"].Value);
                                info.l = Convert.ToString(entry.Properties["l"].Value);
                                info.St = Convert.ToString(entry.Properties["st"].Value);
                                info.StreetAddress = Convert.ToString(entry.Properties["streetAddress"].Value);
                                info.ObjectGUID = entry.Guid.ToString();
                                info.Name = Convert.ToString(entry.Properties["Name"].Value);
                                info.DisplayName = Convert.ToString(entry.Properties["DisplayName"].Value);
                                info.DistinguishedName = Convert.ToString(entry.Properties["DistinguishedName"].Value);
                                info.Company = Convert.ToString(entry.Properties["Company"].Value);
                                info.CN = Convert.ToString(entry.Properties["CN"].Value);
                                info.ObjectClass = "user";
                                info.SAMAccountName = Convert.ToString(entry.Properties["SAMAccountName"].Value);
                                for (int j = 0; j < entry.Properties["proxyAddresses"].Count; j++)
                                {
                                    info.ProxyAddress.Add(Convert.ToString(entry.Properties["proxyAddresses"][j]));
                                }
                                info.SmtpEmail = Convert.ToString(entry.Properties["Mail"].Value);
                                info.SipEmail = null;
                                info.MSExchHideFromAddressLists = false;
                                info.HABSeniorityIndex = entry.Properties["HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(entry.Properties["HABSeniorityIndex"].Value);
                                info.PostalCode = Convert.ToString(entry.Properties["postalCode"].Value);
                                info.AdminCount = 0;
                                info.Description = Convert.ToString(entry.Properties["Description"].Value);
                                info.LegacyExchangeDN = Convert.ToString(entry.Properties["LegacyExchangeDN"].Value);
                                for (int j = 0; j < entry.Properties["Memberof"].Count; j++)
                                {
                                    info.Memberof.Add(Convert.ToString(entry.Properties["Memberof"][j]));
                                }
                                info.WhenChanged = Convert.ToDateTime(entry.Properties["WhenChanged"].Value);
                                info.WhenCreated = Convert.ToDateTime(entry.Properties["WhenCreated"].Value);
                                info.HasThumbnailPhoto = false;
                                info.ColorIndex = 0;
                                info.PhotoDisplayText = null;
                                info.MsExchRecipientTypeDetails = 1;
                                info.ExtensionAttribute1 = Convert.ToString(entry.Properties["ExtensionAttribute1"].Value);
                                info.ExtensionAttribute2 = Convert.ToString(entry.Properties["ExtensionAttribute2"].Value);
                                info.ExtensionAttribute3 = Convert.ToString(entry.Properties["ExtensionAttribute3"].Value);
                                info.ExtensionAttribute4 = Convert.ToString(entry.Properties["ExtensionAttribute4"].Value);
                                info.ExtensionAttribute5 = Convert.ToString(entry.Properties["ExtensionAttribute5"].Value);
                                info.ExtensionAttribute6 = Convert.ToString(entry.Properties["ExtensionAttribute6"].Value);
                                info.ExtensionAttribute7 = Convert.ToString(entry.Properties["ExtensionAttribute7"].Value);
                                info.CanonicalName = new string[0];
                                info.IpPhone = string.Empty;
                                info.HomePhone = string.Empty;
                                users.Add(info);
                            }
                        }
                        list.data = users.Skip((pageindex - 1) * pagesize).Take(pagesize).OrderByDescending(x => x.HABSeniorityIndex).ThenBy(x => x.DisplayName).ToList<object>();
                        strJsonResult = JsonConvert.SerializeObject(list);
                        LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GroupManager调用getGroupsByParentDn异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetUserInfoByEMPLID(Guid transactionid, string EMPLID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"EMPLID:{EMPLID}";
         
            string funname = "GetUserInfoByEMPLID";

            try
            {
                do
                {
                    UserProvider userProvider = new UserProvider();
                    UserInfo user = new UserInfo();
                    if (!userProvider.GetUserInfoByEMPLID(transactionid, EMPLID, out user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(EMPLID, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    user.Mobile = Regex.Replace(user.Mobile.Replace("+86", ""), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(user);
                    LoggerHelper.Info(EMPLID, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(EMPLID, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用GetUserInfoByEMPLID异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SendFUserMobileCode(Guid transactionid, Guid userid, out string strJsonResult)
        {
            bool result = false;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            string funname = "SendFUserMobileCode";

            try
            {
                do
                {
                    if (userid == null || userid == Guid.Empty)
                    {
                        error.Code = ErrorCode.AccountIllegal;
                        strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserProvider userProvider = new UserProvider();
                    UserInfo user = new UserInfo();
                    user.UserID = userid;
                    if (!userProvider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    if (!user.UserStatus)
                    {
                        error.Code = ErrorCode.UserIsDisable;
                        strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserDBProvider dal = new UserDBProvider();
                    string code = GetRandomString(4, true, false, false, false, string.Empty);
                    bool bresult = dal.SendFUserMobileCode(transactionid, user.UserID, user.Mobile, code, out error);
                    if (bresult == true)
                    {
                        if (SendSmsCode(transactionid, user.Mobile, code))
                        {
                            LoggerHelper.Info(user.UserAccount, funname, paramstr, string.Empty, true, transactionid);
                            strJsonResult = JsonHelper.ReturnstrResult(true, "成功！");
                            result = true;
                        }
                        else
                        {
                            LoggerHelper.Info(user.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            strJsonResult = JsonHelper.ReturnstrResult(false, "发送安全码失败，请联系管理员。");
                            result = false;
                        }
                    }
                    else
                    {
                        LoggerHelper.Info(user.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnstrResult(false, "发送安全码失败，" + error.Info);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用SendFUserMobileCode异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnstrResult(false, "发送安全码失败，" + error.Info);
                result = false;
            }
            return result;
        }

        public bool CheckFUserSmsCode(Guid transactionid, Guid userid, string code, out string strJsonResult)
        {
            bool result = false;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||code:{code}";
            string funname = "CheckFUserSmsCode";
            Guid operaterID = Guid.Empty;
            Guid orgID = Guid.Empty;
            try
            {
                do
                {
                    if (userid == null || userid == Guid.Empty)
                    {
                        error.Code = ErrorCode.AccountIllegal;
                        strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    UserProvider userProvider = new UserProvider();
                    UserInfo user = new UserInfo();
                    user.UserID = userid;
                    if (!userProvider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    if (!user.UserStatus)
                    {
                        error.Code = ErrorCode.UserIsDisable;
                        strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserDBProvider dal = new UserDBProvider();
                    Guid codeid = Guid.Empty;
                    bool bresult = dal.CheckFUserSmsCode(transactionid, userid, code, out codeid, out error);
                    if (bresult == true)
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>();
                        dictionary.Add("CodeID", codeid);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnstrResult(true, "验证安全码成功！", dictionary);
                        result = true;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用CheckFContactLoginCode异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                result = false;
            }
            return result;
        }

        public bool SetFUserPass(Guid transactionid, Guid codeid, string password, out string strJsonResult)
        {
            bool result = false;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"codeid:{codeid}";
            paramstr += $"||password:{password}";
            string funname = "SetFUserPass";
            Guid operaterID = Guid.Empty;
            Guid orgID = Guid.Empty;
            try
            {
                do
                {
                    if (codeid == null || codeid == Guid.Empty)
                    {
                        error.Code = ErrorCode.SmsCodeWrong;
                        strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                        LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserDBProvider dal = new UserDBProvider();
                    Guid userid = Guid.Empty;
                    if (!dal.CheckFUserSmsByCodeID(transactionid, codeid, out userid, out error))
                    {
                        LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnstrResult(false, "重置密码失败，" + error.Info);
                        result = false;
                        break;
                    }

                    UserProvider userProvider = new UserProvider();
                    AdminInfo admin = new AdminInfo();
                    UserInfo user = new UserInfo();
                    user.UserID = userid;
                    user.Password = password;
                    user.NextLoginChangePassword = false;
                    user.PasswordNeverExpire = false;
                    if (!userProvider.ResetUserPassword(transactionid, admin, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnstrResult(false, error.Info);
                        LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                    strJsonResult = JsonHelper.ReturnstrResult(true, "重置密码成功！");
                    result = true;
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用SetFUserPass异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnstrResult(false, "重置密码失败，" + error.Info);
                result = false;
            }
            return result;
        }
        #region company
        public bool AddUser(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||SAMAccountName:{user.SAMAccountName}";
            paramstr += $"||UserDisplayName:{user.DisplayName}";
            paramstr += $"||UserFirstName:{user.FirstName}";
            paramstr += $"||UserLastName:{user.LastName}";
            paramstr += $"||UserPassword:{user.Password}";
            paramstr += $"||ParentOuId:{user.ParentOuId}";

            string funname = "AddUser";

            try
            {
                do
                {
                    error = user.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("AddUser异常", paramstr, error.Info, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!MatchPassword(user.Password))
                    {
                        error.Code = ErrorCode.PasswordIllegal;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("AddUser异常", paramstr, error.Info, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.ParentOuId, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("AddUser调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    user.ParentDistinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);
                    //验证权限
                    if (!admin.ParamList.ContainsValue(RoleParamCode.AddUser))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("AddUser异常", paramstr, error.Info, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!admin.ParamList.ContainsValue(RoleParamCode.PublicUser))
                    {
                        if (!roleManager.CheckManagerOuRole(transactionid, user.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                        {
                            error.Code = ErrorCode.UserNotEnoughRole;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Error("AddUser异常", paramstr, error.Info, transactionid);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }
                    DirectoryEntry entry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleEntryByCN(ouEntry.Path, user.DisplayName, out entry, out message))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("AddUser调用GetOneLevelSigleEntryByCN异常", paramstr, message, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), user.SAMAccountName, out entry, out message))
                    {
                        error.Code = ErrorCode.HaveSameAccount;
                        error.SetInfo(Convert.ToString(entry.Parent.Properties["distinguishedName"].Value));
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("AddUser调用GetEntryDataBysAMAccount异常", paramstr, error.Info, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserProvider provider = new UserProvider();
                    if (user.IsCreateMail)
                    {
                        #region 废弃
                        //MailDataBaseDBProvider mailProvider = new MailDataBaseDBProvider();
                        //List<MailDataBaseInfo> maildbs = new List<MailDataBaseInfo>();
                        //if (!mailProvider.GetMailDataBaseBydistinguishedName(transactionid, user.ParentDistinguishedName, out maildbs, out error))
                        //{
                        //    LoggerHelper.Error("AddUser调用GetMailDataBaseBydistinguishedName异常", paramstr, error.Info, transactionid);
                        //}
                        //if (maildbs.Count > 0)
                        //{
                        //    user.UserExchange.Database = maildbs[0].MailboxDB;
                        //}
                        //else
                        //{
                        //    user.UserExchange.Database = ConfigADProvider.GetDefaultMailboxDB();
                        //}
                        #endregion

                        //随机获取邮箱数据库
                        List<string> dblist = ConfigHelper.GetConfigValue("MailDB");
                        Random rd = new Random();
                        int index = rd.Next(dblist.Count);
                        user.UserExchange.Database = dblist[index];
                       
                        byte[] binfo = JsonHelper.SerializeObject(user);
                        ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                        webService.Timeout = -1;

                        if (!webService.NewExchangeUser(transactionid, ref binfo, out message))
                        {
                            error.Code = ErrorCode.Exception;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Error("AddUser调用NewExchangeUser异常", paramstr, message, transactionid);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        user = (UserInfo)JsonHelper.DeserializeObject(binfo);

                        //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                        //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));

                        provider.ChangeUserStatus(transactionid, admin, user, out error);
                    }
                    else
                    {
                        if (!provider.AddUser(transactionid, admin, ref user, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    #region 废弃
                    //将对象添加到HabGroup组内
                    //GroupManager groupManager = new GroupManager(_clientip);
                    //groupManager.AddHabGroupMember(transactionid, admin, user.ParentOuId, user.UserID, out error);
                    #endregion

                    user.Password = string.Empty;
                    string json = JsonConvert.SerializeObject(user);
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加用户。" +
                        $"用户显示名称：{user.DisplayName}，" +
                        $"用户：{user.UserAccount}，" +
                        $"所属OU：{user.ParentOu}" +
                        $"是否开通邮箱：{user.IsCreateMail}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用AddUser异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ChangeUser(Guid transactionid, AdminInfo admin, UserInfo user, bool isSamelevelOu, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||UserDisplayName:{user.DisplayName}";
            paramstr += $"||UserFirstName:{user.FirstName}";
            paramstr += $"||UserLastName:{user.LastName}";

            string funname = "ChangeUser";

            try
            {
                do
                {
                    error = user.ChangeCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserProvider provider = new UserProvider();
                    UserInfo oldUser = new UserInfo();
                    oldUser.UserID = user.UserID;
                    if (!provider.GetUserInfo(transactionid, ref oldUser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #region 判断权限
                    RoleManager roleManager = new RoleManager(_clientip);

                    if (!roleManager.CheckManagerOuRole(transactionid, oldUser.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    
                    #endregion

                    if (!provider.ChangeUser(transactionid, admin, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #region 新增修改邮件别名
                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;

                    string message = string.Empty;
                    if (!webService.UpdateUserAlias(transactionid, user.UserAccount, user.AliasName, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("ChangeUserExchangeInfo调用UpdateExchangeUser异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                    #endregion

                    UserDBProvider userDBProvider = new UserDBProvider();
                    if (!userDBProvider.ModifyUser(transactionid, admin, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    // 添加日志
                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "编辑用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}编辑用户。" +
                      $"用户为：{oldUser.UserAccount}，" +
                      $"原显示名称：{oldUser.DisplayName}，现显示名称：{user.DisplayName}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用ChangeUser异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ResetUserPassword(Guid transactionid, AdminInfo admin, UserInfo user, bool isSamelevelOu, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||Password:{user.Password}";
            paramstr += $"||NextLoginChangePassword:{user.NextLoginChangePassword}";
            paramstr += $"||PasswordNeverExpire:{user.PasswordNeverExpire}";

            string funname = "ResetUserPassword";

            try
            {
                do
                {
                    if (!MatchPassword(user.Password))
                    {
                        error.Code = ErrorCode.PasswordIllegal;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserProvider provider = new UserProvider();
                    UserInfo oldUser = new UserInfo();
                    oldUser.UserID = user.UserID;
                    if (!provider.GetUserInfo(transactionid, ref oldUser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #region 判断权限
                    RoleManager roleManager = new RoleManager(_clientip);

                    if (!roleManager.CheckManagerOuRole(transactionid, oldUser.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    
                    #endregion

                    if (!provider.ResetUserPassword(transactionid, admin, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    string json = JsonConvert.SerializeObject(user);
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    //添加日志
                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "重置用户密码";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}重置用户密码。" +
                      $"重置密码的用户名称为：{user.DisplayName}（{user.UserAccount}）";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用ResetUserPassword异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetUserInfo(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "GetUserInfo";

            try
            {
                do
                {
                    UserProvider userProvider = new UserProvider();
                    if (!userProvider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(user);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用GetUserInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetUserExchangeInfo(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "GetUserExchangeInfo";

            try
            {
                do
                {
                    UserProvider userProvider = new UserProvider();
                    if (!userProvider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;
                    
                    ExchangeUserInfo userExchange = new ExchangeUserInfo();
                    byte[] exchange;

                    if (!webService.GetUserExchange(transactionid, user.UserAccount, out exchange, out message))
                    {
                        paramstr += $"||UserAccount：{user.UserAccount}";
                        LoggerHelper.Error("GetUserExchange", paramstr,message,transactionid);
                        userExchange = (ExchangeUserInfo)JsonHelper.DeserializeObject(exchange);
                        user.UserExchange.ExchangeStatus = false;
                        user.UserExchange.MailSize = userExchange.mailSize;
                        user.UserExchange.Database = userExchange.database;
                        user.UserExchange.Activesync = userExchange.ActiveSyncEnabled;
                        user.UserExchange.Imap4 = userExchange.ActiveSyncEnabled;
                        user.UserExchange.Mapi = userExchange.ActiveSyncEnabled;
                        user.UserExchange.OWA = userExchange.ActiveSyncEnabled;
                        user.UserExchange.POP3 = userExchange.ActiveSyncEnabled;
                    }
                    else
                    {
                        userExchange = (ExchangeUserInfo)JsonHelper.DeserializeObject(exchange);
                        user.UserExchange.ExchangeStatus = true;
                        user.UserExchange.MailSize = userExchange.mailSize;
                        user.UserExchange.Database = userExchange.database;
                        user.UserExchange.Activesync = userExchange.ActiveSyncEnabled;
                        user.UserExchange.Imap4 = userExchange.ImapEnabled;
                        user.UserExchange.Mapi = userExchange.MAPIEnabled;
                        user.UserExchange.OWA = userExchange.OWAEnabled;
                        user.UserExchange.POP3 = userExchange.PopEnabled;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(user);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用GetUserExchangeInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ChangeUserExchangeInfo(Guid transactionid, AdminInfo admin, UserInfo user, bool isSamelevelOu, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "ChangeUserExchangeInfo";

            try
            {
                do
                {
                    string alias = user.AliasName;
                    UserProvider userProvider = new UserProvider();
                    if (!userProvider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #region 判断权限
                    RoleManager roleManager = new RoleManager(_clientip);

                    if (!roleManager.CheckManagerOuRole(transactionid, user.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #endregion

                    #region
                    // string MailboxDataBox = string.Empty;
                    //MailDataBaseDBProvider mailProvider = new MailDataBaseDBProvider();
                    //List<MailDataBaseInfo> maildbs = new List<MailDataBaseInfo>();
                    //if (!mailProvider.GetMailDataBaseBydistinguishedName(transactionid, user.ParentDistinguishedName, out maildbs, out error))
                    //{
                    //    LoggerHelper.Error("ChangeUserExchangeInfo调用GetMailDataBaseBydistinguishedName异常", paramstr, error.Info, transactionid);
                    //}
                    //if (maildbs.Count > 0)
                    //{
                    //    MailboxDataBox = maildbs[0].MailboxDB;
                    //}
                    //else
                    //{
                    //    MailboxDataBox = ConfigADProvider.GetDefaultMailboxDB();
                    //}
                    #endregion
                    List<string> dblist = ConfigHelper.GetConfigValue("MailDB");
                    Random rd = new Random();
                    int index = rd.Next(dblist.Count);
                    user.UserExchange.Database = dblist[index];

                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;
                    byte[] binfo = JsonHelper.SerializeObject(user.UserExchange);
                    if (!webService.UpdateExchangeUser(transactionid, user.UserAccount, user.DisplayName, alias, binfo, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("ChangeUserExchangeInfo调用UpdateExchangeUser异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(user);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    //添加日志
                    #region 操作日志
                    string exchangestatus = user.UserExchange.ExchangeStatus == true ? "启用邮箱" : "禁用邮箱";
                    string mailboxsize = user.UserExchange.MailSize == -1 ? "使用默认邮箱空间" : user.UserExchange.MailSize + "GB";

                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "修改用户邮箱属性";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改用户邮箱属性。" +
                      $"用户名称：{user.DisplayName}（{user.UserAccount}），" +
                      $"邮箱状态：{exchangestatus}，" +
                      $"邮箱空间：{mailboxsize}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("ChangeUserExchangeInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DisableUser(Guid transactionid, AdminInfo admin, UserInfo user, bool isSamelevelOu, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "DisableUser";

            try
            {
                do
                {
                    UserProvider provider = new UserProvider();
                    UserInfo olduser = new UserInfo();
                    olduser.UserID = user.UserID;
                    if (!provider.GetUserInfo(transactionid, ref olduser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #region 判断权限
                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!roleManager.CheckManagerOuRole(transactionid, olduser.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    #endregion

                    if (!provider.ChangeUserStatus(transactionid, admin, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "禁用用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}禁用用户。" +
                      $"用户名称：{olduser.DisplayName}（{olduser.UserAccount}）";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用DisableUser异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool EnableUser(Guid transactionid, AdminInfo admin, UserInfo user, bool isSamelevelOu, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "EnableUser";

            try
            {
                do
                {

                    UserProvider provider = new UserProvider();
                    UserInfo olduser = new UserInfo();
                    olduser.UserID = user.UserID;
                    if (!provider.GetUserInfo(transactionid, ref olduser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #region 判断权限
                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!roleManager.CheckManagerOuRole(transactionid, olduser.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    
                    #endregion

                    if (!provider.ChangeUserStatus(transactionid, admin, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "启用用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}启用用户。" +
                      $"用户名称：{olduser.DisplayName}（{olduser.UserAccount}）";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用EnableUser异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ChangeUserMemberof(Guid transactionid, AdminInfo admin, UserInfo user, bool isSamelevelOu, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "ChangeUserMemberof";

            try
            {
                do
                {
                    UserProvider userProvider = new UserProvider();
                    UserInfo oldUser = new UserInfo();
                    oldUser.UserID = user.UserID;
                    if (!userProvider.GetUserInfo(transactionid, ref oldUser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #region 判断权限
                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!roleManager.CheckManagerOuRole(transactionid, oldUser.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    #endregion

                    if (!userProvider.ClearMemberof(transactionid, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    string groups = string.Empty;
                    foreach (GroupInfo group in user.BelongGroups)
                    {
                        string groupname = string.Empty;
                        if (!userProvider.AddMemberof(transactionid, user, group, out groupname, out error))
                        {
                            continue;
                        }
                        groups += group.DisplayName + ",";
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
                    operateLog.OperateType = "修改用户所属组";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改用户所属组。" +
                        $"用户显示名称：{user.DisplayName}，" +
                        $"用户：{user.UserAccount}，" +
                        $"组列表：{groups}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用ChangeUserMemberof异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteUser(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "DeleteUser";

            try
            {
                do
                {
                    //操作数据库
                    UserProvider provider = new UserProvider();
                    if (!provider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //验证权限
                    if (!admin.ParamList.ContainsValue(RoleParamCode.DeleteUser))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!admin.ParamList.ContainsValue(RoleParamCode.PublicUser))
                    {
                        if (!roleManager.CheckManagerOuRole(transactionid, user.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                        {
                            error.Code = ErrorCode.UserNotEnoughRole;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    UserDBProvider userDBProvider = new UserDBProvider();
                    if (!userDBProvider.DeleteUser(transactionid, admin, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //清除组中成员
                    provider.ClearAllMemberof(transactionid, user, out error);

                    if (!MoveAndDisableUser(transactionid, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    //停用邮箱
                    UserInfo exUser = new UserInfo();
                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;
                    byte[] u;
                    if (webService.GetUser(transactionid, user.UserID.ToString(), out u, out message))
                    {
                        exUser = (UserInfo)JsonHelper.DeserializeObject(u);
                        webService.DisableMailbox(transactionid, user.UserID.ToString(), out message);
                    }

                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "禁用并移动用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}禁用并移动用户。" +
                      $"用户名称：{user.DisplayName}（{user.UserAccount}）";
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用DeleteUser异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ResumeUser(Guid transactionid, AdminInfo admin, UserInfo info, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||TargetOuID:{info.ParentOuId}";
            paramstr += $"||UserID:{info.UserID}";
            paramstr += $"||CreateMail:{info.IsCreateMail}";

            string funname = "ResumeUser";

            try
            {
                do
                {
                    UserProvider userProvider = new UserProvider();
                    UserInfo user = new UserInfo();
                    user.UserID = info.UserID;
                    if (!userProvider.GetUserInfo(transactionid, ref user, out error))
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        result = false;
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(info.ParentOuId, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("ResumeUser调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string ouname = Convert.ToString(ouEntry.Properties["name"].Value);

                    if (!userProvider.MoveAndEnableUser(transactionid, user, info.ParentOuId, out error))
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        result = false;
                        break;
                    }

                    if (info.UseDefaultPassword)
                    {
                        user.Password = ConfigADProvider.GetDefaultPassword();
                        userProvider.ResetUserPassword(transactionid, admin, ref user, out error);
                    }

                    if (info.IsCreateMail)
                    {
                        if (!EnableUserMailBox(transactionid, user, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    //GroupInfo group = new GroupInfo();
                    //GroupProvider groupProvider = new GroupProvider();
                    //if (groupProvider.GetHabGroupInfoByOu(transactionid, info.ParentOuId, out group, out error))
                    //{
                    //    //恢复组中成员
                    //    groupProvider.AddGroupMember(transactionid, group.GroupID, info.UserID, out error);
                    //}

                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "恢复用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}恢复用户。" +
                      $"用户名称：{user.DisplayName}（{user.UserAccount}）" +
                      $"恢复至：{ouname}";
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用ResumeUser异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        #endregion

        #region samelevel
        public bool AddUserNoHab(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||UserDisplayName:{user.DisplayName}";
            paramstr += $"||UserFirstName:{user.FirstName}";
            paramstr += $"||UserLastName:{user.LastName}";
            paramstr += $"||UserPassword:{user.Password}";
            paramstr += $"||ParentOuId:{user.ParentOuId}";

            string funname = "AddUserNoHab";

            try
            {
                do
                {
                    error = user.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("AddUserNoHab异常", paramstr, error.Info, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!MatchPassword(user.Password))
                    {
                        error.Code = ErrorCode.PasswordIllegal;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("AddUserNoHab异常", paramstr, error.Info, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.ParentOuId, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("AddUserNoHab调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    user.ParentDistinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);

                    DirectoryEntry entry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleEntryByCN(ouEntry.Path, user.DisplayName, out entry, out message))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("AddUserNoHab调用GetOneLevelSigleEntryByCN异常", paramstr, message, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), user.SAMAccountName, out entry, out message))
                    {
                        error.Code = ErrorCode.HaveSameAccount;
                        error.SetInfo(Convert.ToString(entry.Parent.Properties["distinguishedName"].Value));
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("AddUserNoHab调用GetEntryDataBysAMAccount异常", paramstr, message, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (user.IsCreateMail)
                    {
                        MailDataBaseDBProvider mailProvider = new MailDataBaseDBProvider();
                        List<MailDataBaseInfo> maildbs = new List<MailDataBaseInfo>();
                        if (!mailProvider.GetMailDataBaseBydistinguishedName(transactionid, user.ParentDistinguishedName, out maildbs, out error))
                        {
                            LoggerHelper.Error("EnableUserMailBox调用GetMailDataBaseBydistinguishedName异常", paramstr, error.Info, transactionid);
                        }
                        if (maildbs.Count > 0)
                        {
                            user.UserExchange.Database = maildbs[0].MailboxDB;
                        }
                        else
                        {
                            user.UserExchange.Database = ConfigADProvider.GetDefaultMailboxDB();
                        }

                        byte[] binfo = JsonHelper.SerializeObject(user);
                        ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                        webService.Timeout = -1;

                        if (!webService.NewExchangeUser(transactionid, ref binfo, out message))
                        {
                            error.Code = ErrorCode.Exception;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Error("AddUserNoHab调用NewExchangeUser异常", paramstr, message, transactionid);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        user = (UserInfo)JsonHelper.DeserializeObject(binfo);

                        //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                        //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));
                    }
                    else
                    {
                        UserProvider provider = new UserProvider();
                        if (!provider.AddUser(transactionid, admin, ref user, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Error("AddUserNoHab调用AddUser异常", paramstr, message, transactionid);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    user.Password = string.Empty;
                    string json = JsonConvert.SerializeObject(user);
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加用户。" +
                        $"用户显示名称：{user.DisplayName}，" +
                        $"用户：{user.UserAccount}，" +
                        $"所属OU：{user.ParentOu}" +
                        $"是否开通邮箱：{user.IsCreateMail}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用AddUserNoHab异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteUserNoHab(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "DeleteUserNoHab";

            try
            {
                do
                {
                    UserProvider provider = new UserProvider();
                    if (!provider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!provider.DeleteUser(transactionid, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "删除用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除用户。" +
                      $"用户名称：{user.DisplayName}（{user.UserAccount}）";
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("UserManager调用DeleteUserNoHab异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        #endregion
        public bool EnableUserMailBox(Guid transactionid, UserInfo user, out ErrorCodeInfo error)
        {
            bool result = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"UserID:{user.UserID}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||UserDisplayName:{user.DisplayName}";
            paramstr += $"||SAMAccountName:{user.SAMAccountName}";

            try
            {
                do
                {
                    DirectoryEntry userEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.UserID, out userEntry, out message))
                    {
                        error.Code = ErrorCode.UserNotExist;
                        LoggerHelper.Error("EnableUserMailBox调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string alias = userEntry.Properties["mailNickname"].Value == null ? "" : Convert.ToString(userEntry.Properties["mailNickname"].Value);

                    #region
                     //string parentPath = Convert.ToString(userEntry.Parent.Properties["distinguishedName"].Value);
                    //MailDataBaseDBProvider mailProvider = new MailDataBaseDBProvider();
                    //List<MailDataBaseInfo> maildbs = new List<MailDataBaseInfo>();
                    //if (!mailProvider.GetMailDataBaseBydistinguishedName(transactionid, parentPath, out maildbs, out error))
                    //{
                    //    LoggerHelper.Error("EnableUserMailBox调用GetMailDataBaseBydistinguishedName异常", paramstr, error.Info, transactionid);
                    //}
                    //if (maildbs.Count > 0)
                    //{
                    //    MailboxDataBox = maildbs[0].MailboxDB;
                    //}
                    //else
                    //{
                    //    MailboxDataBox = ConfigADProvider.GetDefaultMailboxDB();
                    //}
                    #endregion
                    string MailboxDataBox = string.Empty;
                    List<string> dblist = ConfigHelper.GetConfigValue("MailDB");
                    Random rd = new Random();
                    int index = rd.Next(dblist.Count);
                    MailboxDataBox = dblist[index];

                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;

                    if (!webService.EnableMailboxAndMailboxDataBase(transactionid, user.UserAccount, alias, MailboxDataBox, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("EnableUserMailBox调用EnableMailbox异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    #region 废弃
                    //ExchangeUserInfo exchange = new ExchangeUserInfo();
                    //exchange.mailSize = -1;
                    //exchange.MAPIEnabled = true;
                    //exchange.ActiveSyncEnabled = true;
                    //exchange.PopEnabled = true;
                    //exchange.ImapEnabled = true;
                    //exchange.OWAEnabled = true;
                    //exchange.database = "";

                    //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                    //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));

                    //byte[] binfo = JsonHelper.SerializeObject(exchange);
                    //if (!webService.UpdateUserExchange(transactionid, user.UserAccount, user.DisplayName, binfo, out message))
                    //{
                    //    error.Code = ErrorCode.Exception;
                    //    LoggerHelper.Error("EnableUserMailBox调用UpdateUserExchange异常", paramstr, message, transactionid);
                    //    result = false;
                    //    break;
                    //}
                    #endregion

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("EnableUserMailBox异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }
            return result;
        }

        public bool MoveAndDisableUser(Guid transactionid, UserInfo user, out ErrorCodeInfo error)
        {
            bool result = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"UserID:{user.UserID}";

            try
            {
                do
                {
                    DirectoryEntry userEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.UserID, out userEntry, out message))
                    {
                        error.Code = ErrorCode.UserNotExist;
                        LoggerHelper.Error("UserManager调用MoveAndDisableUser异常", paramstr, message, transactionid);
                        result = false;
                    }

                    OuInfo ou = new OuInfo();
                    string parentPath = Convert.ToString(userEntry.Parent.Properties["distinguishedName"].Value);
                    if (!commonProvider.GetRecycleOu(parentPath, out ou, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("UserManager调用MoveAndDisableUser异常", paramstr, message, transactionid);
                        result = false;
                    }

                    UserProvider provider = new UserProvider();
                    if (!provider.MoveAndDisableUser(transactionid, user, ou.id, out error))
                    {
                        LoggerHelper.Error("UserManager调用MoveAndDisableUser异常", paramstr, message, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("UserManager调用MoveAndDisableUser异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }
            return result;
        }

        #region Interface
        public bool AddUserByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||SAMAccountName:{user.SAMAccountName}";
            paramstr += $"||UserDisplayName:{user.DisplayName}";
            paramstr += $"||UserFirstName:{user.FirstName}";
            paramstr += $"||UserLastName:{user.LastName}";
            paramstr += $"||UserPassword:{user.Password}";
            paramstr += $"||IsCreateMail:{user.IsCreateMail}";
            paramstr += $"||ParentOuId:{user.ParentOuId}";

            string funname = "AddUserByInterface";

            try
            {
                do
                {
                    error = user.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!MatchPassword(user.Password))
                    {
                        error.Code = ErrorCode.PasswordIllegal;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.ParentOuId, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("AddUserByInterface调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    user.ParentDistinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);

                    DirectoryEntry entry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleEntryByCN(ouEntry.Path, user.DisplayName, out entry, out message))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), user.SAMAccountName, out entry, out message))
                    {
                        error.Code = ErrorCode.HaveSameAccount;
                        error.SetInfo(Convert.ToString(entry.Parent.Properties["distinguishedName"].Value));
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (user.IsCreateMail)
                    {
                        MailDataBaseDBProvider mailProvider = new MailDataBaseDBProvider();
                        List<MailDataBaseInfo> maildbs = new List<MailDataBaseInfo>();
                        if (!mailProvider.GetMailDataBaseBydistinguishedName(transactionid, user.ParentDistinguishedName, out maildbs, out error))
                        {
                            LoggerHelper.Error("EnableUserMailBox调用GetMailDataBaseBydistinguishedName异常", paramstr, error.Info, transactionid);
                        }
                        if (maildbs.Count > 0)
                        {
                            user.UserExchange.Database = maildbs[0].MailboxDB;
                        }
                        else
                        {
                            user.UserExchange.Database = ConfigADProvider.GetDefaultMailboxDB();
                        }

                        byte[] binfo = JsonHelper.SerializeObject(user);
                        ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                        webService.Timeout = -1;

                        if (!webService.NewExchangeUser(transactionid, ref binfo, out message))
                        {
                            error.Code = ErrorCode.Exception;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        user = (UserInfo)JsonHelper.DeserializeObject(binfo);

                        //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                        //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));
                    }
                    else
                    {
                        UserProvider provider = new UserProvider();
                        if (!provider.AddUser(transactionid, admin, ref user, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }
                    
                    //将对象添加到HabGroup组内
                    GroupManager groupManager = new GroupManager(_clientip);
                    groupManager.AddHabGroupMember(transactionid, admin, user.ParentOuId, user.UserID, out error);

                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    dictionary.Add("UserID", user.UserID);
                    string json = JsonConvert.SerializeObject(dictionary);
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, dictionary);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加用户。" +
                        $"用户显示名称：{user.DisplayName}，" +
                        $"用户：{user.UserAccount}，" +
                        $"所属OU：{user.ParentOu}" +
                        $"是否开通邮箱：{user.IsCreateMail}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("AddUserByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        
        public bool ChangeUserByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||UserDisplayName:{user.DisplayName}";
            paramstr += $"||UserFirstName:{user.FirstName}";
            paramstr += $"||UserLastName:{user.LastName}";

            string funname = "ChangeUserByInterface";

            try
            {
                do
                {
                    error = user.ChangeCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserProvider provider = new UserProvider();
                    UserInfo oldUser = new UserInfo();
                    oldUser.UserID = user.UserID;
                    if (!provider.GetUserInfo(transactionid, ref oldUser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!provider.ChangeUser(transactionid, admin, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserDBProvider userDBProvider = new UserDBProvider();
                    userDBProvider.ModifyUser(transactionid, admin, user, out error);
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    // 添加日志
                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "编辑用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}编辑用户。" +
                      $"用户为：{oldUser.UserAccount}，" +
                      $"原显示名称：{oldUser.DisplayName}，现显示名称：{user.DisplayName}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("ChangeUserByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ChangeUserExchangeInfoByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||ExchangeStatus:{user.UserExchange.ExchangeStatus}";
            paramstr += $"||MailSize:{user.UserExchange.MailSize}";
            
            string funname = "ChangeUserExchangeInfoByInterface";

            try
            {
                do
                {
                    UserProvider userProvider = new UserProvider();
                    if (!userProvider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //string MailboxDataBox = string.Empty;
                    //MailDataBaseDBProvider mailProvider = new MailDataBaseDBProvider();
                    //List<MailDataBaseInfo> maildbs = new List<MailDataBaseInfo>();
                    //if (!mailProvider.GetMailDataBaseBydistinguishedName(transactionid, user.ParentDistinguishedName, out maildbs, out error))
                    //{
                    //    LoggerHelper.Error("ChangeUserExchangeInfo调用GetMailDataBaseBydistinguishedName异常", paramstr, error.Info, transactionid);
                    //}
                    //if (maildbs.Count > 0)
                    //{
                    //    MailboxDataBox = maildbs[0].MailboxDB;
                    //}
                    //else
                    //{
                    //    MailboxDataBox = ConfigADProvider.GetDefaultMailboxDB();
                    //}

                    //user.UserExchange.Database = MailboxDataBox;

                    List<string> dblist = ConfigHelper.GetConfigValue("MailDB");
                    Random rd = new Random();
                    int index = rd.Next(dblist.Count);
                    user.UserExchange.Database = dblist[index];

                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;
                    byte[] binfo = JsonHelper.SerializeObject(user.UserExchange);
                    if (!webService.UpdateExchangeUser(transactionid, user.UserAccount, user.DisplayName,user.AliasName, binfo, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("ChangeUserExchangeInfo调用UpdateExchangeUser异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    #region 操作日志
                    string exchangestatus = user.UserExchange.ExchangeStatus == true ? "启用邮箱" : "禁用邮箱";
                    string mailboxsize = user.UserExchange.MailSize == -1 ? "使用默认邮箱空间" : user.UserExchange.MailSize + "GB";

                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "修改用户邮箱属性";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改用户邮箱属性。" +
                      $"用户名称：{user.DisplayName}（{user.UserAccount}），" +
                      $"邮箱状态：{exchangestatus}，" +
                      $"邮箱空间：{mailboxsize}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("ChangeUserExchangeInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ResetUserPasswordByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||Password:{user.Password}";
            paramstr += $"||NextLoginChangePassword:{user.NextLoginChangePassword}";
            paramstr += $"||PasswordNeverExpire:{user.PasswordNeverExpire}";

            string funname = "ResetUserPasswordByInterface";

            try
            {
                do
                {
                    if (!MatchPassword(user.Password))
                    {
                        error.Code = ErrorCode.PasswordIllegal;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    UserProvider provider = new UserProvider();
                    UserInfo oldUser = new UserInfo();
                    oldUser.UserID = user.UserID;
                    if (!provider.GetUserInfo(transactionid, ref oldUser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!provider.ResetUserPassword(transactionid, admin, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "重置用户密码";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}重置用户密码。" +
                      $"重置密码的用户名称为：{user.DisplayName}（{user.UserAccount}）";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("ResetUserPasswordByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetUserInfoByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "GetUserInfoByInterface";

            try
            {
                do
                {
                    UserProvider userProvider = new UserProvider();
                    if (!userProvider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;
                    byte[] binfo;
                    ExchangeUserInfo userExchange = new ExchangeUserInfo();
                    if (!webService.GetUserExchange(transactionid, user.UserAccount, out binfo, out message))
                    {
                        userExchange = (ExchangeUserInfo)JsonHelper.DeserializeObject(binfo);
                        user.UserExchange.ExchangeStatus = false;
                        user.UserExchange.MailSize = userExchange.mailSize;
                        user.UserExchange.Database = userExchange.database;
                        user.UserExchange.Activesync = userExchange.ActiveSyncEnabled;
                        user.UserExchange.Imap4 = userExchange.ActiveSyncEnabled;
                        user.UserExchange.Mapi = userExchange.ActiveSyncEnabled;
                        user.UserExchange.OWA = userExchange.ActiveSyncEnabled;
                        user.UserExchange.POP3 = userExchange.ActiveSyncEnabled;
                    }
                    else
                    {
                        userExchange = (ExchangeUserInfo)JsonHelper.DeserializeObject(binfo);
                        user.UserExchange.ExchangeStatus = true;
                        user.UserExchange.MailSize = userExchange.mailSize;
                        user.UserExchange.Database = userExchange.database;
                        user.UserExchange.Activesync = userExchange.ActiveSyncEnabled;
                        user.UserExchange.Imap4 = userExchange.ImapEnabled;
                        user.UserExchange.Mapi = userExchange.MAPIEnabled;
                        user.UserExchange.OWA = userExchange.OWAEnabled;
                        user.UserExchange.POP3 = userExchange.PopEnabled;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(user);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GetUserInfoByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DisableUserByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "DisableUserByInterface";

            try
            {
                do
                {
                    UserProvider provider = new UserProvider();
                    UserInfo olduser = new UserInfo();
                    olduser.UserID = user.UserID;
                    if (!provider.GetUserInfo(transactionid, ref olduser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    user.UserStatus = false;
                    if (!provider.ChangeUserStatus(transactionid, admin, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "禁用用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}禁用用户。" +
                      $"用户名称：{olduser.DisplayName}（{olduser.UserAccount}）";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("DisableUserByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool EnableUserByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "EnableUserByInterface";

            try
            {
                do
                {

                    UserProvider provider = new UserProvider();
                    UserInfo olduser = new UserInfo();
                    olduser.UserID = user.UserID;
                    if (!provider.GetUserInfo(transactionid, ref olduser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    user.UserStatus = true;
                    if (!provider.ChangeUserStatus(transactionid, admin, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "启用用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}启用用户。" +
                      $"用户名称：{olduser.DisplayName}（{olduser.UserAccount}）";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("EnableUserByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ChangeUserMemberofByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "ChangeUserMemberofByInterface";

            try
            {
                do
                {
                    UserProvider userProvider = new UserProvider();
                    UserInfo oldUser = new UserInfo();
                    oldUser.UserID = user.UserID;
                    if (!userProvider.GetUserInfo(transactionid, ref oldUser, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    
                    if (!userProvider.ClearMemberof(transactionid, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    string groups = string.Empty;
                    foreach (GroupInfo group in user.BelongGroups)
                    {
                        string groupname = string.Empty;
                        if (!userProvider.AddMemberof(transactionid, user, group, out groupname, out error))
                        {
                            continue;
                        }
                        groups += group.DisplayName + ",";
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
                    operateLog.OperateType = "修改用户所属组";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改用户所属组。" +
                        $"用户显示名称：{user.DisplayName}，" +
                        $"用户：{user.UserAccount}，" +
                        $"组列表：{groups}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("ChangeUserMemberofByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DisableAndMoveUserByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "DisableAndMoveUserByInterface";

            try
            {
                do
                {
                    //操作数据库
                    UserProvider provider = new UserProvider();
                    if (!provider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    
                    UserDBProvider userDBProvider = new UserDBProvider();
                    if (!userDBProvider.DeleteUser(transactionid, admin, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //清除组中成员
                    provider.ClearAllMemberof(transactionid, user, out error);

                    if (!MoveAndDisableUser(transactionid, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //停用邮箱
                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;
                    UserInfo exUser = new UserInfo();
                    byte[] binfo;
                    if (webService.GetUser(transactionid, user.UserID.ToString(), out binfo, out message))
                    {
                        exUser = (UserInfo)JsonHelper.DeserializeObject(binfo);
                        webService.DisableMailbox(transactionid, user.UserID.ToString(), out message);
                    }
                    
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "禁用并移动用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}禁用并移动用户。" +
                      $"用户名称：{user.DisplayName}（{user.UserAccount}）";
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("DisableAndMoveUserByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteUserByInterface(Guid transactionid, AdminInfo admin, UserInfo user, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";

            string funname = "DeleteUserByInterface";

            try
            {
                do
                {
                    UserProvider provider = new UserProvider();
                    if (!provider.GetUserInfo(transactionid, ref user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!provider.DeleteUser(transactionid, user, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "删除用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除用户。" +
                      $"用户名称：{user.DisplayName}（{user.UserAccount}）";
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("DeleteUserByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ResumeUserByInterface(Guid transactionid, AdminInfo admin, UserInfo info, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||TargetOuID:{info.ParentOuId}";
            paramstr += $"||UserID:{info.UserID}";
            paramstr += $"||CreateMail:{info.IsCreateMail}";

            string funname = "ResumeUserByInterface";

            try
            {
                do
                {
                    UserProvider userProvider = new UserProvider();
                    UserInfo user = new UserInfo();
                    user.UserID = info.UserID;
                    if (!userProvider.GetUserInfo(transactionid, ref user, out error))
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        result = false;
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(info.ParentOuId, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("ResumeUserByInterface调用GetADEntryByGuid获取targetOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string ouname = Convert.ToString(ouEntry.Properties["name"].Value);

                    if (!userProvider.MoveAndEnableUser(transactionid, user, info.ParentOuId, out error))
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        result = false;
                        break;
                    }

                    if (info.UseDefaultPassword)
                    {
                        user.Password = ConfigADProvider.GetDefaultPassword();
                        userProvider.ResetUserPassword(transactionid, admin, ref user, out error);
                    }

                    if (info.IsCreateMail)
                    {
                        if (!EnableUserMailBox(transactionid, user, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    GroupInfo group = new GroupInfo();
                    GroupProvider groupProvider = new GroupProvider();
                    if (groupProvider.GetHabGroupInfoByOu(transactionid, info.ParentOuId, out group, out error))
                    {
                        //恢复组中成员
                        groupProvider.AddGroupMember(transactionid, group.GroupID, info.UserID, out error);
                    }

                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    //添加日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "恢复用户";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}恢复用户。" +
                      $"用户名称：{user.DisplayName}（{user.UserAccount}）" +
                      $"恢复至：{ouname}";
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("ResumeUserByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        #endregion

        public bool SendSmsCode(Guid transactionid, string mobile, string code)
        {
            string accessKey = ConfigHelper.ConfigInstance["SMS_AccessKeyID"];
            string accessKeySecret = ConfigHelper.ConfigInstance["SMS_AccessKeySecret"];
            string TemplateCode = ConfigHelper.ConfigInstance["TemplateCode"];
            string SignName = ConfigHelper.ConfigInstance["SignName"];

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKey, accessKeySecret);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            CommonRequest request = new CommonRequest();
            request.Method = MethodType.POST;
            request.Domain = "dysmsapi.aliyuncs.com";
            request.Version = "2017-05-25";
            request.Action = "SendSms";
            // request.Protocol = ProtocolType.HTTP;

            request.AddQueryParameters("PhoneNumbers", mobile);
            request.AddQueryParameters("SignName", SignName);
            request.AddQueryParameters("TemplateCode", TemplateCode);
            request.AddQueryParameters("TemplateParam", "{\"code\":\"" + code + "\"}");

            try
            {
                CommonResponse response = client.GetCommonResponse(request);
                string resultmessage = System.Text.Encoding.UTF8.GetString(response.HttpResponse.Content);
                if (resultmessage.ToLower().Contains("ok"))
                {
                    LoggerHelper.Info(mobile, "SendSmsCode", mobile, resultmessage, true, transactionid);
                    return true;
                }
                else
                {
                    LoggerHelper.Error("SendSmsCode", mobile, resultmessage, transactionid);
                    return false;
                }
            }
            catch (System.Runtime.Remoting.ServerException e)
            {
                LoggerHelper.Error("SendSmsCode", mobile, e.ToString(), transactionid);
                return false;
            }
            catch (ClientException e)
            {
                LoggerHelper.Error("SendSmsCode", mobile, e.ToString(), transactionid);
                return false;
            }
        }

        public bool MatchPassword(string password)
        {
            var regex = new Regex(@"(?=.*[0-9])(?=.*[a-zA-Z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]).{8,20}", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            return regex.IsMatch(password);
        }

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
    }
}
