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
    public class TreeNodeManager
    {
        public bool GetCompanyTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetCompanyTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    if (!commonProvider.GetCommonTreeData(nodeID, admin, SearchType.AllNoHab, out list, out errormsg))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("TreeNodeManager调用GetCompanyTree异常", paramstr, errormsg, transactionid);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, errormsg, false, transactionid);
                        result = false;
                        break;
                    }
                    else
                    {
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetCompanyTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchCompanyTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchCompanyTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetCompanyTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), searchstr, SearchType.AllNoHab, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            NodeInfo node = new NodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                            if (node.Type == NodeType.user)
                            {
                                node.Name = Convert.ToString(entry.Properties["displayname"].Value);
                                node.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                int userAccountControl = Convert.ToInt32(entry.Properties["userAccountControl"][0]);
                                node.Status = UserProvider.IsAccountActive(userAccountControl);
                            }
                          
                            int count = 0;
                            commonProvider.GetMemberCount(entry, out count, out errormsg);
                            node.MemberCount = count;
                            nodelist.Add(node);
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchCompanyTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetOrgTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetOrgTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    if (!commonProvider.GetCommonTreeData(nodeID, admin, SearchType.Ou, out list, out errormsg))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    else
                    {
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetOrgTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchOrgTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchOrgTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetOrgTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), searchstr, SearchType.Ou, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            NodeInfo node = new NodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                            nodelist.Add(node);
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchOrgTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetPublicUserTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetPublicUserTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    string nodePath = string.Empty;
                    if (nodeID == Guid.Empty)
                    {
                        //根节点
                        NodeInfo info = new NodeInfo();
                        if (!commonProvider.GetRootNodeInfo(ConfigADProvider.GetPublicOuDistinguishedName(), out info, out errormsg))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        list.Add(info);
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                    else
                    {
                        if (!commonProvider.GetTreeData(nodeID, out list, out errormsg))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                        else
                        {
                            strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                            error.Code = ErrorCode.None;
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                            result = true;
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetPublicUserTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchPublicUserTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchPublicUserTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetPublicUserTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetPublicOuLdap(), searchstr, SearchType.AllNoHab, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            NodeInfo node = new NodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                            if (node.Type == NodeType.user)
                            {
                                node.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                int userAccountControl = Convert.ToInt32(entry.Properties["userAccountControl"][0]);
                                node.Status = UserProvider.IsAccountActive(userAccountControl);
                            }

                            nodelist.Add(node);
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchPublicUserTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetDeleteOrgTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetDeleteOrgTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    string nodePath = string.Empty;
                    if (nodeID == Guid.Empty)
                    {
                        //根节点
                        NodeInfo info = new NodeInfo();
                        if (!commonProvider.GetRootNodeInfo(ConfigADProvider.GetRecycleOuDistinguishedName(), out info, out errormsg))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        list.Add(info);
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                    else
                    {
                        if (!commonProvider.GetTreeData(nodeID, out list, out errormsg))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                        else
                        {
                            error.Code = ErrorCode.None;
                            strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                            result = true;
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetPublicTreeData异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchDeleteOrgTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchPublicUserTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetPublicUserTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetRecycleOuLdap(), searchstr, SearchType.User, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            NodeInfo node = new NodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                            if (node.Type == NodeType.user)
                            {
                                node.Name = Convert.ToString(entry.Properties["displayname"].Value);
                                node.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                int userAccountControl = Convert.ToInt32(entry.Properties["userAccountControl"][0]);
                                node.Status = UserProvider.IsAccountActive(userAccountControl);
                            }

                            nodelist.Add(node);
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchPublicUserTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetGroupTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetGroupTreeData";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    if (!commonProvider.GetCommonTreeData(nodeID, admin, SearchType.GroupTreeNoHab, out list, out errormsg))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    else
                    {
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetGroupTreeData异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchGroupTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchGroupTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetGroupTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), searchstr, SearchType.GroupAndNoHab, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            NodeInfo node = new NodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                            nodelist.Add(node);
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchGroupTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetExchangeGroupList(Guid transactionid, AdminInfo admin, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";

            string funname = "GetExchangeGroupList";

            try
            {
                do
                {
                    List<DirectoryEntry> lists = new List<DirectoryEntry>();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.SearchEntryData(ConfigADProvider.GetRootADPath(), SearchType.MailGroup, out lists, out errormsg))
                    {
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("TreeNodeManager调用GetExchangeGroupList异常", paramstr, errormsg, transactionid);
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        result = false;
                    }

                    List<GroupInfo> groups = new List<GroupInfo>();
                    if (lists.Count > 0)
                    {
                        foreach (DirectoryEntry entry in lists)
                        {
                            GroupInfo group = new GroupInfo();
                            group.GroupID = entry.Guid;
                            group.DisplayName = entry.Properties["name"].Value == null ? "" : Convert.ToString(entry.Properties["name"].Value);
                            group.Description = entry.Properties["description"].Value == null ? "" : Convert.ToString(entry.Properties["description"].Value);
                            group.Account = entry.Properties["mail"].Value == null ? "" : Convert.ToString(entry.Properties["mail"].Value);
                            groups.Add(group);
                        }
                    }

                    string json = JsonConvert.SerializeObject(groups.OrderBy(x => x.DisplayName).ToList());
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetExchangeGroupList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchExchangeGroupList(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";

            string funname = "SearchExchangeGroupList";

            try
            {
                do
                {
                    List<DirectoryEntry> lists = new List<DirectoryEntry>();
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetHabGroupTree(transactionid, admin.UserAccount, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetRootADPath(), searchstr, SearchType.MailGroup, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.Exception;
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            LoggerHelper.Error("TreeNodeManager调用SearchExchangeGroupList异常", paramstr, errormsg, transactionid);
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            result = false;
                        }

                        List<HabNodeInfo> nodelist = new List<HabNodeInfo>();
                        if (lists.Count > 0)
                        {
                            foreach (DirectoryEntry entry in lists)
                            {
                                HabNodeInfo node = new HabNodeInfo();
                                node.NodeID = entry.Guid;
                                node.Name = Convert.ToString(entry.Properties["name"].Value);
                                node.UserAccount = entry.Properties["mail"].Value == null ? "" : Convert.ToString(entry.Properties["mail"].Value);
                                node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                node.Index = entry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(entry.Properties["msDS-HABSeniorityIndex"].Value);
                                bool isHab = entry.Properties["msOrg-IsOrganizational"].Value == null ? false : Convert.ToBoolean(Convert.ToInt32(entry.Properties["msOrg-IsOrganizational"].Value));
                                node.Type = isHab == true ? NodeType.habgroup : NodeType.group;
                                nodelist.Add(node);
                            }
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Index).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchExchangeGroupList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetMailUserTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetMailUserTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    if (!commonProvider.GetCommonTreeData(nodeID, admin, SearchType.MailUser, out list, out errormsg))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    else
                    {
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Desc).ToList());
                        error.Code = ErrorCode.None;
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetMailUserTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchMailUserTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchMailUserTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                       result = GetMailUserTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), searchstr, SearchType.MailUser, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            NodeInfo node = new NodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                            if (node.Type == NodeType.user)
                            {
                                node.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                int userAccountControl = Convert.ToInt32(entry.Properties["userAccountControl"][0]);
                                node.Status = UserProvider.IsAccountActive(userAccountControl);
                            }
                            nodelist.Add(node);
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchMailUserTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetHabGroupTree(Guid transactionid, string userAccount, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetHabGroupTree";

            try
            {
                do
                {
                    string nodePath = string.Empty;
                    CommonProvider commonProvider = new CommonProvider();
                    GroupProvider groupProvider = new GroupProvider();
                    List<HabNodeInfo> list = new List<HabNodeInfo>();
                    GroupInfo group = new GroupInfo();
                    if (nodeID == Guid.Empty)
                    {
                        //根节点
                        string rootpath = ConfigADProvider.GetCompanyOuDistinguishedName();
                        DirectoryEntry roothabEntry = new DirectoryEntry();

                        if (!commonProvider.GetADEntryByPath(rootpath, out roothabEntry, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        if (!groupProvider.GetHabGroupInfoByOu(transactionid, roothabEntry.Guid, out group, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        HabNodeInfo info = new HabNodeInfo();
                        info.Name = group.DisplayName;
                        info.NodeID = group.GroupID;
                        info.isParent = true;
                        info.MemberCount = group.Members.Count;
                        info.UserAccount = group.Account;
                        info.Index = group.Index;
                        info.Type = NodeType.habgroup;
                        info.isRoot = true;

                        list.Add(info);
                        strJsonResult = JsonConvert.SerializeObject(list.OrderByDescending(x => x.Index).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                    else
                    {
                        if (!groupProvider.GetGroupInfo(transactionid, nodeID, out group, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                        else
                        {
                            if (group.IsOrganizational)
                            {
                                foreach (GroupMember member in group.Members)
                                {
                                    if (member.IsOrganizational)
                                    {
                                        HabNodeInfo info = new HabNodeInfo();
                                        info.Name = member.DisplayName;
                                        info.NodeID = member.ID;
                                        info.UserAccount = member.Account;
                                        info.Index = member.Index;
                                        info.Type = member.Type;
                                        info.isParent = member.MemberCount > 0 ? true : false;
                                        info.MemberCount = member.MemberCount;
                                        list.Add(info);
                                    }
                                }
                            }

                            strJsonResult = JsonConvert.SerializeObject(list.OrderByDescending(m => m.Index).ThenBy(x => x.Name).ToList());
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                            result = true;
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetHabGroupTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchHabGroupTree(Guid transactionid, string userAccount, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"UserAccount:{userAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchHabGroupTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetHabGroupTree(transactionid, userAccount, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        string rootpath = ConfigADProvider.GetCompanyOuDistinguishedName();

                        DirectoryEntry roothabEntry = new DirectoryEntry();
                        if (!commonProvider.GetADEntryByPath(rootpath, out roothabEntry, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        GroupInfo rootGroup = new GroupInfo();
                        GroupProvider groupProvider = new GroupProvider();
                        if (!groupProvider.GetHabGroupInfoByOu(transactionid, roothabEntry.Guid, out rootGroup, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<HabNodeInfo> nodelist = new List<HabNodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), searchstr, SearchType.Hab, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            HabNodeInfo node = new HabNodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Index = entry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(entry.Properties["msDS-HABSeniorityIndex"].Value);
                            node.Type = NodeType.habgroup;
                            node.isParent = entry.Properties["member"].Count > 0 ? true : false;
                            node.isRoot = node.Path == rootGroup.DistinguishedName ? true : false;
                            nodelist.Add(node);
                        }

                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderByDescending(m => m.Index).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchHabGroupTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetHabGroupTreeByUserAccount(Guid transactionid, string userAccount, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetHabGroupTreeByUserAccount";

            try
            {
                do
                {
                    string nodePath = string.Empty;
                    CommonProvider commonProvider = new CommonProvider();
                    GroupProvider groupProvider = new GroupProvider();
                    List<HabNodeInfo> list = new List<HabNodeInfo>();
                    GroupInfo group = new GroupInfo();

                    List<DirectoryEntry> users = new List<DirectoryEntry>();
                    if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), userAccount, SearchType.MailUser, out users, out errormsg))
                    {
                        error.Code = ErrorCode.UserNotExist;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("TreeNodeManager调用GetHabGroupTreeByUserAccount异常", paramstr, errormsg, transactionid);
                        LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (nodeID == Guid.Empty)
                    {
                        //根节点
                        string rootpath = ConfigADProvider.GetCompanyOuDistinguishedName();
                        DirectoryEntry roothabEntry = new DirectoryEntry();

                        if (!commonProvider.GetADEntryByPath(rootpath, out roothabEntry, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        if (!groupProvider.GetHabGroupInfoByOu(transactionid, roothabEntry.Guid, out group, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        HabNodeInfo info = new HabNodeInfo();
                        info.Name = group.DisplayName;
                        info.NodeID = group.GroupID;
                        info.isParent = true;
                        info.MemberCount = group.Members.Count;
                        info.UserAccount = group.Account;
                        info.Index = group.Index;
                        info.Type = NodeType.group;
                        info.isRoot = true;

                        list.Add(info);
                        strJsonResult = JsonConvert.SerializeObject(list.OrderByDescending(x => x.Index).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                    else
                    {
                        if (!groupProvider.GetGroupInfo(transactionid, nodeID, out group, out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                        else
                        {
                            foreach (GroupMember member in group.Members)
                            {
                                HabNodeInfo info = new HabNodeInfo();
                                info.Name = member.DisplayName;
                                info.NodeID = member.ID;
                                info.isParent = member.Type == NodeType.group ? true : false;
                                info.UserAccount = member.Account;
                                info.MemberCount = member.MemberCount;
                                info.Index = member.Index;
                                info.Type = member.Type;
                                list.Add(info);
                            }
                            strJsonResult = JsonConvert.SerializeObject(list.OrderByDescending(m => m.Index).ThenBy(x => x.Name).ToList());
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                            result = true;
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetHabGroupTreeByUserAccount异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchHabGroupTreeByUserAccount(Guid transactionid, string userAccount, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"UserAccount:{userAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchHabGroupTreeByUserAccount";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<DirectoryEntry> users = new List<DirectoryEntry>();
                    if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), userAccount, SearchType.MailUser, out users, out errormsg))
                    {
                        error.Code = ErrorCode.UserNotExist;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Error("TreeNodeManager调用SearchHabGroupTreeByUserAccount异常", paramstr, errormsg, transactionid);
                        LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetHabGroupTree(transactionid, userAccount, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<HabNodeInfo> nodelist = new List<HabNodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), searchstr, SearchType.GroupAndUser, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            HabNodeInfo node = new HabNodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Index = entry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(entry.Properties["msDS-HABSeniorityIndex"].Value);
                            node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                            if (node.Type == NodeType.user)
                            {
                                node.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                int userAccountControl = Convert.ToInt32(entry.Properties["userAccountControl"][0]);
                                node.Status = UserProvider.IsAccountActive(userAccountControl);
                            }
                            nodelist.Add(node);
                        }

                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderByDescending(m => m.Index).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchHabGroupTreeByUserAccount异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetSamelevelOuTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetSamelevelOuTree";

            try
            {
                do
                {
                    string nodePath = string.Empty;
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    if (nodeID == Guid.Empty)
                    {
                        //根节点
                        for (int i = 0; i < admin.SameLevelOuList.Count; i++)
                        {
                            NodeInfo node = new NodeInfo();
                            if (!commonProvider.GetRootNodeInfo(admin.SameLevelOuList[i].SamelevelOuPath, out node, out errormsg))
                            {
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }
                            list.Add(node);
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                    else
                    {
                        if (!commonProvider.GetTreeData(nodeID, admin, SearchType.All, out list, out errormsg))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                        else
                        {
                            error.Code = ErrorCode.None;
                            strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                            result = true;
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetSamelevelOuTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchSamelevelOuTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchSamelevelOuTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetSamelevelOuTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        for (int i = 0; i < admin.SameLevelOuList.Count; i++)
                        {
                            if (!commonProvider.SearchEntryData(ConfigADProvider.GetADPathByLdap(admin.SameLevelOuList[i].SamelevelOuPath), searchstr, SearchType.All, out lists, out errormsg))
                            {
                                error.Code = ErrorCode.SearchADDataError;
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }

                            foreach (DirectoryEntry entry in lists)
                            {
                                NodeInfo node = new NodeInfo();
                                node.NodeID = entry.Guid;
                                node.Name = Convert.ToString(entry.Properties["name"].Value);
                                node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                                if (node.Type == NodeType.user)
                                {
                                    node.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                    int userAccountControl = Convert.ToInt32(entry.Properties["userAccountControl"][0]);
                                    node.Status = UserProvider.IsAccountActive(userAccountControl);
                                }
                                nodelist.Add(node);
                            }
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchMailUserTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetSamelevelOuGroupTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetSamelevelOuGroupTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    if (!commonProvider.GetSamelevelOuGroupTreeData(nodeID, admin, out list, out errormsg))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    else
                    {
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetGroupTreeData异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchSamelevelOuGroupTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchSamelevelOuGroupTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetSamelevelOuGroupTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        for (int i = 0; i < admin.SameLevelOuList.Count; i++)
                        {
                            if (!commonProvider.SearchEntryData(ConfigADProvider.GetADPathByLdap(admin.SameLevelOuList[i].SamelevelOuPath), searchstr, SearchType.GroupAndNoHab, out lists, out errormsg))
                            {
                                error.Code = ErrorCode.SearchADDataError;
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }

                            foreach (DirectoryEntry entry in lists)
                            {
                                NodeInfo node = new NodeInfo();
                                node.NodeID = entry.Guid;
                                node.Name = Convert.ToString(entry.Properties["name"].Value);
                                node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                                nodelist.Add(node);
                            }
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchSamelevelOuGroupTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetSamelevelOuTreeOnlyOu(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetSamelevelOuTreeOnlyOu";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    if (!commonProvider.GetSamelevelOuTreeOnlyOu(nodeID, admin, out list, out errormsg))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    else
                    {
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetSamelevelOuTreeOnlyOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchSamelevelOuTreeOnlyOu(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchSamelevelOuTreeOnlyOu";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetSamelevelOuTreeOnlyOu(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        for (int i = 0; i < admin.SameLevelOuList.Count; i++)
                        {
                            if (!commonProvider.SearchEntryData(ConfigADProvider.GetADPathByLdap(admin.SameLevelOuList[i].SamelevelOuPath), searchstr, SearchType.Ou, out lists, out errormsg))
                            {
                                error.Code = ErrorCode.SearchADDataError;
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }

                            foreach (DirectoryEntry entry in lists)
                            {
                                NodeInfo node = new NodeInfo();
                                node.NodeID = entry.Guid;
                                node.Name = Convert.ToString(entry.Properties["name"].Value);
                                node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                                nodelist.Add(node);
                            }
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchSamelevelOuTreeOnlyOu异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetDefaultCompanyTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetDefaultCompanyTree";

            try
            {
                do
                {
                    string nodePath = string.Empty;
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    if (nodeID == Guid.Empty)
                    {
                        //根节点
                        nodePath = ConfigADProvider.GetCompanyOuDistinguishedName();;
                        NodeInfo info = new NodeInfo();
                        if (!commonProvider.GetRootNodeInfo(nodePath, out info, out errormsg))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        list.Add(info);
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        error.Code = ErrorCode.None;
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        //strJsonResult = json;
                        result = true;
                    }
                    else
                    {
                        if (!commonProvider.GetTreeData(nodeID, out list, out errormsg))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                        else
                        {
                            error.Code = ErrorCode.None;
                            strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                            //strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                            result = true;
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetDefaultCompanyTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchDefaultCompanyTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchDefaultCompanyTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetDefaultCompanyTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), searchstr, SearchType.AllNoHab, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            NodeInfo node = new NodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                            if (node.Type == NodeType.user)
                            {
                                node.Name = Convert.ToString(entry.Properties["displayname"].Value);
                                node.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                int userAccountControl = Convert.ToInt32(entry.Properties["userAccountControl"][0]);
                                node.Status = UserProvider.IsAccountActive(userAccountControl);
                            }
                            
                            int count = 0;
                            commonProvider.GetMemberCount(entry, out count, out errormsg);
                            node.MemberCount = count;
                            nodelist.Add(node);
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchDefaultCompanyTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetDefaultOrgTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetDefaultOrgTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();
                    if (!commonProvider.GetDefaultOrgTreeData(nodeID, admin, out list, out errormsg))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    else
                    {
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetDefaultOrgTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchDefaultOrgTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchDefaultOrgTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        string nodePath = ConfigADProvider.GetCompanyOuDistinguishedName() ;
                        DirectoryEntry info = new DirectoryEntry();
                        if (!commonProvider.GetADEntryByPath(nodePath, out info, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        result = GetDefaultOrgTree(transactionid, admin, info.Guid, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        if (!commonProvider.SearchEntryData(ConfigADProvider.GetCompanyADRootPath(), searchstr, SearchType.Ou, out lists, out errormsg))
                        {
                            error.Code = ErrorCode.SearchADDataError;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }

                        foreach (DirectoryEntry entry in lists)
                        {
                            NodeInfo node = new NodeInfo();
                            node.NodeID = entry.Guid;
                            node.Name = Convert.ToString(entry.Properties["name"].Value);
                            node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            node.isParent = node.Type == NodeType.organizationalUnit ? true : false;

                            nodelist.Add(node);
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Desc).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchDefaultOrgTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetMailBoxDataOrgTree(Guid transactionid, AdminInfo admin, Guid nodeID, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||NodeID:{nodeID}";
            string funname = "GetMailBoxDataOrgTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<NodeInfo> list = new List<NodeInfo>();

                    if (!commonProvider.GetMailBoxDataOrgTree(nodeID, admin, out list, out errormsg))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    else
                    {
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用GetDefaultOrgTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool SearchMailBoxDataOrgTree(Guid transactionid, AdminInfo admin, string searchstr, out string strJsonResult)
        {
             bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string errormsg = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "SearchMailBoxDataOrgTree";

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        result = GetMailBoxDataOrgTree(transactionid, admin, Guid.Empty, out strJsonResult);
                        break;
                    }
                    else
                    {
                        List<DirectoryEntry> lists = new List<DirectoryEntry>();
                        List<NodeInfo> nodelist = new List<NodeInfo>();
                        MailDataBaseDBProvider dBProvider = new MailDataBaseDBProvider();
                        List<string> ouPaths = new List<string>();
                        if (!dBProvider.GetMailDataBaseTreeList(transactionid, admin, out ouPaths, out error))
                        {
                            result = false;
                            break;
                        }
                        for (int i = 0; i < ouPaths.Count; i++)
                        {
                            if (!commonProvider.SearchEntryData(ConfigADProvider.GetADPathByLdap(ouPaths[i]), searchstr, SearchType.Ou, out lists, out errormsg))
                            {
                                error.Code = ErrorCode.SearchADDataError;
                                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                                result = false;
                                break;
                            }

                            foreach (DirectoryEntry entry in lists)
                            {
                                NodeInfo node = new NodeInfo();
                                node.NodeID = entry.Guid;
                                node.Name = Convert.ToString(entry.Properties["name"].Value);
                                node.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                node.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                node.isParent = node.Type == NodeType.organizationalUnit ? true : false;
                                nodelist.Add(node);
                            }
                        }
                        error.Code = ErrorCode.None;
                        strJsonResult = JsonConvert.SerializeObject(nodelist.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("TreeNodeManager调用SearchMailBoxDataOrgTree异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

    }
}
