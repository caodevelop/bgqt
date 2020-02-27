using Common;
using SyncADControl.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SyncADControl
{
    public class CXmlOUList
    {
        static string XmlTargets = "CompanyTree.xml";
        static string LogPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "\\" + XmlTargets;
            }
        }

        XmlDocument m_document;
        public CXmlOUList(string xmlFileName)
        {
            if (xmlFileName.Length > 0)
            {
                XmlTargets = xmlFileName;
            }
            m_document = new XmlDocument();
            m_document.Load(LogPath);
            Log4netHelper.Error(" *********************LogPath " + LogPath);
            _enritys = new List<OUSEntity>();
        }

        public List<CDCEntity> GetDCList()
        {
            List<CDCEntity> dclist = new List<CDCEntity>();
            if (_enritys.Count == 0)
            {
                GetItem();
            }

            foreach (OUSEntity entity in _enritys)
            {
                dclist.Add(entity.DC);
                foreach (DictionaryEntry de in entity.ChildNode)
                {
                    string temp = de.Key.ToString();
                }
            }

            return dclist;
        }

        /// <summary>
        /// 获取ou list
        /// </summary>
        /// <returns></returns>
        public List<XmlOUEntity> GetEntity(string ousName)
        {
            if (_enritys.Count == 0)
            {
                GetItem();
            }
            List<XmlOUEntity> ous = null;
            foreach (OUSEntity enrity in _enritys)
            {
                if (enrity.DC.DCName == ousName)
                {
                    ous = new List<XmlOUEntity>();
                    foreach (XmlOUEntity ou in enrity.ChildNode.Values)
                    {
                        ous.Add(ou);
                    }
                    break;
                }
            }
            return ous;
        }

        /// <summary>
        /// 判断ou是否存在 并返回
        /// </summary>
        /// <param name="ouName"></param>
        /// <param name="dcName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CheckContains(string ouName, string dcName, out XmlOUEntity entity)
        {
            entity = null;
            bool bResult = false;

            if (_enritys.Count == 0)
            {
                GetItem();
            }
            foreach (OUSEntity ouEntity in _enritys)
            {
                if (ouEntity.DC.DCName.Trim().ToLower() == dcName.Trim().ToLower())
                {
                    object obj = (Object)(ouName.Trim());
                    if (ouEntity.ChildNode.Contains(obj))
                    {
                        bResult = true;
                        entity = (XmlOUEntity)ouEntity.ChildNode[obj];
                    }
                    break;
                }
            }
            return bResult;
        }

        private void GetItem()
        {
            XmlNodeList nodes = m_document.GetElementsByTagName("ous");
            if (nodes == null || nodes.Count == 0)
            {

                return;
            }

            foreach (XmlNode node in nodes)
            {
                OUSEntity ousEntiy = new OUSEntity(node.Attributes["name"].InnerText.Trim(), new Hashtable());
                _enritys.Add(ousEntiy);
                Recursion(node.ChildNodes, ousEntiy);
            }
        }

        private void Recursion(XmlNodeList list, OUSEntity ousEntity)
        {
            XmlOUEntity entity = null;
            foreach (XmlNode node in list)
            {
                if (node.Attributes["name"] != null && node.ParentNode != null && node.ParentNode.Attributes != null && node.ParentNode.Attributes["name"] != null)
                {
                    entity = new XmlOUEntity(node.Attributes["name"].InnerText.Trim(), node.ParentNode.Attributes["name"].InnerText.Trim());
                }
                else if (node.Attributes["name"] != null)
                {
                    entity = new XmlOUEntity(node.Attributes["name"].InnerText.Trim(), string.Empty);
                }

                if (node.ChildNodes != null && node.ChildNodes.Count != 0)
                {
                    entity.HaveChildNodes = true;
                }
                ousEntity.ChildNode.Add(entity.Name, entity);
                Recursion(node.ChildNodes, ousEntity);
            }
        }
        private List<OUSEntity> _enritys;

        public string[] GetNamedOUArray()
        {
            string[] TempArray = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(LogPath);
                XmlNodeList nodelist = doc.GetElementsByTagName("NamedOU");

                if (nodelist.Count > 0)
                {
                    TempArray = new string[nodelist.Count];
                    for (int i = 0; i < nodelist.Count; i++)
                    {
                        TempArray[i] = nodelist[i].Attributes["name"].Value;
                    }
                }
                else
                {
                    Log4netHelper.Error(string.Format(" *********************GetNamedOUArray  nodelist count is 0"));
                }
                doc.Save(LogPath);
            }
            catch (Exception ex)
            {
                TempArray = null;
                Log4netHelper.Error(string.Format(" *********************GetNamedOUArray  Exception:{0}", ex.ToString()));
            }


            return TempArray;
        }
    }

    #region 2012.08.21 OU排序读取XML方法
    public class CXMLOUOrder
    {
        static string XmlTargets = "OuOrder.xml";
        static string LogPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "\\" + XmlTargets;
            }
        }

        XmlDocument m_document;
        public CXMLOUOrder(string xmlFileName)
        {
            if (xmlFileName.Length > 0)
            {
                XmlTargets = xmlFileName;
            }
            m_document = new XmlDocument();
            m_document.Load(LogPath);
        }

        /// <summary>
        /// 判断ou是否存在 并返回
        /// </summary>
        /// <param name="ouName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CheckContains(string ouName)
        {
            bool bResult = false;

            foreach (string ou in OuList)
            {
                if (ou == ouName)
                {
                    bResult = true;
                    break;
                }
            }
            return bResult;
        }

        List<string> ouList = null;

        public List<string> OuList
        {
            get
            {
                if (ouList == null)
                {
                    ouList = GetItem();
                }
                return ouList;
            }
            set
            {
                ouList = value;
            }
        }
        private List<string> GetItem()
        {
            XmlNodeList nodes = m_document.GetElementsByTagName("ou");
            List<string> ous = new List<string>();
            foreach (XmlNode node in nodes)
            {
                ous.Add(node.Attributes["name"].InnerText.Trim());
            }
            return ous;
        }
    }
    #endregion
}
