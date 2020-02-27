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
    public class OuManager
    {
        private string _clientip = string.Empty;
        public OuManager(string ip)
        {
            _clientip = ip;
        }

        #region company
        public bool AddOu(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Description:{ou.description}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||ParentId:{ou.parentid}";

            string funname = "AddOu";

            try
            {
                do
                {
                    error = ou.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.parentid, out entry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用AddOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string parentOuPath = Convert.ToString(entry.Properties["distinguishedName"].Value);

                    //验证权限
                    if (!admin.ParamList.ContainsValue(RoleParamCode.AddOu))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!roleManager.CheckManagerOuRole(transactionid, parentOuPath, admin.ControlLimitOuList, out error))
                    {
                        result = false;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }

                   if (!VerificationOuNamePrefix(transactionid, string.Empty, ou.name, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                   
                   //AD添加Ou
                    OuProvider provider = new OuProvider();
                    if (!provider.AddOu(transactionid, admin, ref ou, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //Add RecycleOu
                   // provider.AddRecycleOu(transactionid, admin, ou, out error);

                    //Add HabGroup
                    //GroupManager groupManager = new GroupManager(_clientip);
                    //groupManager.AddHabGroup(transactionid, admin, ou, out error);

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(ou);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加 OU ";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加OU。" +
                         $"OU名称：{ou.name}，" +
                        $"父级OU路径：{parentOuPath}，";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用AddOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ModifyOu(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Description:{ou.description}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||Id:{ou.id}";

            string funname = "ModifyOu";

            try
            {
                do
                {
                    error = ou.ModifyCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.id, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用ModifyOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string ouName = ouEntry.Properties["name"].Value == null ? "" : Convert.ToString(ouEntry.Properties["name"].Value);
                    string ouPath = ouEntry.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(ouEntry.Properties["distinguishedName"].Value);
                    string parentPath = ouEntry.Parent.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(ouEntry.Parent.Properties["distinguishedName"].Value);
                    string recycleOuPath = ConfigADProvider.GetADRecycleOuLdapByLdap(Convert.ToString(ouEntry.Properties["distinguishedName"].Value));
                    
                    //检查权限
                    if (ouPath == ConfigADProvider.GetCompanyOuDistinguishedName())
                    {
                        error.Code = ErrorCode.RootOuNotOperate;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!admin.ParamList.ContainsValue(RoleParamCode.ModfiyName))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!roleManager.CheckManagerOuRole(transactionid, ouPath, admin.ControlLimitOuList, out error))
                    {
                        result = false;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }
                    
                    OuInfo recycleOuInfo = new OuInfo();
                    if (ouName != ou.name)
                    {
                        if (!VerificationOuNamePrefix(transactionid, ouName, ou.name, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        //查找重名
                        string ouParentPath = ouEntry.Parent.Path;
                        DirectoryEntry newEntry = new DirectoryEntry();
                        if (commonProvider.GetOneLevelSigleOuEntry(ouParentPath, ou.name, out newEntry, out message))
                        {
                            error.Code = ErrorCode.HaveSameDisplayName;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("OuManager调用ModifyOu异常", paramstr, message, transactionid);
                            result = false;
                            break;
                        }
                    }

                    //AD修改Ou
                    OuProvider provider = new OuProvider();
                    if (!provider.ModifyOu(transactionid, admin, ref ou, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (ouName != ou.name)
                    {
                        //修改数据库
                        OuDBProvider ouDBProvider = new OuDBProvider();
                        ouDBProvider.ModifyOu(transactionid, admin, ou, out error);

                        //AD 重命名HabGroup
                        //GroupManager groupManager = new GroupManager(_clientip);
                        //groupManager.ReNameHabGroup(transactionid, admin, ou, out error);

                        //provider.ModifyRecycleOu(transactionid, admin, recycleOuPath, ou.name, out error);
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
                    operateLog.OperateType = "编辑 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}编辑OU。" +
                      $"原OU名称：{ouName}，现OU名称：{ou.name}，" +
                      $"OU路径：{parentPath}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用ModifyOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteOu(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Id:{ou.id}";

            string funname = "DeleteOu";

            try
            {
                do
                {
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.id, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("DeleteOu调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    ou.parentid = ouEntry.Parent.Guid;
                    ou.name = Convert.ToString(ouEntry.Properties["name"].Value);
                    ou.distinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);

                    //验证权限
                    if (!admin.ParamList.ContainsValue(RoleParamCode.DeleteOu))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!roleManager.CheckManagerOuRole(transactionid, ou.distinguishedName, admin.ControlLimitOuList, out error))
                    {
                        result = false;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }

                    if (roleManager.CheckRootOu(transactionid, ou.distinguishedName, admin.ControlLimitOuList,admin.SameLevelOuList, out error))
                    {
                        result = false;
                        error.Code = ErrorCode.RootOuNotOperate;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }

                    //判断当前ou下是否还有子对象
                    List<DirectoryEntry> EntryList = new List<DirectoryEntry>();
                    if (!commonProvider.SearchEntryData(ouEntry.Path, SearchType.AllNoHab, out EntryList, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用DeleteOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    for (int i = EntryList.Count - 1; i >= 0; i--)
                    {
                        if (EntryList[i].Path == ouEntry.Path)
                        {
                            EntryList.RemoveAt(i);
                        }
                    }

                    if (EntryList.Count > 0)
                    {
                        error.Code = ErrorCode.HaveMembers;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用DeleteOu异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    //修改数据库
                    OuDBProvider ouDBProvider = new OuDBProvider();
                    ouDBProvider.DeleteOu(transactionid, admin, ou, out error);

                    //AD修改Ou
                    OuProvider provider = new OuProvider();
                    if (!provider.DeleteOu(transactionid, admin, ou, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //Add RecycleOu
                    //string recycleOuPath = ConfigADProvider.GetADRecycleOuLdapByLdap(ou.distinguishedName);
                    //provider.DeleteRecycleOu(transactionid, admin, recycleOuPath, out error);

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
                    operateLog.OperateType = "删除 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除OU。" +
                        $"OU名称：{ou.name}，" +
                         $"OU位置：{ou.distinguishedName}";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用DeleteOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetOuInfo(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Description:{ou.description}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||Id:{ou.id}";

            string funname = "GetOuInfo";

            try
            {
                do
                {
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.id, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用GetOuInfo异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    ou.name = ouEntry.Properties["name"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["name"].Value);
                    ou.description = ouEntry.Properties["description"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["description"].Value);
                    ou.IsProfessionalGroups = ouEntry.Properties["st"].Value == null ? false : Convert.ToBoolean(Convert.ToString(ouEntry.Properties["st"].Value));
                    ou.ou = ouEntry.Properties["name"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["name"].Value);
                    ou.parentid = ouEntry.Parent.Guid;
                    ou.parentDistinguishedName = ouEntry.Parent.Properties["description"].Value == null ? string.Empty : Convert.ToString(ouEntry.Parent.Properties["description"].Value);

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(ou);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用GetOuInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool MoveNodes(Guid transactionid, AdminInfo admin, MoveNodeInfo moveNode, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||TargetNodeID:{moveNode.TargetNode.NodeID}";
            paramstr += $"||MoveNodes:";
            for (int i = 0; i < moveNode.NodeList.Count; i++)
            {
                paramstr += "MoveNode:" + moveNode.NodeList[i].NodeID.ToString() + "(" + moveNode.NodeList[i].Type + "),";
            }

            string funname = "MoveNodes";

            try
            {
                do
                {
                    DirectoryEntry targetOuEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(moveNode.TargetNode.NodeID, out targetOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        result = false;
                        break;
                    }

                    DirectoryEntry souEntry = new DirectoryEntry();
                    string moveNodeMessage = string.Empty;
                    string errormessage = string.Empty;
                    foreach (NodeInfo node in moveNode.NodeList)
                    {
                        if (!commonProvider.GetADEntryByGuid(node.NodeID, out souEntry, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            errormessage = "移动对象不存在";
                            result = false;
                            break;
                        }

                        Guid souOuId = souEntry.Parent.Guid;
                        //to do cs: 判断管理员是有由跨Ou移动的权限
                        if (!admin.ParamList.ContainsValue(RoleParamCode.SpanOUMove))
                        {
                            RoleManager roleManager = new RoleManager(_clientip);
                            if (!roleManager.CheckManagerOuRole(transactionid, Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value), admin.ControlLimitOuList, out error))
                            {
                                result = false;
                                strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                                break;
                            }
                            else if (!roleManager.CheckManagerOuRole(transactionid, Convert.ToString(souEntry.Parent.Properties["distinguishedName"].Value), admin.ControlLimitOuList, out error))
                            {
                                result = false;
                                strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                                break;
                            }
                        }

                        //检查重名
                        DirectoryEntry newoude = new DirectoryEntry();
                        string strTempName = souEntry.Properties["name"].Value == null ? "" : Convert.ToString(souEntry.Properties["name"].Value);
                        if (Convert.ToString(souEntry.Parent.Properties["distinguishedName"].Value) != Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value))
                        {
                            if (commonProvider.GetOneLevelSigleEntryByCN(targetOuEntry.Path, strTempName, out newoude, out message))
                            {
                                error.Code = ErrorCode.MoveNodehaveSameAccount;
                                error.SetInfo(strTempName);
                                result = false;
                                break;
                            }
                        }
                    }
                    if (result)
                    {
                        DirectoryEntry newEntry = new DirectoryEntry();
                        foreach (NodeInfo node in moveNode.NodeList)
                        {
                            if (!commonProvider.GetADEntryByGuid(node.NodeID, out souEntry, out message))
                            {
                                error.Code = ErrorCode.SearchADDataError;
                                result = false;
                                break;
                            }

                            Guid souOuId = souEntry.Parent.Guid;

                            souEntry.MoveTo(targetOuEntry);
                            souEntry.CommitChanges();
                            souEntry.Close();

                            if (!commonProvider.GetADEntryByGuid(node.NodeID, out newEntry, out message))
                            {
                                error.Code = ErrorCode.SearchADDataError;
                                result = false;
                                break;
                            }
                            if (node.Type == NodeType.user)
                            {
                                UserInfo user = new UserInfo();
                                user.UserID = newEntry.Guid;
                                user.UserAccount = newEntry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(newEntry.Properties["userPrincipalName"].Value);
                                user.LastName = newEntry.Properties["sn"].Value == null ? "" : Convert.ToString(newEntry.Properties["sn"].Value);
                                user.FirstName = newEntry.Properties["givenName"].Value == null ? "" : Convert.ToString(newEntry.Properties["givenName"].Value);
                                user.DisplayName = newEntry.Properties["displayName"].Value == null ? "" : Convert.ToString(newEntry.Properties["displayName"].Value);
                                user.ParentOu = newEntry.Parent.Properties["name"].Value == null ? "" : Convert.ToString(newEntry.Parent.Properties["name"].Value);
                                user.DistinguishedName = newEntry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(newEntry.Properties["distinguishedName"].Value);

                                //修改数据库
                                UserDBProvider userDBProvider = new UserDBProvider();
                                if (!userDBProvider.ModifyUser(transactionid, admin, user, out error))
                                {
                                    result = false;
                                    break;
                                }
                            }

                            //AD移动HabGroup
                            //GroupManager groupManager = new GroupManager(_clientip);
                            //groupManager.MoveHabGroupMembers(transactionid, souOuId, targetOuEntry.Guid, newEntry.Guid, out error);
                            moveNodeMessage += souEntry.Properties["displayName"].Value + "(" + souEntry.Properties["distinguishedName"].Value + ")，";
                        }

                        moveNodeMessage = string.IsNullOrEmpty(moveNodeMessage) ? string.Empty : moveNodeMessage.Remove(moveNodeMessage.LastIndexOf('，'), 1);
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
                        operateLog.OperateType = "批量移动对象";
                        operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}批量移动对象。" +
                            $"对象：{moveNodeMessage}，" +
                            $"现位置：{targetOuEntry.Properties["distinguishedName"].Value}";

                        LogManager.AddOperateLog(transactionid, operateLog);
                        #endregion
                        result = true;
                    }
                    else
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveNodes异常", paramstr, error.Info, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用MoveNodes异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        private bool VerificationOuNamePrefix(Guid transactionid, string strOldOuName, string strNewOuName, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;

            string paramstr = string.Empty;
            paramstr += $"strOldOuName:{strOldOuName}";
            paramstr += $"||strNewOuName:{strNewOuName}";

            string funname = "VerificationOuNamePrefix";

            try
            {
                do
                {
                    if (strOldOuName.Trim() == strNewOuName.Trim())
                    {
                        break;
                    }

                    #region 废弃
                    /*
                    //验证是否以英文开头
                    if (strNewOuName[0] < 'A' || (strNewOuName[0] > 'Z' && strNewOuName[0] < 'a') || strNewOuName[0] > 'z')
                    {
                        //strDetailError = string.Format("OU_Manage.VerificationOuNamePrefix Ou名称:{0}不符合规范!，必须以英文字母开头", strNewOuName);
                        error.Code = ErrorCode.NameIllegal;
                        bResult = false;
                        break;
                    }

                    //获取Ou名称前缀
                    string strTempOuNamePerfix = string.Empty;
                    int index = 0;
                    while (index < strNewOuName.Length && CharIsLetter(strNewOuName[index]))
                    {
                        strTempOuNamePerfix += strNewOuName[index].ToString();
                        index++;
                    }

                    if (string.IsNullOrEmpty(strTempOuNamePerfix))
                    {
                        //strDetailError = string.Format("OU_Manage.VerificationOuNamePrefix Ou名称:{0},前缀为空", strNewOuName);
                        error.Code = ErrorCode.NamePrefixEmpty;
                        bResult = false;
                        break;
                    }
                    */
                    #endregion

                    //根据条件查询所有OU
                    List<DirectoryEntry> lists = new List<DirectoryEntry>();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.SearchEntryData(ConfigADProvider.GetRootADPath(), strNewOuName, SearchType.All, out lists, out message))
                    {
                        LoggerHelper.Error(funname, paramstr, message, transactionid);
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    #region 废弃
                    /*
                    //循环检查是否有相同Ou名称前缀的对象
                    for (int i = 0; i < lists.Count; i++)
                    {
                        //获取OuDeList元素ou名称前缀
                        string Temp = lists[i].Properties["name"].Value == null ? "" : Convert.ToString(lists[i].Properties["name"].Value);
                        if (Temp == strOldOuName)
                        {
                            continue;
                        }

                        string strOuDeListItemNamePerfix = string.Empty;
                        int Tempindex = 0;
                        while (Tempindex < Temp.Length && CharIsLetter(Temp[Tempindex]))
                        {
                            strOuDeListItemNamePerfix += Temp[Tempindex].ToString();
                            Tempindex++;
                        }

                        //获取Hab后缀参数,组合为HabGroup名称
                        string strHab_add = ConfigADProvider.GetHab_add();
                        string strTempHabGroupName = strOuDeListItemNamePerfix + strHab_add;

                        //若是user判断user账号与前缀+habvalue 是否一样
                        if (lists[i].SchemaClassName.ToLower().Trim() == "organizationalunit")
                        {
                            if (strOuDeListItemNamePerfix.ToLower().Trim() == strTempOuNamePerfix.ToLower().Trim())
                            {
                                error.Code = ErrorCode.HaveSameNamePrefixOu;
                                error.SetInfo(Convert.ToString(lists[i].Parent.Properties["distinguishedName"].Value));
                                bResult = false;
                                break;
                            }
                        }
                        else
                        {
                            string strTempUserSamaccount = lists[i].Properties["sAMAccountName"].Value == null ? "" : Convert.ToString(lists[i].Properties["sAMAccountName"].Value);
                            if (strTempUserSamaccount.ToLower().Trim() == strTempOuNamePerfix.ToLower().Trim())
                            {
                                error.Code = ErrorCode.HaveSameNamePrefixAccount;
                                error.SetInfo(Convert.ToString(lists[i].Parent.Properties["distinguishedName"].Value));
                                bResult = false;
                                break;
                            }
                        }
                    }
                    */
                    #endregion 
                } while (false);
            }
            catch (Exception ex)
            {
                //strDetailError = string.Format("OU_Manage.VerificationOuNamePrefix  在全DC下验证OuName:{0}前缀是否唯一 Exception:{1}", strNewOuName, ex.ToString());
                LoggerHelper.Error(funname, paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }

            return bResult;
        }

        public bool MoveOu(Guid transactionid, AdminInfo admin, MoveNodeInfo moveNode, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||TargetNodeID:{moveNode.TargetNode.NodeID}";
            paramstr += $"||MoveOuID:{moveNode.NodeList[0].NodeID}";
           
            string funname = "MoveOu";

            try
            {
                do
                {
                    DirectoryEntry targetOuEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(moveNode.TargetNode.NodeID, out targetOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string targetRecycleOuPath = ConfigADProvider.GetADRecycleOuLdapByLdap(Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value));

                    DirectoryEntry souOuEntry = new DirectoryEntry();
                    if (!commonProvider.GetADEntryByGuid(moveNode.NodeList[0].NodeID, out souOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string souOudn = Convert.ToString(souOuEntry.Properties["distinguishedName"].Value);
                    string targetOudn = Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value);

                    if (targetOudn == souOudn)
                    {
                        error.Code = ErrorCode.SameOuPath;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                    else if (targetOudn.Contains(souOudn))
                    {
                        error.Code = ErrorCode.SourceOuContainsTargetOu;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string souRecycleOuPath = ConfigADProvider.GetADRecycleOuLdapByLdap(Convert.ToString(souOuEntry.Properties["distinguishedName"].Value));
                    Guid souOuId = souOuEntry.Parent.Guid;

                    //to do cs: 判断管理员是有由跨Ou移动的权限
                    //验证权限
                    if (!admin.ParamList.ContainsValue(RoleParamCode.MoveOu))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    RoleManager roleManager = new RoleManager(_clientip);
                    if (!roleManager.CheckManagerOuRole(transactionid, Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value), admin.ControlLimitOuList, out error))
                    {
                        result = false;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }
                    if (!roleManager.CheckManagerOuRole(transactionid, Convert.ToString(souOuEntry.Properties["distinguishedName"].Value), admin.ControlLimitOuList, out error))
                    {
                        error.Code = ErrorCode.UserNotEnoughRole;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //检查重名
                    DirectoryEntry newoude = new DirectoryEntry();
                    string strTempName = souOuEntry.Properties["name"].Value == null ? "" : Convert.ToString(souOuEntry.Properties["name"].Value);
                    if(targetOuEntry.Path != souOuEntry.Parent.Path)
                    {
                        if (commonProvider.GetOneLevelSigleOuEntry(targetOuEntry.Path, strTempName, out newoude, out message))
                        {
                            error.Code = ErrorCode.HaveSameOu;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("OuManager调用MoveOu异常", paramstr, message, transactionid);
                            result = false;
                            break;
                        }
                    }
                   
                    souOuEntry.MoveTo(targetOuEntry);
                    souOuEntry.CommitChanges();
                    souOuEntry.Close();

                    DirectoryEntry newOuEntry = new DirectoryEntry();
                    if (!commonProvider.GetADEntryByGuid(moveNode.NodeList[0].NodeID, out newOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    OuInfo ouInfo = new OuInfo();
                    ouInfo.id = newOuEntry.Guid;
                    ouInfo.distinguishedName = Convert.ToString(newOuEntry.Properties["distinguishedName"].Value);
                    ouInfo.name = Convert.ToString(newOuEntry.Properties["name"].Value);
                    ouInfo.parentid = newOuEntry.Parent.Guid;

                    //修改数据库
                    OuDBProvider ouDBProvider = new OuDBProvider();
                    if (!ouDBProvider.ModifyOu(transactionid, admin, ouInfo, out error))
                    {
                        LoggerHelper.Error("OuManager调用MoveOu异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    //GroupInfo habGroupInfo = new GroupInfo();
                    //GroupProvider groupProvider = new GroupProvider();
                    //if (groupProvider.GetHabGroupInfoByOu(transactionid, moveNode.NodeList[0].NodeID, out habGroupInfo, out error))
                    //{
                    //    //AD移动HabGroup
                    //    GroupManager groupManager = new GroupManager(_clientip);
                    //    groupManager.MoveHabGroupMembers(transactionid, souOuId, targetOuEntry.Guid, habGroupInfo.GroupID, out error);
                    //}
                    // //移动RecycleOu
                    //OuProvider provider = new OuProvider();
                    //provider.MoveRecycleOu(transactionid, admin, souRecycleOuPath, targetRecycleOuPath, out error);

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
                    operateLog.OperateType = "移动 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}移动OU。" +
                        $"OU名称：{strTempName}，" +
                         $"原位置：{souOuEntry.Properties["distinguishedName"].Value}，" +
                         $"现位置：{targetOuEntry.Properties["distinguishedName"].Value}";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用MoveOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        #endregion

        #region samelevelOu
        public bool AddOuNoHab(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Description:{ou.description}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||ParentId:{ou.parentid}";

            string funname = "AddOuNoHab";

            try
            {
                do
                {
                    error = ou.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.parentid, out entry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用AddOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string parentOuPath = Convert.ToString(entry.Properties["distinguishedName"].Value);

                    //AD添加Ou
                    OuProvider provider = new OuProvider();
                    if (!provider.AddOu(transactionid, admin, ref ou, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(ou);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加OU。" +
                        $"OU名称：{ou.name}，" +
                        $"父级OU路径：{parentOuPath}，";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用AddOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ModifyOuNoHab(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Description:{ou.description}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||Id:{ou.id}";

            string funname = "ModifyOuNoHab";

            try
            {
                do
                {
                    error = ou.ModifyCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.id, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用ModifyOuNoHab异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string ouName = ouEntry.Properties["name"].Value == null ? "" : Convert.ToString(ouEntry.Properties["name"].Value);
                    string parentPath = ouEntry.Parent.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(ouEntry.Parent.Properties["distinguishedName"].Value);
                    string ouPath = ouEntry.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(ouEntry.Properties["distinguishedName"].Value);

                    RoleManager roleManager = new RoleManager(_clientip);
                    if (roleManager.CheckRootOu(transactionid, ouPath, admin.ControlLimitOuList, admin.SameLevelOuList, out error))
                    {
                        result = false;
                        error.Code = ErrorCode.RootOuNotOperate;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }

                    if (ouName != ou.name)
                    {
                        //查找重名
                        string ouParentPath = ouEntry.Parent.Path;
                        DirectoryEntry newEntry = new DirectoryEntry();
                        if (commonProvider.GetOneLevelSigleOuEntry(ouParentPath, ou.name, out newEntry, out message))
                        {
                            error.Code = ErrorCode.HaveSameDisplayName;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("OuManager调用ModifyOuNoHab异常", paramstr, message, transactionid);
                            result = false;
                            break;
                        }
                    }

                    //AD修改Ou
                    OuProvider provider = new OuProvider();
                    if (!provider.ModifyOu(transactionid, admin, ref ou, out error))
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
                    operateLog.OperateType = "编辑 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}编辑OU。" +
                      $"原OU名称：{ouName}，现OU名称：{ou.name}，" +
                      $"OU路径：{parentPath}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用ModifyOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteOuNoHab(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Id:{ou.id}";

            string funname = "DeleteOuNoHab";

            try
            {
                do
                {
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.id, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用DeleteOuNoHab异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    ou.parentid = ouEntry.Parent.Guid;
                    ou.name = Convert.ToString(ouEntry.Properties["name"].Value);
                    ou.distinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);

                    RoleManager roleManager = new RoleManager(_clientip);
                    if (roleManager.CheckRootOu(transactionid, ou.distinguishedName, admin.ControlLimitOuList, admin.SameLevelOuList, out error))
                    {
                        result = false;
                        error.Code = ErrorCode.RootOuNotOperate;
                        strJsonResult = JsonHelper.ReturnJson(result, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), result, transactionid);
                        break;
                    }

                    //判断当前ou下是否还有子对象
                    List<DirectoryEntry> EntryList = new List<DirectoryEntry>();
                    if (!commonProvider.SearchEntryData(ouEntry.Path, SearchType.AllNoHab, out EntryList, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用DeleteOuNoHab异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    for (int i = EntryList.Count - 1; i >= 0; i--)
                    {
                        if (EntryList[i].Path == ouEntry.Path)
                        {
                            EntryList.RemoveAt(i);
                        }
                    }

                    if (EntryList.Count > 0)
                    {
                        error.Code = ErrorCode.HaveMembers;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用DeleteOuNoHab异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                    
                    //AD修改Ou
                    OuProvider provider = new OuProvider();
                    if (!provider.DeleteOu(transactionid, admin, ou, out error))
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
                    operateLog.OperateType = "删除 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除OU。" +
                        $"OU名称：{ou.name}，" +
                         $"OU位置：{ou.distinguishedName}";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用DeleteOuNoHab异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool MoveOuNoHab(Guid transactionid, AdminInfo admin, MoveNodeInfo moveNode, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||TargetNodeID:{moveNode.TargetNode.NodeID}";
            paramstr += $"||MoveOuID:{moveNode.NodeList[0].NodeID}";

            string funname = "MoveOuNoHab";

            try
            {
                do
                {
                    DirectoryEntry targetOuEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(moveNode.TargetNode.NodeID, out targetOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveOuNoHab异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry souOuEntry = new DirectoryEntry();
                    if (!commonProvider.GetADEntryByGuid(moveNode.NodeList[0].NodeID, out souOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveOuNoHab异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string souOudn = Convert.ToString(souOuEntry.Properties["distinguishedName"].Value);
                    string targetOudn = Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value);

                    if (targetOudn == souOudn)
                    {
                        error.Code = ErrorCode.SameOuPath;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MoveOuNoHab失败", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }
                    else if (targetOudn.Contains(souOudn))
                    {
                        error.Code = ErrorCode.SourceOuContainsTargetOu;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MoveOuNoHab失败", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    Guid souOuId = souOuEntry.Parent.Guid;
                    RoleManager roleManager = new RoleManager(_clientip);
                    if (roleManager.CheckRootOu(transactionid, souOudn, admin.ControlLimitOuList, admin.SameLevelOuList, out error))
                    {
                        error.Code = ErrorCode.RootOuNotOperate;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveOuNoHab异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    //检查重名
                    DirectoryEntry newoude = new DirectoryEntry();
                    string strTempName = souOuEntry.Properties["name"].Value == null ? "" : Convert.ToString(souOuEntry.Properties["name"].Value);
                    if (commonProvider.GetOneLevelSigleOuEntry(targetOuEntry.Path, strTempName, out newoude, out message))
                    {
                        error.Code = ErrorCode.HaveSameOu;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveOuNoHab异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    souOuEntry.MoveTo(targetOuEntry);
                    souOuEntry.CommitChanges();
                    souOuEntry.Close();

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
                    operateLog.OperateType = "移动 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}移动OU。" +
                        $"OU名称：{strTempName}，" +
                         $"原位置：{souOuEntry.Properties["distinguishedName"].Value}，" +
                         $"现位置：{targetOuEntry.Properties["distinguishedName"].Value}";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用MoveOuNoHab异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool MoveNodesNoHab(Guid transactionid, AdminInfo admin, MoveNodeInfo moveNode, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||TargetNodeID:{moveNode.TargetNode.NodeID}";
            paramstr += $"||MoveNodes:";
            for (int i = 0; i < moveNode.NodeList.Count; i++)
            {
                paramstr += "MoveNode:" + moveNode.NodeList[i].NodeID.ToString() + "(" + moveNode.NodeList[i].Type + "),";
            }

            string funname = "MoveNodesNoHab";

            try
            {
                do
                {
                    DirectoryEntry targetOuEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(moveNode.TargetNode.NodeID, out targetOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveNodesNoHab异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry souEntry = new DirectoryEntry();
                    string moveNodeMessage = string.Empty;
                    string errormessage = string.Empty;
                    foreach (NodeInfo node in moveNode.NodeList)
                    {
                        if (!commonProvider.GetADEntryByGuid(node.NodeID, out souEntry, out message))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            errormessage = "移动对象不存在";
                            result = false;
                            break;
                        }

                        //检查重名
                        DirectoryEntry newoude = new DirectoryEntry();
                        string strTempName = souEntry.Properties["name"].Value == null ? "" : Convert.ToString(souEntry.Properties["name"].Value);
                        if (Convert.ToString(souEntry.Parent.Properties["distinguishedName"].Value) != Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value))
                        {
                            if (commonProvider.GetOneLevelSigleEntryByCN(targetOuEntry.Path, strTempName, out newoude, out message))
                            {
                                error.Code = ErrorCode.MoveNodehaveSameAccount;
                                error.SetInfo(strTempName);
                                result = false;
                                break;
                            }
                        }
                    }
                    if (result)
                    {
                        DirectoryEntry newEntry = new DirectoryEntry();
                        foreach (NodeInfo node in moveNode.NodeList)
                        {
                            if (!commonProvider.GetADEntryByGuid(node.NodeID, out souEntry, out message))
                            {
                                error.Code = ErrorCode.SearchADDataError;
                                result = false;
                                break;
                            }

                            Guid souOuId = souEntry.Parent.Guid;

                            souEntry.MoveTo(targetOuEntry);
                            souEntry.CommitChanges();
                            souEntry.Close();

                            if (!commonProvider.GetADEntryByGuid(node.NodeID, out newEntry, out message))
                            {
                                error.Code = ErrorCode.SearchADDataError;
                                result = false;
                                break;
                            }
                            if (node.Type == NodeType.user)
                            {
                                UserInfo user = new UserInfo();
                                user.UserID = newEntry.Guid;
                                user.UserAccount = newEntry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(newEntry.Properties["userPrincipalName"].Value);
                                user.LastName = newEntry.Properties["sn"].Value == null ? "" : Convert.ToString(newEntry.Properties["sn"].Value);
                                user.FirstName = newEntry.Properties["givenName"].Value == null ? "" : Convert.ToString(newEntry.Properties["givenName"].Value);
                                user.DisplayName = newEntry.Properties["displayName"].Value == null ? "" : Convert.ToString(newEntry.Properties["displayName"].Value);
                                user.ParentOu = newEntry.Parent.Properties["name"].Value == null ? "" : Convert.ToString(newEntry.Parent.Properties["name"].Value);
                                user.DistinguishedName = newEntry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(newEntry.Properties["distinguishedName"].Value);
                            }

                            moveNodeMessage += souEntry.Properties["displayName"].Value + "(" + souEntry.Properties["distinguishedName"].Value + ")，";
                        }

                        moveNodeMessage = string.IsNullOrEmpty(moveNodeMessage) ? string.Empty : moveNodeMessage.Remove(moveNodeMessage.LastIndexOf('，'), 1);
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
                        operateLog.OperateType = "批量移动对象";
                        operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}批量移动对象。" +
                            $"对象：{moveNodeMessage}，" +
                            $"现位置：{targetOuEntry.Properties["distinguishedName"].Value}";

                        LogManager.AddOperateLog(transactionid, operateLog);
                        #endregion
                        result = true;
                    }
                    else
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("OuManager调用MoveNodesNoHab异常", paramstr, error.Info, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用MoveNodesNoHab异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        #endregion

        #region Interface
        public bool AddOuByInterface(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            string message = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Description:{ou.description}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||ParentId:{ou.parentid}";

            string funname = "AddOuByInterface";

            try
            {
                do
                {
                    error = ou.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.parentid, out entry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("AddOuByInterface调用AddOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string parentOuPath = Convert.ToString(entry.Properties["distinguishedName"].Value);

                    if (!VerificationOuNamePrefix(transactionid, string.Empty, ou.name, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //AD添加Ou
                    OuProvider provider = new OuProvider();
                    if (!provider.AddOu(transactionid, admin, ref ou, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //Add RecycleOu
                    provider.AddRecycleOu(transactionid, admin, ou, out error);

                    //Add HabGroup
                    GroupManager groupManager = new GroupManager(_clientip);
                    groupManager.AddHabGroup(transactionid, admin, ou, out error);

                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    dictionary.Add("id", ou.id);

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(dictionary);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加 OU ";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加OU。" +
                         $"OU名称：{ou.name}，" +
                        $"父级OU路径：{parentOuPath}，";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("AddOuByInterface调用AddOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

         public bool ModifyOuByInterface(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Description:{ou.description}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||Id:{ou.id}";

            string funname = "ModifyOuByInterface";

            try
            {
                do
                {
                    error = ou.ModifyCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.id, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("ModifyOuByInterface调用ModifyOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string ouName = ouEntry.Properties["name"].Value == null ? "" : Convert.ToString(ouEntry.Properties["name"].Value);
                    string ouPath = ouEntry.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(ouEntry.Properties["distinguishedName"].Value);
                    string parentPath = ouEntry.Parent.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(ouEntry.Parent.Properties["distinguishedName"].Value);
                    string recycleOuPath = ConfigADProvider.GetADRecycleOuLdapByLdap(Convert.ToString(ouEntry.Properties["distinguishedName"].Value));
                    
                    OuInfo recycleOuInfo = new OuInfo();
                    if (ouName != ou.name)
                    {
                        if (!VerificationOuNamePrefix(transactionid, ouName, ou.name, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        //查找重名
                        string ouParentPath = ouEntry.Parent.Path;
                        DirectoryEntry newEntry = new DirectoryEntry();
                        if (commonProvider.GetOneLevelSigleOuEntry(ouParentPath, ou.name, out newEntry, out message))
                        {
                            error.Code = ErrorCode.HaveSameDisplayName;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("ModifyOuByInterface调用ModifyOu异常", paramstr, message, transactionid);
                            result = false;
                            break;
                        }
                    }

                    //AD修改Ou
                    OuProvider provider = new OuProvider();
                    if (!provider.ModifyOu(transactionid, admin, ref ou, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (ouName != ou.name)
                    {
                        //修改数据库
                        OuDBProvider ouDBProvider = new OuDBProvider();
                        ouDBProvider.ModifyOu(transactionid, admin, ou, out error);

                        //AD 重命名HabGroup
                        GroupManager groupManager = new GroupManager(_clientip);
                        groupManager.ReNameHabGroup(transactionid, admin, ou, out error);

                        provider.ModifyRecycleOu(transactionid, admin, recycleOuPath, ou.name, out error);
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
                    operateLog.OperateType = "编辑 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}编辑OU。" +
                      $"原OU名称：{ouName}，现OU名称：{ou.name}，" +
                      $"OU路径：{parentPath}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("ModifyOuByInterface调用ModifyOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteOuByInterface(Guid transactionid, AdminInfo admin, OuInfo ou, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Id:{ou.id}";

            string funname = "DeleteOuByInterface";

            try
            {
                do
                {
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.id, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("DeleteOuByInterface调用DeleteOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    ou.parentid = ouEntry.Parent.Guid;
                    ou.name = Convert.ToString(ouEntry.Properties["name"].Value);
                    ou.distinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);

                    //判断当前ou下是否还有子对象
                    List<DirectoryEntry> EntryList = new List<DirectoryEntry>();
                    if (!commonProvider.SearchEntryData(ouEntry.Path, SearchType.AllNoHab, out EntryList, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("DeleteOuByInterface调用DeleteOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    for (int i = EntryList.Count - 1; i >= 0; i--)
                    {
                        if (EntryList[i].Path == ouEntry.Path)
                        {
                            EntryList.RemoveAt(i);
                        }
                    }

                    if (EntryList.Count > 0)
                    {
                        error.Code = ErrorCode.HaveMembers;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("DeleteOuByInterface调用DeleteOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    //修改数据库
                    OuDBProvider ouDBProvider = new OuDBProvider();
                    ouDBProvider.DeleteOu(transactionid, admin, ou, out error);

                    //AD修改Ou
                    OuProvider provider = new OuProvider();
                    if (!provider.DeleteOu(transactionid, admin, ou, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    //Add RecycleOu
                    string recycleOuPath = ConfigADProvider.GetADRecycleOuLdapByLdap(ou.distinguishedName);
                    provider.DeleteRecycleOu(transactionid, admin, recycleOuPath, out error);

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
                    operateLog.OperateType = "删除 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除OU。" +
                        $"OU名称：{ou.name}，" +
                         $"OU位置：{ou.distinguishedName}";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("DeleteOuByInterface调用DeleteOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool MoveOuByInterface(Guid transactionid, AdminInfo admin, MoveNodeInfo moveNode, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||TargetNodeID:{moveNode.TargetNode.NodeID}";
            paramstr += $"||MoveOuID:{moveNode.NodeList[0].NodeID}";

            string funname = "MoveOuByInterface";

            try
            {
                do
                {
                    DirectoryEntry targetOuEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(moveNode.TargetNode.NodeID, out targetOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MoveOuByInterface调用GetADEntryByGuid获取targetOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string targetRecycleOuPath = ConfigADProvider.GetADRecycleOuLdapByLdap(Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value));

                    DirectoryEntry souOuEntry = new DirectoryEntry();
                    if (!commonProvider.GetADEntryByGuid(moveNode.NodeList[0].NodeID, out souOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MoveOuByInterface调用GetADEntryByGuid获取souOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string souOudn = Convert.ToString(souOuEntry.Properties["distinguishedName"].Value);
                    string targetOudn = Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value);

                    if (targetOudn == souOudn)
                    {
                        error.Code = ErrorCode.SameOuPath;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MoveOuByInterface失败", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                    else if (targetOudn.Contains(souOudn))
                    {
                        error.Code = ErrorCode.SourceOuContainsTargetOu;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MoveOuByInterface失败", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string souRecycleOuPath = ConfigADProvider.GetADRecycleOuLdapByLdap(Convert.ToString(souOuEntry.Properties["distinguishedName"].Value));
                    Guid souOuId = souOuEntry.Parent.Guid;
                    
                    //检查重名
                    DirectoryEntry newoude = new DirectoryEntry();
                    string strTempName = souOuEntry.Properties["name"].Value == null ? "" : Convert.ToString(souOuEntry.Properties["name"].Value);
                    if (targetOuEntry.Path != souOuEntry.Parent.Path)
                    {
                        if (commonProvider.GetOneLevelSigleOuEntry(targetOuEntry.Path, strTempName, out newoude, out message))
                        {
                            error.Code = ErrorCode.HaveSameOu;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("MoveOuByInterface调用GetOneLevelSigleOuEntry异常", paramstr, message, transactionid);
                            result = false;
                            break;
                        }
                    }

                    souOuEntry.MoveTo(targetOuEntry);
                    souOuEntry.CommitChanges();
                    souOuEntry.Close();

                    DirectoryEntry newOuEntry = new DirectoryEntry();
                    if (!commonProvider.GetADEntryByGuid(moveNode.NodeList[0].NodeID, out newOuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MoveOuByInterface调用GetADEntryByGuid获取newOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    OuInfo ouInfo = new OuInfo();
                    ouInfo.id = newOuEntry.Guid;
                    ouInfo.distinguishedName = Convert.ToString(newOuEntry.Properties["distinguishedName"].Value);
                    ouInfo.name = Convert.ToString(newOuEntry.Properties["name"].Value);
                    ouInfo.parentid = newOuEntry.Parent.Guid;

                    //修改数据库
                    OuDBProvider ouDBProvider = new OuDBProvider();
                    if (!ouDBProvider.ModifyOu(transactionid, admin, ouInfo, out error))
                    {
                        LoggerHelper.Error("MoveOuByInterface调用ModifyOu异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    GroupInfo habGroupInfo = new GroupInfo();
                    GroupProvider groupProvider = new GroupProvider();
                    if (!groupProvider.GetHabGroupInfoByOu(transactionid, moveNode.NodeList[0].NodeID, out habGroupInfo, out error))
                    {
                        LoggerHelper.Error("MoveOuByInterface调用GetHabGroupInfoByOu异常", paramstr, message, transactionid);
                        break;
                    }

                    //AD移动HabGroup
                    GroupManager groupManager = new GroupManager(_clientip);
                    groupManager.MoveHabGroupMembers(transactionid, souOuId, targetOuEntry.Guid, habGroupInfo.GroupID, out error);

                    //移动RecycleOu
                    OuProvider provider = new OuProvider();
                    provider.MoveRecycleOu(transactionid, admin, souRecycleOuPath, targetRecycleOuPath, out error);

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
                    operateLog.OperateType = "移动 OU";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}移动OU。" +
                        $"OU名称：{strTempName}，" +
                         $"原位置：{souOuEntry.Properties["distinguishedName"].Value}，" +
                         $"现位置：{targetOuEntry.Properties["distinguishedName"].Value}";

                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("OuManager调用MoveOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool MoveNodesByInterface(Guid transactionid, AdminInfo admin, MoveNodeInfo moveNode, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||TargetNodeID:{moveNode.TargetNode.NodeID}";
            paramstr += $"||MoveNodes:";
            for (int i = 0; i < moveNode.NodeList.Count; i++)
            {
                paramstr += "MoveNode:" + moveNode.NodeList[i].NodeID.ToString() + "(" + moveNode.NodeList[i].Type + "),";
            }

            string funname = "MoveNodesByInterface";

            try
            {
                do
                {
                    DirectoryEntry targetOuEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(moveNode.TargetNode.NodeID, out targetOuEntry, out message))
                    {
                        error.Code = ErrorCode.IdNotExist;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MoveNodesByInterface调用GetADEntryByGuid获取targetOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry souEntry = new DirectoryEntry();
                    string moveNodeMessage = string.Empty;
                    foreach (NodeInfo node in moveNode.NodeList)
                    {
                        if (!commonProvider.GetADEntryByGuid(node.NodeID, out souEntry, out message))
                        {
                            error.Code = ErrorCode.MoveNodeNotExist;
                            error.SetInfo(node.NodeID.ToString());
                            result = false;
                            break;
                        }

                        Guid souOuId = souEntry.Parent.Guid;
                        
                        //检查重名
                        DirectoryEntry newoude = new DirectoryEntry();
                        string strTempName = souEntry.Properties["name"].Value == null ? "" : Convert.ToString(souEntry.Properties["name"].Value);
                        if (Convert.ToString(souEntry.Parent.Properties["distinguishedName"].Value) != Convert.ToString(targetOuEntry.Properties["distinguishedName"].Value))
                        {
                            if (commonProvider.GetOneLevelSigleEntryByCN(targetOuEntry.Path, strTempName, out newoude, out message))
                            {
                                error.Code = ErrorCode.MoveNodehaveSameAccount;
                                error.SetInfo(strTempName);
                                result = false;
                                break;
                            }
                        }
                    }
                    if (result)
                    {
                        DirectoryEntry newEntry = new DirectoryEntry();
                        foreach (NodeInfo node in moveNode.NodeList)
                        {
                            if (!commonProvider.GetADEntryByGuid(node.NodeID, out souEntry, out message))
                            {
                                continue;
                            }

                            Guid souOuId = souEntry.Parent.Guid;

                            souEntry.MoveTo(targetOuEntry);
                            souEntry.CommitChanges();
                            souEntry.Close();

                            if (!commonProvider.GetADEntryByGuid(node.NodeID, out newEntry, out message))
                            {
                                continue;
                            }
                            if (node.Type == NodeType.user)
                            {
                                UserInfo user = new UserInfo();
                                user.UserID = newEntry.Guid;
                                user.UserAccount = newEntry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(newEntry.Properties["userPrincipalName"].Value);
                                user.LastName = newEntry.Properties["sn"].Value == null ? "" : Convert.ToString(newEntry.Properties["sn"].Value);
                                user.FirstName = newEntry.Properties["givenName"].Value == null ? "" : Convert.ToString(newEntry.Properties["givenName"].Value);
                                user.DisplayName = newEntry.Properties["displayName"].Value == null ? "" : Convert.ToString(newEntry.Properties["displayName"].Value);
                                user.ParentOu = newEntry.Parent.Properties["name"].Value == null ? "" : Convert.ToString(newEntry.Parent.Properties["name"].Value);
                                user.DistinguishedName = newEntry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(newEntry.Properties["distinguishedName"].Value);

                                //修改数据库
                                UserDBProvider userDBProvider = new UserDBProvider();
                                userDBProvider.ModifyUser(transactionid, admin, user, out error);
                            }

                            //AD移动HabGroup
                            GroupManager groupManager = new GroupManager(_clientip);
                            groupManager.MoveHabGroupMembers(transactionid, souOuId, targetOuEntry.Guid, newEntry.Guid, out error);
                            moveNodeMessage += souEntry.Properties["displayName"].Value + "(" + souEntry.Properties["distinguishedName"].Value + ")，";
                        }

                        moveNodeMessage = string.IsNullOrEmpty(moveNodeMessage) ? string.Empty : moveNodeMessage.Remove(moveNodeMessage.LastIndexOf('，'), 1);
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
                        operateLog.OperateType = "批量移动对象";
                        operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}批量移动对象。" +
                            $"对象：{moveNodeMessage}，" +
                            $"现位置：{targetOuEntry.Properties["distinguishedName"].Value}";

                        LogManager.AddOperateLog(transactionid, operateLog);
                        #endregion
                        result = true;
                    }
                    else
                    {
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("MoveNodesByInterface异常", paramstr, error.Info, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("MoveNodesByInterface异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        #endregion

        public static bool CharIsLetter(char ch)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
