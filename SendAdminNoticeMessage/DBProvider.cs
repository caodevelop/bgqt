using Common;
using DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendAdminNoticeMessage
{
    public class DBProvider
    {

        private string _ProviderName = "DBProvider";
        public bool GetSendMessageUsers(Guid transactionid, out List<UserInfo> newusers, out List<UserInfo> leaveusers, out string error)
        {
            bool bResult = true;
            newusers = new List<UserInfo>();
            leaveusers = new List<UserInfo>();
            error = string.Empty;
            string paramstr = string.Empty;
            string funname = "GetSendMessageUsers";

            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetSendMessageUsers]", out ds, out error))
                    {
                        LoggerHelper.Error($"{funname}执行prc_GetSendMessageUsers异常", paramstr, error, transactionid);
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
                                case 0:
                                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                    {
                                        DataRow dr = ds.Tables[1].Rows[i];
                                        UserInfo info = new UserInfo();
                                        info.EMPLID = Convert.ToString(dr["EMPLID"]);
                                        info.NAME = Convert.ToString(dr["NAME"]);
                                        info.PHONE1 = Convert.ToString(dr["PHONE1"]);
                                        info.userPrincipalName = Convert.ToString(dr["userPrincipalName"]);
                                        info.Entry_DT = Convert.ToString(dr["Entry_DT"]);
                                        newusers.Add(info);
                                    }
                                    for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                                    {
                                        DataRow dr = ds.Tables[2].Rows[i];
                                        UserInfo info = new UserInfo();
                                        info.EMPLID = Convert.ToString(dr["EMPLID"]);
                                        info.NAME = Convert.ToString(dr["NAME"]);
                                        info.PHONE1 = Convert.ToString(dr["PHONE1"]);
                                        info.userPrincipalName = Convert.ToString(dr["userPrincipalName"]);
                                        info.TERMINATION_DT = Convert.ToString(dr["TERMINATION_DT"]);
                                        leaveusers.Add(info);
                                    }
                                    bResult = true;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error = "数据库异常";
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error = "数据库异常";
                        LoggerHelper.Error($"{_ProviderName}调用{funname}异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error = "数据库异常";
                LoggerHelper.Error($"{_ProviderName}调用{funname}异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }
    }
}
