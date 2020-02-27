using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class RoleInfo
    {
        private Guid _RoleID = Guid.Empty;
        public Guid RoleID
        {
            get { return _RoleID; }
            set { _RoleID = value; }
        }

        private string _RoleName = string.Empty;
        public string RoleName
        {
            get { return _RoleName; }
            set { _RoleName = value; }
        }

        private int _Count = 0;
        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        private DateTime _CreateTime = DateTime.Now;
        public DateTime CreateTime
        {
            get { return _CreateTime; }
            set { _CreateTime = value; }
        }

        public string CreateTimeName
        {
            get { return _CreateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        private string _CreateUser = string.Empty;
        public string CreateUser
        {
            get { return _CreateUser; }
            set { _CreateUser = value; }
        }

        private ControlLimitType _ControlLimit = ControlLimitType.ALLCompany;
        public ControlLimitType ControlLimit
        {
            get { return _ControlLimit; }
            set { _ControlLimit = value; }
        }

        private Guid _ControlLimitID = Guid.Empty;
        public Guid ControlLimitID
        {
            get { return _ControlLimitID; }
            set { _ControlLimitID = value; }
        }

        private string _ControlLimitPath = string.Empty;
        public string ControlLimitPath
        {
            get { return _ControlLimitPath; }
            set { _ControlLimitPath = value; }
        }

        private List<ControlLimitOuInfo> _ControlLimitOuList = new List<ControlLimitOuInfo>();
        public List<ControlLimitOuInfo> ControlLimitOuList
        {
            get { return _ControlLimitOuList; }
            set { _ControlLimitOuList = value; }
        }

        private int _IsDefault = 0;
        public int IsDefault
        {
            get { return _IsDefault; }
            set { _IsDefault = value; }
        }

        private List<RoleModuleList> _RoleList = new List<RoleModuleList>();
        public List<RoleModuleList> RoleList
        {
            get { return _RoleList; }
            set { _RoleList = value; }
        }

        private List<UserInfo> _UserList = new List<UserInfo>();
        public List<UserInfo> UserList
        {
            get { return _UserList; }
            set { _UserList = value; }
        }

        private string _UserNameList = string.Empty;
        public string UserNameList
        {
            get { return _UserNameList; }
            set { _UserNameList = value; }
        }

        private List<SameLevelOuInfo> _SameLevelOuList = new List<SameLevelOuInfo>();
        public List<SameLevelOuInfo> SameLevelOuList
        {
            get { return _SameLevelOuList; }
            set { _SameLevelOuList = value; }
        }

        public ErrorCodeInfo AddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (string.IsNullOrEmpty(_RoleName))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            return error;
        }
        public ErrorCodeInfo ChangeCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_RoleID == null || _RoleID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            if (string.IsNullOrEmpty(_RoleName))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            return error;
        }
    }

    [Serializable]
    public class RoleParam
    {
        private Guid _ParamID = Guid.Empty;
        public Guid ParamID
        {
            get { return _ParamID; }
            set { _ParamID = value; }
        }

        private string _ParamValue = string.Empty;
        public string ParamValue
        {
            get { return _ParamValue; }
            set { _ParamValue = value; }
        }

        private RoleModuleType _ModuleType = RoleModuleType.Ou;
        public RoleModuleType ModuleType
        {
            get { return _ModuleType; }
            set { _ModuleType = value; }
        }

        private string _ParamCode = string.Empty;
        public string ParamCode
        {
            get { return _ParamCode; }
            set { _ParamCode = value; }
        }
    }

    [Serializable]
    public class RoleModuleList
    {
        private RoleModuleType _ModuleType = RoleModuleType.Ou;
        public RoleModuleType ModuleType
        {
            get { return _ModuleType; }
            set { _ModuleType = value; }
        }

        private string _ModuleTypeName = string.Empty;
        public string ModuleTypeName
        {
            get { return _ModuleTypeName; }
            set { _ModuleTypeName = value; }
        }

        private List<RoleParam> _RoleParamList = new List<RoleParam>();
        public List<RoleParam> RoleParamList
        {
            get { return _RoleParamList; }
            set { _RoleParamList = value; }
        }
    }

    [Serializable]
    public class SameLevelOuInfo
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _SamelevelOuPath = string.Empty;
        public string SamelevelOuPath
        {
            get { return _SamelevelOuPath; }
            set { _SamelevelOuPath = value; }
        }
    }

    [Serializable]
    public class ControlLimitOuInfo
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private Guid _RoleID = Guid.Empty;
        public Guid RoleID
        {
            get { return _RoleID; }
            set { _RoleID = value; }
        }

        private Guid _OuID = Guid.Empty;
        public Guid OuID
        {
            get { return _OuID; }
            set { _OuID = value; }
        }

        private string _OUdistinguishedName = string.Empty;
        public string OUdistinguishedName
        {
            get { return _OUdistinguishedName; }
            set { _OUdistinguishedName = value; }
        }
    }

    [Serializable]
    public enum ControlLimitType
    {
        ALLCompany = 1,
        AppointOu = 2,
    }

    [Serializable]
    public enum RoleModuleType
    {
        Login = 1,
        Ou = 2,
        Group = 3,
        User = 4,
        PublicUser = 5,
        GroupAdmin = 6,
        SensitiveMail = 7,
        SameRootOu = 8,
        HabGroup = 9,
        RecycleOu = 10,
        MailboxDataBase = 11,
        MailAudit = 12,
    }

    [Serializable]
    public enum RoleParamCode
    {
        ModfiyName,
        AddOu,
        AddGroup,
        DeleteOu,
        RecycleOu,
        SameLevelOu,
        MoveOu,
        PublicUser,
        GroupAdmin,
        SpanOUMove,
        DeleteGroup,
        HabGroup,
        Login,
        SensitiveMail,
        MailAudit,
        AddUser,
        MailboxDataBase,
        DeleteUser,
        ModifyProfessionalGroup,
        SyncHab,
    }
}
