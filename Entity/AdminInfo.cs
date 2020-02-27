using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class AdminInfo : UserInfo
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

        private string _ControlLimitPath = string.Empty;
        public string ControlLimitPath
        {
            get { return _ControlLimitPath; }
            set { _ControlLimitPath = value; }
        }

        private bool _IsCompanyAdmin = false;
        public bool IsCompanyAdmin
        {
            get { return _IsCompanyAdmin; }
            set { _IsCompanyAdmin = value; }
        }

        private string _Token = string.Empty;
        public string Token
        {
            get { return _Token; }
            set { _Token = value; }
        }

        private List<RoleModuleList> _RoleList = new List<RoleModuleList>();
        public List<RoleModuleList> RoleList
        {
            get { return _RoleList; }
            set { _RoleList = value; }
        }

        private List<SameLevelOuInfo> _SameLevelOuList = new List<SameLevelOuInfo>();
        public List<SameLevelOuInfo> SameLevelOuList
        {
            get { return _SameLevelOuList; }
            set { _SameLevelOuList = value; }
        }

        private List<ControlLimitOuInfo> _ControlLimitOuList = new List<ControlLimitOuInfo>();
        public List<ControlLimitOuInfo> ControlLimitOuList
        {
            get { return _ControlLimitOuList; }
            set { _ControlLimitOuList = value; }
        }

        private Hashtable _ParamList = new Hashtable();
        public Hashtable ParamList
        {
            get { return _ParamList; }
            set { _ParamList = value; }
        }
    }
}
