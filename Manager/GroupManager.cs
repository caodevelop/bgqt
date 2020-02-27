using Entity;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provider.ADProvider;
using Newtonsoft.Json;
using System.DirectoryServices;

namespace Manager
{
    public class GroupManager
    {
        private string _clientip = string.Empty;
        public GroupManager(string ip)
        {
            _clientip = ip;
        }

        public bool AddGroup(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||DisplayName:{group.DisplayName}";
            paramstr += $"||Description:{group.Description}";
            paramstr += $"||Account:{group.Account}";
            paramstr += $"||Type:{group.Type}";
            paramstr += $"||ParentOuId:{group.ParentOuId}";

            string funname = "AddGroup";

            try
            {
                do
                {
                    error = group.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        result = false;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.ParentOuId, out ouEntry, out message))
                    {
                        result = false;
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        LoggerHelper.Error("AddGroup调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        break;
                    }

                    group.ParentDistinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);

                    //验证权限
                    if (!admin.ParamList.ContainsValue(RoleParamCode.AddGroup))
                    {
                        result = false;
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }

                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!roleManager.CheckManagerOuRole(transactionid, group.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                    {
                        result = false;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }

                    DirectoryEntry entry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleEntryByCN(ouEntry.Path, group.DisplayName, out entry, out message))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), group.DisplayName, out entry, out message))
                    {
                        error.Code = ErrorCode.RootHaveSameDisplayName;
                        error.SetInfo(Convert.ToString(entry.Parent.Properties["distinguishedName"].Value));
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!string.IsNullOrEmpty(group.Account))
                    {
                        List<DirectoryEntry> entries = new List<DirectoryEntry>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetRootADPath(), group.Account, SearchType.GroupAndUser, out entries, out message))
                        {
                            error.Code = ErrorCode.Exception;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                        if (entries.Count > 0)
                        {
                            
                            error.Code = ErrorCode.HaveSameAccount;
                            error.SetInfo(Convert.ToString(entries[0].Parent.Properties["distinguishedName"].Value));
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        GroupProvider provider = new GroupProvider();
                        if (group.Type == GroupType.GROUP_TYPE_UNIVERSAL_GROUP || group.Type == GroupType.GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL)
                        {
                            ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                            webService.Timeout = -1;

                            byte[] binfo = JsonHelper.SerializeObject(group);
                            if (!webService.NewDistributionGroup(transactionid, ref binfo, out message))
                            {
                                error.Code = ErrorCode.Exception;
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }

                            group = (GroupInfo)JsonHelper.DeserializeObject(binfo);

                            //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                            //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));

                            if (!string.IsNullOrEmpty(group.Description))
                            {
                                provider.ModifyGroup(transactionid, admin, group, out error);
                            }
                        }
                        else
                        {
                            if (!provider.AddGroup(transactionid, admin, ref group, out error))
                            {
                                result = false;
                                strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                                break;
                            }
                        }
                        //添加成员
                        if (group.Members.Count > 0)
                        {
                            for (int i = 0; i < group.Members.Count; i++)
                            {
                                if (!provider.AddGroupMember(transactionid, admin, group, group.Members[i], out error))
                                {
                                    continue;
                                }
                            }
                        }
                        List<Guid> newgroupadminids = new List<Guid>();
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
                                newgroupadminids.Add(group.Admins[i].UserID);

                                if (!provider.AddGroupManagedBy(transactionid, group.GroupID, group.Admins[i].UserID, out error))
                                {
                                    continue;
                                }
                            }
                        }

                        //hab组添加成员
                       // AddHabGroupMember(transactionid, admin, group.ParentOuId, group.GroupID, out error);

                        error.Code = ErrorCode.None;
                        string json = JsonConvert.SerializeObject(group);
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
                        operateLog.OperateType = "添加通讯组";
                        operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加通讯组。" +
                            $"通讯组名称：{group.DisplayName}";
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
                LoggerHelper.Error("AddGroup异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ModifyGroup(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||DisplayName:{group.DisplayName}";
            paramstr += $"||Description:{group.Description}";
            paramstr += $"||Account:{group.Account}";
            paramstr += $"||Type:{group.Type}";
            paramstr += $"||GroupID:{group.GroupID}";

            string funname = "ModifyGroup";

            try
            {
                do
                {
                    error = group.ModifyCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    GroupProvider provider = new GroupProvider();
                    GroupInfo oldgroup = new GroupInfo();
                    oldgroup.GroupID = group.GroupID;
                    if (!provider.GetGroupInfo(transactionid, admin, ref oldgroup, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #region 判断权限
                    RoleManager roleManager = new RoleManager(_clientip);

                    if (!roleManager.CheckManagerOuRole(transactionid, oldgroup.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                   
                    #endregion
                    if (oldgroup.DisplayName != group.DisplayName)
                    {
                        if (commonProvider.GetOneLevelSigleEntryByCN(ConfigADProvider.GetADPathByLdap(oldgroup.ParentDistinguishedName), group.DisplayName, out entry, out message))
                        {
                            error.Code = ErrorCode.HaveSameDisplayName;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), group.DisplayName, out entry, out message))
                        {
                            error.Code = ErrorCode.RootHaveSameDisplayName;
                            error.SetInfo(Convert.ToString(entry.Parent.Properties["distinguishedName"].Value));
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    if (!provider.ModifyGroup(transactionid, admin, group, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //添加成员
                    if (group.Members.Count > 0)
                    {
                        for (int i = 0; i < group.Members.Count; i++)
                        {
                            if (!provider.AddGroupMember(transactionid, admin, group, group.Members[i], out error))
                            {
                                continue;
                            }
                        }
                    }

                    List<Guid> newgroupuserids = new List<Guid>();
                    if (group.Admins.Count > 0)
                    {
                        for (int i = 0; i < group.Admins.Count; i++)
                        {
                            if (!commonProvider.GetADEntryByGuid(group.Admins[i].UserID, out entry, out message))
                            {
                                continue;
                            }

                            newgroupuserids.Add(group.Admins[i].UserID);
                            if (!provider.AddGroupManagedBy(transactionid, group.GroupID, group.Admins[i].UserID, out error))
                            {
                                continue;
                            }
                        }
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
                    operateLog.OperateType = "编辑通讯组";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}编辑通讯组。" +
                      $"原显示名称：{oldgroup.DisplayName}，现显示名称：{oldgroup.DisplayName}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("ModifyGroup异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteGroup(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{group.GroupID}";

            string funname = "DeleteGroup";

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

                    #region 判断权限
                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!roleManager.CheckManagerOuRole(transactionid, group.ParentDistinguishedName, admin.ControlLimitOuList, out error))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    else
                    {
                        if (!admin.ParamList.ContainsValue(RoleParamCode.DeleteGroup))
                        {
                            error.Code = ErrorCode.UserNotEnoughRole;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }
                    #endregion
                    if (!provider.DeleteGroup(transactionid, admin, ref group, out error))
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
                    operateLog.OperateType = "删除通讯组";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除通讯组。" +
                        $"通讯组名称：{group.DisplayName}，" +
                         $"通讯组位置：{group.DistinguishedName}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GroupManager调用DeleteGroup异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetGroupInfo(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{group.GroupID}";

            string funname = "GetGroupInfo";

            try
            {
                do
                {
                    //AD添加User
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
                LoggerHelper.Error("GroupManager调用GetGroupInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool AddHabGroup(Guid transactionid, AdminInfo admin, OuInfo ou, out ErrorCodeInfo error)
        {
            bool result = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string strGroupName = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||ParentId:{ou.id}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||ParentId:{ou.parentid}";

            try
            {
                do
                {
                    //获取Hab后缀参数,组合为HabGroup名称
                    string strHab_add = ConfigADProvider.GetHab_add();
                    strGroupName = ou.name + strHab_add;

                    //唯一性验证
                    DirectoryEntry TempGroup = null;
                    //cn同层下判断是否重复
                    CommonProvider commonProvider = new CommonProvider();
                    if (commonProvider.GetOneLevelSigleGroupEntryByName(ConfigADProvider.GetADPathByLdap(ou.distinguishedName), strGroupName, out TempGroup, out message))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        LoggerHelper.Error("AddHabGroup调用GetOneLevelSigleGroupEntryByName异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    //samaccountname 全dc下判断是否重复
                    if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), strGroupName, out TempGroup, out message))
                    {
                        error.Code = ErrorCode.HaveSameAccount;
                        error.SetInfo(Convert.ToString(TempGroup.Parent.Properties["distinguishedName"].Value));
                        LoggerHelper.Error("AddHabGroup调用GetEntryDataBysAMAccount异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string strTempMailUri = string.Empty;
                    int index = 0;
                    while (index < ou.name.Length && ConfigADProvider.CharIsLetter(ou.name[index]))
                    {
                        strTempMailUri += ou.name[index].ToString();
                        index++;
                    }
                    strTempMailUri = strTempMailUri + strHab_add + "@" + ConfigADProvider.GetADDomain();

                    GroupInfo group = new GroupInfo();
                    group.ParentOuId = ou.id;
                    group.DisplayName = strGroupName;
                    group.Type = GroupType.GROUP_TYPE_UNIVERSAL_GROUP;
                    group.Account = strTempMailUri;
                    group.ParentDistinguishedName = ou.distinguishedName;

                    byte[] binfo = JsonHelper.SerializeObject(group);
                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;

                    if (!webService.NewDistributionGroup(transactionid, ref binfo, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("AddHabGroup调用NewDistributionGroup异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                    group = (GroupInfo)JsonHelper.DeserializeObject(binfo);

                    //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                    //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));

                    GroupProvider groupProvider = new GroupProvider();
                    if (!groupProvider.ModifyHabGroup(transactionid, admin, group, out error))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("AddHabGroup调用ModifyHabGroup异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    GroupInfo parentGroup = new GroupInfo();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, ou.parentid, out parentGroup, out error))
                    {
                        LoggerHelper.Error("AddHabGroup调用GetHabGroupInfoByOu异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    if (!groupProvider.AddGroupMember(transactionid, parentGroup.GroupID, group.GroupID, out error))
                    {
                        LoggerHelper.Error("AddHabGroup调用AddGroupMember异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("AddHabGroup异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }

            return result;
        }

        public bool AddHabGroup(Guid transactionid, AdminInfo admin, OuInfo ou, int groupSenIndex, out GroupInfo group, out ErrorCodeInfo error)
        {
            bool result = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string strGroupName = string.Empty;
            group = new GroupInfo();

            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||ParentId:{ou.id}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||ParentId:{ou.parentid}";
            paramstr += $"||groupSenIndex:{groupSenIndex}";

            try
            {
                do
                {
                    //获取Hab后缀参数,组合为HabGroup名称
                    string strHab_add = ConfigADProvider.GetHab_add();
                    strGroupName = ou.name + strHab_add;

                    //唯一性验证
                    DirectoryEntry TempGroup = null;
                    //cn同层下判断是否重复
                    CommonProvider commonProvider = new CommonProvider();
                    if (commonProvider.GetOneLevelSigleGroupEntryByName(ConfigADProvider.GetADPathByLdap(ou.distinguishedName), strGroupName, out TempGroup, out message))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        LoggerHelper.Error("AddHabGroup调用GetOneLevelSigleGroupEntryByName异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    //samaccountname 全dc下判断是否重复
                    if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), strGroupName, out TempGroup, out message))
                    {
                        error.Code = ErrorCode.HaveSameAccount;
                        error.SetInfo(Convert.ToString(TempGroup.Parent.Properties["distinguishedName"].Value));
                        LoggerHelper.Error("AddHabGroup调用GetEntryDataBysAMAccount异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string strTempMailUri = string.Empty;
                    int index = 0;
                    while (index < ou.name.Length && ConfigADProvider.CharIsLetter(ou.name[index]))
                    {
                        strTempMailUri += ou.name[index].ToString();
                        index++;
                    }
                    strTempMailUri = strTempMailUri + strHab_add + "@" + ConfigADProvider.GetADDomain();

                    group.ParentOuId = ou.id;
                    group.DisplayName = strGroupName;
                    group.Type = GroupType.GROUP_TYPE_UNIVERSAL_GROUP;
                    group.Index = groupSenIndex;
                    group.ParentDistinguishedName = ou.distinguishedName;
                    group.Account = strTempMailUri;

                    byte[] binfo = JsonHelper.SerializeObject(group);
                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;

                    if (!webService.NewDistributionGroup(transactionid, ref binfo, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("AddHabGroup调用NewDistributionGroup异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                    group = (GroupInfo)JsonHelper.DeserializeObject(binfo);

                    //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                    //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));

                    GroupProvider groupProvider = new GroupProvider();
                    if (!groupProvider.ModifyHabGroup(transactionid, admin, group, out error))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("AddHabGroup调用ModifyHabGroup异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    GroupInfo parentGroup = new GroupInfo();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, ou.parentid, out parentGroup, out error))
                    {
                        LoggerHelper.Error("AddHabGroup调用GetHabGroupInfoByOu异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    if (!groupProvider.AddGroupMember(transactionid, parentGroup.GroupID, group.GroupID, out error))
                    {
                        LoggerHelper.Error("AddHabGroup调用AddGroupMember异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("AddHabGroup异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }

            return result;
        }

        public bool AddHabGroupNoAddMembers(Guid transactionid, AdminInfo admin, OuInfo ou, int groupSenIndex, out GroupInfo group, out ErrorCodeInfo error)
        {
            bool result = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string strGroupName = string.Empty;
            group = new GroupInfo();

            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||ParentId:{ou.id}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||ParentId:{ou.parentid}";
            paramstr += $"||groupSenIndex:{groupSenIndex}";

            try
            {
                do
                {
                    //获取Hab后缀参数,组合为HabGroup名称
                    string strHab_add = ConfigADProvider.GetHab_add();
                    strGroupName = ou.name + strHab_add;

                    //唯一性验证
                    DirectoryEntry TempGroup = null;
                    //cn同层下判断是否重复
                    CommonProvider commonProvider = new CommonProvider();
                    if (commonProvider.GetOneLevelSigleGroupEntryByName(ConfigADProvider.GetADPathByLdap(ou.distinguishedName), strGroupName, out TempGroup, out message))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        LoggerHelper.Error("AddHabGroupNoAddMembers调用GetOneLevelSigleGroupEntryByName异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    //samaccountname 全dc下判断是否重复
                    if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), strGroupName, out TempGroup, out message))
                    {
                        error.Code = ErrorCode.HaveSameAccount;
                        error.SetInfo(Convert.ToString(TempGroup.Parent.Properties["distinguishedName"].Value));
                        LoggerHelper.Error("AddHabGroupNoAddMembers调用GetEntryDataBysAMAccount异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string strTempMailUri = string.Empty;
                    int index = 0;
                    while (index < ou.name.Length && ConfigADProvider.CharIsLetter(ou.name[index]))
                    {
                        strTempMailUri += ou.name[index].ToString();
                        index++;
                    }
                    strTempMailUri = strTempMailUri + strHab_add + "@" + ConfigADProvider.GetADDomain();

                    group.ParentOuId = ou.id;
                    group.DisplayName = strGroupName;
                    group.Type = GroupType.GROUP_TYPE_UNIVERSAL_GROUP;
                    group.Index = groupSenIndex;
                    group.ParentDistinguishedName = ou.distinguishedName;
                    group.Account = strTempMailUri;

                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;

                    byte[] binfo = JsonHelper.SerializeObject(group);
                    if (!webService.NewDistributionGroup(transactionid, ref binfo, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("AddHabGroupNoAddMembers调用NewDistributionGroup异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    group = (GroupInfo)JsonHelper.DeserializeObject(binfo);

                    //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                    //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));

                    GroupProvider provider = new GroupProvider();
                    if (!provider.ModifyHabGroup(transactionid, admin, group, out error))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("AddHabGroupNoAddMembers调用ModifyHabGroup异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("AddHabGroupNoAddMembers异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }

            return result;
        }

        public bool ReNameHabGroup(Guid transactionid, AdminInfo admin, OuInfo ou, out ErrorCodeInfo error)
        {
            bool result = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string strGroupName = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||ParentId:{ou.id}";
            paramstr += $"||Name:{ou.name}";

            try
            {
                do
                {
                    string strHab_add = ConfigADProvider.GetHab_add();
                    strGroupName = ou.name + strHab_add;

                    GroupInfo group = new GroupInfo();
                    GroupProvider groupProvider = new GroupProvider();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, ou.id, out group, out error))
                    {
                        LoggerHelper.Error("GroupManager调用GetHabGroupInfoByOu异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    if (!groupProvider.RenameHabGroup(transactionid, group, strGroupName, out error))
                    {
                        LoggerHelper.Error("GroupManager调用ReNameHabGroup异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    string strTempMailUri = string.Empty;
                    int index = 0;
                    while (index < ou.name.Length && ConfigADProvider.CharIsLetter(ou.name[index]))
                    {
                        strTempMailUri += ou.name[index].ToString();
                        index++;
                    }
                    strTempMailUri = strTempMailUri + strHab_add + "@" + ConfigADProvider.GetADDomain();

                    string strHab_CustomName = ConfigADProvider.GetHab_CustomName();
                    string strHab_CustomValue = ConfigADProvider.GetHab_CustomValue();

                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;

                    if (!webService.SetDistributionGroup(transactionid, group.GroupID.ToString(), strGroupName, strTempMailUri, out message))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("GroupManager调用ReNameHabGroup异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("GroupManager调用AddHabGroup异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }

            return result;
        }

        public bool AddHabGroupMember(Guid transactionid, AdminInfo admin, Guid ouId, Guid memberId, out ErrorCodeInfo error)
        {
            bool result = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string strGroupName = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||ParentId:{ouId}";
            paramstr += $"||MemberId:{memberId}";

            try
            {
                do
                {
                    GroupInfo parentHabGroup = new GroupInfo();
                    GroupProvider groupProvider = new GroupProvider();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, ouId, out parentHabGroup, out error))
                    {
                        LoggerHelper.Error("GroupManager调用GetHabGroupInfoByOu异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    if (!groupProvider.AddGroupMember(transactionid, parentHabGroup.GroupID, memberId, out error))
                    {
                        LoggerHelper.Error("GroupManager调用AddHabGroupMember异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("GroupManager调用AddHabGroupMember异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }

            return result;
        }

        public bool AddHabGroupMembers(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            strJsonResult = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||GroupId:{group.GroupID}";
            paramstr += $"||Members:";
            for (int i = 0; i < group.Members.Count; i++)
            {
                paramstr += "MemberID:" + group.Members[i].ID.ToString() + ",";
            }
            string funname = "AddHabGroupMembers";

            try
            {
                do
                {
                    GroupProvider groupProvider = new GroupProvider();
                    GroupInfo oldgroup = new GroupInfo();
                    oldgroup.GroupID = group.GroupID;
                    if (!groupProvider.GetGroupInfo(transactionid, admin, ref oldgroup, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!groupProvider.ClearGroupMembers(transactionid, group, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    string members = string.Empty;
                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    for (int i = 0; i < group.Members.Count; i++)
                    {
                        if (commonProvider.GetADEntryByGuid(group.Members[i].ID, out entry, out message))
                        {
                            if (!groupProvider.AddGroupMember(transactionid, group.GroupID, group.Members[i].ID, out error))
                            {
                                LoggerHelper.Error("GroupManager调用AddHabGroupMember异常", paramstr, error.Info, transactionid);
                                continue;
                            }

                            members += Convert.ToString(entry.Properties["name"].Value) + "，";
                        }
                    }

                    members = string.IsNullOrEmpty(members) ? string.Empty : members.Remove(members.LastIndexOf('，'), 1);

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
                    operateLog.OperateType = "添加 HAB 通讯组成员";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加 HAB 通讯组成员。" +
                      $"通讯组为：{oldgroup.DisplayName}，" +
                      $"成员为：{members}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GroupManager调用AddHabGroupMembers异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }

            return result;
        }

        public bool AddHabGroupMembers1(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            strJsonResult = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||GroupId:{group.GroupID}";
            paramstr += $"||Members:";
            for (int i = 0; i < group.Members.Count; i++)
            {
                paramstr += "MemberID:" + group.Members[i].ID.ToString() + ",";
            }
            string funname = "AddHabGroupMembers1";

            try
            {
                do
                {
                    GroupProvider groupProvider = new GroupProvider();
                    GroupInfo oldgroup = new GroupInfo();
                    oldgroup.GroupID = group.GroupID;
                    if (!groupProvider.GetGroupInfo(transactionid, admin, ref oldgroup, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    string members = string.Empty;
                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    for (int i = 0; i < group.Members.Count; i++)
                    {
                        if (commonProvider.GetADEntryByGuid(group.Members[i].ID, out entry, out message))
                        {
                            var queryresult = oldgroup.Members.Where(n => n.ID.Equals(group.Members[i].ID));
                            if (!queryresult.Any())
                            {
                                if (!groupProvider.AddGroupMember(transactionid, group.GroupID, group.Members[i].ID, out error))
                                {
                                    LoggerHelper.Error("GroupManager调用AddHabGroupMembers1异常", paramstr, error.Info, transactionid);
                                    continue;
                                }

                                members += Convert.ToString(entry.Properties["name"].Value) + "，";
                            }
                        }
                    }

                    members = string.IsNullOrEmpty(members) ? string.Empty : members.Remove(members.LastIndexOf('，'), 1);

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
                    operateLog.OperateType = "添加 HAB 通讯组成员";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加 HAB 通讯组成员。" +
                      $"通讯组为：{oldgroup.DisplayName}，" +
                      $"成员为：{members}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GroupManager调用AddHabGroupMembers1异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }

            return result;
        }

        public bool GetHabGroupMember(Guid transactionid, AdminInfo admin, Guid groupID, int curpage, int pagesize, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            BaseListInfo items = new BaseListInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{groupID}";

            string funname = "GetHabGroupMember";

            try
            {
                do
                {
                    //AD添加User
                    GroupInfo group = new GroupInfo();
                    group.GroupID = groupID;
                    GroupProvider provider = new GroupProvider();
                    if (!provider.GetGroupInfo(transactionid, admin, ref group, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    items.RecordCount = group.Members.Count;
                    items.PageCount = (items.RecordCount + pagesize - 1) / pagesize;

                    items.Lists = group.Members.Skip((curpage - 1) * pagesize).Take(pagesize).OrderByDescending(m => m.Index).ThenByDescending(m => m.Type).ThenBy(m => m.DisplayName).ToList<object>();

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(items);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GroupManager调用GetGroupInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SetHabGroupIndex(Guid transactionid, AdminInfo admin, List<GroupMember> members, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string funname = "SetHabGroupIndex";
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||Members:";
            for (int i = 0; i < members.Count; i++)
            {
                paramstr += "MemberID:" + members[i].ID.ToString() + " Index：" + members[i].Index + ",";
            }

            try
            {
                do
                {
                    GroupProvider groupProvider = new GroupProvider();
                    string message = string.Empty;
                    for (int i = 0; i < members.Count; i++)
                    {
                        GroupInfo group = new GroupInfo();
                        group.GroupID = members[i].ID;
                        if (!groupProvider.GetGroupInfo(transactionid, admin, ref group, out error))
                        {
                            continue;
                        }
                        if (!groupProvider.SetHabGroupIndex(transactionid, members[i].ID, members[i].Index, out error))
                        {
                            continue;
                        }
                        message += $"通讯组：{ group.DisplayName } 编号：{members[i].Index}，";
                    }

                    message = string.IsNullOrEmpty(message) ? string.Empty : message.Remove(message.LastIndexOf('，'), 1);
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
                    operateLog.OperateType = "编辑 HAB 通讯组排序编号";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}编辑 HAB 通讯组排序编号。{message}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GroupManager调用SetHabGroupIndex异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SyncAllHabData(Guid transactionid, AdminInfo admin, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";

            string funname = "SyncAllHabData";

            try
            {
                do
                {
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    string rootPath = ConfigADProvider.GetCompanyOuDistinguishedName();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByPath(rootPath, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                        result = true;
                    }

                    GroupInfo habgroup = new GroupInfo();
                    SyncCompanyHabData(transactionid, admin, ouEntry.Guid, out habgroup, out error);

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
                    operateLog.OperateType = "全量同步 HAB 通讯组和成员";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}全量同步 HAB 通讯组和成员。";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GroupManager调用SyncAllHabData异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        private bool SyncCompanyHabData(Guid transactionid, AdminInfo admin, Guid ouId, out GroupInfo habGroup, out ErrorCodeInfo error)
        {
            habGroup = new GroupInfo();
            string strError = String.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"OuId:{ouId}";

            try
            {
                // get all child OU
                // get all child HAB Group/User/Contact/Mail Group
                OuInfo ouInfo = new OuInfo();
                List<OuInfo> childrenOuList = new List<OuInfo>();
                List<UserInfo> childrenUserList = new List<UserInfo>();
                List<GroupInfo> childrenGroupList = new List<GroupInfo>();
                List<GroupInfo> childrenHabGroupList = new List<GroupInfo>();

                List<GroupInfo> childhabList = new List<GroupInfo>();
                GroupProvider groupProvider = new GroupProvider();
                CommonProvider commonProvider = new CommonProvider();
                commonProvider.GetOUOneLevelChildMembers(ouId, out ouInfo,
                    out childrenOuList, out childrenUserList, out childrenGroupList,
                    out childrenHabGroupList, out strError);

                // foreach child OU SyncOU(childOU)
                foreach (OuInfo ou in childrenOuList)
                {
                    GroupInfo childHabGroup = new GroupInfo();
                    SyncCompanyHabData(transactionid, admin, ou.id, out childHabGroup, out error);
                    if (childHabGroup.GroupID != Guid.Empty)
                    {
                        childhabList.Add(childHabGroup);
                    }
                }

                #region hab group funtion
                string _AddHab = ConfigADProvider.GetHab_add();

                String groupName = ouInfo.name + _AddHab;
                string ouName1 = ouInfo.name;
                if (ouInfo.distinguishedName == ConfigADProvider.GetCompanyOuDistinguishedName())
                {
                    if (!groupProvider.UpdateHabGroupMember(groupName, childhabList, childrenUserList, childrenGroupList, out habGroup, out strError))
                    {
                        LoggerHelper.Error("GroupManager调用SyncCompanyHabData异常", paramstr, "UpdateGroupMember RootOU is error.  " + strError, transactionid);
                        return false;
                    }
                }
                else
                {
                    // delete all other HAB Group
                    int groupSenIndex = 1;
                    for (int i = 0; i < childrenHabGroupList.Count; i++)
                    {
                        GroupInfo habgroup = childrenHabGroupList[i];
                        string ouName = ouInfo.name + _AddHab;
                        if (habgroup.DisplayName == ouName)
                        {
                            if (habgroup.Index != 0)
                            {
                                groupSenIndex = habgroup.Index;
                            }
                        }
                        groupProvider.DeleteGroup(transactionid, admin, ref habgroup, out error);
                    }
                    String groupEmail = String.Empty;
                    int index = 0;
                    while (index < ouName1.Length && OuManager.CharIsLetter(ouName1[index]))
                    {
                        groupEmail += ouName1[index].ToString();
                        index++;
                    }
                    if (!String.IsNullOrEmpty(groupEmail))
                    {
                        if (AddHabGroupNoAddMembers(transactionid, admin, ouInfo, groupSenIndex, out habGroup, out error))
                        {
                            if (!groupProvider.UpdateHabGroupMember(groupName, childhabList, childrenUserList, childrenGroupList, out habGroup, out strError))
                            {
                                LoggerHelper.Error("GroupManager调用SyncCompanyHabData异常", paramstr, "UpdateGroupMember RootOU is error.  " + strError, transactionid);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        LoggerHelper.Error("GroupManager调用SyncCompanyHabData异常", paramstr, "groupName 不包含英文或数字. groupName is :" + groupName, transactionid);
                        return false;
                    }
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                String exerr = ex.ToString();
                LoggerHelper.Error("GroupManager调用SyncCompanyHabData异常", paramstr, ex.ToString(), transactionid);
                return false;
            }
        }

        public bool SyncAppointHabData(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{group.GroupID}";

            string funname = "SyncAppointHabData";

            try
            {
                do
                {
                    DirectoryEntry groupEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out groupEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                        result = true;
                    }

                    string displayname = Convert.ToString(groupEntry.Properties["name"].Value);
                    GroupInfo habgroup = new GroupInfo();
                    SyncCompanyHabData(transactionid, admin, groupEntry.Parent.Guid, out habgroup, out error);

                    //处理指定节点后，需要将自己加到上级节点
                    if (groupEntry.Parent.Path != ConfigADProvider.GetCompanyADRootPath())
                    {
                        GroupInfo parentHabGroup = new GroupInfo();
                        GroupInfo habGroup = new GroupInfo();
                        GroupProvider groupProvider = new GroupProvider();
                        if (!groupProvider.GetHabGroupInfoByOu(transactionid, groupEntry.Parent.Parent.Guid, out parentHabGroup, out error))
                        {
                            LoggerHelper.Error("GroupManager调用GetHabGroupInfoByOu异常", paramstr, error.Info, transactionid);
                            result = false;
                            break;
                        }

                        if (!groupProvider.GetHabGroupInfoByOu(transactionid, groupEntry.Parent.Guid, out habGroup, out error))
                        {
                            LoggerHelper.Error("GroupManager调用GetHabGroupInfoByOu异常", paramstr, error.Info, transactionid);
                            result = false;
                            break;
                        }

                        groupProvider.AddGroupMember(transactionid, parentHabGroup.GroupID, habGroup.GroupID, out error);
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
                    operateLog.OperateType = "同步指定 HAB 通讯组和成员";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}同步指定 HAB 通讯组和成员。" +
                           $"指定 HAB 通讯组：{displayname}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GroupManager调用SyncAppointHabData异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SyncHabMembers(Guid transactionid, AdminInfo admin, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";

            string funname = "SyncHabMembers";

            try
            {
                do
                {
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    string rootPath = ConfigADProvider.GetCompanyOuDistinguishedName();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByPath(rootPath, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                        result = true;
                    }

                    GroupInfo habgroup = new GroupInfo();
                    SyncCompanyHabMembers(transactionid, admin, ouEntry.Guid, out habgroup, out error);

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
                    operateLog.OperateType = "全量同步 HAB 通讯组下成员";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}全量同步 HAB 通讯组下成员。";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("GroupManager调用SyncHabMembers异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SyncCompanyHabMembers(Guid transactionid, AdminInfo admin, Guid ouId, out GroupInfo habGroup, out ErrorCodeInfo error)
        {
            habGroup = new GroupInfo();
            string strError = String.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"OuId:{ouId}";

            try
            {
                OuInfo ouInfo = new OuInfo();
                List<OuInfo> childrenOuList = new List<OuInfo>();
                List<UserInfo> childrenUserList = new List<UserInfo>();
                List<GroupInfo> childrenGroupList = new List<GroupInfo>();
                List<GroupInfo> childrenHabGroupList = new List<GroupInfo>();

                List<GroupInfo> childhabList = new List<GroupInfo>();
                GroupProvider groupProvider = new GroupProvider();
                CommonProvider commonProvider = new CommonProvider();
                commonProvider.GetOUOneLevelChildMembers(ouId, out ouInfo,
                    out childrenOuList, out childrenUserList, out childrenGroupList,
                    out childrenHabGroupList, out strError);

                // foreach child OU SyncOU(childOU)
                foreach (OuInfo ou in childrenOuList)
                {
                    GroupInfo childHabGroup = new GroupInfo();
                    SyncCompanyHabMembers(transactionid, admin, ou.id, out childHabGroup, out error);
                    if (childHabGroup.GroupID != Guid.Empty)
                    {
                        childhabList.Add(childHabGroup);
                    }
                }

                #region hab group funtion
                string _AddHab = ConfigADProvider.GetHab_add();

                String groupName = ouInfo.name + _AddHab;
                string ouName1 = ouInfo.name;
                if (!groupProvider.IncrementUpdateGroupMember(groupName, childhabList, childrenUserList, childrenGroupList, out habGroup, out strError))
                {
                    LoggerHelper.Error("GroupManager调用SyncCompanyHabMembers异常", paramstr, "UpdateGroupMember RootOU is error.  " + strError, transactionid);
                    return false;
                }

                #endregion
                return true;
            }
            catch (Exception ex)
            {
                String exerr = ex.ToString();
                LoggerHelper.Error("GroupManager调用SyncCompanyHabMembers异常", paramstr, ex.ToString(), transactionid);
                return false;
            }
        }

        public bool MoveHabGroup(Guid transactionid, Guid souOuId, Guid targetOuId, Guid ouId, out ErrorCodeInfo error)
        {
            bool result = true;
            string message = string.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"souOuId:{souOuId}";
            paramstr += $"||targetOuId:{targetOuId}";
            paramstr += $"||ouId:{ouId}";

            try
            {
                do
                {
                    GroupInfo souHabGroupInfo = new GroupInfo();
                    GroupProvider groupProvider = new GroupProvider();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, souOuId, out souHabGroupInfo, out error))
                    {
                        LoggerHelper.Error("OuManager调用MoveHabGroup异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    GroupInfo targetHabGroupInfo = new GroupInfo();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, targetOuId, out targetHabGroupInfo, out error))
                    {
                        LoggerHelper.Error("OuManager调用MoveHabGroup异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    GroupInfo habGroupInfo = new GroupInfo();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, ouId, out habGroupInfo, out error))
                    {
                        LoggerHelper.Error("OuManager调用MoveHabGroup异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    //从原组剔除 在新组添加 两步不能截断 但都要记录日志
                    //从原隶属于HabGroup组内剔除 
                    if (!groupProvider.RemoveHabMember(transactionid, habGroupInfo.GroupID, souHabGroupInfo, out error))
                    {
                        LoggerHelper.Error("OuManager调用MoveHabGroup异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    //隶属于现在的HabGroup组
                    if (!groupProvider.AddGroupMember(transactionid, targetHabGroupInfo.GroupID, habGroupInfo.GroupID, out error))
                    {
                        LoggerHelper.Error("OuManager调用MoveHabGroup异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                String exerr = ex.ToString();
                LoggerHelper.Error("GroupManager调用SyncCompanyHabMembers异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }

            return result;
        }

        public bool MoveHabGroupMembers(Guid transactionid, Guid souOuId, Guid targetOuId, Guid memberID, out ErrorCodeInfo error)
        {
            bool result = true;
            string message = string.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"souOuId:{souOuId}";
            paramstr += $"||targetOuId:{targetOuId}";
            paramstr += $"||memberID:{memberID}";

            try
            {
                do
                {
                    GroupInfo souHabGroupInfo = new GroupInfo();
                    GroupProvider groupProvider = new GroupProvider();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, souOuId, out souHabGroupInfo, out error))
                    {
                        LoggerHelper.Error("MoveHabGroupMembers调用GetHabGroupInfoByOu异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    GroupInfo targetHabGroupInfo = new GroupInfo();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, targetOuId, out targetHabGroupInfo, out error))
                    {
                        LoggerHelper.Error("MoveHabGroupMembers调用GetHabGroupInfoByOu异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }
                    
                    //从原组剔除 在新组添加 两步不能截断 但都要记录日志
                    //从原隶属于HabGroup组内剔除 
                    if (!groupProvider.RemoveHabMember(transactionid, memberID, souHabGroupInfo, out error))
                    {
                        LoggerHelper.Error("MoveHabGroupMembers调用RemoveHabMember异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    //隶属于现在的HabGroup组
                    if (!groupProvider.AddGroupMember(transactionid, targetHabGroupInfo.GroupID, memberID, out error))
                    {
                        LoggerHelper.Error("MoveHabGroupMembers调用AddGroupMember异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                String exerr = ex.ToString();
                LoggerHelper.Error("MoveHabGroupMembers异常", paramstr, error.Info, transactionid);
                result = false;
            }

            return result;
        }

        public bool getGroupsByParentDn(Guid transactionid, string DistinguishedName, string IsOrganizational, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||DistinguishedName:{DistinguishedName}";
            string funname = "getGroupsByParentDn";
            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    DirectoryEntry groupEntry = new DirectoryEntry();
                    List<HabAddressGroupInfo> lists = new List<HabAddressGroupInfo>();
                    if (string.IsNullOrEmpty(DistinguishedName))
                    {
                        string rootpath = ConfigADProvider.GetCompanyOuDistinguishedName();
                        DirectoryEntry roothabEntry = new DirectoryEntry();

                        if (!commonProvider.GetADEntryByPath(rootpath, out roothabEntry, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        if (!commonProvider.GetOneLevelSigleHabGroupEntry(roothabEntry.Path, out groupEntry, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(string.Empty, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        HabAddressGroupInfo info = new HabAddressGroupInfo();
                        for (int i = 0; i < groupEntry.Properties["member"].Count; i++)
                        {
                            info.Members.Add(Convert.ToString(groupEntry.Properties["member"][i]));
                        }
                        info.IsOrganizational = true;
                        info.SAMAccountType = Convert.ToInt32(groupEntry.Properties["SAMAccountType"].Value);
                        info.GroupType = Convert.ToInt32(groupEntry.Properties["grouptype"].Value);
                        info.isParent = groupEntry.Properties["member"].Count > 0 ? true : false;
                        info.iconOpen = null;
                        info.iconClose = null;
                        info.icon = null;
                        info.ObjectGUID = groupEntry.Guid.ToString();
                        info.Name = Convert.ToString(groupEntry.Properties["Name"].Value);
                        info.DisplayName = Convert.ToString(groupEntry.Properties["DisplayName"].Value);
                        info.DistinguishedName = Convert.ToString(groupEntry.Properties["DistinguishedName"].Value);
                        info.Company = Convert.ToString(groupEntry.Properties["Company"].Value);
                        info.CN = Convert.ToString(groupEntry.Properties["CN"].Value);
                        info.ObjectClass = groupEntry.SchemaClassName;
                        info.SAMAccountName = Convert.ToString(groupEntry.Properties["SAMAccountName"].Value);
                        for (int i = 0; i < groupEntry.Properties["proxyAddresses"].Count; i++)
                        {
                            info.ProxyAddress.Add(Convert.ToString(groupEntry.Properties["proxyAddresses"][i]));
                        }
                        info.SmtpEmail = Convert.ToString(groupEntry.Properties["mail"].Value);
                        info.SipEmail = null;
                        info.MSExchHideFromAddressLists = false;
                        info.HABSeniorityIndex = Convert.ToInt32(groupEntry.Properties["HABSeniorityIndex"].Value);
                        info.PostalCode = null;
                        info.AdminCount = 0;
                        info.Description = Convert.ToString(groupEntry.Properties["Description"].Value);
                        info.LegacyExchangeDN = Convert.ToString(groupEntry.Properties["LegacyExchangeDN"].Value);
                        info.Memberof = new string[0];
                        info.WhenChanged = Convert.ToDateTime(groupEntry.Properties["WhenChanged"].Value);
                        info.WhenCreated = Convert.ToDateTime(groupEntry.Properties["WhenCreated"].Value);
                        info.HasThumbnailPhoto = false;
                        info.ColorIndex = 0;
                        info.PhotoDisplayText = null;
                        info.MsExchRecipientTypeDetails = 0;
                        info.ExtensionAttribute1 = null;
                        info.ExtensionAttribute2 = null;
                        info.ExtensionAttribute3 = null;
                        info.ExtensionAttribute4 = null;
                        info.ExtensionAttribute5 = null;
                        info.ExtensionAttribute6 = null;
                        info.ExtensionAttribute7 = null;
                        info.CanonicalName = new string[0];
                        info.IpPhone = string.Empty;
                        info.HomePhone = string.Empty;

                        lists.Add(info);

                        strJsonResult = JsonConvert.SerializeObject(lists.OrderBy(x => x.HABSeniorityIndex).ThenBy(x => x.Name).ToList());
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

                        DirectoryEntry memberEntry = new DirectoryEntry();
                        for (int i = 0; i < groupEntry.Properties["member"].Count; i++)
                        {
                            HabAddressGroupInfo info = new HabAddressGroupInfo();

                            string memberDn = Convert.ToString(groupEntry.Properties["member"][i]);
                            if (!commonProvider.GetADEntryByPath(memberDn, out memberEntry, out message))
                            {
                                continue;
                            }

                            object[] objectClasses = memberEntry.Properties["objectClass"].Value == null ? new object[] { } : (object[])(memberEntry.Properties["objectClass"].Value);
                            foreach (string objectClass in objectClasses)
                            {
                                if (objectClass == "user")
                                {
                                    info.ObjectClass = "user";
                                    break;
                                }
                                else if (objectClass == "group")
                                {
                                    info.ObjectClass = "group";
                                    break;
                                }
                            }

                            if (info.ObjectClass == "group" && memberEntry.Properties["proxyAddresses"].Count > 0)
                            {
                                for (int j = 0; j < memberEntry.Properties["member"].Count; j++)
                                {
                                    info.Members.Add(Convert.ToString(memberEntry.Properties["member"][j]));
                                }

                                info.IsOrganizational = true;
                                info.SAMAccountType = Convert.ToInt32(memberEntry.Properties["SAMAccountType"].Value);
                                info.GroupType = Convert.ToInt32(memberEntry.Properties["grouptype"].Value);
                                info.isParent = memberEntry.Properties["member"].Count > 0 ? true : false;
                                info.iconOpen = null;
                                info.iconClose = null;
                                info.icon = null;
                                info.ObjectGUID = memberEntry.Guid.ToString();
                                info.Name = Convert.ToString(memberEntry.Properties["Name"].Value);
                                info.DisplayName = Convert.ToString(memberEntry.Properties["DisplayName"].Value);
                                info.DistinguishedName = Convert.ToString(memberEntry.Properties["DistinguishedName"].Value);
                                info.Company = Convert.ToString(memberEntry.Properties["Company"].Value);
                                info.CN = Convert.ToString(memberEntry.Properties["CN"].Value);
                                info.SAMAccountName = Convert.ToString(memberEntry.Properties["SAMAccountName"].Value);
                                for (int j = 0; j < memberEntry.Properties["proxyAddresses"].Count; j++)
                                {
                                    info.ProxyAddress.Add(Convert.ToString(memberEntry.Properties["proxyAddresses"][j]));
                                }
                                info.SmtpEmail = Convert.ToString(memberEntry.Properties["mail"].Value);
                                info.SipEmail = null;
                                info.MSExchHideFromAddressLists = false;
                                info.HABSeniorityIndex = Convert.ToInt32(memberEntry.Properties["HABSeniorityIndex"].Value);
                                info.PostalCode = null;
                                info.AdminCount = 0;
                                info.Description = Convert.ToString(memberEntry.Properties["Description"].Value);
                                info.LegacyExchangeDN = Convert.ToString(memberEntry.Properties["LegacyExchangeDN"].Value);
                                info.Memberof = new string[0];
                                info.WhenChanged = Convert.ToDateTime(memberEntry.Properties["WhenChanged"].Value);
                                info.WhenCreated = Convert.ToDateTime(memberEntry.Properties["WhenCreated"].Value);
                                info.HasThumbnailPhoto = false;
                                info.ColorIndex = 0;
                                info.PhotoDisplayText = null;
                                info.MsExchRecipientTypeDetails = 0;
                                info.ExtensionAttribute1 = null;
                                info.ExtensionAttribute2 = null;
                                info.ExtensionAttribute3 = null;
                                info.ExtensionAttribute4 = null;
                                info.ExtensionAttribute5 = null;
                                info.ExtensionAttribute6 = null;
                                info.ExtensionAttribute7 = null;
                                info.CanonicalName = new string[0];
                                info.IpPhone = string.Empty;
                                info.HomePhone = string.Empty;

                                lists.Add(info);
                            }
                        }

                        strJsonResult = JsonConvert.SerializeObject(lists.OrderBy(x => x.HABSeniorityIndex).ThenBy(x => x.Name).ToList());
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

        #region samelevelOu
        public bool AddGroupNoHab(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||DisplayName:{group.DisplayName}";
            paramstr += $"||Description:{group.Description}";
            paramstr += $"||Account:{group.Account}";
            paramstr += $"||Type:{group.Type}";
            paramstr += $"||ParentOuId:{group.ParentOuId}";

            string funname = "AddGroupNoHab";

            try
            {
                do
                {
                    error = group.AddCheckProp();
                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.ParentOuId, out ouEntry, out message))
                    {
                        result = false;
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        LoggerHelper.Error("AddGroupNoHab调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        break;
                    }

                    group.ParentDistinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);

                    DirectoryEntry entry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleEntryByCN(ouEntry.Path, group.DisplayName, out entry, out message))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), group.DisplayName, out entry, out message))
                    {
                        error.Code = ErrorCode.RootHaveSameDisplayName;
                        error.SetInfo(Convert.ToString(entry.Parent.Properties["distinguishedName"].Value));
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!string.IsNullOrEmpty(group.Account))
                    {
                        List<DirectoryEntry> entries = new List<DirectoryEntry>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetRootADPath(), group.Account, SearchType.GroupAndUser, out entries, out message))
                        {
                            error.Code = ErrorCode.Exception;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                        if (entries.Count > 0)
                        {
                            error.Code = ErrorCode.HaveSameAccount;
                            error.SetInfo(Convert.ToString(entries[0].Parent.Properties["distinguishedName"].Value));
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        GroupProvider provider = new GroupProvider();
                        if (group.Type == GroupType.GROUP_TYPE_UNIVERSAL_GROUP || group.Type == GroupType.GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL)
                        {
                            ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                            webService.Timeout = -1;

                            byte[] binfo = JsonHelper.SerializeObject(group);
                            if (!webService.NewDistributionGroup(transactionid, ref binfo, out message))
                            {
                                error.Code = ErrorCode.Exception;
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }

                            group = (GroupInfo)JsonHelper.DeserializeObject(binfo);

                            //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                            //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));

                            if (!string.IsNullOrEmpty(group.Description))
                            {
                                provider.ModifyGroup(transactionid, admin, group, out error);
                            }
                        }
                        else
                        {
                            if (!provider.AddGroup(transactionid, admin, ref group, out error))
                            {
                                result = false;
                                strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                                break;
                            }
                        }

                        //添加成员
                        if (group.Members.Count > 0)
                        {
                            for (int i = 0; i < group.Members.Count; i++)
                            {
                                if (!provider.AddGroupMember(transactionid, admin, group, group.Members[i], out error))
                                {
                                    continue;
                                }
                            }
                        }
                        
                        List<Guid> newgroupadminids = new List<Guid>();
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
                                newgroupadminids.Add(group.Admins[i].UserID);

                                if (!provider.AddGroupManagedBy(transactionid, group.GroupID, group.Admins[i].UserID, out error))
                                {
                                    continue;
                                }
                            }
                        }

                        error.Code = ErrorCode.None;
                        string json = JsonConvert.SerializeObject(group);
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
                        operateLog.OperateType = "添加通讯组";
                        operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加通讯组。" +
                            $"通讯组名称：{group.DisplayName}";
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
                LoggerHelper.Error("AddGroupNoHab异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        #endregion

        #region Interface
        public bool AddGroupByInterface(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||DisplayName:{group.DisplayName}";
            paramstr += $"||Description:{group.Description}";
            paramstr += $"||Account:{group.Account}";
            paramstr += $"||Type:{group.Type}";
            paramstr += $"||ParentOuId:{group.ParentOuId}";

            string funname = "AddGroupByInterface";

            try
            {
                do
                {
                    error = group.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        result = false;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.ParentOuId, out ouEntry, out message))
                    {
                        result = false;
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        LoggerHelper.Error("AddGroupByInterface调用GetADEntryByGuid获取ParentOu异常", paramstr, message, transactionid);
                        break;
                    }

                    group.ParentDistinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);

                    DirectoryEntry entry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleEntryByCN(ouEntry.Path, group.DisplayName, out entry, out message))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), group.DisplayName, out entry, out message))
                    {
                        error.Code = ErrorCode.RootHaveSameDisplayName;
                        error.SetInfo(Convert.ToString(entry.Parent.Properties["distinguishedName"].Value));
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!string.IsNullOrEmpty(group.Account))
                    {
                        List<DirectoryEntry> entries = new List<DirectoryEntry>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetRootADPath(), group.Account, SearchType.GroupAndUser, out entries, out message))
                        {
                            error.Code = ErrorCode.Exception;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                        if (entries.Count > 0)
                        {
                            error.Code = ErrorCode.HaveSameAccount;
                            error.SetInfo(Convert.ToString(entries[0].Parent.Properties["distinguishedName"].Value));
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        GroupProvider provider = new GroupProvider();
                        if (group.Type == GroupType.GROUP_TYPE_UNIVERSAL_GROUP || group.Type == GroupType.GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL)
                        {
                            ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                            webService.Timeout = -1;

                            byte[] binfo = JsonHelper.SerializeObject(group);
                            if (!webService.NewDistributionGroup(transactionid, ref binfo, out message))
                            {
                                error.Code = ErrorCode.Exception;
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }

                            group = (GroupInfo)JsonHelper.DeserializeObject(binfo);

                            //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                            //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));

                            if (!string.IsNullOrEmpty(group.Description))
                            {
                                provider.ModifyGroup(transactionid, admin, group, out error);
                            }
                        }
                        else
                        {
                            if (!provider.AddGroup(transactionid, admin, ref group, out error))
                            {
                                result = false;
                                strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                                break;
                            }
                        }

                        //添加成员
                        if (group.Members.Count > 0)
                        {
                            for (int i = 0; i < group.Members.Count; i++)
                            {
                                if (!provider.AddGroupMember(transactionid, admin, group, group.Members[i], out error))
                                {
                                    continue;
                                }
                            }
                        }
                        
                        List<Guid> newgroupadminids = new List<Guid>();
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
                                newgroupadminids.Add(group.Admins[i].UserID);

                                if (!provider.AddGroupManagedBy(transactionid, group.GroupID, group.Admins[i].UserID, out error))
                                {
                                    continue;
                                }
                            }
                        }

                        //hab组添加成员
                        AddHabGroupMember(transactionid, admin, group.ParentOuId, group.GroupID, out error);

                        Dictionary<string, object> dictionary = new Dictionary<string, object>();
                        dictionary.Add("GroupID", group.GroupID);

                        string json = JsonConvert.SerializeObject(dictionary);
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
                        operateLog.OperateType = "添加通讯组";
                        operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加通讯组。" +
                            $"通讯组名称：{group.DisplayName}";
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
                LoggerHelper.Error("AddGroupByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ModifyGroupByInterface(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||DisplayName:{group.DisplayName}";
            paramstr += $"||Description:{group.Description}";
            paramstr += $"||Account:{group.Account}";
            paramstr += $"||Type:{group.Type}";
            paramstr += $"||GroupID:{group.GroupID}";

            string funname = "ModifyGroupByInterface";

            try
            {
                do
                {
                    error = group.ModifyCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    GroupProvider provider = new GroupProvider();
                    GroupInfo oldgroup = new GroupInfo();
                    oldgroup.GroupID = group.GroupID;
                    if (!provider.GetGroupInfo(transactionid, admin, ref oldgroup, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (oldgroup.DisplayName != group.DisplayName)
                    {
                        if (commonProvider.GetOneLevelSigleEntryByCN(ConfigADProvider.GetADPathByLdap(oldgroup.ParentDistinguishedName), group.DisplayName, out entry, out message))
                        {
                            error.Code = ErrorCode.HaveSameDisplayName;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), group.DisplayName, out entry, out message))
                        {
                            error.Code = ErrorCode.RootHaveSameDisplayName;
                            error.SetInfo(Convert.ToString(entry.Parent.Properties["distinguishedName"].Value));
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }

                    if (!provider.ModifyGroup(transactionid, admin, group, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //添加成员
                    if (group.Members.Count > 0)
                    {
                        for (int i = 0; i < group.Members.Count; i++)
                        {
                            if (!provider.AddGroupMember(transactionid, admin, group, group.Members[i], out error))
                            {
                                continue;
                            }
                        }
                    }

                    List<Guid> newgroupuserids = new List<Guid>();
                    if (group.Admins.Count > 0)
                    {
                        for (int i = 0; i < group.Admins.Count; i++)
                        {
                            if (!commonProvider.GetADEntryByGuid(group.Admins[i].UserID, out entry, out message))
                            {
                                continue;
                            }

                            newgroupuserids.Add(group.Admins[i].UserID);
                            if (!provider.AddGroupManagedBy(transactionid, group.GroupID, group.Admins[i].UserID, out error))
                            {
                                continue;
                            }
                        }
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
                    operateLog.OperateType = "编辑通讯组";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}编辑通讯组。" +
                      $"原显示名称：{oldgroup.DisplayName}，现显示名称：{oldgroup.DisplayName}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("ModifyGroupByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteGroupByInterface(Guid transactionid, AdminInfo admin, GroupInfo group, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{group.GroupID}";

            string funname = "DeleteGroupByInterface";

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

                    if (!provider.DeleteGroup(transactionid, admin, ref group, out error))
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
                    operateLog.OperateType = "删除通讯组";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除通讯组。" +
                        $"通讯组名称：{group.DisplayName}，" +
                         $"通讯组位置：{group.DistinguishedName}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("DeleteGroupByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        #endregion

    }
}
