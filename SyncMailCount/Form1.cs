using Common;
using DBUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncMailCount
{
    public partial class Form1 : Form
    {
        private CBaseDB m_db = new CBaseDB(Conntection.strConnection);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime startDate = new DateTime(2020, 9, 13);
            DateTime endDate = DateTime.Now;
            for (DateTime dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                SyncSystemMailCount(dt, dt.AddDays(1).AddSeconds(-1));
                SyncUserMailCount(dt, dt.AddDays(1).AddSeconds(-1));
            }
        }

        private void SyncSystemMailCount(DateTime startTime,DateTime endTime)
        {
            ExchangeWebservice.ManagerWebService service = new ExchangeWebservice.ManagerWebService();
            service.Timeout = -1;
            //DateTime startTime = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 0, 0, 1);
            //DateTime endTime = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 23, 23, 59);
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

        private void SyncUserMailCount(DateTime startTime, DateTime endTime)
        {
            ExchangeWebservice.ManagerWebService service = new ExchangeWebservice.ManagerWebService();
            service.Timeout = -1;
            //DateTime startTime = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 0, 0, 1);
            //DateTime endTime = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 23, 23, 59);
            string strError = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                //读取ad user
                string strsql = "select sAMAccountName,UserPrincipalName from dbo.T_Base_ADUser";

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
                        //SqlParameter paraUserPrincipalName = new SqlParameter("@UserPrincipalName", Convert.ToString(ds.Tables[0].Rows[i]["UserPrincipalName"]));
                        //parameters.Add(paraUserPrincipalName);
                        SqlParameter paraStartTime = new SqlParameter("@TotalDate", startTime);
                        parameters.Add(paraStartTime);
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
    }
}
