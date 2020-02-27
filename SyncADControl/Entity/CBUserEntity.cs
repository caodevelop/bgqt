using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncADControl.Entity
{
    public class CBUserEntity
    {
        public CBUserEntity()
        {
        }

        #region MyRegion
        private Guid _userid = Guid.Empty;
        private string _sAMAccountName = string.Empty;
        private string _displayname = string.Empty;
        private string _distinguishedName = string.Empty;
        private string _UserPrincipalName = string.Empty;
        private bool _PasswordNerverExpire = true;
        private DateTime _PasswordExpireTime = Convert.ToDateTime("1900-01-01");
        private bool _PasswordExpired = false;
        private DateTime _WhenChanged = Convert.ToDateTime("1900-01-01");
        private DateTime _WhenCreated = Convert.ToDateTime("1900-01-01");
        private bool _IsDisable = false;
        private DateTime _LastLogon = Convert.ToDateTime("1900-01-01");
        private string _Company = string.Empty;
        private string _Department = string.Empty;

        public Guid UserID
        {
            get { return _userid; }
            set { _userid = value; }
        }

        /// <summary>
        /// AD用户帐号
        /// </summary>
        public string SAMAccountName
        {
            get { return _sAMAccountName; }
            set { _sAMAccountName = value; }
        }

        /// <summary>
        /// AD用户显示名称
        /// </summary>
        public string Displayname
        {
            get { return _displayname; }
            set { _displayname = value; }
        }

        /// <summary>
        /// AD用户 distinguishedname属性
        /// </summary>
        public string DistinguishedName
        {
            get { return _distinguishedName; }
            set { _distinguishedName = value; }
        }

        /// <summary>
        /// AD用户 upn属性
        /// </summary>
        public string UserPrincipalName
        {
            get { return _UserPrincipalName; }
            set { _UserPrincipalName = value; }
        }

        /// <summary>
        /// AD用户密码永不过期 属性
        /// </summary>
        public bool PasswordNerverExpire
        {
            get { return _PasswordNerverExpire; }
            set { _PasswordNerverExpire = value; }
        }

        /// <summary>
        /// AD用户密码过期时间
        /// </summary>
        public DateTime PasswordExpireTime
        {
            get { return _PasswordExpireTime; }
            set { _PasswordExpireTime = value; }
        }

        /// <summary>
        /// AD用户密码已过期 属性
        /// </summary>
        public bool PasswordExpired
        {
            get { return _PasswordExpired; }
            set { _PasswordExpired = value; }
        }

        /// <summary>
        /// AD用户最后一次修改时间
        /// </summary>
        public DateTime WhenChanged
        {
            get { return _WhenChanged; }
            set { _WhenChanged = value; }
        }

        /// <summary>
        /// AD用户创建时间
        /// </summary>
        public DateTime WhenCreated
        {
            get { return _WhenCreated; }
            set { _WhenCreated = value; }
        }

        /// <summary>
        /// AD用户是否已禁用 属性
        /// </summary>
        public bool IsDisable
        {
            get { return _IsDisable; }
            set { _IsDisable = value; }
        }

        /// <summary>
        /// AD用户最后一次登录时间
        /// </summary>
        public DateTime LastLogon
        {
            get { return _LastLogon; }
            set { _LastLogon = value; }
        }

        /// <summary>
        /// AD用户公司属性
        /// </summary>
        public string Company
        {
            get { return _Company; }
            set { _Company = value; }
        }

        /// <summary>
        /// AD用户部门属性
        /// </summary>
        public string Department
        {
            get { return _Department; }
            set { _Department = value; }
        }
        #endregion
    }
}
