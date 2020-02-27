using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class GroupInfo
    {
        private Guid _GroupID = Guid.Empty;
        public Guid GroupID
        {
            get { return _GroupID; }
            set { _GroupID = value; }
        }

        private string _DisplayName = string.Empty;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        private string _Account = string.Empty;
        public string Account
        {
            get { return _Account; }
            set { _Account = value; }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _DistinguishedName = string.Empty;
        /// <summary>
        /// AD distinguishedName属性值
        /// </summary>
        public string DistinguishedName
        {
            get { return _DistinguishedName; }
            set { _DistinguishedName = value; }
        }

        private string _ParentDistinguishedName = string.Empty;
        /// <summary>
        /// AD distinguishedName属性值
        /// </summary>
        public string ParentDistinguishedName
        {
            get { return _ParentDistinguishedName; }
            set { _ParentDistinguishedName = value; }
        }

        private bool _IsOrganizational = false;
        public bool IsOrganizational
        {
            get { return _IsOrganizational; }
            set { _IsOrganizational = value; }
        }

        public NodeType GroupMold
        {
            get
            {
                NodeType _GroupType = NodeType.group;
                if (_IsOrganizational)
                {
                    _GroupType = NodeType.habgroup;
                }
                return _GroupType;
            }
        }

        private int _Index = 1;
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        private GroupType _Type = GroupType.GROUP_TYPE_UNIVERSAL_GROUP;
        public GroupType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private string _Description = string.Empty;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private Guid _ParentOuId = Guid.Empty;
        public Guid ParentOuId
        {
            get { return _ParentOuId; }
            set { _ParentOuId = value; }
        }

        private List<GroupMember> _Members = new List<GroupMember>();
        public List<GroupMember> Members
        {
            get { return _Members; }
            set { _Members = value; }
        }

        private List<UserInfo> _Admins = new List<UserInfo>();
        public List<UserInfo> Admins
        {
            get { return _Admins; }
            set { _Admins = value; }
        }

        private string _AdminsName = string.Empty;
        public string AdminsName
        {
            get { return _AdminsName; }
            set { _AdminsName = value; }
        }

        private int _AdminsCount = 0;
        public int AdminsCount
        {
            get { return _AdminsCount; }
            set { _AdminsCount = value; }
        }

        public ErrorCodeInfo AddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_ParentOuId == null || _ParentOuId == Guid.Empty)
            {
                error.Code = ErrorCode.ParentIdEmpty;
            }
            else if (string.IsNullOrEmpty(_DisplayName))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            else if (_Type == GroupType.GROUP_TYPE_UNIVERSAL_GROUP || _Type == GroupType.GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL)
            {
                if (string.IsNullOrEmpty(_Account))
                {
                    error.Code = ErrorCode.AccountEmpty;
                }
            }
            return error;
        }

        public ErrorCodeInfo ModifyCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_GroupID == null || _GroupID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            else if (string.IsNullOrEmpty(_DisplayName))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            return error;
        }
    }

    [Serializable]
    public enum GroupType
    {
        /// <summary>
        ///  通用+通讯组 1
        /// </summary>
        GROUP_TYPE_UNIVERSAL_GROUP = 1,

        /// <summary>
        /// 全局+通讯组 2
        /// </summary>
        GROUP_TYPE_ACCOUNT_GROUP = 2,

        /// <summary>
        /// 通用+安全组 3
        /// </summary>
        GROUP_TYPE_SECURITY_ENABLED_UNIVERSAL = 3,

        /// <summary>
        ///  全局+安全组 4
        /// </summary>
        GROUP_TYPE_SECURITY_ENABLED_ACCOUNT = 4,
    }

    [Serializable]
    public class GroupMember
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _DisplayName = string.Empty;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        private string _Account = string.Empty;
        public string Account
        {
            get { return _Account; }
            set { _Account = value; }
        }

        private NodeType _Type = NodeType.user;
        public NodeType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private int _Index = 0;
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        private int _MemberCount = 0;
        public int MemberCount
        {
            get { return _MemberCount; }
            set { _MemberCount = value; }
        }

        private bool _IsOrganizational = false;
        public bool IsOrganizational
        {
            get { return _IsOrganizational; }
            set { _IsOrganizational = value; }
        }
    }
}
