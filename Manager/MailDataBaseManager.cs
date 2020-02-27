using Common;
using Entity;
using Newtonsoft.Json;
using Provider.ADProvider;
using Provider.DBProvider;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class MailDataBaseManager
    {
        private string _clientip = string.Empty;
        public MailDataBaseManager(string ip)
        {
            _clientip = ip;
        }

        public bool GetMailDataBaseList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "GetMailDataBaseList";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    MailDataBaseDBProvider Provider = new MailDataBaseDBProvider();
                    if (!Provider.GetMailDataBaseList(transactionid, admin, curpage, pagesize, searchstr, out lists, out error))
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
                LoggerHelper.Error("MailDataBaseManager调用GetMailDataBaseList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetExchangeMailDBList(Guid transactionid, AdminInfo admin, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            string funname = "GetExchangeMailDBList";

            try
            {
                do
                {
                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;
                    byte[] maillist;
                    if (!webService.GetDatabaseList(transactionid, out maillist, out errormsg))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("MailDataBaseManager调用GetExchangeMailDBList异常", paramstr, errormsg, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                        result = false;
                    }

                    error.Code = ErrorCode.None;
                    List<MailDataBaseInfo> list = (List<MailDataBaseInfo>)JsonHelper.DeserializeObject(maillist);
                    string json = JsonConvert.SerializeObject(list.OrderBy(x => x.MailboxDB).ToList());
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;


                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailDataBaseManager调用GetExchangeMailDBList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetMailDataBaseInfo(Guid transactionid, AdminInfo admin, MailDataBaseInfo maildb, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            string funname = "GetMailDataBaseInfo";

            try
            {
                do
                {
                    MailDataBaseDBProvider Provider = new MailDataBaseDBProvider();
                    if (!Provider.GetMailDataBaseInfo(transactionid,admin, ref maildb, out error))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("MailDataBaseManager调用GetMailDataBaseInfo异常", paramstr, errormsg, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                        result = false;
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(maildb);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;


                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailDataBaseManager调用GetMailDataBaseInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool AddMailDataBase(Guid transactionid, AdminInfo admin, MailDataBaseInfo maildb, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||OuID:{maildb.OuID}";
            paramstr += $"||MailboxDB:{maildb.MailboxDB}";
            paramstr += $"||MailboxServer:{maildb.MailboxServer}";

            string funname = "AddMailDataBase";

            try
            {
                do
                {
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(maildb.OuID, out ouEntry, out errormsg))
                    {
                        result = false;
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        LoggerHelper.Error("GetADEntryByGuid异常", paramstr, errormsg, transactionid);
                        break;
                    }

                    maildb.OUdistinguishedName = ouEntry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["distinguishedName"].Value);
                    maildb.OuName = ouEntry.Properties["name"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["name"].Value);
                    
                    MailDataBaseDBProvider Provider = new MailDataBaseDBProvider();
                    if (!Provider.AddMailDataBase(transactionid, admin, ref maildb, out error))
                    {
                        result = false;
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        break;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(maildb);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info, json);
                    //添加日志
                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加邮箱数据库对应关系";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加邮箱数据库对应关系。" +
                        $"OU：{maildb.OUdistinguishedName}，" +
                        $"MailboxDataBase：{maildb.MailboxDB}";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;


                } while (false);
            }
            catch (Exception ex)
            {
                result = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailDataBaseManager调用AddMailDataBase异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
            }
            return result;
        }

        public bool DeleteMailDataBase(Guid transactionid, AdminInfo admin, MailDataBaseInfo maildb, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{maildb.ID}";
           
            string funname = "DeleteMailDataBase";

            try
            {
                do
                {
                    MailDataBaseDBProvider Provider = new MailDataBaseDBProvider();
                    if (!Provider.GetMailDataBaseInfo(transactionid, admin, ref maildb, out error))
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        result = false;
                        break;
                    }

                    if (!Provider.DeleteMailDataBase(transactionid, admin, maildb, out error))
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
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
                    operateLog.OperateType = "删除邮箱数据库对应关系";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除邮箱数据库对应关系。" +
                        $"OU：{maildb.OUdistinguishedName}，" +
                        $"MailboxDataBase：{maildb.MailboxDB}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailDataBaseManager调用DeleteMailDataBase异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ChangeMailDataBase(Guid transactionid, AdminInfo admin, MailDataBaseInfo maildb, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||OuID:{maildb.OuID}";
            paramstr += $"||MailboxDB:{maildb.MailboxDB}";

            string funname = "ChangeMailDataBase";

            try
            {
                do
                {
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(maildb.OuID, out ouEntry, out errormsg))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("GetADEntryByGuid异常", paramstr, errormsg, transactionid);
                        result = false;
                        break;
                    }

                    maildb.OUdistinguishedName = ouEntry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["distinguishedName"].Value);
                    maildb.OuName = ouEntry.Properties["name"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["name"].Value);

                    MailDataBaseDBProvider Provider = new MailDataBaseDBProvider();
                    MailDataBaseInfo oldinfo = new MailDataBaseInfo();
                    oldinfo.ID = maildb.ID;
                    if (!Provider.GetMailDataBaseInfo(transactionid, admin, ref oldinfo, out error))
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        result = false;
                        break;
                    }
                    if (!Provider.ChangeMailDataBase(transactionid, admin, maildb, out error))
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
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
                    operateLog.OperateType = "修改邮箱数据库对应关系";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改邮箱数据库对应关系。" +
                        $"原OU：{oldinfo.OUdistinguishedName}，现OU：{maildb.OUdistinguishedName}，" +
                        $"原MailboxDataBase：{oldinfo.MailboxDB}，现MailboxDataBase：{maildb.MailboxDB}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;


                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MailDataBaseManager调用ChangeMailDataBase异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
    }
}
