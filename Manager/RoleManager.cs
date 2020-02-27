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
    public class RoleManager
    {
        private string _clientip = string.Empty;
        public RoleManager(string ip)
        {
            _clientip = ip;
        }

        public bool GetRoleList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            string funname = "GetRoleList";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    RoleDBProvider Provider = new RoleDBProvider();
                    if (!Provider.GetRoleList(transactionid, admin, curpage, pagesize, out lists, out error))
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
                LoggerHelper.Error("RoleManager调用GetRoleList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetRoleModuleList(Guid transactionid, AdminInfo admin, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += "useraccount:" + admin.UserAccount;
            paramstr += "||UserID:" + admin.UserID;
            string funname = "GetRoleModuleList";

            try
            {
                do
                {
                    List<RoleModuleList> lists = new List<RoleModuleList>();
                    RoleDBProvider Provider = new RoleDBProvider();
                    if (!Provider.GetRoleModuleList(transactionid, admin, out lists, out error))
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
                LoggerHelper.Error("RoleManager调用GetRoleModuleList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetRoleInfo(Guid transactionid, AdminInfo admin, Guid roleID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += "useraccount:" + admin.UserAccount;
            paramstr += "||UserID:" + admin.UserID;
            paramstr += "||RoleID:" + roleID;
            string funname = "GetRoleInfo";

            try
            {
                do
                {
                    RoleInfo info = new RoleInfo();
                    RoleDBProvider Provider = new RoleDBProvider();
                    if (!Provider.GetRoleInfo(transactionid, admin, roleID, out info, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    if (info.UserList.Count > 0)
                    {
                        DirectoryEntry entry = new DirectoryEntry();
                        CommonProvider commonProvider = new CommonProvider();
                        for (int i = 0; i < info.UserList.Count; i++)
                        {
                            if (!commonProvider.GetADEntryByGuid(info.UserList[i].UserID, out entry, out message))
                            {
                                LoggerHelper.Error("GetRoleInfo调用GetADEntryByGuid异常", paramstr, message, transactionid);
                                continue;
                            }

                            info.UserList[i].DisplayName = entry.Properties["displayname"].Value == null ? "" : Convert.ToString(entry.Properties["displayname"].Value);
                            info.UserList[i].UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                        }
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(info);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("RoleManager调用GetRoleInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool AddRole(Guid transactionid, AdminInfo admin, RoleInfo role, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();

            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||RoleName:{role.RoleName}";
            //paramstr += $"||ControlLimit:{role.ControlLimit.ToString()}";
            //paramstr += $"||ControlLimitID:{role.ControlLimitID}";
            paramstr += $"||Members:";
            for (int i = 0; i < role.UserList.Count; i++)
            {
                paramstr += role.UserList[i].UserID + ",";
            }
            paramstr += $"||ControlLimitOus:";
            for (int i = 0; i < role.ControlLimitOuList.Count; i++)
            {
                paramstr += role.ControlLimitOuList[i].OuID + ",";
            }

            string funname = "AddRole";

            try
            {
                do
                {
                    error = role.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    RoleDBProvider provider = new RoleDBProvider();

                    List<ControlLimitOuInfo> controlLimitOus = new List<ControlLimitOuInfo>();
                    List<string> controlOUdistinguishedNames = new List<string>();
                    for (int i = 0; i < role.ControlLimitOuList.Count; i++)
                    {
                        if (!commonProvider.GetADEntryByGuid(role.ControlLimitOuList[i].OuID, out entry, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("AddRole调用GetADEntryByGuid异常", paramstr, message, transactionid);
                            result = false;
                            break;
                        }
                        string OUdistinguishedName = Convert.ToString(entry.Properties["distinguishedName"].Value);

                        if (!controlOUdistinguishedNames.Contains(OUdistinguishedName))
                        {
                            controlOUdistinguishedNames.Add(OUdistinguishedName);
                            ControlLimitOuInfo controlLimitOu = new ControlLimitOuInfo();
                            controlLimitOu.OuID = role.ControlLimitOuList[i].OuID;
                            controlLimitOu.OUdistinguishedName = OUdistinguishedName;
                            controlLimitOus.Add(controlLimitOu);
                        }

                    }
                    if (result)
                    {
                        if (controlOUdistinguishedNames.Count == 0)
                        {
                            error.Code = ErrorCode.ControlOUPathNotEmpty;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("AddRole异常", paramstr, error.Info, transactionid);
                            result = false;
                            break;
                        }

                        if (!CheckControlOUdistinguishedNames(transactionid, controlOUdistinguishedNames, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("AddRole异常", paramstr, error.Info, transactionid);
                            result = false;
                            break;
                        }

                        string members = string.Empty;
                        for (int i = 0; i < role.UserList.Count; i++)
                        {
                            if (!commonProvider.GetADEntryByGuid(role.UserList[i].UserID, out entry, out message))
                            {
                                LoggerHelper.Error("AddRole调用GetADEntryByGuid异常", paramstr, message, transactionid);
                                continue;
                            }

                            string DisplayName = entry.Properties["cn"].Value == null ? "" : Convert.ToString(entry.Properties["cn"].Value);
                            string UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);

                            AdminInfo userRole = new AdminInfo();
                            if (provider.GetUserRole(transactionid, role.UserList[i].UserID, ref userRole, out error))
                            {
                                error.Code = ErrorCode.UserHaveRole;
                                string errormessage = DisplayName + "(" + UserAccount + ") 已存在角色";
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), errormessage);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                LoggerHelper.Error("AddRole调用GetUserRole异常", paramstr, errormessage, transactionid);
                                result = false;
                                break;
                            }

                            members += DisplayName + "(" + UserAccount + ")，";
                        }
                        if (result)
                        {
                            members = string.IsNullOrEmpty(members) ? string.Empty : members.Remove(members.LastIndexOf('，'), 1);
                            //检查权限

                            List<RoleParam> roleParams = new List<RoleParam>();
                            for (int i = 0; i < role.RoleList.Count; i++)
                            {
                                foreach (RoleParam param in role.RoleList[i].RoleParamList)
                                {
                                    RoleParam roleParam = new RoleParam();
                                    if (provider.GetRoleParam(transactionid, param.ParamID, out roleParam, out error))
                                    {
                                        roleParams.Add(roleParam);
                                    }
                                }
                            }

                            var query = from r in roleParams where r.ParamCode.Equals("SameLevelOu") select r;
                            if (query.Any())
                            {
                                if (role.SameLevelOuList.Count == 0)
                                {
                                    error.Code = ErrorCode.MustHaveSameLevelOuPath;
                                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                    strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                    result = false;
                                    break;
                                }
                            }

                            //AD添加User
                            if (!provider.AddRole(transactionid, admin, ref role, out error))
                            {
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }

                            for (int i = 0; i < role.RoleList.Count; i++)
                            {
                                foreach (RoleParam param in role.RoleList[i].RoleParamList)
                                {
                                    if (!provider.AddRoleModuleParam(transactionid, role.RoleID, param, out error))
                                    {
                                        continue;
                                    }
                                }
                            }

                            for (int i = 0; i < role.UserList.Count; i++)
                            {
                                if (!provider.AddRoleMembers(transactionid, role.RoleID, role.UserList[i], out error))
                                {
                                    continue;
                                }
                            }

                            for (int i = 0; i < controlLimitOus.Count; i++)
                            {
                                if (!provider.AddControlLimitOu(transactionid, role.RoleID, controlLimitOus[i], out error))
                                {
                                    continue;
                                }
                            }

                            for (int i = 0; i < role.SameLevelOuList.Count; i++)
                            {
                                if (!provider.AddSameLevelOu(transactionid, role.RoleID, role.SameLevelOuList[i], out error))
                                {
                                    continue;
                                }
                            }
                            error.Code = ErrorCode.None;
                            string json = JsonConvert.SerializeObject(role);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                            strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                            #region 操作日志
                            LogInfo operateLog = new LogInfo();
                            operateLog.AdminID = admin.UserID;
                            operateLog.AdminAccount = admin.UserAccount;
                            operateLog.RoleID = admin.RoleID;
                            operateLog.ClientIP = _clientip;
                            operateLog.OperateResult = true;
                            operateLog.OperateType = "添加角色";
                            operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加角色。角色名称：{role.RoleName}，" +
                                $"管理范围：{role.ControlLimitPath}，成员：{members}";
                            LogManager.AddOperateLog(transactionid, operateLog);
                            #endregion
                            result = true;
                        }
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("RoleManager调用AddRole异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteRole(Guid transactionid, AdminInfo admin, RoleInfo role, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();

            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||RoleID:{role.RoleID}";

            string funname = "DeleteRole";
            try
            {
                do
                {
                    RoleDBProvider provider = new RoleDBProvider();
                    if (!provider.GetRoleInfo(transactionid, admin, role.RoleID, out role, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (role.IsDefault == 1)
                    {
                        error.Code = ErrorCode.IsDefaultRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!provider.DeleteRole(transactionid, admin, role, out error))
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
                    operateLog.OperateType = "删除角色";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除角色。" +
                        $"删除名称：{role.RoleName}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("RoleManager调用DeleteRole异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ChangeRole(Guid transactionid, AdminInfo admin, RoleInfo role, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();

            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||RoleID:{role.RoleID}";
            paramstr += $"||RoleName:{role.RoleName}";
            paramstr += $"||ControlLimit:{role.ControlLimit.ToString()}";
            paramstr += $"||ControlLimitID:{role.ControlLimitID}";
            paramstr += $"||Members:";
            for (int i = 0; i < role.UserList.Count; i++)
            {
                paramstr += role.UserList[i].UserID + ",";
            }

            string funname = "ChangeRole";

            try
            {
                do
                {
                    error = role.ChangeCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    RoleInfo oldrole = new RoleInfo();
                    RoleDBProvider provider = new RoleDBProvider();
                    if (!provider.GetRoleInfo(transactionid, admin, role.RoleID, out oldrole, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (oldrole.IsDefault == 1 && role.UserList.Count == 0)
                    {
                        error.Code = ErrorCode.MustHaveMember;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();

                    List<ControlLimitOuInfo> controlLimitOus = new List<ControlLimitOuInfo>();
                    List<string> controlOUdistinguishedNames = new List<string>();
                    for (int i = 0; i < role.ControlLimitOuList.Count; i++)
                    {
                        if (!commonProvider.GetADEntryByGuid(role.ControlLimitOuList[i].OuID, out entry, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("AddRole调用GetADEntryByGuid异常", paramstr, message, transactionid);
                            result = false;
                            break;
                        }
                        string OUdistinguishedName = Convert.ToString(entry.Properties["distinguishedName"].Value);

                        if (!controlOUdistinguishedNames.Contains(OUdistinguishedName))
                        {
                            controlOUdistinguishedNames.Add(OUdistinguishedName);
                            ControlLimitOuInfo controlLimitOu = new ControlLimitOuInfo();
                            controlLimitOu.OuID = role.ControlLimitOuList[i].OuID;
                            controlLimitOu.OUdistinguishedName = OUdistinguishedName;
                            controlLimitOus.Add(controlLimitOu);
                        }

                    }
                    if (result)
                    {
                        if (controlOUdistinguishedNames.Count == 0)
                        {
                            error.Code = ErrorCode.ControlOUPathNotEmpty;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("ChangeRole异常", paramstr, error.Info, transactionid);
                            result = false;
                            break;
                        }

                        if (!CheckControlOUdistinguishedNames(transactionid, controlOUdistinguishedNames, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("ChangeRole异常", paramstr, error.Info, transactionid);
                            result = false;
                            break;
                        }

                        string members = string.Empty;
                        for (int i = 0; i < role.UserList.Count; i++)
                        {
                            if (!commonProvider.GetADEntryByGuid(role.UserList[i].UserID, out entry, out message))
                            {
                                LoggerHelper.Error("ChangeRole调用GetADEntryByGuid异常", paramstr, message, transactionid);
                                continue;
                            }

                            string DisplayName = entry.Properties["cn"].Value == null ? "" : Convert.ToString(entry.Properties["cn"].Value);
                            string UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);

                            AdminInfo userRole = new AdminInfo();
                            if (provider.GetUserRole(transactionid, role.UserList[i].UserID, ref userRole, out error))
                            {
                                if (userRole.RoleID != role.RoleID)
                                {
                                    error.Code = ErrorCode.UserHaveRole;
                                    string errormessage = DisplayName + "(" + UserAccount + ") 已存在角色";
                                    strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), errormessage);
                                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                    LoggerHelper.Error("ChangeRole调用GetADEntryByGuid异常", paramstr, message, transactionid);
                                    result = false;
                                    break;
                                }
                            }

                            members += DisplayName + "(" + UserAccount + ")，";
                        }
                        members = string.IsNullOrEmpty(members) ? string.Empty : members.Remove(members.LastIndexOf('，'), 1);
                        if (result)
                        {
                            //检查权限
                            List<RoleParam> roleParams = new List<RoleParam>();
                            for (int i = 0; i < role.RoleList.Count; i++)
                            {
                                foreach (RoleParam param in role.RoleList[i].RoleParamList)
                                {
                                    RoleParam roleParam = new RoleParam();
                                    if (provider.GetRoleParam(transactionid, param.ParamID, out roleParam, out error))
                                    {
                                        roleParams.Add(roleParam);
                                    }
                                }
                            }

                            var query = from r in roleParams where r.ParamCode.Equals("SameLevelOu") select r;
                            if (query.Any())
                            {
                                if (role.SameLevelOuList.Count == 0)
                                {
                                    error.Code = ErrorCode.MustHaveSameLevelOuPath;
                                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                    strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                    result = false;
                                    break;
                                }
                            }

                            if (!provider.ChangeRole(transactionid, admin, role, out error))
                            {
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }

                            for (int i = 0; i < role.RoleList.Count; i++)
                            {
                                foreach (RoleParam param in role.RoleList[i].RoleParamList)
                                {
                                    if (!provider.AddRoleModuleParam(transactionid, role.RoleID, param, out error))
                                    {
                                        continue;
                                    }
                                }
                            }

                            for (int i = 0; i < role.UserList.Count; i++)
                            {
                                if (!provider.AddRoleMembers(transactionid, role.RoleID, role.UserList[i], out error))
                                {
                                    continue;
                                }
                            }

                            for (int i = 0; i < role.SameLevelOuList.Count; i++)
                            {
                                if (!provider.AddSameLevelOu(transactionid, role.RoleID, role.SameLevelOuList[i], out error))
                                {
                                    continue;
                                }
                            }

                            for (int i = 0; i < controlLimitOus.Count; i++)
                            {
                                if (!provider.AddControlLimitOu(transactionid, role.RoleID, controlLimitOus[i], out error))
                                {
                                    continue;
                                }
                            }
                            error.Code = ErrorCode.None;
                            string json = JsonConvert.SerializeObject(role);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                            strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                            #region 操作日志
                            LogInfo operateLog = new LogInfo();
                            operateLog.AdminID = admin.UserID;
                            operateLog.AdminAccount = admin.UserAccount;
                            operateLog.RoleID = admin.RoleID;
                            operateLog.ClientIP = _clientip;
                            operateLog.OperateResult = true;
                            operateLog.OperateType = "修改角色";
                            operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改角色。" +
                                $"原角色名称：{oldrole.RoleName}，现角色名称{role.RoleName}；" +
                                $"原管理范围：{oldrole.ControlLimitPath}，现管理范围：{role.ControlLimitPath}；" +
                                $"现成员：{members}";
                            LogManager.AddOperateLog(transactionid, operateLog);
                            #endregion

                            result = true;
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("RoleManager调用ChangeRole异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetSameLevelOuList(Guid transactionid, AdminInfo admin, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += "useraccount:" + admin.UserAccount;
            paramstr += "||UserID:" + admin.UserID;
            string funname = "GetSameLevelOuList";

            try
            {
                do
                {
                    List<SameLevelOuInfo> lists = new List<SameLevelOuInfo>();
                    RoleDBProvider Provider = new RoleDBProvider();
                    if (!Provider.GetSameLevelOuList(transactionid, admin, out lists, out error))
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
                LoggerHelper.Error("RoleManager调用GetSameLevelOuList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetAppAccessToken(Guid transactionid, string appid, string secret, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += "appid:" + appid;
            paramstr += "||secret:" + secret;
            string funname = "GetAppAccessToken";

            try
            {
                do
                {
                    AdminInfo info = new AdminInfo();
                    RoleDBProvider Provider = new RoleDBProvider();
                    if (!Provider.GetAppInfo(transactionid, appid, secret, ref info, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(appid, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    info.Token = TokenManager.GenToken(appid,secret);
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    dictionary.Add("accesstoken", info.Token);
                    dictionary.Add("expiresin", 7200);
                    string json = JsonConvert.SerializeObject(dictionary);
                    LoggerHelper.Info(appid, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = info.UserID;
                    operateLog.AdminAccount = "Interface";
                    operateLog.RoleID = info.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "Inertface 获取 AccessToken";
                    operateLog.OperateLog = "Inertface于" + DateTime.Now + "获取 AccessToken";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(appid, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("RoleManager调用GetAppAccessToken异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool CheckControlOUdistinguishedNames(Guid transactionid, List<string> oudistinguishedNames, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();

            try
            {
                do
                {
                    string rootOuPath = ConfigADProvider.GetCompanyOuDistinguishedName();
                    foreach (string oudistinguishedName in oudistinguishedNames)
                    {
                        string ouPath = oudistinguishedName;
                        do
                        {
                            ouPath = ouPath.Substring(ouPath.IndexOf(',') + 1);
                            if (oudistinguishedNames.Contains(ouPath))
                            {
                                bResult = false;
                                error.Code = ErrorCode.HaveParentOu;
                                error.SetInfo(oudistinguishedName);
                                break;
                            }

                        } while (ouPath.ToLower().Contains("ou="));
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("CheckControlOUdistinguishedNames", string.Empty, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }

            return bResult;
        }

        public bool CheckManagerOuRole(Guid transactionid, string oudistinguishedName, List<ControlLimitOuInfo> controlLimitOus, out ErrorCodeInfo error)
        {
            bool bResult = false;
            error = new ErrorCodeInfo();

            try
            {
                foreach (ControlLimitOuInfo controlLimitOu in controlLimitOus)
                {
                    if (oudistinguishedName.Contains(controlLimitOu.OUdistinguishedName))
                    {
                        bResult = true;
                        break;
                    }
                }
                if (!bResult)
                {
                    error.Code = ErrorCode.UserNotEnoughRole;
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("CheckManagerOuRole", string.Empty, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }

            return bResult;
        }

        public bool CheckManagerOuRole(Guid transactionid, string oudistinguishedName, List<SameLevelOuInfo> sameLevelOus, out ErrorCodeInfo error)
        {
            bool bResult = false;
            error = new ErrorCodeInfo();

            try
            {

                foreach (SameLevelOuInfo SameLevelOu in sameLevelOus)
                {
                    if (oudistinguishedName.Contains(SameLevelOu.SamelevelOuPath))
                    {
                        bResult = true;
                        break;
                    }
                }
                if (!bResult)
                {
                    error.Code = ErrorCode.UserNotEnoughRole;
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("CheckManagerOuRole", string.Empty, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }

            return bResult;
        }

        public bool CheckRootOu(Guid transactionid, string oudistinguishedName, List<ControlLimitOuInfo> controlLimitOus, List<SameLevelOuInfo> sameLevelOus, out ErrorCodeInfo error)
        {
            bool bResult = false;
            error = new ErrorCodeInfo();

            try
            {
                if (oudistinguishedName == ConfigADProvider.GetPublicOuDistinguishedName())
                {
                    bResult = true;
                }
                else
                {
                    foreach (ControlLimitOuInfo controlLimitOu in controlLimitOus)
                    {
                        if (oudistinguishedName.Equals(controlLimitOu.OUdistinguishedName))
                        {
                            bResult = true;
                            break;
                        }
                    }

                    foreach (SameLevelOuInfo sameLimitOu in sameLevelOus)
                    {
                        if (oudistinguishedName.Equals(sameLimitOu.SamelevelOuPath))
                        {
                            bResult = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("CheckRootOu", string.Empty, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }

            return bResult;
        }
    }
}
