using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class MailAuditInfo
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private List<UserInfo> _Audits = new List<UserInfo>();
        public List<UserInfo> Audits
        {
            get { return _Audits; }
            set { _Audits = value; }
        }

        private string _AuditUsers = string.Empty;
        public string AuditUsers
        {
            get { return _AuditUsers; }
            set { _AuditUsers = value; }
        }

        private GroupInfo _Group = new GroupInfo();
        public GroupInfo Group
        {
            get { return _Group; }
            set { _Group = value; }
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

        private DateTime _UpdateTime = DateTime.Now;
        public DateTime UpdateTime
        {
            get { return _UpdateTime; }
            set { _UpdateTime = value; }
        }

        public string UpdateTimeName
        {
            get { return _UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        private Guid _RoleID = Guid.Empty;
        public Guid RoleID
        {
            get { return _RoleID; }
            set { _RoleID = value; }
        }

        private Guid _CreateUserID = Guid.Empty;
        public Guid CreateUserID
        {
            get { return _CreateUserID; }
            set { _CreateUserID = value; }
        }

        public ErrorCodeInfo AddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_Audits.Count <= 0)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            if (_Group.GroupID == null || _Group.GroupID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            return error;
        }

        public ErrorCodeInfo ChangeCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_ID == null || _ID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            if (_Audits.Count <= 0)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            if (_Group.GroupID == null || _Group.GroupID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }

            return error;
        }
    }
}
