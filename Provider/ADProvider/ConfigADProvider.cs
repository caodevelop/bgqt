using Common;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Provider.ADProvider
{
    public class ConfigADProvider
    {
        public static string GetADPathByLdap(string ldap)
        {
            string dc = Common.ConfigHelper.ConfigInstance["AD_DC"];
            if (!string.IsNullOrEmpty(ldap) && !string.IsNullOrEmpty(dc))
                return string.Format("LDAP://{0}/{1}", dc, ldap);
            return string.Empty;
        }

        public static string GetRootADPath()
        {
            string dc = Common.ConfigHelper.ConfigInstance["AD_DC"];
            string rootLdap = Common.ConfigHelper.ConfigInstance["AD_RootLDAP"];
            if (!string.IsNullOrEmpty(rootLdap) && !string.IsNullOrEmpty(dc))
                return string.Format("LDAP://{0}/{1}", dc, rootLdap);

            return string.Empty;
        }

        public static string GetDCPath()
        {
            string dc = string.Format("LDAP://{0}/", Common.ConfigHelper.ConfigInstance["AD_DC"]);
            return dc;
        }

        public static string GetCompanyADRootPath()
        {
            string dc = Common.ConfigHelper.ConfigInstance["AD_DC"];
            string CompanyLdap = Common.ConfigHelper.ConfigInstance["AD_DefaultCompany"]; ;
            if (!string.IsNullOrEmpty(CompanyLdap) && !string.IsNullOrEmpty(dc))
                return string.Format("LDAP://{0}/{1}", dc, CompanyLdap);
            return string.Empty;
        }

        public static string GetCompanyOuDistinguishedName()
        {
            string CompanyOu = Common.ConfigHelper.ConfigInstance["AD_DefaultCompany"];
            return CompanyOu;
        }

        public static string GetPublicOuLdap()
        {
            string dc = Common.ConfigHelper.ConfigInstance["AD_DC"];
            string PublicOuLdap = Common.ConfigHelper.ConfigInstance["AD_PublicOU"];
           
            if (!string.IsNullOrEmpty(PublicOuLdap) && !string.IsNullOrEmpty(dc))
                return string.Format("LDAP://{0}/{1}", dc, PublicOuLdap);

            return string.Empty;
        }

        public static string GetDepartmentOuLdap()
        {
            string dc = Common.ConfigHelper.ConfigInstance["AD_DC"];
            string DepartmentOuLdap = Common.ConfigHelper.ConfigInstance["AD_DepartmentOU"];
            if (!string.IsNullOrEmpty(DepartmentOuLdap) && !string.IsNullOrEmpty(dc))
                return string.Format("LDAP://{0}/{1}", dc, DepartmentOuLdap);

            return string.Empty;
        }

        public static string GetPublicOuDistinguishedName()
        {
            string PublicOuLdap = Common.ConfigHelper.ConfigInstance["AD_PublicOU"];
            return PublicOuLdap;
        }

        public static string GetStaticGroupOuLdap()
        {
            string dc = Common.ConfigHelper.ConfigInstance["AD_DC"];
            string StaticGroupOuLdap = Common.ConfigHelper.ConfigInstance["AD_StaticGroupOU"];
            if (!string.IsNullOrEmpty(StaticGroupOuLdap) && !string.IsNullOrEmpty(dc))
                return string.Format("LDAP://{0}/{1}", dc, StaticGroupOuLdap);
            return string.Empty;
        }

        public static string GetStaticGroupOuDistinguishedName()
        {
            string StaticGroupOuLdap = Common.ConfigHelper.ConfigInstance["AD_StaticGroupOU"];
            return StaticGroupOuLdap;
        }

        public static string GetRecycleOuLdap()
        {
            string dc = Common.ConfigHelper.ConfigInstance["AD_DC"];
            string RecycleOuLdap = Common.ConfigHelper.ConfigInstance["AD_RecycleOu"];

            if (!string.IsNullOrEmpty(RecycleOuLdap) && !string.IsNullOrEmpty(dc))
                return string.Format("LDAP://{0}/{1}", dc, RecycleOuLdap);

            return string.Empty;
        }

        public static string GetADRecycleOuLdapByLdap(string ldap)
        {
            string strResult = string.Empty;
            string RecycleOuLdap = Common.ConfigHelper.ConfigInstance["AD_RecycleOu"];
            string[] TempArray = ldap.Split(new string[] { "OU=" }, StringSplitOptions.RemoveEmptyEntries);

            //截取离职Ou路径
            if (TempArray.Length >= 2)
            {
                strResult = ldap.Substring(0, ldap.LastIndexOf("OU")) + RecycleOuLdap;//"OU=" + TempArray[TempArray.Length - 2] + RecycleOuLdap;
            }
            else
            {
                strResult = RecycleOuLdap;
            }
            return strResult;
        }

        public static string GetADDomain()
        {
            string ADDomain = Common.ConfigHelper.ConfigInstance["AD_Domain"];
            return ADDomain;
        }

        public static string GetADDC()
        {
            string ADDC = Common.ConfigHelper.ConfigInstance["AD_DC"];
            return ADDC;
        }

        public static string GetADAdmin()
        {
            string ADDomain = Common.ConfigHelper.ConfigInstance["AD_Admin"];
            return ADDomain;
        }

        public static string GetADPassword()
        {
            string ADPassword = Common.ConfigHelper.ConfigInstance["AD_Password"];
            return ADPassword;
        }

        public static string GetExchangePSUri()
        {
            string ExchangePSUri = Common.ConfigHelper.ConfigInstance["ExchangePS_Uri"];
            return ExchangePSUri;
        }

        public static string GetOWAMailboxPolicy()
        {
            string ExchangePSUri = Common.ConfigHelper.ConfigInstance["OWAMailboxPolicy"];
            return ExchangePSUri;
        }

        public static string GetHab_add()
        {
            string Add_Hab = Common.ConfigHelper.ConfigInstance["Add_Hab"];
            return Add_Hab;
        }

        public static string GetDefaultMailboxDB()
        {
            string DefaultMailboxDB = Common.ConfigHelper.ConfigInstance["DefaultMailboxDB"];
            return DefaultMailboxDB;
        }

        public static string GetRecycleOuDistinguishedName()
        {
            string RecycleOu = Common.ConfigHelper.ConfigInstance["AD_RecycleOu"];
            return RecycleOu;
        }

        public static string GetAdministratorAccount()
        {
            string AdministratorAccount = Common.ConfigHelper.ConfigInstance["AD_AdminAccount"];
            return AdministratorAccount;
        }

        public static string GetHab_CustomName()
        {
            string Hab_CustomName = Common.ConfigHelper.ConfigInstance["Hab_CustomName"];
            return Hab_CustomName;
        }

        public static string GetHab_CustomValue()
        {
            string Hab_CustomValue = Common.ConfigHelper.ConfigInstance["Hab_CustomValue"];
            return Hab_CustomValue;
        }

        public static string GetDefaultPassword()
        {
            string DefaultPassword = Common.ConfigHelper.ConfigInstance["DefaultPassword"];
            return DefaultPassword;
        }

        public static string GetExchange_OAB()
        {
            string strExchange_OAB = Common.ConfigHelper.ConfigInstance["Hab_CustomValue"];
            return strExchange_OAB;
        }

        public static string GetThreadSleep()
        {
            string strThreadSleepTime = Common.ConfigHelper.ConfigInstance["Thread_SleepTime"];
            return strThreadSleepTime;
        }

        public static bool SecurityVerification()
        {
            bool bResult = true;
            string strError = string.Empty;
            
            do
            {
                string admin = Common.ConfigHelper.ConfigInstance["AD_Admin"];
                string password = Common.ConfigHelper.ConfigInstance["AD_Password"];

                bResult = SecurityVerification(admin, password, out strError);
            } while (false);

            return bResult;
        }

        public static bool SecurityVerification(string account, string pwd, out string strError)
        {
            //模拟身份安全 begin
            SubmitSecurity subSecurity = new SubmitSecurity();
            string domain = Common.ConfigHelper.ConfigInstance["AD_Domain"];
           
            strError = string.Empty;
            if (!string.IsNullOrEmpty(domain) && !string.IsNullOrEmpty(pwd) && !string.IsNullOrEmpty(account))
            {
                if (!subSecurity.impersonateValidUser(account, domain, pwd))
                {
                    strError = "用户名或密码错误,验证失败";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            strError = "传值失败";
            strError = string.Empty;
            return false;
        }


        /// <summary>
        /// 将distinguishedName转换为规范名称 如OU=xxx,OU=yyy,DC=zzz,DC=com 转换后卫zzz.com/yyy/xxx
        /// </summary>
        /// <param name="strDistinguishedName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static string DistinguishedNameTransformationCriterionName(string strDistinguishedName)
        {
            string strResult = string.Empty;
            string strTemp = strDistinguishedName;

            try
            {
                string[] array = strTemp.Split(new string[] { "DC=" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 1; i < array.Length; i++)
                {
                    strResult += array[i].Trim(new char[] { ',' }) + ".";
                }
                strResult = strResult.TrimEnd(new char[] { '.' });

                strTemp = array[0];

                string[] strArray = strTemp.Split(',');

                for (int i = strArray.Length - 1; i >= 0; i--)
                {
                    strResult += strArray[i].Substring(strArray[i].IndexOf("=") + 1) + "/";
                }

                strResult = strResult.TrimEnd(new char[] { '/' });
            }
            catch (Exception ex)
            {
                strResult = string.Empty;
                LoggerHelper.Error("DistinguishedNameTransformationCriterionName", "DistinguishedName:" + strDistinguishedName, "Exception:" + ex.ToString(), Guid.NewGuid());
            }

            return strResult;
        }

        /// <summary>
        /// 将distinguishedName转换为路径名称 如OU=xxx,OU=yyy,DC=zzz,DC=com 转换后卫\yyy\xxx
        /// </summary>
        /// <param name="strDistinguishedName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static string DistinguishedNameTransformationFolderName(string strDistinguishedName, out string strError)
        {
            string strResult = string.Empty;
            string strTemp = strDistinguishedName;
            strError = string.Empty;

            try
            {
                string[] array = strTemp.Split(new string[] { "DC=" }, StringSplitOptions.RemoveEmptyEntries);

                strTemp = array[0];

                string[] strArray = strTemp.Split(',');

                for (int i = strArray.Length - 1; i >= 0; i--)
                {
                    strResult += strArray[i].Substring(strArray[i].IndexOf("=") + 1) + "\\";
                }

                strResult = strResult.TrimEnd(new char[] { '\\' });
            }
            catch (Exception ex)
            {
                strResult = string.Empty;
                strError = string.Format("Config_ADManage.DistinguishedNameTransformationFolderName DistinguishedName:{0} Transformation Exception:{1}", strDistinguishedName, ex.ToString());
            }

            return strResult;
        }

        /// <summary>
        ///  将规范名称转换为distinguishedName
        /// </summary>
        /// <param name="strCriterionName"></param>
        /// <param name="strDistinguishedName"></param>
        /// <param name="strName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static bool CriterionNameTransformationDistinguishedName(string strCriterionName, out string strDistinguishedName, out string strName, out string strError)
        {
            strDistinguishedName = string.Empty;
            strName = string.Empty;
            bool bResult = true;
            strError = string.Empty;

            try
            {
                string TempCriterionName = strCriterionName;
                string[] TempArray = TempCriterionName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = TempArray.Length - 1; i >= 0; i--)
                {
                    if (i != 0)
                    {
                        strDistinguishedName += "OU=" + TempArray[i] + ",";
                    }
                    else
                    {
                        string[] TempArray1 = TempArray[i].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                        for (int m = 0; m < TempArray1.Length; m++)
                        {
                            strDistinguishedName += "DC=" + TempArray1[m] + ",";
                        }
                    }
                }

                strDistinguishedName = strDistinguishedName.TrimEnd(new char[] { ',' });
                strName = TempArray[TempArray.Length - 1];
            }
            catch (Exception ex)
            {
                strError = string.Format("Config_ADManage.CriterionNameTransformationDistinguishedName CriterionName:{0} Transformation Exception:{1}", strCriterionName, ex.ToString());
                bResult = false;
            }

            return bResult;
        }
        
        public static bool CharIsLetter(char ch)
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

    public class SubmitSecurity
    {
        public SubmitSecurity()
        {
        }

        //模拟安全代码 变量声明 begin
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;
        //protected System.Web.UI.HtmlControls.HtmlInputText TB_Doman;

        WindowsImpersonationContext impersonationContext;

        [DllImport("advapi32.dll")]
        public static extern int LogonUserA(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        public static extern bool CloseHandle(IntPtr handle);

        //模拟安全 函数 begin
        public bool impersonateValidUser(String userName, String domain, String password)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                    LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                        if (impersonationContext != null)
                        {
                            CloseHandle(token);
                            CloseHandle(tokenDuplicate);
                            return true;
                        }
                    }
                }
            }
            if (token != IntPtr.Zero)
                CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                CloseHandle(tokenDuplicate);
            return false;
        }
    }
}
