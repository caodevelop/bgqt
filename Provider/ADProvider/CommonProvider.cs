using Common;
using Entity;
using Provider.DBProvider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.ADProvider
{
    public class CommonProvider
    {
        public bool GetADEntryByPath(string dnPath, out DirectoryEntry entry, out string strError)
        {
            bool bResult = false;
            strError = string.Empty;
            entry = null;
            if (string.IsNullOrEmpty(dnPath))
            {
                strError = "CommonProvider.GetADEntryByPath Path路径为空";
                return false;
            }
            try
            {
                string path = ConfigADProvider.GetADPathByLdap(dnPath);
                entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                if (entry.Guid != null || entry.Guid != Guid.Empty)
                {
                    bResult = true;
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetADEntryByPath 通过路径:{0}实例对象 Exception:{1}", dnPath, ex.ToString());
                bResult = false;
            }
            return bResult;
        }

        public bool GetADEntryByGuid(Guid id, out DirectoryEntry entry, out string strError)
        {
            bool bResult = false;
            strError = string.Empty;
            entry = null;
            if (id == Guid.Empty)
            {
                strError = "Common_ADProvider.GetADEntryByGuid Path路径为空";
                return false;
            }
            try
            {
                string objectGUID = Guid2OctetString(id);//转换GUID
                string path = ConfigADProvider.GetRootADPath();
                DirectoryEntry de = new DirectoryEntry(path);
                DirectorySearcher deSearch = new DirectorySearcher(de);
                deSearch.SearchRoot = de;
                //deSearch.Filter = "(&(objectGUID=" + objectGUID + ")(!msOrg-IsOrganizational=TRUE))";
                deSearch.Filter = "(&(objectGUID=" + objectGUID + "))";
                deSearch.SearchScope = SearchScope.Subtree;
                SearchResult result = deSearch.FindOne();
                if (result != null)
                {
                    entry = result.GetDirectoryEntry();
                    bResult = true;
                }
            }
            catch (Exception ex)
            {
                //日志
                strError = string.Format("CommonProvider.GetADEntryByGuid 通过路径:{0}实例对象 Exception:{1}", id, ex.ToString());
                bResult = false;
            }
            return bResult;
        }

        public bool GetRootNodeInfo(string dnPath, out NodeInfo info, out string strError)
        {
            bool bResult = false;
            strError = string.Empty;
            info = new NodeInfo();
            if (string.IsNullOrEmpty(dnPath))
            {
                strError = "Common_ADProvider.GetADEntryByPath Path路径为空";
                return false;
            }
            try
            {
                int count = 0;
                DirectoryEntry entry = new DirectoryEntry(ConfigADProvider.GetADPathByLdap(dnPath), ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                if (entry != null)
                {
                    //GetMemberCount(entry, out count, out strError);
                    info.MemberCount = count;
                    info.NodeID = entry.Guid;
                    info.Name = Convert.ToString(entry.Properties["name"].Value);
                    info.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                    info.Desc = entry.Properties["description"].Value == null ? "" : Convert.ToString(entry.Properties["description"].Value);
                    info.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                    info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                    if (info.Type == NodeType.user)
                    {
                        info.Name = Convert.ToString(entry.Properties["displayname"].Value);
                        info.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                        int userAccountControl = Convert.ToInt32(entry.Properties["userAccountControl"][0]);
                        info.Status = UserProvider.IsAccountActive(userAccountControl);
                    }
                }
                bResult = true;
            }
            catch (Exception ex)
            {
                //日志
                strError = string.Format("CommonProvider.GetRootNodeInfo 通过路径:{0}实例对象 Exception:{1}", dnPath, ex.ToString());
                bResult = false;
            }
            return bResult;
        }

        public bool GetTreeData(Guid nodeID, out List<NodeInfo> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<NodeInfo>();
            int count = 0;

            try
            {
                do
                {
                    DirectoryEntry entry = new DirectoryEntry();
                    if (!GetADEntryByGuid(nodeID, out entry, out strError))
                    {
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = new DirectoryEntry(entry.Path);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.SearchRoot = de;
                    deSearch.Filter = "(&(|(objectClass=organizationalUnit)(objectClass=user)(objectClass=group))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))";
                    deSearch.SearchScope = SearchScope.OneLevel;
                    SearchResultCollection results = deSearch.FindAll();

                    if (results != null && results.Count > 0)
                    {
                        foreach (SearchResult Result in results)
                        {
                            DirectoryEntry item = Result.GetDirectoryEntry();
                            if (item.Path == entry.Path)
                            {
                                continue;
                            }
                            NodeInfo info = new NodeInfo();
                            info.NodeID = item.Guid;
                            info.Name = Convert.ToString(item.Properties["name"].Value);
                            info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                            info.Desc = item.Properties["description"].Value == null ? "" : Convert.ToString(item.Properties["description"].Value);
                            info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                            info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                            if (info.Type == NodeType.user)
                            {
                                info.Name = Convert.ToString(item.Properties["displayname"].Value);
                                info.UserAccount = item.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(item.Properties["userPrincipalName"].Value);
                                int userAccountControl = Convert.ToInt32(item.Properties["userAccountControl"][0]);
                                info.Status = UserProvider.IsAccountActive(userAccountControl);
                            }
                            //GetMemberCount(item, out count, out strError);
                            info.MemberCount = count;
                            items.Add(info);
                        }
                    }

                } while (false);

            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetTreeData 路径:{0}查找所有对象 Exception:{1}", nodeID, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetTreeData(Guid nodeID, AdminInfo admin, SearchType searchType, out List<NodeInfo> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<NodeInfo>();
            int count = 0;

            try
            {
                do
                {
                    DirectoryEntry entry = new DirectoryEntry();
                    if (!GetADEntryByGuid(nodeID, out entry, out strError))
                    {
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = new DirectoryEntry(entry.Path);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.SearchRoot = de;
                    deSearch.Filter = GetSearchType(searchType, string.Empty);
                    deSearch.SearchScope = SearchScope.OneLevel;
                    SearchResultCollection results = deSearch.FindAll();

                    if (results != null && results.Count > 0)
                    {
                        foreach (SearchResult Result in results)
                        {
                            DirectoryEntry item = Result.GetDirectoryEntry();
                            if (item.Path == entry.Path)
                            {
                                continue;
                            }
                            NodeInfo info = new NodeInfo();
                            info.NodeID = item.Guid;
                            info.Name = Convert.ToString(item.Properties["name"].Value);
                            info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                            info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                            info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                            if (info.Type == NodeType.user)
                            {
                                info.Name = Convert.ToString(item.Properties["displayname"].Value);
                                info.UserAccount = item.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(item.Properties["userPrincipalName"].Value);
                                int userAccountControl = Convert.ToInt32(item.Properties["userAccountControl"][0]);
                                info.Status = UserProvider.IsAccountActive(userAccountControl);
                            }
                            //GetMemberCount(item, out count, out strError);
                            info.MemberCount = count;
                            items.Add(info);
                        }
                    }

                } while (false);

            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetTreeData 路径:{0}查找所有对象 Exception:{1}", nodeID, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetCommonTreeData(Guid nodeID, AdminInfo admin, SearchType searchType, out List<NodeInfo> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<NodeInfo>();
            DirectoryEntry entry = new DirectoryEntry();
            int count = 0;

            try
            {
                do
                {
                    var rootAdmin = admin.ControlLimitOuList.Where(n => n.OUdistinguishedName.Equals(ConfigADProvider.GetCompanyOuDistinguishedName()));
                    //系统管理员
                    if (rootAdmin.Any())
                    {
                        //根节点
                        if (nodeID == Guid.Empty)
                        {
                            for (int i = 0; i < admin.ControlLimitOuList.Count; i++)
                            {
                                string path = ConfigADProvider.GetADPathByLdap(admin.ControlLimitOuList[i].OUdistinguishedName);
                                entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                                if (entry != null)
                                {
                                    NodeInfo item = new NodeInfo();
                                    item.NodeID = entry.Guid;
                                    item.Name = Convert.ToString(entry.Properties["name"].Value);
                                    item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                    item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                    item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                    items.Add(item);
                                }
                            }
                        }
                        else
                        {
                            if (!GetADEntryByGuid(nodeID, out entry, out strError))
                            {
                                bResult = false;
                                break;
                            }

                            DirectoryEntry de = new DirectoryEntry(entry.Path);
                            DirectorySearcher deSearch = new DirectorySearcher(de);
                            deSearch.SearchRoot = de;
                            deSearch.Filter = GetSearchType(searchType, string.Empty);
                            deSearch.SearchScope = SearchScope.OneLevel;
                            SearchResultCollection results = deSearch.FindAll();

                            if (results != null && results.Count > 0)
                            {
                                foreach (SearchResult Result in results)
                                {
                                    DirectoryEntry item = Result.GetDirectoryEntry();
                                    if (item.Path == entry.Path)
                                    {
                                        continue;
                                    }
                                    NodeInfo info = new NodeInfo();
                                    info.NodeID = item.Guid;
                                    info.Name = Convert.ToString(item.Properties["name"].Value);
                                    info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                                    info.Desc = item.Properties["description"].Value == null ? "" : Convert.ToString(item.Properties["description"].Value);
                                    info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                                    info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                                    if (info.Type == NodeType.user)
                                    {
                                        info.Name = Convert.ToString(item.Properties["displayname"].Value);
                                        info.UserAccount = item.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(item.Properties["userPrincipalName"].Value);
                                        int userAccountControl = Convert.ToInt32(item.Properties["userAccountControl"][0]);
                                        info.Status = UserProvider.IsAccountActive(userAccountControl);
                                    }
                                    info.MemberCount = count;
                                    items.Add(info);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (nodeID == Guid.Empty)
                        {
                            string path = ConfigADProvider.GetCompanyADRootPath();
                            entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                            if (entry != null)
                            {
                                NodeInfo item = new NodeInfo();
                                item.NodeID = entry.Guid;
                                item.Name = Convert.ToString(entry.Properties["name"].Value);
                                item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                item.Desc = entry.Properties["description"].Value == null ? "" : Convert.ToString(entry.Properties["description"].Value);
                                item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(item);
                            }
                        }
                        else
                        {
                            if (!GetADEntryByGuid(nodeID, out entry, out strError))
                            {
                                bResult = false;
                                break;
                            }

                            if (Convert.ToString(entry.Properties["distinguishedName"].Value) != ConfigADProvider.GetCompanyOuDistinguishedName())
                            {
                                DirectoryEntry de = new DirectoryEntry(entry.Path);
                                DirectorySearcher deSearch = new DirectorySearcher(de);
                                deSearch.SearchRoot = de;
                                deSearch.Filter = GetSearchType(searchType, string.Empty);
                                deSearch.SearchScope = SearchScope.OneLevel;
                                SearchResultCollection results = deSearch.FindAll();

                                if (results != null && results.Count > 0)
                                {
                                    foreach (SearchResult Result in results)
                                    {
                                        DirectoryEntry item = Result.GetDirectoryEntry();
                                        if (item.Path == entry.Path)
                                        {
                                            continue;
                                        }
                                        NodeInfo info = new NodeInfo();
                                        info.NodeID = item.Guid;
                                        info.Name = Convert.ToString(item.Properties["name"].Value);
                                        info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                                        info.Desc = item.Properties["description"].Value == null ? "" : Convert.ToString(item.Properties["description"].Value);
                                        info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                                        info.isParent = info.Type == NodeType.organizationalUnit ? true : false;

                                        if (info.Type == NodeType.user)
                                        {
                                            info.Name = Convert.ToString(item.Properties["displayname"].Value);
                                            info.UserAccount = item.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(item.Properties["userPrincipalName"].Value);
                                            int userAccountControl = Convert.ToInt32(item.Properties["userAccountControl"][0]);
                                            info.Status = UserProvider.IsAccountActive(userAccountControl);
                                        }

                                        info.MemberCount = count;
                                        items.Add(info);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < admin.ControlLimitOuList.Count; i++)
                                {
                                    string path = ConfigADProvider.GetADPathByLdap(admin.ControlLimitOuList[i].OUdistinguishedName);
                                    entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                                    if (entry != null)
                                    {
                                        NodeInfo item = new NodeInfo();
                                        item.NodeID = entry.Guid;
                                        //item.MemberCount = 5;
                                        item.Name = Convert.ToString(entry.Properties["name"].Value);
                                        item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                        item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                        item.Desc = entry.Properties["description"].Value == null ? "" : Convert.ToString(entry.Properties["description"].Value);
                                        item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                        items.Add(item);
                                    }
                                }
                            }
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetTreeData 路径:{0}查找所有对象 Exception:{1}", nodeID, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetOrgTreeData(Guid nodeID, AdminInfo admin, out List<NodeInfo> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<NodeInfo>();
            DirectoryEntry entry = new DirectoryEntry();

            try
            {
                do
                {
                    var numMethod = admin.ControlLimitOuList.Where(n => n.OUdistinguishedName.Equals(ConfigADProvider.GetCompanyOuDistinguishedName()));
                    if (numMethod.Any())
                    {
                        if (nodeID == Guid.Empty)
                        {
                            for (int i = 0; i < admin.ControlLimitOuList.Count; i++)
                            {
                                string path = ConfigADProvider.GetADPathByLdap(admin.ControlLimitOuList[i].OUdistinguishedName);
                                entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                                if (entry != null)
                                {
                                    NodeInfo item = new NodeInfo();
                                    item.NodeID = entry.Guid;
                                    //item.MemberCount = 5;
                                    item.Name = Convert.ToString(entry.Properties["name"].Value);
                                    item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                    item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                    item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                    items.Add(item);
                                }
                            }
                        }
                        else
                        {
                            if (!GetADEntryByGuid(nodeID, out entry, out strError))
                            {
                                bResult = false;
                                break;
                            }

                            DirectoryEntry de = new DirectoryEntry(entry.Path);
                            DirectorySearcher deSearch = new DirectorySearcher(de);
                            deSearch.SearchRoot = de;
                            deSearch.Filter = "(&(|(objectClass=organizationalUnit))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))";
                            deSearch.SearchScope = SearchScope.OneLevel;
                            SearchResultCollection results = deSearch.FindAll();

                            if (results != null && results.Count > 0)
                            {
                                foreach (SearchResult Result in results)
                                {
                                    DirectoryEntry item = Result.GetDirectoryEntry();
                                    if (item.Path == entry.Path)
                                    {
                                        continue;
                                    }
                                    NodeInfo info = new NodeInfo();
                                    info.NodeID = item.Guid;
                                    info.Name = Convert.ToString(item.Properties["name"].Value);
                                    info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                                    info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                                    info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                                    items.Add(info);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (nodeID == Guid.Empty)
                        {
                            string path = ConfigADProvider.GetCompanyADRootPath();
                            entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                            if (entry != null)
                            {
                                NodeInfo item = new NodeInfo();
                                item.NodeID = entry.Guid;
                                //item.MemberCount = 5;
                                item.Name = Convert.ToString(entry.Properties["name"].Value);
                                item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(item);
                            }
                        }
                        else
                        {
                            if (!GetADEntryByGuid(nodeID, out entry, out strError))
                            {
                                bResult = false;
                                break;
                            }

                            if (Convert.ToString(entry.Properties["distinguishedName"].Value) != ConfigADProvider.GetCompanyOuDistinguishedName())
                            {
                                DirectoryEntry de = new DirectoryEntry(entry.Path);
                                DirectorySearcher deSearch = new DirectorySearcher(de);
                                deSearch.SearchRoot = de;
                                deSearch.Filter = "(&(|(objectClass=organizationalUnit))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))";
                                deSearch.SearchScope = SearchScope.OneLevel;
                                SearchResultCollection results = deSearch.FindAll();

                                if (results != null && results.Count > 0)
                                {
                                    foreach (SearchResult Result in results)
                                    {
                                        DirectoryEntry item = Result.GetDirectoryEntry();
                                        if (item.Path == entry.Path)
                                        {
                                            continue;
                                        }
                                        NodeInfo info = new NodeInfo();
                                        info.NodeID = item.Guid;
                                        info.Name = Convert.ToString(item.Properties["name"].Value);
                                        info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                                        info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                                        info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                                        items.Add(info);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < admin.ControlLimitOuList.Count; i++)
                                {
                                    string path = ConfigADProvider.GetADPathByLdap(admin.ControlLimitOuList[i].OUdistinguishedName);
                                    entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                                    if (entry != null)
                                    {
                                        NodeInfo item = new NodeInfo();
                                        item.NodeID = entry.Guid;
                                        //item.MemberCount = 5;
                                        item.Name = Convert.ToString(entry.Properties["name"].Value);
                                        item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                        item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                        item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                        items.Add(item);
                                    }
                                }
                            }
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetOrgTreeData 路径:{0}查找所有对象 Exception:{1}", nodeID, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetDefaultOrgTreeData(Guid nodeID, AdminInfo admin, out List<NodeInfo> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<NodeInfo>();
            DirectoryEntry entry = new DirectoryEntry();

            try
            {
                do
                {
                    if (nodeID == Guid.Empty)
                    {
                        string path = ConfigADProvider.GetCompanyADRootPath();
                        entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                        if (entry != null)
                        {
                            NodeInfo item = new NodeInfo();
                            item.NodeID = entry.Guid;
                            //item.MemberCount = 5;
                            item.Name = Convert.ToString(entry.Properties["name"].Value);
                            item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                            item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                            item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                            items.Add(item);
                        }
                    }
                    else
                    {
                        if (!GetADEntryByGuid(nodeID, out entry, out strError))
                        {
                            bResult = false;
                            break;
                        }

                        DirectoryEntry de = new DirectoryEntry(entry.Path);
                        DirectorySearcher deSearch = new DirectorySearcher(de);
                        deSearch.SearchRoot = de;
                        deSearch.Filter = "(&(|(objectClass=organizationalUnit))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))";
                        deSearch.SearchScope = SearchScope.OneLevel;
                        SearchResultCollection results = deSearch.FindAll();

                        if (results != null && results.Count > 0)
                        {
                            foreach (SearchResult Result in results)
                            {
                                DirectoryEntry item = Result.GetDirectoryEntry();
                                if (item.Path == entry.Path)
                                {
                                    continue;
                                }
                                NodeInfo info = new NodeInfo();
                                info.NodeID = item.Guid;
                                info.Name = Convert.ToString(item.Properties["name"].Value);
                                info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                                info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                                info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(info);
                            }
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetOrgTreeData 路径:{0}查找所有对象 Exception:{1}", nodeID, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetGroupTreeData(Guid nodeID, AdminInfo admin, out List<NodeInfo> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<NodeInfo>();
            DirectoryEntry entry = new DirectoryEntry();

            try
            {
                do
                {
                    if (nodeID == Guid.Empty)
                    {
                        for (int i = 0; i < admin.ControlLimitOuList.Count; i++)
                        {
                            string path = ConfigADProvider.GetADPathByLdap(admin.ControlLimitOuList[i].OUdistinguishedName);
                            entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                            if (entry != null)
                            {
                                NodeInfo item = new NodeInfo();
                                item.NodeID = entry.Guid;
                                //item.MemberCount = 5;
                                item.Name = Convert.ToString(entry.Properties["name"].Value);
                                item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(item);
                            }
                        }
                    }
                    else
                    {
                        if (!GetADEntryByGuid(nodeID, out entry, out strError))
                        {
                            bResult = false;
                            break;
                        }

                        DirectoryEntry de = new DirectoryEntry(entry.Path);
                        DirectorySearcher deSearch = new DirectorySearcher(de);
                        deSearch.SearchRoot = de;
                        deSearch.Filter = "(&(|(objectClass=organizationalUnit)(objectClass=group))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))";
                        deSearch.SearchScope = SearchScope.OneLevel;
                        SearchResultCollection results = deSearch.FindAll();

                        if (results != null && results.Count > 0)
                        {
                            foreach (SearchResult Result in results)
                            {
                                DirectoryEntry item = Result.GetDirectoryEntry();
                                if (item.Path == entry.Path)
                                {
                                    continue;
                                }
                                NodeInfo info = new NodeInfo();
                                info.NodeID = item.Guid;
                                info.Name = Convert.ToString(item.Properties["name"].Value);
                                info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                                info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                                info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(info);
                            }
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetOrgTreeData 路径:{0}查找所有对象 Exception:{1}", nodeID, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetSamelevelOuGroupTreeData(Guid nodeID, AdminInfo admin, out List<NodeInfo> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<NodeInfo>();
            DirectoryEntry entry = new DirectoryEntry();

            try
            {
                do
                {
                    if (nodeID == Guid.Empty)
                    {
                        for (int i = 0; i < admin.SameLevelOuList.Count; i++)
                        {
                            string path = admin.SameLevelOuList[i].SamelevelOuPath;
                            entry = new DirectoryEntry(ConfigADProvider.GetADPathByLdap(path), ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                            if (entry != null)
                            {
                                NodeInfo item = new NodeInfo();
                                item.NodeID = entry.Guid;
                                //item.MemberCount = 5;
                                item.Name = Convert.ToString(entry.Properties["name"].Value);
                                item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(item);
                            }
                        }
                    }
                    else
                    {
                        if (!GetADEntryByGuid(nodeID, out entry, out strError))
                        {
                            bResult = false;
                            break;
                        }

                        DirectoryEntry de = new DirectoryEntry(entry.Path);
                        DirectorySearcher deSearch = new DirectorySearcher(de);
                        deSearch.SearchRoot = de;
                        deSearch.Filter = "(&(|(objectClass=organizationalUnit)(objectClass=group))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))";
                        deSearch.SearchScope = SearchScope.OneLevel;
                        SearchResultCollection results = deSearch.FindAll();

                        if (results != null && results.Count > 0)
                        {
                            foreach (SearchResult Result in results)
                            {
                                DirectoryEntry item = Result.GetDirectoryEntry();
                                if (item.Path == entry.Path)
                                {
                                    continue;
                                }
                                NodeInfo info = new NodeInfo();
                                info.NodeID = item.Guid;
                                info.Name = Convert.ToString(item.Properties["name"].Value);
                                info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                                info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                                info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(info);
                            }
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetOrgTreeData 路径:{0}查找所有对象 Exception:{1}", nodeID, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetSamelevelOuTreeOnlyOu(Guid nodeID, AdminInfo admin, out List<NodeInfo> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<NodeInfo>();
            DirectoryEntry entry = new DirectoryEntry();

            try
            {
                do
                {
                    if (nodeID == Guid.Empty)
                    {
                        for (int i = 0; i < admin.SameLevelOuList.Count; i++)
                        {
                            string path = admin.SameLevelOuList[i].SamelevelOuPath;
                            entry = new DirectoryEntry(ConfigADProvider.GetADPathByLdap(path), ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                            if (entry != null)
                            {
                                NodeInfo item = new NodeInfo();
                                item.NodeID = entry.Guid;
                                //item.MemberCount = 5;
                                item.Name = Convert.ToString(entry.Properties["name"].Value);
                                item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(item);
                            }
                        }
                    }
                    else
                    {
                        if (!GetADEntryByGuid(nodeID, out entry, out strError))
                        {
                            bResult = false;
                            break;
                        }

                        DirectoryEntry de = new DirectoryEntry(entry.Path);
                        DirectorySearcher deSearch = new DirectorySearcher(de);
                        deSearch.SearchRoot = de;
                        deSearch.Filter = "(&(|(objectClass=organizationalUnit))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))";
                        deSearch.SearchScope = SearchScope.OneLevel;
                        SearchResultCollection results = deSearch.FindAll();

                        if (results != null && results.Count > 0)
                        {
                            foreach (SearchResult Result in results)
                            {
                                DirectoryEntry item = Result.GetDirectoryEntry();
                                if (item.Path == entry.Path)
                                {
                                    continue;
                                }
                                NodeInfo info = new NodeInfo();
                                info.NodeID = item.Guid;
                                info.Name = Convert.ToString(item.Properties["name"].Value);
                                info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                                info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                                info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(info);
                            }
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetSamelevelOuTreeOnlyOu 路径:{0}查找所有对象 Exception:{1}", nodeID, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetMailBoxDataOrgTree(Guid nodeID, AdminInfo admin, out List<NodeInfo> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<NodeInfo>();
            DirectoryEntry entry = new DirectoryEntry();
            Guid transactionid = new Guid();
            ErrorCodeInfo error = new ErrorCodeInfo();
            try
            {
                do
                {
                    if (nodeID == Guid.Empty)
                    {
                        List<string> ouPaths = new List<string>();
                        MailDataBaseDBProvider dBProvider = new MailDataBaseDBProvider();
                        if (!dBProvider.GetMailDataBaseTreeList(transactionid, admin, out ouPaths, out error))
                        {
                            bResult = false;
                            break;
                        }

                        for (int i = 0; i < ouPaths.Count; i++)
                        {
                            string path = ConfigADProvider.GetADPathByLdap(ouPaths[i]);
                            entry = new DirectoryEntry(path, ConfigADProvider.GetADAdmin(), ConfigADProvider.GetADPassword());
                            if (entry != null)
                            {
                                NodeInfo item = new NodeInfo();
                                item.NodeID = entry.Guid;
                                //item.MemberCount = 5;
                                item.Name = Convert.ToString(entry.Properties["name"].Value);
                                item.Path = Convert.ToString(entry.Properties["distinguishedName"].Value);
                                item.Type = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                                item.isParent = item.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(item);
                            }
                        }
                       
                    }
                    else
                    {
                        if (!GetADEntryByGuid(nodeID, out entry, out strError))
                        {
                            bResult = false;
                            break;
                        }

                        DirectoryEntry de = new DirectoryEntry(entry.Path);
                        DirectorySearcher deSearch = new DirectorySearcher(de);
                        deSearch.SearchRoot = de;
                        deSearch.Filter = "(&(|(objectClass=organizationalUnit))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))";
                        deSearch.SearchScope = SearchScope.OneLevel;
                        SearchResultCollection results = deSearch.FindAll();

                        if (results != null && results.Count > 0)
                        {
                            foreach (SearchResult Result in results)
                            {
                                DirectoryEntry item = Result.GetDirectoryEntry();
                                if (item.Path == entry.Path)
                                {
                                    continue;
                                }
                                NodeInfo info = new NodeInfo();
                                info.NodeID = item.Guid;
                                info.Name = Convert.ToString(item.Properties["name"].Value);
                                info.Path = Convert.ToString(item.Properties["distinguishedName"].Value);
                                info.Type = (NodeType)Enum.Parse(typeof(NodeType), item.SchemaClassName);
                                info.isParent = info.Type == NodeType.organizationalUnit ? true : false;
                                items.Add(info);
                            }
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetMailBoxDataOrgTree 路径:{0}查找所有对象 Exception:{1}", nodeID, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetMemberCount(DirectoryEntry entry, out int count, out string strError)
        {
            count = 0;
            strError = string.Empty;
            bool bResult = false;
            try
            {

                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(objectClass=user)";
                search.SearchScope = SearchScope.Subtree;
                SearchResultCollection Results = search.FindAll();
                count = Results.Count;

                bResult = true;
            }
            catch (Exception ex)
            {
                //日志
                strError = string.Format("Common_ADProvider.GetADEntryByPath 通过路径:{0}实例对象 Exception:{1}", entry.Name, ex.ToString());
                bResult = false;
            }
            return bResult;

        }

        public bool GetHabGroupEntryByName(string strGroupName, out DirectoryEntry AD_Group, out string strError)
        {
            bool bResult = false;
            strError = string.Empty;
            AD_Group = null;
            string path = string.Empty;
            try
            {
                do
                {
                    path = ConfigADProvider.GetCompanyADRootPath();
                    DirectoryEntry de = null;
                    de = new DirectoryEntry(path);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.Filter = "(&(objectClass=group)(cn=" + strGroupName + ")(msOrg-IsOrganizational=TRUE))";
                    deSearch.SearchScope = SearchScope.Subtree;
                    SearchResult resultDE = deSearch.FindOne();
                    if (resultDE == null)
                    {
                        strError = string.Format("CommonProvider.GetGroupEntryByName 路径:{0}中不存在name:{1}的Group", path, strGroupName);
                        bResult = false;
                        break;
                    }
                    AD_Group = resultDE.GetDirectoryEntry();
                    bResult = true;
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetGroupEntryByName 路径:{0}查找Group:{1} Exception:{2}", path, strGroupName, ex.ToString());
                bResult = false;
            }
            return bResult;
        }

        public bool GetStaticGroupData(int curpage, int pagesize, string searchstr, out BaseListInfo items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new BaseListInfo();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    List<DirectoryEntry> entrys = new List<DirectoryEntry>();
                    DirectoryEntry entry = new DirectoryEntry();
                    // items.Skip((curpage - 1) * pagesize).Take(pagesize)
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        if (!SearchEntryData(ConfigADProvider.GetStaticGroupOuLdap(), SearchType.GroupAndUserAndNoHab, out entrys, out strError))
                        {
                            bResult = false;
                            break;
                        }
                        items.RecordCount = entrys.Count;
                        items.PageCount = (items.RecordCount + pagesize - 1) / pagesize;
                        List<GroupInfo> groups = new List<GroupInfo>();
                        foreach (DirectoryEntry GroupEntry in entrys)
                        {
                            GroupInfo info = new GroupInfo();
                            info.GroupID = GroupEntry.Guid;
                            info.DisplayName = GroupEntry.Properties["name"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["name"].Value);
                            info.Description = GroupEntry.Properties["description"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["description"].Value);
                            info.Account = GroupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["mail"].Value);
                            
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

                            Hashtable ht = new Hashtable();
                            if (GroupEntry.Properties["managedBy"].Value != null)
                            {
                                string adminDnName = Convert.ToString(GroupEntry.Properties["managedBy"].Value);
                                if (!commonProvider.GetADEntryByPath(adminDnName, out entry, out strError))
                                {
                                    continue;
                                }

                                UserInfo user = new UserInfo();
                                user.UserID = entry.Guid;
                                user.DisplayName = entry.Properties["name"].Value == null ? "" : Convert.ToString(entry.Properties["name"].Value);
                                user.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                info.AdminsName += user.DisplayName + "(" + user.UserAccount + ")，";
                                if (!ht.ContainsKey(user.UserID))
                                {
                                    info.Admins.Add(user);
                                    ht.Add(user.UserID, user);
                                }
                            }
                            for (int i = 0; i < GroupEntry.Properties["msExchCoManagedByLink"].Count; i++)
                            {
                                string MemberDnName = Convert.ToString(GroupEntry.Properties["msExchCoManagedByLink"][i]);
                                if (!commonProvider.GetADEntryByPath(MemberDnName, out entry, out strError))
                                {
                                    continue;
                                }

                                UserInfo user = new UserInfo();
                                user.UserID = entry.Guid;
                                user.DisplayName = entry.Properties["name"].Value == null ? "" : Convert.ToString(entry.Properties["name"].Value);
                                user.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                info.AdminsName += user.DisplayName + "(" + user.UserAccount + ")，";
                                if (!ht.ContainsKey(user.UserID))
                                {
                                    info.Admins.Add(user);
                                    ht.Add(user.UserID, user);
                                }
                            }

                            info.AdminsName = string.IsNullOrEmpty(info.AdminsName) ? string.Empty : info.AdminsName.Remove(info.AdminsName.LastIndexOf('，'), 1);
                            info.AdminsCount = info.Admins.Count;
                            info.Type = (GroupType)TempGroupType;
                            groups.Add(info);
                        }

                        items.Lists = groups.OrderBy(x => x.DisplayName).Skip((curpage - 1) * pagesize).Take(pagesize).ToList<object>();
                    }
                    else
                    {
                        if (!SearchEntryData(ConfigADProvider.GetStaticGroupOuLdap(), searchstr, SearchType.GroupAndUserAndNoHab, out entrys, out strError))
                        {
                            bResult = false;
                            break;
                        }
                        if (entrys.Count > 0)
                        {
                            items.RecordCount = entrys.Count;
                            items.PageCount = (items.RecordCount + pagesize - 1) / pagesize;
                            List<GroupInfo> groups = new List<GroupInfo>();
                            foreach (DirectoryEntry GroupEntry in entrys)
                            {
                                GroupInfo info = new GroupInfo();
                                info.DisplayName = GroupEntry.Properties["name"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["name"].Value);
                                info.Description = GroupEntry.Properties["description"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["description"].Value);
                                info.Account = GroupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(GroupEntry.Properties["mail"].Value);

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

                                Hashtable ht = new Hashtable();
                                if (GroupEntry.Properties["managedBy"].Value != null)
                                {
                                    string adminDnName = Convert.ToString(GroupEntry.Properties["managedBy"].Value);
                                    if (!commonProvider.GetADEntryByPath(adminDnName, out entry, out strError))
                                    {
                                        continue;
                                    }

                                    UserInfo user = new UserInfo();
                                    user.UserID = entry.Guid;
                                    user.DisplayName = entry.Properties["name"].Value == null ? "" : Convert.ToString(entry.Properties["name"].Value);
                                    user.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                    info.AdminsName += user.DisplayName + "(" + user.UserAccount + ")，";
                                    if (!ht.ContainsKey(user.UserID))
                                    {
                                        info.Admins.Add(user);
                                        ht.Add(user.UserID, user);
                                    }
                                }
                                for (int i = 0; i < GroupEntry.Properties["msExchCoManagedByLink"].Count; i++)
                                {
                                    string MemberDnName = Convert.ToString(GroupEntry.Properties["msExchCoManagedByLink"][i]);
                                    if (!commonProvider.GetADEntryByPath(MemberDnName, out entry, out strError))
                                    {
                                        continue;
                                    }

                                    UserInfo user = new UserInfo();
                                    user.UserID = entry.Guid;
                                    user.DisplayName = entry.Properties["name"].Value == null ? "" : Convert.ToString(entry.Properties["name"].Value);
                                    user.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "-" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                                    info.AdminsName += user.DisplayName + "(" + user.UserAccount + ")，";
                                    if (!ht.ContainsKey(user.UserID))
                                    {
                                        info.Admins.Add(user);
                                        ht.Add(user.UserID, user);
                                    }
                                }
                                info.AdminsCount = info.Admins.Count;
                                info.AdminsName = string.IsNullOrEmpty(info.AdminsName) ? string.Empty : info.AdminsName.Remove(info.AdminsName.LastIndexOf('，'), 1);
                                info.Type = (GroupType)TempGroupType;
                                groups.Add(info);
                            }

                            items.Lists = groups.OrderBy(x => x.DisplayName).Skip((curpage - 1) * pagesize).Take(pagesize).ToList<object>();
                        }
                    }
                } while (false);

            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetStaticGroupData Exception:{0}", ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetRecycleOu(string ouPath, out OuInfo ou, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            ou = new OuInfo();
            try
            {
                do
                {
                    string recycleOu = string.Empty;
                    string RecycleOuLdap = ConfigADProvider.GetRecycleOuDistinguishedName();
                    string[] TempArray = ouPath.Split(new string[] { "OU=" }, StringSplitOptions.RemoveEmptyEntries);
                    DirectoryEntry ouEntry = new DirectoryEntry();

                    //截取离职Ou路径
                    if (TempArray.Length < 2)
                    {
                        recycleOu = RecycleOuLdap;
                        if (!GetADEntryByPath(recycleOu, out ouEntry, out strError))
                        {
                            bResult = false;
                            break;
                        }
                        ou.id = ouEntry.Guid;
                        ou.distinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);
                        ou.name = Convert.ToString(ouEntry.Properties["name"].Value);
                        break;
                    }
                    else
                    {
                        recycleOu = ouPath.Substring(0, ouPath.LastIndexOf("OU")) + RecycleOuLdap;
                        if (!GetADEntryByPath(recycleOu, out ouEntry, out strError))
                        {
                            recycleOu = recycleOu.Substring(recycleOu.IndexOf(",") + 1);
                            GetRecycleOu(recycleOu, out ou, out strError);
                        }
                        else
                        {
                            ou.id = ouEntry.Guid;
                            ou.distinguishedName = Convert.ToString(ouEntry.Properties["distinguishedName"].Value);
                            ou.name = Convert.ToString(ouEntry.Properties["name"].Value);
                            break;
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                //日志
                strError = string.Format("CommonProvider.GetRecycleOu 通过路径:{0}实例对象 Exception:{1}", ouPath, ex.ToString());
                bResult = false;
            }
            return bResult;
        }
        
        public bool SearchEntryData(string strPath, string strSearchCondition, SearchType iType, out List<DirectoryEntry> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<DirectoryEntry>();

            try
            {
                do
                {
                    if (string.IsNullOrEmpty(strPath))
                    {
                        strError = "Common_ADProvider.SearchEntryData Path路径为空";
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = null;
                    de = new DirectoryEntry(strPath);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.SearchRoot = de;
                    string strFilter = GetSearchType(iType, strSearchCondition);
                    deSearch.Filter = strFilter;
                    deSearch.SearchScope = SearchScope.Subtree;
                    SearchResultCollection results = deSearch.FindAll();

                    if (results != null && results.Count > 0)
                    {
                        foreach (SearchResult Result in results)
                        {
                            items.Add(Result.GetDirectoryEntry());
                        }
                    }

                } while (false);

            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.SearchEntryData 路径:{0}条件:{1}搜索所有对象 Exception:{2}", strPath, strSearchCondition, ex.ToString());
                // LoggerHelper.Error()
                bResult = false;
            }

            return bResult;
        }

        public bool SearchEntryData(string strPath, SearchType iType, out List<DirectoryEntry> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<DirectoryEntry>();

            try
            {
                do
                {
                    if (string.IsNullOrEmpty(strPath))
                    {
                        strError = "CommonProvider.SearchEntryData Path路径为空";
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = null;
                    de = new DirectoryEntry(strPath);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.SearchRoot = de;
                    string strFilter = GetSearchType(iType, string.Empty);
                    deSearch.Filter = strFilter;
                    deSearch.SearchScope = SearchScope.Subtree;
                    SearchResultCollection results = deSearch.FindAll();

                    if (results != null && results.Count > 0)
                    {
                        foreach (SearchResult Result in results)
                        {
                            items.Add(Result.GetDirectoryEntry());
                        }
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("Common_ADProvider.SearchEntryData 路径:{0}搜索所有对象 Exception:{1}", strPath, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetOneLevelSigleOuEntry(string path, string name, out DirectoryEntry entry, out string strError)
        {
            bool bResult = false;
            strError = string.Empty;
            entry = null;
            try
            {
                do
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        strError = "CommonProvider.GetOneLeverSigleOuEntry Path路径为空";
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = null;
                    de = new DirectoryEntry(path);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.Filter = "(&(objectClass=organizationalUnit)(ou=" + name + "))";
                    deSearch.SearchScope = SearchScope.OneLevel;
                    SearchResult resultDE = deSearch.FindOne();
                    if (resultDE == null)
                    {
                        strError = string.Format("CommonProvider.GetOneLeverSigleOuEntry 路径:{0}中不存在该 OU :{1}", path, name);
                        bResult = false;
                        break;

                    }
                    entry = resultDE.GetDirectoryEntry();
                    bResult = true;
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetOneLeverSigleOuEntry 路径:{0}查找 OU :{1} Exception:{2}", path, name, ex.ToString());
                bResult = false;
            }
            return bResult;
        }

        public bool GetOneLevelSigleGroupEntryByName(string path, string name, out DirectoryEntry entry, out string strError)
        {
            bool bResult = false;
            strError = string.Empty;
            entry = null;
            try
            {
                do
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        strError = "CommonProvider.GetOneLevelSigleGroupEntryByName Path路径为空";
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = null;
                    de = new DirectoryEntry(path);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.Filter = "(&(objectClass=group)(cn=" + name + "))";
                    deSearch.SearchScope = SearchScope.OneLevel;
                    SearchResult resultDE = deSearch.FindOne();
                    if (resultDE == null)
                    {
                        strError = string.Format("CommonProvider.GetOneLevelSigleGroupEntryByName 路径:{0}中不存在name:{1}的Group", path, name);
                        bResult = false;
                        break;
                    }
                    entry = resultDE.GetDirectoryEntry();
                    bResult = true;
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetOneLevelSigleGroupEntryByName 路径:{0}查找Group:{1} Exception:{2}", path, name, ex.ToString());
                bResult = false;
            }
            return bResult;
        }

        public bool GetOneLevelSigleEntryByCN(string path, string strEntryCN, out DirectoryEntry item, out string strError)
        {
            bool bResult = false;
            strError = string.Empty;
            item = null;
            try
            {
                do
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        strError = "CommonProvider.GetOneLevelSigleEntryByCN Path路径为空";
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = null;
                    de = new DirectoryEntry(path);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.Filter = "(&(cn=" + strEntryCN + "))";
                    deSearch.SearchScope = SearchScope.OneLevel;
                    SearchResult resultDE = deSearch.FindOne();
                    if (resultDE == null)
                    {
                        strError = string.Format("CommonProvider.GetOneLevelSigleEntryByCN 路径:{0}中不存在name:{1}的对象", path, strEntryCN);
                        bResult = false;
                        break;

                    }
                    item = resultDE.GetDirectoryEntry();
                    bResult = true;
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetOneLevelSigleEntryByCN 路径:{0}查找对象:{1} Exception:{2}", path, strEntryCN, ex.ToString());
                bResult = false;
            }
            return bResult;
        }

        public bool GetEntryDataBysAMAccount(string path, string strsAMAccountName, out DirectoryEntry items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = null;
            try
            {
                do
                {
                    if (string.IsNullOrEmpty(path)
                        || string.IsNullOrEmpty(strsAMAccountName))
                    {
                        strError = "CommonProvider.GetEntryDataBysAMAccount 参数不能为空";
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = null;
                    de = new DirectoryEntry(path);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.SearchRoot = de;
                    string strFilter = string.Empty;


                    strFilter = string.Format("(&(sAMAccountName={0}))", strsAMAccountName);

                    deSearch.Filter = strFilter;

                    deSearch.SearchScope = SearchScope.Subtree;
                    SearchResult result = deSearch.FindOne();

                    if (result == null)
                    {
                        bResult = false;
                    }
                    items = result.GetDirectoryEntry();

                } while (false);

            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetEntryDataBysAMAccount 路径:{0} sAMAccountName:{1}查找对象 Exception:{2}", path, strsAMAccountName, ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool GetOneLevelSigleHabGroupEntry(string path, out DirectoryEntry entry, out string strError)
        {
            bool bResult = false;
            strError = string.Empty;
            entry = null;
            try
            {
                do
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        strError = "CommonProvider.GetOneLevelSigleHabGroupEntry Path路径为空";
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = null;
                    de = new DirectoryEntry(path);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.Filter = "(&(!objectClass=computer)(objectClass=group)(msOrg-IsOrganizational=TRUE))";
                    deSearch.SearchScope = SearchScope.OneLevel;
                    SearchResult resultDE = deSearch.FindOne();
                    if (resultDE == null)
                    {
                        strError = string.Format("CommonProvider.GetOneLevelSigleHabGroupEntry 路径:{0}中不存在该对象", path);
                        bResult = false;
                        break;

                    }
                    entry = resultDE.GetDirectoryEntry();
                    bResult = true;
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetOneLevelSigleHabGroupEntry 路径:{0}查找HabGroup Exception:{1}", path, ex.ToString());
                bResult = false;
            }
            return bResult;
        }

        public bool GetOUOneLevelChildMembers(Guid ouId, out OuInfo ouInfo,
            out List<OuInfo> childrenOuList, out List<UserInfo> childrenUserList, 
            out List<GroupInfo> childrenGroupList, out List<GroupInfo> childrenHabGroupList, 
            out string strError)
        {
            bool bResult = false;
            strError = string.Empty;
            ouInfo = new OuInfo();
            childrenOuList = new List<OuInfo>();
            childrenUserList = new List<UserInfo>();
            childrenGroupList = new List<GroupInfo>();
            childrenHabGroupList = new List<GroupInfo>();
            try
            {
                do
                {
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    if (!GetADEntryByGuid(ouId, out ouEntry, out strError))
                    {
                        strError = string.Format("CommonProvider.GetADEntryByGuid 路径:{0}中不存在该对象", ouId);
                        bResult = false;
                        break;
                    }

                    ouInfo.id = ouEntry.Guid;
                    ouInfo.name = ouEntry.Properties["name"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["name"].Value);
                    ouInfo.description = ouEntry.Properties["description"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["description"].Value);
                    ouInfo.distinguishedName = ouEntry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(ouEntry.Properties["distinguishedName"].Value);
                    ouInfo.parentid = ouEntry.Parent.Guid;

                    foreach (DirectoryEntry entry in ouEntry.Children)
                    {
                        if (entry.SchemaClassName.Equals("organizationalUnit"))
                        {
                            OuInfo childOu = new OuInfo();
                            childOu.id = entry.Guid;
                            childOu.name = entry.Properties["name"].Value == null ? string.Empty : Convert.ToString(entry.Properties["name"].Value);
                            childOu.description = entry.Properties["description"].Value == null ? string.Empty : Convert.ToString(entry.Properties["description"].Value);
                            childOu.distinguishedName = entry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(entry.Properties["distinguishedName"].Value);
                            childOu.parentid = entry.Parent.Guid;
                            childrenOuList.Add(childOu);
                        }
                        else if (entry.SchemaClassName.Equals("user"))
                        {
                            UserInfo childUser = new UserInfo();
                            childUser.UserID = entry.Guid;
                            childUser.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                            childUser.LastName = entry.Properties["sn"].Value == null ? "" : Convert.ToString(entry.Properties["sn"].Value);
                            childUser.FirstName = entry.Properties["givenName"].Value == null ? "" : Convert.ToString(entry.Properties["givenName"].Value);
                            childUser.DisplayName = entry.Properties["displayName"].Value == null ? "" : Convert.ToString(entry.Properties["displayName"].Value);
                            childUser.ParentOu = entry.Parent.Properties["name"].Value == null ? "" : Convert.ToString(entry.Parent.Properties["name"].Value);
                            childUser.DistinguishedName = entry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(entry.Properties["distinguishedName"].Value);
                            childrenUserList.Add(childUser);
                        }
                        else if (entry.SchemaClassName.Equals("group"))
                        {
                            GroupInfo childGroup = new GroupInfo();
                            childGroup.GroupID = entry.Guid;
                            childGroup.DisplayName = entry.Properties["name"].Value == null ? "" : Convert.ToString(entry.Properties["name"].Value);
                            childGroup.Description = entry.Properties["description"].Value == null ? "" : Convert.ToString(entry.Properties["description"].Value);
                            childGroup.Account = entry.Properties["mail"].Value == null ? "" : Convert.ToString(entry.Properties["mail"].Value);
                            childGroup.Index = entry.Properties["msDS-HABSeniorityIndex"].Value == null ? 1 : Convert.ToInt32(entry.Properties["msDS-HABSeniorityIndex"].Value);
                            childGroup.DistinguishedName = entry.Properties["distinguishedName"].Value == null ? string.Empty : Convert.ToString(entry.Properties["distinguishedName"].Value);

                            if (entry.Properties["msOrg-IsOrganizational"].Value != null)
                            {
                                if ((bool)entry.Properties["msOrg-IsOrganizational"].Value == true)
                                {
                                    
                                    childrenHabGroupList.Add(childGroup);
                                }
                                else
                                {
                                    childrenGroupList.Add(childGroup);
                                }
                            }
                            else
                            {
                                childrenGroupList.Add(childGroup);
                            }
                        }
                    }

                    bResult = true;
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("CommonProvider.GetOUOneLevelChildMembers 路径:{0}查找ChildMembers Exception:{1}", ouId, ex.ToString());
                bResult = false;
            }
            return bResult;
        }

        public string Guid2OctetString(Guid objectGuid)
        {
            byte[] byteGuid = objectGuid.ToByteArray();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteGuid)
            {
                sb.Append(@"\" + b.ToString("x2"));
            }
            return sb.ToString();
        }

        public string GetSearchType(SearchType iType, string strSearchCondition)
        {
            string strFilter = string.Empty;
            if (string.IsNullOrEmpty(strSearchCondition))
            {
                switch (iType)
                {
                    case SearchType.AllNoHab:
                        strFilter = "(&(!objectClass=computer)(|(objectClass=user)(objectClass=group)(objectClass=organizationalUnit)(objectClass=msExchDynamicDistributionList))(!msOrg-IsOrganizational=TRUE))";
                        break;
                    case SearchType.Ou:
                        strFilter = "(&(|(objectClass=organizationalUnit))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))";
                        break;
                    case SearchType.GroupAndNoHab:
                        strFilter = "(&(objectClass=group)(!msOrg-IsOrganizational=TRUE))";
                        break;
                    case SearchType.Group:
                        strFilter = "(&(|(objectClass=group))(!objectClass=computer))";
                        break;
                    case SearchType.User:
                        strFilter = "(&(!objectClass=computer)(objectClass=user)(msOrg-IsOrganizational=TRUE))";
                        break;
                    case SearchType.GroupAndUserAndNoHab:
                        strFilter = "(&(!objectClass=computer)(|(objectClass=user)(objectClass=group))(!msOrg-IsOrganizational=TRUE))";
                        break;
                    case SearchType.Hab:
                        strFilter = "(&(!objectClass=computer)(objectClass=group)(msOrg-IsOrganizational=TRUE))";
                        break;
                    case SearchType.GroupAndUser:
                        strFilter = "(&(!objectClass=computer)(|(objectClass=user)(objectClass=group)))";
                        break;
                    case SearchType.MailGroupNoHab:
                        strFilter = string.Format("(&(objectClass=group)(mail=*)(!msOrg-IsOrganizational=TRUE))");
                        break;
                    case SearchType.MailGroup:
                        strFilter = string.Format("(&(objectClass=group)(mail=*))");
                        break;
                    case SearchType.All:
                        strFilter = string.Format("(&(!objectClass=computer)(|(objectClass=user)(objectClass=group)(objectClass=organizationalUnit)(objectClass=msExchDynamicDistributionList)))");
                        break;
                    case SearchType.GroupTreeNoHab:
                        strFilter = string.Format("(&(|(objectClass=organizationalUnit)(objectClass=group))(!objectClass=computer)(!msOrg-IsOrganizational=TRUE))");
                        break;
                    case SearchType.GroupTree:
                        strFilter = string.Format("(&(|(objectClass=organizationalUnit)(objectClass=group))(!objectClass=computer))");
                        break;
                    case SearchType.MailUserTree:
                        strFilter = string.Format("(&(!objectClass=computer)(|(objectClass=user)(objectClass=organizationalUnit))(mail=*))");
                        break;
                    case SearchType.MailUser:
                        strFilter = string.Format("(&(objectClass=user)(mail=*))");
                        break;
                }
            }
            else
            {
                switch (iType)
                {
                    case SearchType.AllNoHab:
                        strFilter = string.Format("(&(!objectClass=computer)(|(objectClass=user)(objectClass=group)(objectClass=organizationalUnit)(objectClass=msExchDynamicDistributionList))(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*))(!msOrg-IsOrganizational=TRUE))", strSearchCondition);
                        break;
                    case SearchType.Ou:
                        strFilter = string.Format("(&((objectClass=organizationalUnit))(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*)))", strSearchCondition);
                        break;
                    case SearchType.GroupAndNoHab:
                        strFilter = string.Format("(&(objectClass=group)(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*))(!msOrg-IsOrganizational=TRUE))", strSearchCondition);
                        break;
                    case SearchType.User:
                        strFilter = string.Format("(&(objectClass=user)(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*)))", strSearchCondition);
                        break;
                    case SearchType.GroupAndUserAndNoHab:
                        strFilter = string.Format("(&(!objectClass=computer)(|(objectClass=user)(objectClass=group))(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*))(!msOrg-IsOrganizational=TRUE))", strSearchCondition);
                        break;
                    case SearchType.Hab:
                        strFilter = string.Format("(&(!objectClass=computer)(objectClass=group)(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*))(msOrg-IsOrganizational=TRUE))", strSearchCondition);
                        break;
                    case SearchType.GroupAndUser:
                        strFilter = string.Format("(&(!objectClass=computer)(|(objectClass=user)(objectClass=group))(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*)(mail=*{0}*)))", strSearchCondition);
                        break;
                   case SearchType.All:
                        strFilter = string.Format("(&(!objectClass=computer)(|(objectClass=user)(objectClass=group)(objectClass=organizationalUnit)(objectClass=msExchDynamicDistributionList))(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*)))", strSearchCondition);
                        break;
                    case SearchType.MailUser:
                        strFilter = string.Format("(&(objectClass=user)(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*))(mail=*))", strSearchCondition);
                        break;
                    case SearchType.MailGroup:
                        strFilter = string.Format("(&(objectClass=group)(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*))(mail=*))", strSearchCondition);
                        break;
                    case SearchType.MailUserByMailAddress:
                        strFilter = string.Format("(&(objectClass=user)(mail={0}))", strSearchCondition);
                        break;
                    case SearchType.MailGroupByMailAddress:
                        strFilter = string.Format("(&(objectClass=group)(mail={0}))", strSearchCondition);
                        break;
                    case SearchType.MailGroupNoHab:
                        strFilter = string.Format("(&(objectClass=group)(|(sAMAccountName=*{0}*)(displayName=*{0}*)(name=*{0}*))(mail=*)(!msOrg-IsOrganizational=TRUE))", strSearchCondition);
                        break;
                }
            }
            return strFilter;
        }
    }

    /// <summary>
    /// NoHabMember = 搜索所有(不含有habgroup)
    /// Ou = 搜索ou
    /// GroupAndNoHab = 搜索group(不含有habgroup)
    /// User = 索搜user
    /// GroupAndUserAndNoHab = 搜索group,user(不含有habgroup)
    /// Hab = 搜索HabGroup
    /// GroupAndUser = 搜索group,user(含有habgroup)
    /// All = 搜索所有(含有habgroup)
    /// MailUser = 搜索具有邮箱的user
    /// AllNoHab =搜索所有(不含有habgroup)
    /// MailGroup = 搜索具有邮箱的group(含有habgroup)
    /// MailGroupNoHab = 搜索具有邮箱的group(不含有habgroup)
    /// GroupTreeNoHab = 搜索ou,group(不含有habgroup)
    /// MailUserTree = 搜索ou,具有邮箱的user
    /// </summary>
    public enum SearchType
    {
        Ou = 1,
        GroupAndNoHab = 2,
        User = 3,
        GroupAndUserAndNoHab = 4,
        Hab = 5,
        GroupAndUser = 6,
        All = 7,
        MailUser = 8,
        AllNoHab = 9,
        MailGroup = 10,
        MailGroupNoHab =11,
        GroupTreeNoHab = 12,
        MailUserTree =13,
        MailUserByMailAddress = 14,
        Group =15,
        GroupTree =16,
        MailGroupByMailAddress =17,
    }
}
