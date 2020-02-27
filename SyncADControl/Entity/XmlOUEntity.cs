using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncADControl.Entity
{
    public class XmlOUEntity
    {
        public XmlOUEntity(string strName, string strParentsName)
        {
            _name = strName;
            _parentsName = strParentsName;
            _haveChildNodes = false;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _parentsName;
        public string ParentsName
        {
            get { return _parentsName; }
        }

        private bool _haveChildNodes;
        public bool HaveChildNodes
        {
            get { return _haveChildNodes; }
            set { _haveChildNodes = value; }
        }
    }

    public class OUSEntity
    {
        public OUSEntity(string name, Hashtable childNode)
        {
            string strLDAP = string.Empty;
            if (name.IndexOf('.') != -1)
            {
                strLDAP = name.Substring(name.IndexOf('.') + 1);
                if (strLDAP.IndexOf('.') != -1)
                {
                }
                else
                {
                    strLDAP = name;
                }
            }

            string[] strArr = strLDAP.Split('.');
            string strldap = string.Empty;
            for (int i = 0; i < strArr.Length; i++)
            {
                if (strldap.Length != 0)
                    strldap += ",";
                strldap += string.Format("DC={0}", strArr[i]);
            }
            m_DC = new CDCEntity(name, strldap);
            m_childNode = childNode;
        }
        public OUSEntity()
        {
        }
        private CDCEntity m_DC;
        public CDCEntity DC
        {
            get { return m_DC; }
            set { m_DC = value; }
        }
        private Hashtable m_childNode;
        public Hashtable ChildNode
        {
            get { return m_childNode; }
            set { m_childNode = value; }
        }

    }
}
