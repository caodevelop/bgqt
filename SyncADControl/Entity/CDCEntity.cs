using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncADControl.Entity
{
    public class CDCEntity
    {
        public CDCEntity(string cdName, string ldap)
        {
            m_dcName = cdName;
            m_ldapPath = ldap;
        }

        private string m_dcName = string.Empty;
        public string DCName
        {
            get { return m_dcName; }
            set { m_dcName = value; }
        }

        private string m_ldapPath = string.Empty;
        public string LDAPPath
        {
            get { return m_ldapPath; }
            set { m_ldapPath = value; }
        }


    }
}
