using Common;
using Entity;
using Newtonsoft.Json;
using Provider.ADProvider;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class StaticGroupManager
    {
        private string _clientip = string.Empty;
        public StaticGroupManager(string ip)
        {
            _clientip = ip;
        }

        public bool GetStaticGroupList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out string strJsonResult)
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
            string funname = "GetStaticGroupList";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    BaseListInfo lists = new BaseListInfo();
                    if (!commonProvider.GetStaticGroupData(curpage, pagesize, searchstr, out lists, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("StaticGroupManager调用GetStaticGroupData异常", paramstr, message, transactionid);
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
                LoggerHelper.Error("StaticGroupManager调用GetStaticGroupData异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetStaticGroupInfo(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += "useraccount:" + admin.UserAccount;
            paramstr += "||UserID:" + admin.UserID;
            paramstr += "||GroupID:" + group.GroupID;
            string funname = "GetStaticGroupInfo";

            try
            {
                do
                {
                    GroupProvider provider = new GroupProvider();
                    if (!provider.GetGroupInfo(transactionid, admin, ref group, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(group);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("StaticGroupManager调用GetStaticGroupInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetStaticGroupInfo(Guid transactionid, AdminInfo admin, ref GroupInfo group, out ErrorCodeInfo error)
        {
            bool result = true;
            string message = string.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += "useraccount:" + admin.UserAccount;
            paramstr += "||UserID:" + admin.UserID;
            paramstr += "||GroupID:" + group.GroupID;
            string funname = "GetStaticGroupInfo";

            try
            {
                do
                {
                    GroupProvider provider = new GroupProvider();
                    if (!provider.GetGroupInfo(transactionid, admin, ref group, out error))
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("StaticGroupManager调用GetStaticGroupInfo异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }
            return result;
        }

        public bool ChangeStaticGroupInfo(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{group.GroupID}";
            for (int i = 0; i < group.Admins.Count; i++)
            {
                paramstr += $"||UserID:{group.Admins[i].UserID}";
            }

            string funname = "ChangeStaticGroupInfo";

            try
            {
                do
                {
                    GroupInfo oldgroup = new GroupInfo();
                    oldgroup.GroupID = group.GroupID;
                    if (!GetStaticGroupInfo(transactionid, admin, ref oldgroup, out error))
                    {
                        LoggerHelper.Error("StaticGroupManager调用GetStaticGroupInfo异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    group.DisplayName = oldgroup.DisplayName;
                    group.Account = oldgroup.Account;
                    group.Description = oldgroup.Description;

                    GroupProvider groupProvider = new GroupProvider();
                    groupProvider.ClearGroupManagedBy(transactionid, group, out error);

                    //判断管理员有效性
                    DirectoryEntry entry = new DirectoryEntry();
                    List<Guid> newgroupuserids = new List<Guid>();
                    CommonProvider commonProvider = new CommonProvider();
                    if (group.Admins.Count > 0)
                    {
                        for (int i = 0; i < group.Admins.Count; i++)
                        {
                            if (!commonProvider.GetADEntryByGuid(group.Admins[i].UserID, out entry, out message))
                            {
                                continue;
                            }

                            group.Admins[i].DisplayName = entry.Properties["cn"].Value == null ? "" : Convert.ToString(entry.Properties["cn"].Value);
                            group.Admins[i].UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                            group.AdminsName += group.Admins[i].DisplayName + "(" + group.Admins[i].UserAccount + ")，";
                            newgroupuserids.Add(group.Admins[i].UserID);

                            if (!groupProvider.AddGroupManagedBy(transactionid, group.GroupID, group.Admins[i].UserID, out error))
                            {
                                continue;
                            }
                        }
                    }
                    group.AdminsName = string.IsNullOrEmpty(group.AdminsName) ? string.Empty : group.AdminsName.Remove(group.AdminsName.LastIndexOf('，'), 1);

                    //Set Group Exchange
                    //if (!ExchangeProvider.SetDistributionGroupManagedBy(group.GroupID.ToString(), new List<Guid>(), newgroupuserids, ref message))
                    //{
                    //    error.Code = ErrorCode.Exception;
                    //    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                    //    LoggerHelper.Error("StaticGroupManager调用ChangeStaticGroupInfo异常", paramstr, message, transactionid);
                    //    strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                    //    result = false;
                    //}
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
                    operateLog.OperateType = "修改静态通讯组成员管理员";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改静态通讯组成员管理员。" +
                        $"组名称：{group.DisplayName}；" +
                        $"现成员管理员：{group.AdminsName}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

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
    }
}
