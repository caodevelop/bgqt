using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class SystemReportInfo
    {
        private Guid _UserID = Guid.Empty;
        public Guid UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
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

        private String _DistinguishedName = string.Empty;
        public String DistinguishedName
        {
            get { return _DistinguishedName; }
            set { _DistinguishedName = value; }
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

        private DateTime _LastLoginTime = DateTime.Now;
        public DateTime LastLoginTime
        {
            get { return _LastLoginTime; }
            set { _LastLoginTime = value; }
        }
        public string LastLoginTimeName
        {
            get
            {
                string _LastLoginTimeName = string.Empty;
                _LastLoginTimeName = _LastLoginTime.Year > 2000 ? _LastLoginTime.ToString("yyyy-MM-dd HH:mm:ss") : "-";
                return _LastLoginTimeName;
            }
        }

        private DateTime _PasswordExpireTime = DateTime.Now;
        public DateTime PasswordExpireTime
        {
            get { return _PasswordExpireTime; }
            set { _PasswordExpireTime = value; }
        }

        public string PasswordExpireTimeName
        {
            get { return _PasswordExpireTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        private State _UserSatus = State.Enable;
        public State UserSatus
        {
            get { return _UserSatus; }
            set { _UserSatus = value; }
        }

        private string _StatusName = string.Empty;
        public string StatusName
        {
           
            get {
                switch (_UserSatus)
                {
                    case State.Enable:
                        _StatusName = "启用";
                        break;
                    case State.Delete:
                        _StatusName = "已删除";
                        break;
                    case State.Disable:
                        _StatusName = "停用";
                        break;
                }
                return _StatusName;
            }
            
        }

        private PasswordType _Type = PasswordType.NerverExpire;
        public PasswordType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private string _PasswordTypeName = string.Empty;
        public string PasswordTypeName
        {

            get
            {
                switch (_Type)
                {
                    case PasswordType.NerverExpire:
                        _StatusName = "永不过期";
                        break;
                    case PasswordType.Expired:
                        _StatusName = "已过期";
                        break;
                    case PasswordType.WillExpire:
                        _StatusName = "未过期";
                        break;
                }
                return _StatusName;
            }
        }
    }


}
