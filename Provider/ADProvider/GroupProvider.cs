using Common;
using Entity;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.ADProvider
{
    public class GroupProvider
    {
        public bool AddGroup(Guid transactionid, AdminInfo admin, ref GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||DisplayName:{group.DisplayName}";
            //paramstr += $"||SAMAccountName:{group.SAMAccountName}";
            paramstr += $"||Description:{group.Description}";
            paramstr += $"||Account:{group.Account}";
            paramstr += $"||Type:{group.Type}";
            paramstr += $"||ParentOuId:{group.ParentOuId}";

            DirectoryEntry OuEntry = new DirectoryEntry();
            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.ParentOuId, out OuEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    GroupEntry = OuEntry.Children.Add(string.Format("CN = {0}", group.DisplayName), "group");
                    GroupEntry.Properties["name"].Value = group.DisplayName;
                    GroupEntry.Properties["sAMAccountName"].Value = group.DisplayName;
                    GroupEntry.Properties["displayName"].Value = group.DisplayName;
                    if (!string.IsNullOrEmpty(group.Account))
                    {
                        GroupEntry.Properties["mail"].Value = group.Account;
                    }
                    GroupEntry.CommitChanges();
                    if (string.IsNullOrEmpty(group.Description.Trim()))
                    {
                        GroupEntry.Properties["description"].Clear();
                    }
                    else
                    {
                        GroupEntry.Properties["description"].Value = group.Description;
                    }

                    /// <summary>
                    ///  通用+通讯组 1
                    /// </summary>
                    // GROUP_TYPE_UNIVERSAL_GROUP = 8,

                    /// <summary>
                    /// 全局+通讯组 2
                    /// </summary>
                    //GROUP_TYPE_ACCOUNT_GROUP = 2,

                    /// <summary>
                    /// 通用+安全组 3
                    /// </summary>
                    //GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL = -2147483640,

                    /// <summary>
                    ///  全局+安全组 4
                    /// </summary>
                    // GROUP_TYPE_SECURITY_ENABLED_ACCOUNT = -2147483646,

                    int TempGroupValue = 8;
                    switch (group.Type)
                    {
                        case GroupType.GROUP_TYPE_UNIVERSAL_GROUP:
                            TempGroupValue = 8;
                            break;
                        case GroupType.GROUP_TYPE_ACCOUNT_GROUP:
                            TempGroupValue = 2;
                            break;
                        case GroupType.GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL:
                            TempGroupValue = -2147483640;
                            break;
                        case GroupType.GROUP_TYPE_SECURITY_ENABLED_ACCOUNT:
                            TempGroupValue = -2147483646;
                            break;
                    }
                    GroupEntry.Properties["grouptype"].Value = TempGroupValue;

                    GroupEntry.CommitChanges();
                    group.GroupID = GroupEntry.Guid;
                    GroupEntry.Close();
                    OuEntry.CommitChanges();
                    OuEntry.Close();



                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用AddGroup异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (OuEntry != null)
                {
                    OuEntry.Close();
                }

                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }

        /// <summary>
        /// 添加Hab组
        /// </summary>
        /// <param name="strParentPath"></param>
        /// <param name="strGroupName"></param>
        /// <param name="iGroupType"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool AddHabGroup(Guid transactionid, AdminInfo admin, ref GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            string message = string.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||DisplayName:{group.DisplayName}";

            paramstr += $"||Account:{group.Account}";
            paramstr += $"||Type:{group.Type}";
            paramstr += $"||ParentOuId:{group.ParentOuId}";

            DirectoryEntry OuEntry = new DirectoryEntry();
            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.ParentOuId, out OuEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    GroupEntry = OuEntry.Children.Add(string.Format("CN = {0}", group.DisplayName), "group");
                    GroupEntry.Properties["name"].Value = group.DisplayName;
                    GroupEntry.Properties["sAMAccountName"].Value = group.DisplayName;
                    GroupEntry.Properties["displayName"].Value = group.DisplayName;
                    GroupEntry.Properties["msOrg-IsOrganizational"].Value = "TRUE";
                    GroupEntry.Properties["msDS-HABSeniorityIndex"].Value = group.Index;
                    
                    int TempGroupValue = 8;
                    switch (group.Type)
                    {
                        case GroupType.GROUP_TYPE_UNIVERSAL_GROUP:
                            TempGroupValue = 8;
                            break;
                        case GroupType.GROUP_TYPE_ACCOUNT_GROUP:
                            TempGroupValue = 2;
                            break;
                        case GroupType.GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL:
                            TempGroupValue = -2147483640;
                            break;
                        case GroupType.GROUP_TYPE_SECURITY_ENABLED_ACCOUNT:
                            TempGroupValue = -2147483646;
                            break;
                    }
                    GroupEntry.Properties["grouptype"].Value = TempGroupValue;
                    
                    OuEntry.CommitChanges();
                    OuEntry.Close();
                    GroupEntry.CommitChanges();
                    group.GroupID = GroupEntry.Guid;
                    group.DistinguishedName = Convert.ToString(GroupEntry.Properties["distinguishedName"].Value);
                    GroupEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用AddHabGroup异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (OuEntry != null)
                {
                    OuEntry.Close();
                }

                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }

            return bResult;
        }

        public bool DeleteGroup(Guid transactionid, AdminInfo admin, ref GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{group.GroupID}";

            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    group.DisplayName = Convert.ToString(GroupEntry.Properties["name"].Value);
                    group.DistinguishedName = Convert.ToString(GroupEntry.Properties["DistinguishedName"].Value);
                    GroupEntry.Parent.Children.Remove(GroupEntry);
                    GroupEntry.CommitChanges();

                    GroupEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用DeleteGroup异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }

        public bool ModifyGroup(Guid transactionid, AdminInfo admin, GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||DisplayName:{group.DisplayName}";
            paramstr += $"||Description:{group.Description}";
            paramstr += $"||Account:{group.Account}";
            paramstr += $"||Type:{group.Type}";
            paramstr += $"||GroupID:{group.GroupID}";

            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    if (Convert.ToString(GroupEntry.Properties["displayname"].Value) != group.DisplayName)
                    {
                        GroupEntry.Rename(string.Format("CN = {0}", group.DisplayName));
                        GroupEntry.Properties["sAMAccountName"].Value = group.DisplayName;
                        GroupEntry.Properties["displayName"].Value = group.DisplayName;
                    }

                    if (string.IsNullOrEmpty(group.Description.Trim()))
                    {
                        GroupEntry.Properties["description"].Clear();
                    }
                    else
                    {
                        GroupEntry.Properties["description"].Value = group.Description.Trim();
                    }

                    GroupEntry.Properties["member"].Clear();
                    GroupEntry.Properties["managedBy"].Clear();
                    GroupEntry.Properties["msExchCoManagedByLink"].Clear();

                    GroupEntry.CommitChanges();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用ModifyGroup异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }
        
        public bool RenameHabGroup(Guid transactionid, GroupInfo group, string newGroupName, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||GroupID:{group.GroupID}";
            paramstr += $"||NewGroupName:{newGroupName}";

            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    string groupOldName = Convert.ToString(GroupEntry.Properties["name"].Value);
                    if (groupOldName != newGroupName)
                    {
                        DirectoryEntry entry = new DirectoryEntry();
                        if (commonProvider.GetOneLevelSigleEntryByCN(GroupEntry.Parent.Path, newGroupName, out entry, out strError))
                        {
                            error.Code = ErrorCode.HaveSameDisplayName;
                            bResult = false;
                            break;
                        }
                        if (commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetRootADPath(), newGroupName, out entry, out strError))
                        {
                            error.Code = ErrorCode.RootHaveSameDisplayName;
                            error.SetInfo(Convert.ToString(entry.Parent.Properties["distinguishedName"].Value));
                            bResult = false;
                            break;
                        }
                    }

                    GroupEntry.Rename(string.Format("CN = {0}", newGroupName));
                    GroupEntry.Properties["sAMAccountName"].Value = newGroupName;
                    GroupEntry.Properties["displayName"].Value = newGroupName;
                    GroupEntry.CommitChanges();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用RenameHabGroup异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }

        public bool GetGroupInfo(Guid transactionid, AdminInfo admin, ref GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{group.GroupID}";

            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    group.DisplayName = GroupEntry.Properties["displayName"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["displayName"].Value);
                    group.Name = GroupEntry.Properties["name"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["name"].Value);
                    group.Description = GroupEntry.Properties["description"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["description"].Value);
                    group.Account = GroupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["mail"].Value);
                    group.Index = GroupEntry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(GroupEntry.Properties["msDS-HABSeniorityIndex"].Value);
                    group.DistinguishedName = GroupEntry.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["distinguishedName"].Value);
                    group.ParentOuId = GroupEntry.Parent.Guid;
                    group.ParentDistinguishedName = GroupEntry.Parent.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(GroupEntry.Parent.Properties["distinguishedName"].Value);

                    /// <summary>
                    ///  通用+通讯组 1
                    /// </summary>
                    // GROUP_TYPE_UNIVERSAL_GROUP = 8,

                    /// <summary>
                    /// 全局+通讯组 2
                    /// </summary>
                    //GROUP_TYPE_ACCOUNT_GROUP = 2,

                    /// <summary>
                    /// 通用+安全组 3
                    /// </summary>
                    //GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL = -2147483640,

                    /// <summary>
                    ///  全局+安全组 4
                    /// </summary>
                    // GROUP_TYPE_SECURITY_ENABLED_ACCOUNT = -2147483646,

                    int TempGroupValue = GroupEntry.Properties["grouptype"].Value == null ? 0 : Convert.ToInt32(GroupEntry.Properties["grouptype"].Value);
                    int TempGroupType = 1;
                    switch (TempGroupValue)
                    {
                        case 8:
                            TempGroupType = 1;
                            break;
                        case 2:
                            TempGroupType = 2;
                            break;
                        case -2147483640:
                            TempGroupType = 3;
                            break;
                        case -2147483646:
                            TempGroupType = 4;
                            break;
                    }

                    group.Type = (GroupType)TempGroupType;

                    DirectoryEntry Entry = new DirectoryEntry();
                    //string TempLdapRootPath = GroupEntry.Path.Substring(0, GroupEntry.Path.LastIndexOf("/") + 1);
                    for (int i = 0; i < GroupEntry.Properties["member"].Count; i++)
                    {
                        string MemberDnName = Convert.ToString(GroupEntry.Properties["member"][i]);
                        if (!commonProvider.GetADEntryByPath(MemberDnName, out Entry, out strError))
                        {
                            continue;
                        }

                        GroupMember member = new GroupMember();
                        member.ID = Entry.Guid;
                        //member.Account = Entry.Properties["mail"].Value == null ? "-" : Convert.ToString(Entry.Properties["mail"].Value);
                        member.DisplayName = Entry.Properties["name"].Value == null ? "" : Convert.ToString(Entry.Properties["name"].Value);
                        member.Index = Entry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(Entry.Properties["msDS-HABSeniorityIndex"].Value);
                        member.Type = (NodeType)Enum.Parse(typeof(NodeType), Entry.SchemaClassName);
                        if (member.Type == NodeType.user)
                        {
                            member.DisplayName = Entry.Properties["displayname"].Value == null ? "" : Convert.ToString(Entry.Properties["displayname"].Value);
                            member.Account = Entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(Entry.Properties["userPrincipalName"].Value);
                        }
                        else if (member.Type == NodeType.group)
                        {
                            member.MemberCount = Entry.Properties["member"].Count;
                            member.IsOrganizational = Entry.Properties["msOrg-IsOrganizational"].Value == null ? false : Convert.ToBoolean(Convert.ToInt32(Entry.Properties["msOrg-IsOrganizational"].Value));
                            member.Type = member.IsOrganizational ? NodeType.habgroup : NodeType.group;
                            member.Account = Entry.Properties["mail"].Value == null ? "-" : Convert.ToString(Entry.Properties["mail"].Value);
                        }
                        group.Members.Add(member);
                    }
                    if (GroupEntry.Properties["managedBy"].Value != null)
                    {
                        string adminDnName = Convert.ToString(GroupEntry.Properties["managedBy"].Value);
                        if (!commonProvider.GetADEntryByPath(adminDnName, out Entry, out strError))
                        {
                            continue;
                        }

                        UserInfo user = new UserInfo();
                        user.UserID = Entry.Guid;
                        user.DisplayName = Entry.Properties["displayname"].Value == null ? "" : Convert.ToString(Entry.Properties["displayname"].Value);
                        user.UserAccount = Entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(Entry.Properties["userPrincipalName"].Value);
                        group.AdminsName += user.DisplayName + "(" + user.UserAccount + ")，";
                        group.Admins.Add(user);
                    }
                    for (int i = 0; i < GroupEntry.Properties["msExchCoManagedByLink"].Count; i++)
                    {
                        string MemberDnName = Convert.ToString(GroupEntry.Properties["msExchCoManagedByLink"][i]);
                        if (!commonProvider.GetADEntryByPath(MemberDnName, out Entry, out strError))
                        {
                            continue;
                        }

                        UserInfo user = new UserInfo();
                        user.UserID = Entry.Guid;
                        user.DisplayName = Entry.Properties["displayname"].Value == null ? "" : Convert.ToString(Entry.Properties["displayname"].Value);
                        user.UserAccount = Entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(Entry.Properties["userPrincipalName"].Value);
                        group.AdminsName += user.DisplayName + "(" + user.UserAccount + ")，";
                        if (!group.Admins.Any(u => u.UserID == user.UserID))
                        {
                            group.Admins.Add(user);
                        }
                    }

                    group.AdminsName = string.IsNullOrEmpty(group.AdminsName) ? string.Empty : group.AdminsName.Remove(group.AdminsName.LastIndexOf('，'), 1);
                    group.AdminsCount = group.Admins.Count;

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用GetGroupInfo异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }

        public bool GetGroupInfo(Guid transactionid, Guid groupID, out GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            group = new GroupInfo();
            string paramstr = string.Empty;
            paramstr += $"||GroupID:{groupID}";

            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    group.GroupID = groupID;
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    group.DisplayName = GroupEntry.Properties["displayName"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["displayName"].Value);
                    group.Name = GroupEntry.Properties["name"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["name"].Value);
                    group.Description = GroupEntry.Properties["description"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["description"].Value);
                    group.Account = GroupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["mail"].Value);
                    group.Index = GroupEntry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(GroupEntry.Properties["msDS-HABSeniorityIndex"].Value);
                    group.DistinguishedName = GroupEntry.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["distinguishedName"].Value);
                    group.ParentOuId = GroupEntry.Parent.Guid;
                    group.ParentDistinguishedName = GroupEntry.Parent.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(GroupEntry.Parent.Properties["distinguishedName"].Value);
                    group.IsOrganizational = GroupEntry.Properties["msOrg-IsOrganizational"].Value == null ? false : Convert.ToBoolean(Convert.ToInt32(GroupEntry.Properties["msOrg-IsOrganizational"].Value));
                    int TempGroupValue = GroupEntry.Properties["grouptype"].Value == null ? 0 : Convert.ToInt32(GroupEntry.Properties["grouptype"].Value);
                    int TempGroupType = 1;
                    switch (TempGroupValue)
                    {
                        case 8:
                            TempGroupType = 1;
                            break;
                        case 2:
                            TempGroupType = 2;
                            break;
                        case -2147483640:
                            TempGroupType = 3;
                            break;
                        case -2147483646:
                            TempGroupType = 4;
                            break;
                    }

                    group.Type = (GroupType)TempGroupType;

                    DirectoryEntry Entry = new DirectoryEntry();
                    //string TempLdapRootPath = GroupEntry.Path.Substring(0, GroupEntry.Path.LastIndexOf("/") + 1);
                    for (int i = 0; i < GroupEntry.Properties["member"].Count; i++)
                    {
                        string MemberDnName = Convert.ToString(GroupEntry.Properties["member"][i]);
                        if (!commonProvider.GetADEntryByPath(MemberDnName, out Entry, out strError))
                        {
                            continue;
                        }

                        GroupMember member = new GroupMember();
                        member.ID = Entry.Guid;
                        member.Account = Entry.Properties["mail"].Value == null ? "-" : Convert.ToString(Entry.Properties["mail"].Value);
                        member.DisplayName = Entry.Properties["name"].Value == null ? "" : Convert.ToString(Entry.Properties["name"].Value);
                        member.Index = Entry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(Entry.Properties["msDS-HABSeniorityIndex"].Value);
                        member.Type = (NodeType)Enum.Parse(typeof(NodeType), Entry.SchemaClassName);
                        if (member.Type == NodeType.user)
                        {
                            member.Account = Entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(Entry.Properties["userPrincipalName"].Value);
                        }
                        else if (member.Type == NodeType.group)
                        {
                            member.MemberCount = Entry.Properties["member"].Count;
                            member.IsOrganizational = Entry.Properties["msOrg-IsOrganizational"].Value == null ? false : Convert.ToBoolean(Convert.ToInt32(Entry.Properties["msOrg-IsOrganizational"].Value));
                            member.Type = member.IsOrganizational ? NodeType.habgroup : NodeType.group;
                            member.Account = Entry.Properties["mail"].Value == null ? "-" : Convert.ToString(Entry.Properties["mail"].Value);
                        }
                        group.Members.Add(member);
                    }
                    if (GroupEntry.Properties["managedBy"].Value != null)
                    {
                        string adminDnName = Convert.ToString(GroupEntry.Properties["managedBy"].Value);
                        if (!commonProvider.GetADEntryByPath(adminDnName, out Entry, out strError))
                        {
                            continue;
                        }

                        UserInfo user = new UserInfo();
                        user.UserID = Entry.Guid;
                        user.DisplayName = Entry.Properties["name"].Value == null ? "" : Convert.ToString(Entry.Properties["name"].Value);
                        user.UserAccount = Entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(Entry.Properties["userPrincipalName"].Value);
                        group.AdminsName += user.DisplayName + "(" + user.UserAccount + ")，";
                        group.Admins.Add(user);
                    }
                    for (int i = 0; i < GroupEntry.Properties["msExchCoManagedByLink"].Count; i++)
                    {
                        string MemberDnName = Convert.ToString(GroupEntry.Properties["msExchCoManagedByLink"][i]);
                        if (!commonProvider.GetADEntryByPath(MemberDnName, out Entry, out strError))
                        {
                            continue;
                        }

                        UserInfo user = new UserInfo();
                        user.UserID = Entry.Guid;
                        user.DisplayName = Entry.Properties["name"].Value == null ? "" : Convert.ToString(Entry.Properties["name"].Value);
                        user.UserAccount = Entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(Entry.Properties["userPrincipalName"].Value);
                        group.AdminsName += user.DisplayName + "(" + user.UserAccount + ")，";
                        if (!group.Admins.Any(u => u.UserID == user.UserID))
                        {
                            group.Admins.Add(user);
                        }
                    }

                    group.AdminsName = string.IsNullOrEmpty(group.AdminsName) ? string.Empty : group.AdminsName.Remove(group.AdminsName.LastIndexOf('，'), 1);
                    group.AdminsCount = group.Admins.Count;

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用GetGroupInfo异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }

        public bool AddGroupMember(Guid transactionid, AdminInfo admin, GroupInfo group, GroupMember member, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{group.GroupID}";
            paramstr += $"||MemberID:{member.ID}";

            DirectoryEntry Entry = new DirectoryEntry();
            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    if (!commonProvider.GetADEntryByGuid(member.ID, out Entry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    if (group.GroupID != member.ID)
                    {
                        if (Entry.Properties["distinguishedName"].Value != null)
                        {
                            GroupEntry.Properties["member"].Add(Convert.ToString(Entry.Properties["distinguishedName"].Value));
                        }

                        GroupEntry.CommitChanges();
                        GroupEntry.Close();
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用AddGroupMember异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }

                if (Entry != null)
                {
                    Entry.Close();
                }
            }
            return bResult;
        }

        public bool AddGroupMember(Guid transactionid, Guid parentID, Guid memberID, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"parentID:{parentID}";
            paramstr += $"||memberID:{memberID}";

            DirectoryEntry parentEntry = new DirectoryEntry();
            DirectoryEntry memberEntry = new DirectoryEntry();
            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(parentID, out parentEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    if (!commonProvider.GetADEntryByGuid(memberID, out memberEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    if (parentID != memberID)
                    {
                        parentEntry.Properties["member"].Add(Convert.ToString(memberEntry.Properties["distinguishedName"].Value));
                        parentEntry.CommitChanges();
                        parentEntry.Close();
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用AddGroupMember异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (parentEntry != null)
                {
                    parentEntry.Close();
                }

                if (memberEntry != null)
                {
                    memberEntry.Close();
                }
            }
            return bResult;
        }

        public bool UpdateHabGroupMember(string groupName, List<GroupInfo> childhabList, List<UserInfo> userList, List<GroupInfo> groupList, out GroupInfo habGroup, out string error)
        {
            error = string.Empty;
            DirectoryEntry GroupEntry = null;
            habGroup = new GroupInfo();
            try
            {
                CommonProvider commonProvider = new CommonProvider();
                if (commonProvider.GetHabGroupEntryByName(groupName, out GroupEntry, out error))
                {
                    habGroup.GroupID = GroupEntry.Guid;
                    habGroup.DisplayName = GroupEntry.Properties["name"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["name"].Value);
                    habGroup.Description = GroupEntry.Properties["description"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["description"].Value);
                    habGroup.Account = GroupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["mail"].Value);
                    habGroup.Index = GroupEntry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(GroupEntry.Properties["msDS-HABSeniorityIndex"].Value);
                    habGroup.DistinguishedName = GroupEntry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(GroupEntry.Properties["distinguishedName"].Value);

                    GroupEntry.Properties["member"].Clear();
                    GroupEntry.CommitChanges();

                    foreach (GroupInfo childhab in childhabList)
                    {
                        GroupEntry.Properties["member"].Add(childhab.DistinguishedName);
                        GroupEntry.CommitChanges();
                    }

                    foreach (UserInfo childuser in userList)
                    {
                        GroupEntry.Properties["member"].Add(childuser.DistinguishedName);
                        GroupEntry.CommitChanges();
                    }
                    foreach (GroupInfo childgroup in groupList)
                    {
                        GroupEntry.Properties["member"].Add(childgroup.DistinguishedName);
                        GroupEntry.CommitChanges();
                    }
                    GroupEntry.Close();
                }

                else
                {
                    return false;
                }
            }

            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
            return true;
        }

        public bool IncrementUpdateGroupMember(string groupName, List<GroupInfo> childhabList, List<UserInfo> userList, List<GroupInfo> groupList, out GroupInfo habGroup, out string error)
        {
            error = string.Empty;
            DirectoryEntry GroupEntry = null;
            habGroup = new GroupInfo();
            try
            {
                CommonProvider commonProvider = new CommonProvider();
                if (commonProvider.GetHabGroupEntryByName(groupName, out GroupEntry, out error))
                {
                    habGroup.GroupID = GroupEntry.Guid;
                    habGroup.DisplayName = GroupEntry.Properties["name"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["name"].Value);
                    habGroup.Description = GroupEntry.Properties["description"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["description"].Value);
                    habGroup.Account = GroupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["mail"].Value);
                    habGroup.Index = GroupEntry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(GroupEntry.Properties["msDS-HABSeniorityIndex"].Value);
                    habGroup.DistinguishedName = GroupEntry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(GroupEntry.Properties["distinguishedName"].Value);

                    foreach (GroupInfo childhab in childhabList)
                    {
                        GroupEntry.Properties["member"].Add(childhab.DistinguishedName);
                        GroupEntry.CommitChanges();
                    }

                    foreach (UserInfo childuser in userList)
                    {
                        GroupEntry.Properties["member"].Add(childuser.DistinguishedName);
                        GroupEntry.CommitChanges();
                    }
                    foreach (GroupInfo childgroup in groupList)
                    {
                        GroupEntry.Properties["member"].Add(childgroup.DistinguishedName);
                        GroupEntry.CommitChanges();
                    }
                    GroupEntry.Close();
                }

                else
                {
                    return false;
                }
            }

            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
            return true;
        }

        public bool GetHabGroupInfoByOu(Guid transactionid, Guid ouID, out GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            group = new GroupInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"ouID:{ouID}";

            DirectoryEntry OuEntry = new DirectoryEntry();
            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ouID, out OuEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    if (!commonProvider.GetOneLevelSigleHabGroupEntry(OuEntry.Path, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    group.GroupID = GroupEntry.Guid;
                    group.DisplayName = GroupEntry.Properties["name"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["name"].Value);
                    group.Description = GroupEntry.Properties["description"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["description"].Value);
                    group.Account = GroupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["mail"].Value);
                    group.Index = GroupEntry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(GroupEntry.Properties["msDS-HABSeniorityIndex"].Value);
                    group.DistinguishedName = GroupEntry.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["distinguishedName"].Value);
                    group.ParentDistinguishedName = GroupEntry.Parent.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(GroupEntry.Parent.Properties["distinguishedName"].Value);

                    /// <summary>
                    ///  通用+通讯组 1
                    /// </summary>
                    // GROUP_TYPE_UNIVERSAL_GROUP = 8,

                    /// <summary>
                    /// 全局+通讯组 2
                    /// </summary>
                    //GROUP_TYPE_ACCOUNT_GROUP = 2,

                    /// <summary>
                    /// 通用+安全组 3
                    /// </summary>
                    //GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL = -2147483640,

                    /// <summary>
                    ///  全局+安全组 4
                    /// </summary>
                    // GROUP_TYPE_SECURITY_ENABLED_ACCOUNT = -2147483646,

                    int TempGroupValue = GroupEntry.Properties["grouptype"].Value == null ? 0 : Convert.ToInt32(GroupEntry.Properties["grouptype"].Value);
                    int TempGroupType = 1;
                    switch (TempGroupValue)
                    {
                        case 8:
                            TempGroupType = 1;
                            break;
                        case 2:
                            TempGroupType = 2;
                            break;
                        case -2147483640:
                            TempGroupType = 3;
                            break;
                        case -2147483646:
                            TempGroupType = 4;
                            break;
                    }

                    group.Type = (GroupType)TempGroupType;

                    DirectoryEntry Entry = new DirectoryEntry();
                    //string TempLdapRootPath = GroupEntry.Path.Substring(0, GroupEntry.Path.LastIndexOf("/") + 1);
                    for (int i = 0; i < GroupEntry.Properties["member"].Count; i++)
                    {
                        string MemberDnName = Convert.ToString(GroupEntry.Properties["member"][i]);
                        if (!commonProvider.GetADEntryByPath(MemberDnName, out Entry, out strError))
                        {
                            continue;
                        }


                        GroupMember member = new GroupMember();
                        member.ID = Entry.Guid;
                        member.Account = Entry.Properties["mail"].Value == null ? "" : Convert.ToString(Entry.Properties["mail"].Value);
                        member.DisplayName = Entry.Properties["name"].Value == null ? "" : Convert.ToString(Entry.Properties["name"].Value);
                        member.Type = (NodeType)Enum.Parse(typeof(NodeType), Entry.SchemaClassName);
                        if (member.Type == NodeType.user)
                        {
                            member.Account = Entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(Entry.Properties["userPrincipalName"].Value);
                        }
                        else if (member.Type == NodeType.group)
                        {
                            member.MemberCount = Entry.Properties["member"].Count;
                            member.IsOrganizational = Entry.Properties["msOrg-IsOrganizational"].Value == null ? false : Convert.ToBoolean(Convert.ToInt32(Entry.Properties["msOrg-IsOrganizational"].Value));
                            member.Type = member.IsOrganizational ? NodeType.habgroup : NodeType.group;
                            member.Account = Entry.Properties["mail"].Value == null ? "-" : Convert.ToString(Entry.Properties["mail"].Value);
                        }
                        group.Members.Add(member);
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用GetGroupInfo异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }

        public bool SetHabGroupIndex(Guid transactionid, Guid memberID, int index, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"MemberID:{memberID}";
            paramstr += $"||Index:{index}";

            DirectoryEntry memberEntry = new DirectoryEntry();
            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(memberID, out memberEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    memberEntry.Properties["msDS-HABSeniorityIndex"].Value = index;
                    memberEntry.CommitChanges();
                    memberEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用SetHabGroupIndex异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (memberEntry != null)
                {
                    memberEntry.Close();
                }
            }
            return bResult;
        }

        public bool ClearGroupMembers(Guid transactionid, GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||GroupID:{group.GroupID}";

            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    GroupEntry.Properties["member"].Clear();
                    GroupEntry.CommitChanges();
                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("ClearGroupMembers异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }

        public bool RemoveHabMember(Guid transactionid, Guid memberID, GroupInfo habParentGroup, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||MemberID:{memberID}";
            paramstr += $"||ParentGroupID:{habParentGroup.GroupID}";

            DirectoryEntry MemberEntry = null;
            DirectoryEntry HabParentGroupEntry = null;
            try
            {
                do
                {

                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(memberID, out MemberEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("GroupProvider调用RemoveHabMember异常", paramstr, strError, transactionid);
                        bResult = false;
                        break;
                    }

                    if (!commonProvider.GetADEntryByGuid(habParentGroup.GroupID, out HabParentGroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("GroupProvider调用RemoveHabMember异常", paramstr, strError, transactionid);
                        bResult = false;
                        break;
                    }

                    HabParentGroupEntry.Properties["member"].Remove(MemberEntry.Properties["distinguishedName"].Value);
                    HabParentGroupEntry.CommitChanges();
                    HabParentGroupEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用RemoveHabMember异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (HabParentGroupEntry != null)
                {
                    HabParentGroupEntry.Close();
                }
                if (MemberEntry != null)
                {
                    MemberEntry.Close();
                }
            }
            return bResult;
        }

        public bool ClearGroupManagedBy(Guid transactionid, GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||GroupID:{group.GroupID}";

            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    GroupEntry.Properties["managedBy"].Clear();
                    GroupEntry.Properties["msExchCoManagedByLink"].Clear();
                    GroupEntry.CommitChanges();
                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("ClearGroupManagedBy异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }

        public bool AddGroupManagedBy(Guid transactionid, Guid groupID, Guid userID, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"groupID:{groupID}";
            paramstr += $"||userID:{userID}";

            DirectoryEntry groupEntry = new DirectoryEntry();
            DirectoryEntry adminEntry = new DirectoryEntry();
            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(groupID, out groupEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    if (!commonProvider.GetADEntryByGuid(userID, out adminEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    if (groupID != userID)
                    {
                        groupEntry.Properties["managedBy"].Value = adminEntry.Properties["distinguishedName"].Value;
                        groupEntry.Properties["msExchCoManagedByLink"].Add(Convert.ToString(adminEntry.Properties["distinguishedName"].Value));
                        groupEntry.CommitChanges();
                        groupEntry.Close();
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用AddGroupMember异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (groupEntry != null)
                {
                    groupEntry.Close();
                }

                if (adminEntry != null)
                {
                    adminEntry.Close();
                }
            }
            return bResult;
        }

        public bool ModifyHabGroup(Guid transactionid, AdminInfo admin, GroupInfo group, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{group.GroupID}";

            DirectoryEntry GroupEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out GroupEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    GroupEntry.Properties["msOrg-IsOrganizational"].Value = "TRUE";
                    GroupEntry.Properties["msDS-HABSeniorityIndex"].Value = group.Index;

                    GroupEntry.CommitChanges();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GroupProvider调用ModifyHabGroup异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (GroupEntry != null)
                {
                    GroupEntry.Close();
                }
            }
            return bResult;
        }
    }
}
