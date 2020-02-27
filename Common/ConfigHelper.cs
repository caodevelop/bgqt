using DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ConfigHelper
    {
        private static ConfigHelper _ConfigInstance;
        public static ConfigHelper ConfigInstance
        {
            get
            {
                if (_ConfigInstance == null)
                {
                    _ConfigInstance = new ConfigHelper();
                }
                return _ConfigInstance;
            }
            set
            {
                _ConfigInstance = value;
            }
        }
        public string this[string index]
        {
            get
            {
                string value = string.Empty;
                if (!string.IsNullOrEmpty(index))
                {
                    string strError = string.Empty;
                    if (!GetConfigValueByNameByProc(index, out value, out strError))
                    {
                        value = string.Empty;
                    }
                }
                return value;
            }
            set
            {
            }
        }

        public static List<string> GetConfigValue(string index)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(index))
            {
                string strError = string.Empty;
                GetConfigValueByNameByProc(index, out list, out strError);
            }
            return list;
        }

        private object o = new object();

        private bool GetConfigValueByNameByProc(string name, out string value, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            value = string.Empty;
            try
            {

                #region 参数变量
                CParameters paras = new CParameters();
                SqlParameter paraAccount = new SqlParameter("@Name", name);
                paras.Add(paraAccount);
                #endregion

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                DataSet ds = null;
                if (!_db.ExcuteByTransaction(paras, "[prc_GetCommonDataByName]", out ds, out strError))
                {
                    strError = "执行存储过程GetConfigValueByNameByProc异常";
                    bResult = false;
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    value = Convert.ToString(ds.Tables[0].Rows[0]["Comm_Value"]);
                    if (name == "AD_DC")
                    {
                        lock (o)
                        {
                            //判断DC有效性
                            GetDC(ref value);
                        }
                    }
                    bResult = true;
                }
                else
                {
                    strError = "执行存储过程GetConfigValueByNameByProc，输出的DataSet为空或无效";
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                strError = "接口方法GetConfigValueByNameByProc异常，error：" + ex.ToString();
                bResult = false;
            }
            return bResult;
        }

        private static bool GetConfigValueByNameByProc(string name, out List<string> list, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            list = new List<string>();
            try
            {

                #region 参数变量
                CParameters paras = new CParameters();
                SqlParameter paraAccount = new SqlParameter("@Name", name);
                paras.Add(paraAccount);
                #endregion

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                DataSet ds = null;
                if (!_db.ExcuteByTransaction(paras, "[prc_GetCommonDataByName]", out ds, out strError))
                {
                    strError = "执行存储过程GetConfigValueByNameByProc异常";
                    bResult = false;
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string value = Convert.ToString(dr["Comm_Value"]);
                        list.Add(value);
                    }
                    bResult = true;
                }
                else
                {
                    strError = "执行存储过程GetConfigValueByNameByProc，输出的DataSet为空或无效";
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                strError = "接口方法GetConfigValueByNameByProc异常，error：" + ex.ToString();
                bResult = false;
            }
            return bResult;
        }

        private bool GetAvailabilityDC(string dcName)
        {
            DirectoryEntry domain = new DirectoryEntry();
            try
            {
                domain.Path = string.Format("LDAP://{0}", dcName);
                domain.Username = ConfigInstance["AD_Admin"];
                domain.Password = ConfigInstance["AD_Password"]; ;
                domain.AuthenticationType = AuthenticationTypes.Secure;

                domain.RefreshCache();

                return true;
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("[IsConnected方法]错误信息：" + ex.Message);
                return false;
            }
        }

        private bool GetDC(ref string dcName)
        {
            string strError = string.Empty;
            bool bResult = true;
            if (!GetAvailabilityDC(dcName))
            {
                #region 参数变量
                CParameters paras = new CParameters();
                SqlParameter paraSpareDC = new SqlParameter("@Name", "SpareDC");
                paras.Add(paraSpareDC);
                #endregion

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                DataSet ds = null;
                if (!_db.ExcuteByTransaction(paras, "[prc_GetCommonDataByName]", out ds, out strError))
                {
                    strError = "执行存储过程GetConfigValueByNameByProc异常";
                    bResult = false;
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string dc = Convert.ToString(ds.Tables[0].Rows[0]["Comm_Value"]);
                        if (GetAvailabilityDC(dc))
                        {
                            UpdateConfigValueByName("AD_DC", dc);
                            dcName = dc;
                            break;
                        }
                    }

                    bResult = true;
                }
                else
                {
                    strError = "执行存储过程GetConfigValueByNameByProc，输出的DataSet为空或无效";
                    bResult = false;
                }

                return bResult;
            }
            else
            {
                return true;
            }
        }

        private bool UpdateConfigValueByName(string name, string value)
        {
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraName = new SqlParameter("@Name", name);
                paras.Add(paraName);
                SqlParameter paraValue = new SqlParameter("@Value", value);
                paras.Add(paraValue);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.prc_ModifyCommon", out ds, out strError))
                    {
                        Log4netHelper.Error("prc_ModifyCommon 执行错误,Error:" + strError);
                        bResult = false;
                        break;
                    }
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int iResult = 0;
                            iResult = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            switch (iResult)
                            {
                                case 1:
                                    bResult = true;
                                    break;
                                default:
                                    bResult = false;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                Log4netHelper.Error(string.Format("UpdateConfigValueByName 异常:{0}", ex.ToString()));
                bResult = false;
            }
            return bResult;
        }
    }
}
