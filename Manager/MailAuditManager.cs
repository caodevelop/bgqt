using Common;
using Entity;
using Newtonsoft.Json;
using Provider.ADProvider;
using Provider.DBProvider;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;

namespace Manager
{
    public class MailAuditManager
    {
        private string _clientip = string.Empty;
        public MailAuditManager(string ip)
        {
            _clientip = ip;
        }

        public bool AddMailAudit(Guid transactionid, AdminInfo admin, MailAuditInfo mailAuditInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{mailAuditInfo.Group.GroupID}";
            paramstr += $"||Audits:";
            for (int i = 0; i < mailAuditInfo.Audits.Count; i++)
            {
                paramstr += mailAuditInfo.Audits[i].UserID + ",";
            }
                
            string funname = "AddMailAudit";

            try
            {
                do
                {
                    error = mailAuditInfo.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //判断审批人有效性
                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    List<UserInfo> audits = new List<UserInfo>();
                    List<Guid> users = new List<Guid>();
                    if (mailAuditInfo.Audits.Count > 0)
                    {
                        for (int i = 0; i < mailAuditInfo.Audits.Count; i++)
                        {
                            UserInfo user = new UserInfo();
                            if (!commonProvider.GetADEntryByGuid(mailAuditInfo.Audits[i].UserID, out entry, out message))
                            {
                                continue;
                            }

                            mailAuditInfo.Audits[i].DisplayName = entry.Properties["cn"].Value == null ? "" : Convert.ToString(entry.Properties["cn"].Value);
                            mailAuditInfo.Audits[i].UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                            mailAuditInfo.Audits[i].IsCreateMail = entry.Properties["mail"].Value == null ? false : true;

                            if (!mailAuditInfo.Audits[i].IsCreateMail)
                            {
                                error.Code = ErrorCode.UserNotExchange;
                                error.SetInfo(mailAuditInfo.Audits[i].DisplayName + "(" + mailAuditInfo.Audits[i].UserAccount + ")");
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }
                            user.UserID = mailAuditInfo.Audits[i].UserID;
                            user.DisplayName = mailAuditInfo.Audits[i].DisplayName;
                            user.UserAccount = mailAuditInfo.Audits[i].UserAccount;
                           
                            mailAuditInfo.AuditUsers += user.DisplayName + "(" + user.UserAccount + ")，";
                            users.Add(user.UserID);
                            audits.Add(user);
                        }
                    }

                    if (result)
                    {
                        mailAuditInfo.AuditUsers = string.IsNullOrEmpty(mailAuditInfo.AuditUsers) ? string.Empty : mailAuditInfo.AuditUsers.Remove(mailAuditInfo.AuditUsers.LastIndexOf('，'), 1);
                        DirectoryEntry groupEntry = new DirectoryEntry();
                        if (!commonProvider.GetADEntryByGuid(mailAuditInfo.Group.GroupID, out groupEntry, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("AddMailAudit调用GetADEntryByGuid异常", paramstr, message, transactionid);
                            result = false;
                            break;
                        }

                        mailAuditInfo.Group.Account = groupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(groupEntry.Properties["mail"].Value);
                        mailAuditInfo.Group.DisplayName = groupEntry.Properties["cn"].Value == null ? "" : Convert.ToString(groupEntry.Properties["cn"].Value);

                        ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                        webService.Timeout = -1;
                        //Set Group Exchange
                        if (!webService.SetDistributionGroupModeratedBy(transactionid, mailAuditInfo.Group.GroupID.ToString(), true, users.ToArray(), out message))
                        {
                            error.Code = ErrorCode.Exception;
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("MailAuditManager调用AddMailAudit异常", paramstr, message, transactionid);
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            result = false;
                            break;
                        }

                        //AD添加User
                        MailAuditDBProvider provider = new MailAuditDBProvider();
                        if (!provider.AddMailAudit(transactionid, admin, ref mailAuditInfo, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (UserInfo u in audits)
                        {
                            if (!provider.AddMailAuditUsers(transactionid, mailAuditInfo, u, out error))
                            {
                                continue;
                            }
                        }

                        error.Code = ErrorCode.None;
                        string json = JsonConvert.SerializeObject(mailAuditInfo);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                        #region 操作日志
                        LogInfo operateLog = new LogInfo();
                        operateLog.AdminID = admin.UserID;
                        operateLog.AdminAccount = admin.UserAccount;
                        operateLog.RoleID = admin.RoleID;
                        operateLog.ClientIP = _clientip;
                        operateLog.OperateResult = true;
                        operateLog.OperateType = "添加邮件审批规则";
                        operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加邮件审批规则。" +
                            $"审批人：{mailAuditInfo.AuditUsers}，" +
                            $"审批对象：{mailAuditInfo.Group.DisplayName}";

                        LogManager.AddOperateLog(transactionid, operateLog);
                        #endregion

                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailAuditManager调用AddMailAudit异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetMailAuditList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "GetMailAuditList";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    MailAuditDBProvider Provider = new MailAuditDBProvider();
                    if (!Provider.GetMailAuditList(transactionid, admin, curpage, pagesize, searchstr, out lists, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(lists);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailAuditManager调用GetMailAuditList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetMailAuditInfo(Guid transactionid, AdminInfo admin, MailAuditInfo mailAuditInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"ID:{mailAuditInfo.ID}";
            string funname = "GetMailAuditInfo";

            try
            {
                do
                {
                    MailAuditDBProvider Provider = new MailAuditDBProvider();
                    if (!Provider.GetMailAuditInfo(transactionid, admin, ref mailAuditInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(mailAuditInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailAuditManager调用GetMailAuditInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteMailAudit(Guid transactionid, AdminInfo admin, MailAuditInfo mailAuditInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{mailAuditInfo.ID}";

            string funname = "DeleteMailAudit";

            try
            {
                do
                {
                    MailAuditDBProvider Provider = new MailAuditDBProvider();
                    if (!Provider.GetMailAuditInfo(transactionid, admin, ref mailAuditInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!Provider.DeleteMailAudit(transactionid, admin, mailAuditInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;

                    //Set Group Exchange
                    if (!webService.SetDistributionGroupModeratedBy(transactionid, mailAuditInfo.Group.GroupID.ToString(), false, new List<Guid>().ToArray(), out message))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MailAuditManager调用AddMailAudit异常", paramstr, message, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        result = false;
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
                    operateLog.OperateType = "删除邮件审批规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除邮件审批规则。" +
                        $"对象：{mailAuditInfo.Group.DisplayName}，" +
                        $"审批人：{mailAuditInfo.AuditUsers}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailAuditManager调用DeleteMailAudit异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ModifyMailAudit(Guid transactionid, AdminInfo admin, MailAuditInfo mailAuditInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{mailAuditInfo.Group.GroupID}";
            for (int i = 0; i < mailAuditInfo.Audits.Count; i++)
            {
                paramstr += $"||AuditID:{mailAuditInfo.Audits[i].UserID}";
            }

            string funname = "ModifyMailAudit";

            try
            {
                do
                {
                    error = mailAuditInfo.ChangeCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    MailAuditDBProvider provider = new MailAuditDBProvider();
                    MailAuditInfo oldMailAuditInfo = new MailAuditInfo();
                    oldMailAuditInfo.ID = mailAuditInfo.ID;
                    if (!provider.GetMailAuditInfo(transactionid, admin, ref oldMailAuditInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //判断审批人有效性
                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    List<UserInfo> audits = new List<UserInfo>();
                    List<Guid> users = new List<Guid>();
                    if (mailAuditInfo.Audits.Count > 0)
                    {
                        for (int i = 0; i < mailAuditInfo.Audits.Count; i++)
                        {
                            if (!commonProvider.GetADEntryByGuid(mailAuditInfo.Audits[i].UserID, out entry, out message))
                            {
                                continue;
                            }

                            mailAuditInfo.Audits[i].DisplayName = entry.Properties["cn"].Value == null ? "" : Convert.ToString(entry.Properties["cn"].Value);
                            mailAuditInfo.Audits[i].UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                            mailAuditInfo.Audits[i].IsCreateMail = entry.Properties["mail"].Value == null ? false : true;

                            if (!mailAuditInfo.Audits[i].IsCreateMail)
                            {
                                error.Code = ErrorCode.UserNotExchange;
                                error.SetInfo(mailAuditInfo.Audits[i].DisplayName + "(" + mailAuditInfo.Audits[i].UserAccount + ")");
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }
                            
                            mailAuditInfo.AuditUsers += mailAuditInfo.Audits[i].DisplayName + "(" + mailAuditInfo.Audits[i].UserAccount + ")，";
                            users.Add(mailAuditInfo.Audits[i].UserID);
                            audits.Add(mailAuditInfo.Audits[i]);
                        }
                    }
                    if (result)
                    {
                        mailAuditInfo.AuditUsers = string.IsNullOrEmpty(mailAuditInfo.AuditUsers) ? string.Empty : mailAuditInfo.AuditUsers.Remove(mailAuditInfo.AuditUsers.LastIndexOf('，'), 1);
                        DirectoryEntry groupEntry = new DirectoryEntry();
                        if (!commonProvider.GetADEntryByGuid(mailAuditInfo.Group.GroupID, out groupEntry, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("ModifyMailAudit调用GetADEntryByGuid异常", paramstr, message, transactionid);
                            result = false;
                            break;
                        }

                        mailAuditInfo.Group.Account = groupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(groupEntry.Properties["mail"].Value);
                        mailAuditInfo.Group.DisplayName = groupEntry.Properties["cn"].Value == null ? "" : Convert.ToString(groupEntry.Properties["cn"].Value);

                        ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                        webService.Timeout = -1;
                        //Set Group Exchange
                        webService.SetDistributionGroupModeratedBy(transactionid, mailAuditInfo.Group.GroupID.ToString(), false, new List<Guid>().ToArray(), out message);
                        //Set Group Exchange
                        if (!webService.SetDistributionGroupModeratedBy(transactionid, mailAuditInfo.Group.GroupID.ToString(), true, users.ToArray(), out message))
                        {
                            error.Code = ErrorCode.Exception;
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("MailAuditManager调用AddMailAudit异常", paramstr, message, transactionid);
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            result = false;
                            break;
                        }

                        if (!provider.ModifyMailAudit(transactionid, admin, mailAuditInfo, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (UserInfo u in audits)
                        {
                            if (!provider.AddMailAuditUsers(transactionid, mailAuditInfo, u, out error))
                            {
                                continue;
                            }
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
                        operateLog.OperateType = "修改邮件审批规则";
                        operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改邮件审批规则。" +
                            $"原对象：{oldMailAuditInfo.Group.DisplayName}，现对象：{mailAuditInfo.Group.DisplayName}；" +
                            $"原审批人：{oldMailAuditInfo.AuditUsers}，现审批人：{mailAuditInfo.AuditUsers}";
                        LogManager.AddOperateLog(transactionid, operateLog);
                        #endregion

                        result = true;
                    }
                    
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailAuditManager调用ModifySensitiveMail异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetMailAuditInfoByExchange(Guid transactionid, AdminInfo admin, MailAuditInfo mailAuditInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"ID:{mailAuditInfo.Group.GroupID}";
            string funname = "GetMailAuditInfoByExchange";

            try
            {
                do
                {
                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;
                    //List<string> members = new List<string>();
                    string[] members = new List<string>().ToArray();
                    if (!webService.GetDistributionGroupModeratedBy(transactionid, mailAuditInfo.Group.GroupID.ToString(), out members, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, message, false, transactionid);
                        result = false;
                        break;
                    }

                    if (members.Length > 0)
                    {
                        foreach (string user in members)
                        {
                            UserInfo info = new UserInfo();
                           byte[] strbyte;
                            if (!webService.GetUser(transactionid, user, out strbyte, out message))
                            {
                                continue;
                            }
                            info = (UserInfo)JsonHelper.DeserializeObject(strbyte);
                            mailAuditInfo.Audits.Add(info);
                        }
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(mailAuditInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailAuditManager调用GetMailAuditInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
    }
}