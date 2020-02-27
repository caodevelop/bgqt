using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class LogInfo
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _LogNum = string.Empty;
        public string LogNum
        {
            get { return _LogNum; }
            set { _LogNum = value; }
        }

        private string _AdminAccount = string.Empty;
        public string AdminAccount
        {
            get { return _AdminAccount; }
            set { _AdminAccount = value; }
        }

        private Guid _AdminID = Guid.Empty;
        public Guid AdminID
        {
            get { return _AdminID; }
            set { _AdminID = value; }
        }

        private Guid _RoleID = Guid.Empty;
        public Guid RoleID
        {
            get { return _RoleID; }
            set { _RoleID = value; }
        }

        private string _ClientIP = string.Empty;
        public string ClientIP
        {
            get { return _ClientIP; }
            set { _ClientIP = value; }
        }

        private string _OperateType = string.Empty;
        public string OperateType
        {
            get { return _OperateType; }
            set { _OperateType = value; }
        }

        private string _OperateLog = string.Empty;
        public string OperateLog
        {
            get { return _OperateLog; }
            set { _OperateLog = value; }
        }

        private DateTime _OperateTime = DateTime.Now;
        public DateTime OperateTime
        {
            get { return _OperateTime; }
            set { _OperateTime = value; }
        }

      
        public string OperateTimeName
        {
            get { return _OperateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        private bool _OperateResult = true;
        public bool OperateResult
        {
            get { return _OperateResult; }
            set { _OperateResult = value; }
        }
    }
}
