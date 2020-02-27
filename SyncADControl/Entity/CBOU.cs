using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncADControl.Entity
{
    public class CBOU
    {
        public CBOU()
        { }

        #region old

        //private int m_id;
        //public int Id
        //{
        //    get { return m_id; }
        //    set { m_id = value; }
        //}

        //private string m_name = string.Empty;
        //public string Name
        //{
        //    get { return m_name; }
        //    set { m_name = value; }
        //}

        //private string m_path = string.Empty;
        //public string Path
        //{
        //    get { return m_path; }
        //    set { m_path = value; }
        //}

        //private string m_description = string.Empty;
        //public string Description
        //{
        //    get { return m_description; }
        //    set { m_description = value; }
        //}

        //private string m_site;
        //public string Site
        //{
        //    get { return m_site; }
        //    set { m_site = value; }
        //}

        //private string m_guid;
        //public string Guid
        //{
        //    get { return m_guid; }
        //    set { m_guid = value; }
        //}

        //private CBOU m_parentOU;
        //public CBOU ParentOu
        //{
        //    get { return m_parentOU; }
        //    set { m_parentOU = value; }
        //}

        //private string m_parentOUPath = string.Empty;
        //public string ParentOuPath
        //{
        //    get { return m_parentOUPath; }
        //    set { m_parentOUPath = value; }
        //}
        #endregion

        #region new
        private string _Guid = string.Empty;
        private string _DisplayName = string.Empty;
        private string _ParentGuid = string.Empty;
        private string _LdapPath = string.Empty;
        private string _ParentLdapPath = string.Empty;


        /// <summary>
        /// ou Gudi唯一标示
        /// </summary>
        public string Guid
        {
            get { return _Guid; }
            set { _Guid = value; }
        }

        /// <summary>
        /// ou 显示名称
        /// </summary>
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        /// <summary>
        /// ou 父节点唯一标示
        /// </summary>
        public string ParentGuid
        {
            get { return _ParentGuid; }
            set { _ParentGuid = value; }
        }

        /// <summary>
        /// ou Ldap路径
        /// </summary>
        public string LdapPath
        {
            get { return _LdapPath; }
            set { _LdapPath = value; }
        }

        /// <summary>
        /// ou 父节点Ldap路径
        /// </summary>
        public string ParentLdapPath
        {
            get { return _ParentLdapPath; }
            set { _ParentLdapPath = value; }
        }
        #endregion
    }
}
