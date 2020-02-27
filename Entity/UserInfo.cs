using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class UserInfo
    {
        private Guid _UserID = Guid.Empty;
        public Guid UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private String _FirstName = String.Empty;
        public String FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; }
        }

        private String _LastName = String.Empty;
        public String LastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }

        /// <summary>
        /// 账号(50)
        /// </summary>
        private String _UserAccount = String.Empty;
        public String UserAccount
        {
            get { return _UserAccount; }
            set { _UserAccount = value; }
        }

        /// <summary>
        /// 姓名(50)
        /// </summary>
        private String _DisplayName = String.Empty;
        public String DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        public String AliasName
        { get; set; } = String.Empty;

        private String _sAMAccountName = String.Empty;
        public String SAMAccountName
        {
            get { return _sAMAccountName; }
            set { _sAMAccountName = value; }
        }

        private String _Password = String.Empty;
        public String Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private bool _NextLoginChangePassword = false;
        public bool NextLoginChangePassword
        {
            get { return _NextLoginChangePassword; }
            set { _NextLoginChangePassword = value; }
        }

        private bool _PasswordNeverExpire = false;
        public bool PasswordNeverExpire
        {
            get { return _PasswordNeverExpire; }
            set { _PasswordNeverExpire = value; }
        }

        private bool _UserStatus = true;
        public bool UserStatus
        {
            get { return _UserStatus; }
            set { _UserStatus = value; }
        }
        
        private bool _IsCreateMail = false;
        public bool IsCreateMail
        {
            get { return _IsCreateMail; }
            set { _IsCreateMail = value; }
        }

        private bool _UseDefaultPassword = false;
        public bool UseDefaultPassword
        {
            get { return _UseDefaultPassword; }
            set { _UseDefaultPassword = value; }
        }

        private Guid _ParentOuId = Guid.Empty;
        public Guid ParentOuId
        {
            get { return _ParentOuId; }
            set { _ParentOuId = value; }
        }

        private string _ParentOu = string.Empty;
        public string ParentOu
        {
            get { return _ParentOu; }
            set { _ParentOu = value; }
        }

        private string _ParentDistinguishedName = string.Empty;
        public string ParentDistinguishedName
        {
            get { return _ParentDistinguishedName; }
            set { _ParentDistinguishedName = value; }
        }

        private string _Phone = string.Empty;
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }

        private string _Mobile = string.Empty;
        public string Mobile
        {
            get { return _Mobile; }
            set { _Mobile = value; }
        }

        private string _Office = string.Empty;
        public string Office
        {
            get { return _Office; }
            set { _Office = value; }
        }

        private string _Fax = string.Empty;
        public string Fax
        {
            get { return _Fax; }
            set { _Fax = value; }
        }

        private string _Company = string.Empty;
        public string Company
        {
            get { return _Company; }
            set { _Company = value; }
        }

        private string _Department = string.Empty;
        public string Department
        {
            get { return _Department; }
            set { _Department = value; }
        }

        private string _Post = string.Empty;
        public string Post
        {
            get { return _Post; }
            set { _Post = value; }
        }

        private string _Description = string.Empty;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
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

        private List<GroupInfo> _BelongGroups = new List<GroupInfo>();
        public List<GroupInfo> BelongGroups
        {
            get { return _BelongGroups; }
            set { _BelongGroups = value; }
        }

        private ExchangeInfo _UserExchange = new ExchangeInfo();
        public ExchangeInfo UserExchange
        {
            get { return _UserExchange; }
            set { _UserExchange = value; }
        }

        public ErrorCodeInfo AddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_ParentOuId == null || _ParentOuId == Guid.Empty)
            {
                error.Code = ErrorCode.ParentIdEmpty;
            }
            else if (string.IsNullOrEmpty(_UserAccount))
            {
                error.Code = ErrorCode.AccountEmpty;
            }
            else if (string.IsNullOrEmpty(_DisplayName))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            else if (string.IsNullOrEmpty(_sAMAccountName))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            else if (_sAMAccountName.Length > 20)
            {
                error.Code = ErrorCode.MaxAccountLength;
            }
            else if (_sAMAccountName.Contains("@"))
            {
                error.Code = ErrorCode.AccountIllegal;
            }
            else if (string.IsNullOrEmpty(_Password))
            {
                error.Code = ErrorCode.PasswordIllegal;
            }
            return error;
        }

        public ErrorCodeInfo ChangeCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_UserID == null || _UserID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            else if (string.IsNullOrEmpty(_DisplayName))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            else if (string.IsNullOrEmpty(_FirstName))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            else if (string.IsNullOrEmpty(_LastName))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            return error;
        }
    }

    [Serializable]
    public class ExchangeInfo
    {
        private bool _ExchangeStatus = true;
        public bool ExchangeStatus
        {
            get { return _ExchangeStatus; }
            set { _ExchangeStatus = value; }
        }

        private bool _Activesync = true;
        public bool Activesync
        {
            get { return _Activesync; }
            set { _Activesync = value; }
        }

        private bool _Mapi = true;
        public bool Mapi
        {
            get { return _Mapi; }
            set { _Mapi = value; }
        }

        private bool _POP3 = true;
        public bool POP3
        {
            get { return _POP3; }
            set { _POP3 = value; }
        }

        private bool _Imap4 = true;
        public bool Imap4
        {
            get { return _Imap4; }
            set { _Imap4 = value; }
        }

        private bool _OWA = true;
        public bool OWA
        {
            get { return _OWA; }
            set { _OWA = value; }
        }

        private int _MailSize = 0;
        public int MailSize
        {
            get { return _MailSize; }
            set { _MailSize = value; }
        }

        private string _Database = string.Empty;
        public string Database
        {
            get { return _Database; }
            set { _Database = value; }
        }
    }

    [Serializable]
    ///
    ///用户登录验证结果
    ///
    public enum LoginResult
    {
        ///
        ///正常登录
        ///
        LOGIN_USER_OK = 0,
        ///
        ///用户不存在
        ///
        LOGIN_USER_DOESNT_EXIST,
        ///
        ///用户帐号被禁用
        ///
        LOGIN_USER_ACCOUNT_INACTIVE,
        ///
        ///用户密码不正确
        ///
        LOGIN_USER_PASSWORD_INCORRECT
    }

    [Serializable]
    ///
    ///用户属性定义标志
    ///
    public enum ADS_USER_FLAG_ENUM
    {
        ///
        ///登录脚本标志。如果通过 ADSI LDAP 进行读或写操作时，该标志失效。如果通过 ADSI WINNT，该标志为只读。
        ///
        ADS_UF_SCRIPT = 0X0001,
        ///
        ///用户帐号禁用标志
        ///
        ADS_UF_ACCOUNTDISABLE = 0X0002,
        ///
        ///主文件夹标志
        ///
        ADS_UF_HOMEDIR_REQUIRED = 0X0008,
        ///
        ///过期标志
        ///
        ADS_UF_LOCKOUT = 0X0010,
        ///
        ///用户密码不是必须的
        ///
        ADS_UF_PASSWD_NOTREQD = 0X0020,
        ///
        ///密码不能更改标志
        ///
        ADS_UF_PASSWD_CANT_CHANGE = 0X0040,
        ///
        ///使用可逆的加密保存密码
        ///
        ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080,
        ///
        ///本地帐号标志
        ///
        ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0X0100,
        ///
        ///普通用户的默认帐号类型
        ///
        ADS_UF_NORMAL_ACCOUNT = 0X0200,
        ///
        ///跨域的信任帐号标志
        ///
        ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0X0800,
        ///
        ///工作站信任帐号标志
        ///
        ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,
        ///
        ///服务器信任帐号标志
        ///
        ADS_UF_SERVER_TRUST_ACCOUNT = 0X2000,
        ///
        ///密码永不过期标志
        ///
        ADS_UF_DONT_EXPIRE_PASSWD = 0X10000,
        ///
        /// MNS 帐号标志
        ///
        ADS_UF_MNS_LOGON_ACCOUNT = 0X20000,
        ///
        ///交互式登录必须使用智能卡
        ///
        ADS_UF_SMARTCARD_REQUIRED = 0X40000,
        ///
        ///当设置该标志时，服务帐号（用户或计算机帐号）将通过 Kerberos 委托信任
        ///
        ADS_UF_TRUSTED_FOR_DELEGATION = 0X80000,
        ///
        ///当设置该标志时，即使服务帐号是通过 Kerberos 委托信任的，敏感帐号不能被委托
        ///
        ADS_UF_NOT_DELEGATED = 0X100000,
        ///
        ///此帐号需要 DES 加密类型
        ///
        ADS_UF_USE_DES_KEY_ONLY = 0X200000,
        ///
        ///不要进行 Kerberos 预身份验证
        ///
        ADS_UF_DONT_REQUIRE_PREAUTH = 0X4000000,
        ///
        ///用户密码过期标志
        ///
        ADS_UF_PASSWORD_EXPIRED = 0X800000,
        ///
        ///用户帐号可委托标志
        ///
        ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0X1000000
    }

    [Serializable]
    public enum PasswordType
    {
        NerverExpire = 1,
        WillExpire = 2,
        Expired = 3,
    }
}
