using Common;
using DBUtility;
using SyncADControl.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SyncADControl
{
    public class SyncADManager
    {
        private CBaseDB m_db;
        private CXmlOUList m_cOuList;
        private List<string> strNamedOUArray = new List<string>();
        private string strRootOU = string.Empty;

        public SyncADManager()
        {
            m_db = new CBaseDB(Conntection.strConnection);
        }

        public bool Load(out string strError)
        {
            strError = string.Empty;
            bool bResult = true;
            int count = 10;
            do
            {
                count--;
                if (!m_db.ExcuteBySql(" truncate table dbo.[T_Base_Temp_ADUser]", out strError))
                {
                    Log4netHelper.Error(" *********************清空User信息失败, error:" + strError);
                    return false;
                }

                bResult = run();
            } while (!bResult && count != 0);

            if (count == 0 && !bResult)
            {
                strError = "*********************获取AD数据失败**********";
                Log4netHelper.Error(strError);
                return false;
            }

            CParameters parametersUser = new CParameters();
            if (!m_db.ExcuteByTransaction(parametersUser, "prc_TranscationUserData", out strError))
            {
                Log4netHelper.Error(" *********************导入User信息失败, error:" + strError);
                return false;
            }

            SyncUserMailSize();

            SyncSystemMailCount();

            SyncUserMailCount();

            return true;
        }

        private bool run()
        {
            bool bResult = true;
            //赋予全局变量dc，ldap集合的值
            if (!SynchronizationOUData(ConfigHelper.ConfigInstance["AD_DC"], ConfigHelper.ConfigInstance["AD_DefaultCompany"]))
            {
                bResult = false;
            }
            return bResult;
        }

        private bool SynchronizationOUData(string dcName, string dcLDAP)
        {
            bool bResult = true;
            List<CBOU> listOus = new List<CBOU>();
            List<CBUserEntity> userList = new List<CBUserEntity>();
            string strError = string.Empty;
            try
            {
                do
                {
                    Log4netHelper.Info("============================获取数据 site :" + dcName + "============================");
                    if (!ADSynchronization(dcName, dcLDAP, out listOus, out userList, out strError))
                    {
                        Log4netHelper.Error(" **********************LDAP获取信息失败, error:" + strError);
                        bResult = false;
                        break;
                    }

                    if (userList != null && userList.Count > 0)
                    {
                        Log4netHelper.Info("============================获取User数据成功! count:" + userList.Count.ToString() + "============================");
                        ArrayList arrSql = new ArrayList();
                        int i = 0;
                        foreach (CBUserEntity entity in userList)
                        {
                            i++;
                            arrSql.Add(string.Format(" insert into dbo.T_Base_Temp_ADUser(UserID"
                                                                                  + ",sAMAccountName"
                                                                                  + ",displayName"
                                                                                  + ",distinguishedName"
                                                                                  + ",UserPrincipalName"
                                                                                  + ",Mail"
                                                                                  + ",PasswordNerverExpire"
                                                                                  + ",PasswordExpireTime"
                                                                                  + ",PasswordExpired"
                                                                                  + ",WhenChanged"
                                                                                  + ",WhenCreated"
                                                                                  + ",IsDisable"
                                                                                  + ",LastLogon"
                                                                                  + ",Company"
                                                                                  + ",department)"
                                                      + " values (N'{0}',N'{1}',N'{2}',N'{3}',N'{4}',N'{5}',N'{6}',N'{7}',N'{8}',N'{9}',N'{10}',N'{11}',N'{12}',N'{13}',N'{14}') ",
                                entity.UserID
                                , entity.SAMAccountName
                               , entity.Displayname
                               , entity.DistinguishedName
                               , entity.UserPrincipalName
                               , entity.Mail
                               , Convert.ToInt32(entity.PasswordNerverExpire)
                               , entity.PasswordExpireTime
                               , Convert.ToInt32(entity.PasswordExpired)
                               , entity.WhenChanged
                               , entity.WhenCreated
                               , Convert.ToInt32(entity.IsDisable)
                               , entity.LastLogon
                               , entity.Company
                               , entity.Department));
                            if (arrSql.Count == 500 || i == userList.Count)
                            {
                                if (!m_db.ExcuteByArrayList(arrSql, out strError))
                                {
                                    strError += string.Format("ExcuteByArrayList Error:{0} ", strError);
                                    bResult = false;
                                }
                                arrSql.Clear();
                            }

                        }
                    }

                    if (string.IsNullOrEmpty(strError))
                    {
                        Log4netHelper.Info(" *********************插入User信息成功");
                    }
                    else
                    {
                        Log4netHelper.Error(" *********************插入User信息失败, error:" + strError);
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("SynchronizationOUData error:" + ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        private bool ADSynchronization(string dcName, string ldap, out List<CBOU> ouList, out List<CBUserEntity> userList, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            ouList = new List<CBOU>();
            userList = new List<CBUserEntity>();
            DirectoryEntry Ad_DC = null;
            try
            {
                Ad_DC = new DirectoryEntry(string.Format("LDAP://{0}/{1}", dcName, ldap));
                Log4netHelper.Info(string.Format("dc name:{0}, dc ldap:{1}, dc guid:{2} ", dcName, string.Format("LDAP://{0}/{1}", dcName, ldap), Ad_DC.Guid));

                if (SearchProfessionalGroupOus(Ad_DC, out strNamedOUArray, out strError))
                {
                    bResult = SearchOUByldap(Ad_DC, dcName, SearchScope.OneLevel, ref ouList, ref userList);
                }
            }
            catch (Exception error)
            {
                strError = error.ToString();
                Log4netHelper.Error(string.Format("ADSynchronization Exception：{0} ", error.ToString()));
                bResult = false;
            }
            finally
            {
                if (Ad_DC != null)

                {
                    Ad_DC.Close();
                    Ad_DC.Dispose();
                    Ad_DC = null;
                }
            }

            return bResult;
        }

        /// <summary>
        /// 调用递归方法，获取指定ou子节点数据
        /// </summary>
        /// <param name="Ad_OU_CN"></param>
        /// <param name="searchScope"></param>
        /// <param name="ouADlist"></param>
        private bool SearchOUByldap(DirectoryEntry Ad_OU_CN, string dcName, SearchScope searchScope, ref List<CBOU> ouList, ref List<CBUserEntity> userList)
        {
            bool bResult = false;
            try
            {
                DirectorySearcher Ad_deSearch = new DirectorySearcher();
                Ad_deSearch.SearchRoot = Ad_OU_CN;
                Ad_deSearch.Filter = "(objectClass=organizationalUnit)";
                Ad_deSearch.SearchScope = searchScope;
                Ad_deSearch.PageSize = 100000;
                SearchResultCollection results = Ad_deSearch.FindAll();

                //获取DC下的全部OU
                if (results != null && results.Count > 0)
                {
                    CBOU ouADInfo = null;
                    DirectoryEntry CN = null;
                    foreach (SearchResult Result in results)
                    {
                        ouADInfo = new CBOU();
                        CN = Result.GetDirectoryEntry();
                        //(排除)如果遍历为Subtree，会重复添加当前节点，判断如果遍历name和当前ou相同则跳过不重复添加
                        if (Ad_OU_CN.Name == CN.Name)
                        {
                            if (results.Count == 1)
                            {
                                bResult = true;
                            }
                            continue;
                        }
                        ouADInfo.LdapPath = CN.Path;
                        ouADInfo.Guid = CN.Guid.ToString();
                        ouADInfo.DisplayName = CN.Properties["name"].Value == null ? "" : CN.Properties["name"].Value.ToString();

                        ouADInfo.ParentGuid = CN.Parent.SchemaClassName == "domainDNS" ? "0" : Convert.ToString(CN.Parent.Guid);
                        ouADInfo.ParentLdapPath = CN.Parent.SchemaClassName == "domainDNS" ? "0" : Convert.ToString(CN.Parent.Path);


                        //继续递归
                        if (searchScope == SearchScope.OneLevel)
                        {
                            //加载所有子节点，调用Subtree，获取所有子节点信息
                            bResult = SearchOUByldap(CN, dcName, SearchScope.Subtree, ref ouList, ref userList);
                            if (!bResult)
                                break;
                        }
                        else if (searchScope == SearchScope.Subtree)
                        {

                        }

                        ouList.Add(ouADInfo);
                        bResult = SearchUserByOU(CN, ref userList);
                        if (!bResult)
                            break;

                        CN.Close();
                        CN.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("SearchOUByldap Exception: " + ex.ToString());
            }
            finally
            {
            }

            return bResult;
        }

        private bool SearchUserByOU(DirectoryEntry ouEntry, ref List<CBUserEntity> userADlist)
        {
            bool bResult = false;
            try
            {

                DirectorySearcher Ad_deSearch = new DirectorySearcher();
                Ad_deSearch.SearchRoot = ouEntry;
                Ad_deSearch.Filter = "(&(objectClass=user))";
                Ad_deSearch.SearchScope = SearchScope.OneLevel;
                Ad_deSearch.PageSize = 100000;
                SearchResultCollection results = Ad_deSearch.FindAll();
                if (results != null && results.Count > 0)
                {
                    CBUserEntity user = null;
                    DirectoryEntry CN = null;
                    foreach (SearchResult Result in results)
                    {
                        user = new CBUserEntity();
                        CN = Result.GetDirectoryEntry();
                        if (CN.Properties["userPrincipalName"].Value != null)
                        {
                            int TempuserAccountControl = 0;
                            if (CN.Properties["userAccountControl"].Value != null)
                                TempuserAccountControl = Convert.ToInt32(CN.Properties["userAccountControl"][0]);

                            user.UserID = CN.Guid;
                            user.SAMAccountName = CN.Properties["sAMAccountName"].Value == null ? "" : Convert.ToString(CN.Properties["sAMAccountName"].Value);
                            user.Displayname = CN.Properties["displayname"].Value == null ? "" : Convert.ToString(CN.Properties["displayname"].Value);
                            user.DistinguishedName = CN.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(CN.Properties["distinguishedName"].Value);
                            user.UserPrincipalName = CN.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(CN.Properties["userPrincipalName"].Value);
                            user.Mail = CN.Properties["mail"].Value == null ? "" : Convert.ToString(CN.Properties["mail"].Value);

                            user.PasswordExpired = (TempuserAccountControl & 8388608) != 0 ? true : false;
                            ActiveDs.IADsUser native = (ActiveDs.IADsUser)CN.NativeObject;
                            user.PasswordExpireTime = native == null ? Convert.ToDateTime("1900-01-01") : native.PasswordExpirationDate;
                            //user.PasswordExpireTime = DateTime.Parse(CN.InvokeGet("PasswordExpirationDate").ToString());
                            user.PasswordNerverExpire = (TempuserAccountControl & 65536) != 0 ? true : false;

                            user.WhenChanged = CN.Properties["whenChanged"].Value == null ? Convert.ToDateTime("1900-01-01") : DateTime.Parse(Convert.ToString(CN.Properties["whenChanged"].Value)).ToLocalTime();
                            user.WhenCreated = CN.Properties["whenCreated"].Value == null ? Convert.ToDateTime("1900-01-01") : DateTime.Parse(Convert.ToString(CN.Properties["whenCreated"].Value)).ToLocalTime();

                            user.IsDisable = (TempuserAccountControl & 2) != 0 ? true : false;


                            long Templastlogon = 0;
                            if (CN.Properties["lastLogonTimestamp"].Value != null)
                                Templastlogon = GetLongValue((IADsLargeInteger)CN.Properties["lastLogonTimestamp"].Value);
                            user.LastLogon = Templastlogon > 0 ? DateTime.FromFileTimeUtc(Templastlogon).AddHours(8) : Convert.ToDateTime("1900-01-01");
                            #region 从两台AD取lastlogon
                            #endregion

                            //AD用户 与 部门不从AD属性中读取
                            //DirectoryEntry TempParent = CN.Parent;
                            //string strTempParentdistinguishedName = TempParent.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(TempParent.Properties["distinguishedName"].Value);

                            //string strCompany = string.Empty;
                            //string strDepartment = string.Empty;
                            //string strSetUserCompanyAndDepartmentError = string.Empty;
                            //if (SetUserCompanyAndDepartment(strTempParentdistinguishedName, out strCompany, out strDepartment, out strSetUserCompanyAndDepartmentError))
                            //{
                            //    //更改AD该用户的属性
                            //    CN.Properties["company"].Value = strCompany;
                            //    CN.Properties["department"].Value = strDepartment;

                            //    CN.CommitChanges();
                            //}
                            //else
                            //{
                            //    Log4netHelper.Error(string.Format("SetUserCompanyAndDepartment 设置User:{0}公司/部门属性 Error:{1} ", user.DistinguishedName, strSetUserCompanyAndDepartmentError));
                            //}
                            userADlist.Add(user);
                        }
                        CN.Close();
                        CN.Dispose();
                    }
                }

                bResult = true;
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("SearchUserByOU Exception: " + ex.ToString());
            }
            finally
            {
            }

            return bResult;
        }
        
        [ComImport]
        [Guid("9068270B-0939-11D1-8BE1-00C04FD8D503")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        internal interface IADsLargeInteger
        {
            [DispId(0x00000002)]
            int HighPart { get; set; }
            [DispId(0x00000003)]
            int LowPart { get; set; }
        }

        internal static long GetLongValue(IADsLargeInteger value)
        {
            return (long)(((ulong)value.HighPart << 32) + (ulong)value.LowPart);
        }
        
        /// <summary>
        /// 同步系统邮件个数
        /// </summary>
        private void SyncSystemMailCount()
        {
            ExchangeWebservice.ManagerWebService service = new ExchangeWebservice.ManagerWebService();
            service.Timeout = -1;
            DateTime startTime = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 0, 0, 1);
            DateTime endTime = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 23, 23, 59);
            int iSendMailCount = 0;
            int iReceiveMailCount = 0;
            string strError = string.Empty;

            try
            {

                if (!service.GetServerMailCount(startTime, endTime, out iSendMailCount, out iReceiveMailCount, out strError))
                {
                    Log4netHelper.Error("获取系统邮件个数, error:" + strError);
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("获取系统邮件个数, Exception:" + ex.ToString());
            }
            
            try
            {
                CParameters parameters = new CParameters();
                SqlParameter paraDateTime = new SqlParameter("@TotalDate", startTime);
                parameters.Add(paraDateTime);
                SqlParameter paraSendCount = new SqlParameter("@SendMailCount", iSendMailCount);
                parameters.Add(paraSendCount);
                SqlParameter paraReceiveCount = new SqlParameter("@ReceiveMailCount", iReceiveMailCount);
                parameters.Add(paraReceiveCount);

                int iResult = 1;
                if (!m_db.ExcuteByTransaction(parameters, "dbo.prc_InsertADSystemMailCount", out iResult, out strError))
                {
                    Log4netHelper.Error("系统邮件个数存入数据库, error:" + strError);
                }

                if (iResult == -1)
                {
                    Log4netHelper.Error("系统邮件个数存入数据库, error: 调用存储过程 spInsertADSystemMailCount 返回值-1");
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("系统邮件个数存入数据库, Exception:" + ex.ToString());
            }

        }

        /// <summary>
        /// 同步用户邮件个数
        /// </summary>
        private void SyncUserMailCount()
        {
            ExchangeWebservice.ManagerWebService service = new ExchangeWebservice.ManagerWebService();
            service.Timeout = -1;
            DateTime startTime = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 0, 0, 1);
            DateTime endTime = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 23, 23, 59);
            string strError = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                //读取ad user
                string strsql = "select sAMAccountName,UserPrincipalName,Mail from dbo.T_Base_ADUser";

                if (!m_db.ExcuteByDataAdapter(strsql, out ds, out strError))
                {
                    Log4netHelper.Error("获取User信息失败, Error:" + strError);
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("获取User信息失败, Exception:" + ex.ToString());
            }


            //循环调用接口同步用户邮件个数
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    int iSendMailCount = 0;
                    int iReceiveMailCount = 0;
                    try
                    {
                        if (!service.GetUserMailCount(startTime, endTime, Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"]), out iSendMailCount, out iReceiveMailCount, out strError))
                        {
                            Log4netHelper.Error(string.Format("获取用户邮件个数, sAMAccountName:{0} UserPrincipalName:{1} error:{2}", Convert.ToString(ds.Tables[0].Rows[i]["sAMAccountName"]), Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"]), strError));
                        }
                    }
                    catch (Exception ex)
                    {

                        Log4netHelper.Error(string.Format("获取用户邮件个数, sAMAccountName:{0} UserPrincipalName:{1} Exception:{2}", Convert.ToString(ds.Tables[0].Rows[i]["sAMAccountName"]), Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"]), ex.ToString()));
                    }

                    try
                    {
                        //添加到数据库 
                        CParameters parameters = new CParameters();
                        SqlParameter parasAMAccountName = new SqlParameter("@sAMAccountName", Convert.ToString(ds.Tables[0].Rows[i]["sAMAccountName"]));
                        parameters.Add(parasAMAccountName);
                        SqlParameter parasMail = new SqlParameter("@Mail", Convert.ToString(ds.Tables[0].Rows[i]["Mail"]));
                        parameters.Add(parasMail);
                        SqlParameter paraDateTime = new SqlParameter("@TotalDate", startTime);
                        parameters.Add(paraDateTime);
                        SqlParameter paraSendCount = new SqlParameter("@SendMailCount", iSendMailCount);
                        parameters.Add(paraSendCount);
                        SqlParameter paraReceiveCount = new SqlParameter("@ReceiveMailCount", iReceiveMailCount);
                        parameters.Add(paraReceiveCount);

                        int iResult = 1;
                        if (!m_db.ExcuteByTransaction(parameters, "dbo.prc_InsertADUserMailCount", out iResult, out strError))
                        {
                            Log4netHelper.Error(string.Format("用户邮件个数存入数据库, sAMAccountName:{0} UserPrincipalName:{1} error:{2}", Convert.ToString(ds.Tables[0].Rows[i]["sAMAccountName"]), Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"]), strError));
                        }

                        if (iResult == -1)
                        {
                            Log4netHelper.Error(string.Format("用户邮件个数存入数据库, sAMAccountName:{0} UserPrincipalName:{1} 调用存储过程spInsertADUserMailCount返回值-1", Convert.ToString(ds.Tables[0].Rows[i]["sAMAccountName"]), Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"])));
                        }
                    }
                    catch (Exception ex)
                    {

                        Log4netHelper.Error(string.Format("用户邮件个数存入数据库, sAMAccountName:{0} UserPrincipalName:{1} Exception:{2}", Convert.ToString(ds.Tables[0].Rows[i]["sAMAccountName"]), Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"]), ex.ToString()));
                    }
                }
            }
        }

        private void SyncUserMailSize()
        {
            ExchangeWebservice.ManagerWebService service = new ExchangeWebservice.ManagerWebService();
            service.Timeout = -1;
            string strError = string.Empty;
            DataSet ds = new DataSet();

            try
            {
                do
                {
                    //读取ad user
                    string strsql = "select sAMAccountName,UserPrincipalName from dbo.T_Base_ADUser";

                    if (!m_db.ExcuteByDataAdapter(strsql, out ds, out strError))
                    {
                        Log4netHelper.Error("获取User信息失败, Error:" + strError);
                        break;
                    }

                    //循环调用接口同步用户邮件个数
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //GB
                            long mailSize = 0;
                            long usedMailSize = 0;
                            string databaseName = string.Empty;
                            string sizename = string.Empty;
                            string usedsizename = string.Empty;

                            if (!service.GetUserMailSize(Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"]), out sizename, out mailSize, out usedsizename, out usedMailSize, out databaseName, out strError))
                            {
                                Log4netHelper.Error(string.Format("获取用户邮箱空间, sAMAccountName:{0} UserPrincipalName:{1} error:{2}", Convert.ToString(ds.Tables[0].Rows[i]["sAMAccountName"]), Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"]), strError));
                                continue;
                            }
                            //添加到数据库 
                            CParameters parameters = new CParameters();
                            SqlParameter parasAMAccountName = new SqlParameter("@sAMAccountName", Convert.ToString(ds.Tables[0].Rows[i]["sAMAccountName"]));
                            parameters.Add(parasAMAccountName);
                            SqlParameter paraSizeName = new SqlParameter("@sizename", sizename);
                            parameters.Add(paraSizeName);
                            SqlParameter paraMailSize = new SqlParameter("@mailSize", mailSize);
                            parameters.Add(paraMailSize);
                            SqlParameter paraUsedSizeName = new SqlParameter("@usedsizename", usedsizename);
                            parameters.Add(paraUsedSizeName);
                            SqlParameter paraUsedMailSize = new SqlParameter("@usedMailSize", usedMailSize);
                            parameters.Add(paraUsedMailSize);
                            SqlParameter paraDatabaseName = new SqlParameter("@databaseName", databaseName);
                            parameters.Add(paraDatabaseName);

                            int iResult = 1;
                            if (!m_db.ExcuteByTransaction(parameters, "dbo.prc_UpdateADUserMailSize", out iResult, out strError))
                            {
                                Log4netHelper.Error(string.Format("用户已用空间存入数据库, sAMAccountName:{0} UserPrincipalName:{1} error:{2}", Convert.ToString(ds.Tables[0].Rows[i]["sAMAccountName"]), Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"]), strError));
                                continue;
                            }
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                Log4netHelper.Error($"同步用户已用空间异常, Exception：{ex.ToString()}");
            }
        }
        
        /// <summary>
        /// 截取用户的公司/部门属性
        /// </summary>
        /// <param name="strParentdistinguishedName"></param>
        /// <param name="strCompany"></param>
        /// <param name="strDepartment"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        private bool SetUserCompanyAndDepartment(string strParentdistinguishedName, out string strCompany, out string strDepartment, out string strError)
        {
            strCompany = string.Empty;
            strDepartment = string.Empty;
            strError = string.Empty;
            bool bResult = true;
            try
            {
                do
                {
                    if (string.IsNullOrEmpty(strParentdistinguishedName))
                    {
                        strError = "SetUserCompanyAndDepartment strParentdistinguishedName is Empty";
                        bResult = false;
                        break;
                    }

                    //截取OU数据
                    string[] array = strParentdistinguishedName.Split(new string[] { "OU=" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = array[i].Trim(',');
                        if (array[i].Contains("DC="))
                        {
                            array[i] = array[i].Remove(array[i].IndexOf("DC=")).Trim(',');
                        }
                    }

                    if (array == null || array.Length == 0)
                    {
                        strError = string.Format("SetUserCompanyAndDepartment ParentdistinguishedName:{0} 不符合截取规范", strParentdistinguishedName);
                        bResult = false;
                        break;
                    }

                    //判断array元素是否在指定OU列表内

                    if (strNamedOUArray != null && strNamedOUArray.Count > 0)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            strCompany = array[i];
                            if (strNamedOUArray.Contains(strCompany))
                            {
                                break;
                            }

                            strCompany = string.Empty;
                        }
                    }

                    //若不存在则按照一般逻辑处理
                    if (string.IsNullOrEmpty(strCompany))
                    {
                        strCompany = array.Length > 1 ? array[array.Length - 2] : array[array.Length - 1];
                    }

                    strDepartment = array[0];

                    //剔除英文
                    int CompanyIndex = 0;
                    while (CompanyIndex < strCompany.Length)
                    {
                        if (!CharIsLetter(strCompany[CompanyIndex]))
                        {
                            strCompany = strCompany.Substring(CompanyIndex);
                            break;
                        }
                        CompanyIndex++;
                    }
                    int DepartmentIndex = 0;
                    while (DepartmentIndex < strDepartment.Length)
                    {
                        if (!CharIsLetter(strDepartment[DepartmentIndex]))
                        {
                            strDepartment = strDepartment.Substring(DepartmentIndex);
                            break;
                        }
                        DepartmentIndex++;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("SetUserCompanyAndDepartMent Exception:{0}", ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        public bool SearchProfessionalGroupOus(DirectoryEntry rootEntry, out List<string> items, out string strError)
        {
            bool bResult = true;
            strError = string.Empty;
            items = new List<string>();

            try
            {
                do
                {
                    DirectorySearcher deSearch = new DirectorySearcher(rootEntry);
                    deSearch.SearchRoot = rootEntry;
                    string strFilter = "(&(|(objectClass=organizationalUnit))(!objectClass=computer)(st=True))";
                    deSearch.Filter = strFilter;
                    deSearch.SearchScope = SearchScope.Subtree;
                    SearchResultCollection results = deSearch.FindAll();

                    if (results != null && results.Count > 0)
                    {
                        foreach (SearchResult result in results)
                        {
                            DirectoryEntry entry = result.GetDirectoryEntry();
                            items.Add(Convert.ToString(entry.Properties["name"].Value));
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                strError = string.Format("SearchProfessionalGroupOus 搜索所有对象 Exception:{0}", ex.ToString());
                bResult = false;
            }

            return bResult;
        }

        private bool CharIsLetter(char ch)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
