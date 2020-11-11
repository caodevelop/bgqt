using Common;
using DBUtility;
using Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.DBProvider
{
    public class SystemReportDBProvider
    {
        public bool GetNoLoginUsers(Guid transactionid, AdminInfo admin, int curpage, int pagesize, DateTime start, DateTime end, string searchstr, out BaseListInfo lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BaseListInfo();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraPageIndex = new SqlParameter("@PageIndex", curpage);
                paras.Add(paraPageIndex);
                SqlParameter paraPageSize = new SqlParameter("@PageSize", pagesize);
                paras.Add(paraPageSize);
                SqlParameter paraSearchstr = new SqlParameter("@Searchstr", $"%{searchstr}%");
                paras.Add(paraSearchstr);
                SqlParameter paraStartTime = new SqlParameter("@StartTime", start);
                paras.Add(paraStartTime);
                SqlParameter paraEndTime = new SqlParameter("@EndTime", end);
                paras.Add(paraEndTime);
                SqlParameter paraRecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);
                paraRecordCount.Direction = ParameterDirection.Output;
                paras.Add(paraRecordCount);
                SqlParameter paraPageCount = new SqlParameter("@PageCount", SqlDbType.Int);
                paraPageCount.Direction = ParameterDirection.Output;
                paras.Add(paraPageCount);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetNoLoginUsers]", out ds, out strError))
                    {
                        strError = "prc_GetNoLoginUsers数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    lists.RecordCount = (int)paraRecordCount.Value;
                    lists.PageCount = (int)paraPageCount.Value;

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            SystemReportInfo info = new SystemReportInfo();
                            info.UserID = Guid.Parse(Convert.ToString(sdr["UserID"]));
                            info.UserAccount = Convert.ToString(sdr["UserPrincipalName"]);
                            info.DisplayName = Convert.ToString(sdr["displayName"]);
                            info.CreateTime = Convert.ToDateTime(sdr["WhenCreated"]);
                            info.LastLoginTime = Convert.ToDateTime(sdr["LastLogon"]);
                            info.PasswordExpireTime = Convert.ToDateTime(sdr["PasswordExpireTime"]);
                            info.DistinguishedName = Convert.ToString(sdr["distinguishedName"]);
                            info.UserSatus = Convert.ToBoolean(sdr["IsDisable"]) == true ? State.Disable : State.Enable;
                            if (Convert.ToBoolean(sdr["PasswordNerverExpire"]) == true)
                            {
                                info.Type = PasswordType.NerverExpire;
                            }
                            else if (Convert.ToBoolean(sdr["PasswordExpired"]) == true)
                            {
                                info.Type = PasswordType.Expired;
                            }
                            else
                            {
                                info.Type = PasswordType.WillExpire;
                            }

                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetMailDataBaseList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailDataBaseDBProvider调用prc_GetMailDataBaseList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetPasswordStateUsers(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, PasswordType type, out BaseListInfo lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BaseListInfo();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraPageIndex = new SqlParameter("@PageIndex", curpage);
                paras.Add(paraPageIndex);
                SqlParameter paraPageSize = new SqlParameter("@PageSize", pagesize);
                paras.Add(paraPageSize);
                SqlParameter paraType = new SqlParameter("@type", type);
                paras.Add(paraType);
                SqlParameter paraSearchstr = new SqlParameter("@Searchstr", $"%{searchstr}%");
                paras.Add(paraSearchstr);
                SqlParameter paraRecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);
                paraRecordCount.Direction = ParameterDirection.Output;
                paras.Add(paraRecordCount);
                SqlParameter paraPageCount = new SqlParameter("@PageCount", SqlDbType.Int);
                paraPageCount.Direction = ParameterDirection.Output;
                paras.Add(paraPageCount);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetPasswordStateUsers]", out ds, out strError))
                    {
                        strError = "prc_GetPasswordStateUsers数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    lists.RecordCount = (int)paraRecordCount.Value;
                    lists.PageCount = (int)paraPageCount.Value;

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            SystemReportInfo info = new SystemReportInfo();
                            info.UserID = Guid.Parse(Convert.ToString(sdr["UserID"]));
                            info.UserAccount = Convert.ToString(sdr["UserPrincipalName"]);
                            info.DisplayName = Convert.ToString(sdr["displayName"]);
                            info.CreateTime = Convert.ToDateTime(sdr["WhenCreated"]);
                            info.LastLoginTime = Convert.ToDateTime(sdr["LastLogon"]);
                            info.PasswordExpireTime = Convert.ToDateTime(sdr["PasswordExpireTime"]);
                            info.DistinguishedName = Convert.ToString(sdr["distinguishedName"]);
                            info.UserSatus = Convert.ToBoolean(sdr["IsDisable"]) == true ? State.Disable : State.Enable;
                            if (Convert.ToBoolean(sdr["PasswordNerverExpire"]) == true)
                            {
                                info.Type = PasswordType.NerverExpire;
                            }
                            else if (Convert.ToBoolean(sdr["PasswordExpired"]) == true)
                            {
                                info.Type = PasswordType.Expired;
                            }
                            else
                            {
                                info.Type = PasswordType.WillExpire;
                            }

                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetPasswordStateUsers失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用prc_GetPasswordStateUsers异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetDisableUsers(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out BaseListInfo lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BaseListInfo();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraPageIndex = new SqlParameter("@PageIndex", curpage);
                paras.Add(paraPageIndex);
                SqlParameter paraPageSize = new SqlParameter("@PageSize", pagesize);
                paras.Add(paraPageSize);
                SqlParameter paraSearchstr = new SqlParameter("@Searchstr", $"%{searchstr}%");
                paras.Add(paraSearchstr);
                SqlParameter paraRecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);
                paraRecordCount.Direction = ParameterDirection.Output;
                paras.Add(paraRecordCount);
                SqlParameter paraPageCount = new SqlParameter("@PageCount", SqlDbType.Int);
                paraPageCount.Direction = ParameterDirection.Output;
                paras.Add(paraPageCount);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetDisableUsers]", out ds, out strError))
                    {
                        strError = "prc_GetDisableUsers数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    lists.RecordCount = (int)paraRecordCount.Value;
                    lists.PageCount = (int)paraPageCount.Value;

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            SystemReportInfo info = new SystemReportInfo();
                            info.UserID = Guid.Parse(Convert.ToString(sdr["UserID"]));
                            info.UserAccount = Convert.ToString(sdr["UserPrincipalName"]);
                            info.DisplayName = Convert.ToString(sdr["displayName"]);
                            info.CreateTime = Convert.ToDateTime(sdr["WhenCreated"]);
                            info.LastLoginTime = Convert.ToDateTime(sdr["LastLogon"]);
                            info.PasswordExpireTime = Convert.ToDateTime(sdr["PasswordExpireTime"]);
                            info.UserSatus = Convert.ToBoolean(sdr["IsDisable"]) == true ? State.Disable : State.Enable;
                            info.DistinguishedName = Convert.ToString(sdr["distinguishedName"]);
                            if (Convert.ToBoolean(sdr["PasswordNerverExpire"]) == true)
                            {
                                info.Type = PasswordType.NerverExpire;
                            }
                            else if (Convert.ToBoolean(sdr["PasswordExpired"]) == true)
                            {
                                info.Type = PasswordType.Expired;
                            }
                            else
                            {
                                info.Type = PasswordType.WillExpire;
                            }

                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetDisableUsers失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用prc_GetDisableUsers异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetUserCreateTime(Guid transactionid, AdminInfo admin, int curpage, int pagesize, DateTime start, DateTime end, string searchstr, out BaseListInfo lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BaseListInfo();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraPageIndex = new SqlParameter("@PageIndex", curpage);
                paras.Add(paraPageIndex);
                SqlParameter paraPageSize = new SqlParameter("@PageSize", pagesize);
                paras.Add(paraPageSize);
                SqlParameter paraSearchstr = new SqlParameter("@Searchstr", $"%{searchstr}%");
                paras.Add(paraSearchstr);
                SqlParameter paraStartTime = new SqlParameter("@StartTime", start);
                paras.Add(paraStartTime);
                SqlParameter paraEndTime = new SqlParameter("@EndTime", end);
                paras.Add(paraEndTime);
                SqlParameter paraRecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);
                paraRecordCount.Direction = ParameterDirection.Output;
                paras.Add(paraRecordCount);
                SqlParameter paraPageCount = new SqlParameter("@PageCount", SqlDbType.Int);
                paraPageCount.Direction = ParameterDirection.Output;
                paras.Add(paraPageCount);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetUserCreateTime]", out ds, out strError))
                    {
                        strError = "prc_GetUserCreateTime数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    lists.RecordCount = (int)paraRecordCount.Value;
                    lists.PageCount = (int)paraPageCount.Value;

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            SystemReportInfo info = new SystemReportInfo();
                            info.UserID = Guid.Parse(Convert.ToString(sdr["UserID"]));
                            info.UserAccount = Convert.ToString(sdr["UserPrincipalName"]);
                            info.DisplayName = Convert.ToString(sdr["displayName"]);
                            info.CreateTime = Convert.ToDateTime(sdr["WhenCreated"]);
                            info.LastLoginTime = Convert.ToDateTime(sdr["LastLogon"]);
                            info.PasswordExpireTime = Convert.ToDateTime(sdr["PasswordExpireTime"]);
                            info.DistinguishedName = Convert.ToString(sdr["distinguishedName"]);
                            info.UserSatus = Convert.ToBoolean(sdr["IsDisable"]) == true ? State.Disable : State.Enable;
                            if (Convert.ToBoolean(sdr["PasswordNerverExpire"]) == true)
                            {
                                info.Type = PasswordType.NerverExpire;
                            }
                            else if (Convert.ToBoolean(sdr["PasswordExpired"]) == true)
                            {
                                info.Type = PasswordType.Expired;
                            }
                            else
                            {
                                info.Type = PasswordType.WillExpire;
                            }

                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetUserCreateTime失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用prc_GetUserCreateTime异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetSystemUserCount(Guid transactionid, AdminInfo admin, out int count, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            count = 0;
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetSystemUserCount]", out ds, out strError))
                    {
                        strError = "prc_GetSystemUserCount数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        count = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetSystemUserCount失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用prc_GetSystemUserCount异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetEntryAndDepartureUserCount(Guid transactionid, AdminInfo admin, out List<EntryAndDepartureUserInfo> entryAndDepartureUserInfos, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            entryAndDepartureUserInfos = new List<EntryAndDepartureUserInfo>();
            string strError = string.Empty;
            string paramstr = string.Empty;
            bool bResult = true;
            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetUserMonthData]", out ds, out strError))
                    {
                        strError = "prc_GetUserMonthData数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
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
                                    for (int i = 1; i <= 12; i++)
                                    {
                                        int entryCount = 0;
                                        int departureCount = 0;
                                        foreach (DataRow row in ds.Tables[1].Rows)
                                        {
                                            if (Convert.ToInt32(row["hm"]) == i)
                                            {
                                                entryCount = Convert.ToInt32(row["usercount"]);
                                            }
                                        }
                                        foreach (DataRow row in ds.Tables[2].Rows)
                                        {
                                            if (Convert.ToInt32(row["hm"]) == i)
                                            {
                                                departureCount = Convert.ToInt32(row["usercount"]);
                                            }
                                        }
                                        entryAndDepartureUserInfos.Add(new EntryAndDepartureUserInfo { month = i, EntryCount = entryCount, DepartureCount = departureCount });
                                    }
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotLoginRole;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RSystemReportDBProvider调用GetEntryAndDepartureUserCount异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("RSystemReportDBProvider调用GetEntryAndDepartureUserCount异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用prc_GetUserMonthData异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetUserUsedMailBoxList(Guid transactionid, AdminInfo admin, out List<UserUsedMailInfo> userUsedMailInfos, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            userUsedMailInfos = new List<UserUsedMailInfo>();
            int topcount = Convert.ToInt32(Common.ConfigHelper.ConfigInstance["TopCount"]);
            string strError = string.Empty;
            string paramstr = string.Empty;
            bool bResult = true;
            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetUserUsedMailBox]", out ds, out strError))
                    {
                        strError = "prc_GetUserUsedMailBox数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
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
                                        if (i < topcount)
                                        {
                                            UserUsedMailInfo info = new UserUsedMailInfo();
                                            info.displayname = Convert.ToString(dr["displayname"]);
                                            info.mail = Convert.ToString(dr["Mail"]);
                                            info.usedMailSize = Convert.ToInt64(dr["UsedMailSize"]) / 1024 / 1024;
                                            info.MailSize = Convert.ToInt64(dr["MailSize"]) / 1024 / 1024;
                                            info.UsedSizeName = Convert.ToString(dr["UsedSizeName"]);
                                            info.SizeName = Convert.ToString(dr["SizeName"]);
                                            info.usableMailSize = info.MailSize - info.usedMailSize;
                                            userUsedMailInfos.Add(info);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotLoginRole;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RSystemReportDBProvider调用GetUserUsedMailBoxList异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("RSystemReportDBProvider调用GetUserUsedMailBoxList异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用GetUserUsedMailBoxList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetMailBoxDBUsedList(Guid transactionid, AdminInfo admin, out List<MailBoxDBUsedInfo> mailBoxDBUsedInfos, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            mailBoxDBUsedInfos = new List<MailBoxDBUsedInfo>();
            string strError = string.Empty;
            string paramstr = string.Empty;
            bool bResult = true;
            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetMailBoxDBCount]", out ds, out strError))
                    {
                        strError = "prc_GetMailBoxDBCount数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
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
                                    foreach (DataRow dr in ds.Tables[1].Rows)
                                    {
                                        MailBoxDBUsedInfo info = new MailBoxDBUsedInfo();
                                        info.mailboxdbname = Convert.ToString(dr["databaseName"]);
                                        info.usercount = Convert.ToInt32(dr["usercount"]);
                                        info.usedmailsize = Convert.ToInt64(dr["usedmailsize"]) / 1024 / 1024;
                                        mailBoxDBUsedInfos.Add(info);
                                    }
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotLoginRole;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SystemReportDBProvider调用GetMailBoxDBUsedList异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("SystemReportDBProvider调用GetMailBoxDBUsedList异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用GetMailBoxDBUsedList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetSystemMailBoxCount(Guid transactionid, AdminInfo admin, out List<SystemMailCountInfo> systemMailCounts, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            systemMailCounts = new List<SystemMailCountInfo>();
            string strError = string.Empty;
            string paramstr = string.Empty;
            bool bResult = true;
            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetSystemMailBoxCount]", out ds, out strError))
                    {
                        strError = "prc_GetSystemMailBoxCount数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
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
                                    for (int i = 1; i <= 12; i++)
                                    {
                                        int SendCount = 0;
                                        int ReceiveCount = 0;
                                        int TotalCount = 0;
                                        foreach (DataRow row in ds.Tables[1].Rows)
                                        {
                                            if (Convert.ToInt32(row["month"]) == i)
                                            {
                                                SendCount = Convert.ToInt32(row["SendCount"]);
                                                ReceiveCount = Convert.ToInt32(row["ReceiveCount"]);
                                                TotalCount = SendCount + ReceiveCount;
                                            }
                                        }

                                        systemMailCounts.Add(new SystemMailCountInfo { month = i, SendMailCount = SendCount, ReceiveMailCount = ReceiveCount, TotalMailCount = TotalCount });
                                    }
                                    
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotLoginRole;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SystemReportDBProvider调用GetSystemMailBoxCount异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("SystemReportDBProvider调用GetSystemMailBoxCount异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用GetSystemMailBoxCount异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetCompanyMailCount(Guid transactionid, AdminInfo admin, out List<CompanyMailCountInfo> companyMailCounts, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            companyMailCounts = new List<CompanyMailCountInfo>();
            int topcount = Convert.ToInt32(Common.ConfigHelper.ConfigInstance["TopCount"]);
            string strError = string.Empty;
            string paramstr = string.Empty;
            bool bResult = true;
            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetCompanyMailCount]", out ds, out strError))
                    {
                        strError = "prc_GetCompanyMailCount数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
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
                                    int mailcount = 0;
                                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                    {
                                        DataRow dr = ds.Tables[1].Rows[i];
                                        if (i < topcount)
                                        {
                                            companyMailCounts.Add(new CompanyMailCountInfo { HPS_WORK_COMPANY =Convert.ToInt32(dr["HPS_WORK_COMPANY"]), HPS_WORK_COMP_DESC = Convert.ToString(dr["HPS_WORK_COMP_DESC"]), MailCount = Convert.ToInt32(dr["MailCount"]) });
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        //else
                                        //{
                                        //    mailcount += Convert.ToInt32(dr["MailCount"]);
                                        //}
                                    }
                                    //companyMailCounts.Add(new CompanyMailCountInfo { HPS_WORK_COMPANY = 0, HPS_WORK_COMP_DESC = "其他", MailCount = mailcount });
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotLoginRole;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SystemReportDBProvider调用GetCompanyMailCount异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("SystemReportDBProvider调用GetCompanyMailCount异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用GetCompanyMailCount异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetDeptMailCount(Guid transactionid, AdminInfo admin, out List<DeptMailCountInfo> deptMailCounts, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            deptMailCounts = new List<DeptMailCountInfo>();
            int topcount = Convert.ToInt32(Common.ConfigHelper.ConfigInstance["TopCount"]);
            string strError = string.Empty;
            string paramstr = string.Empty;
            bool bResult = true;
            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetDeptMailCount]", out ds, out strError))
                    {
                        strError = "prc_GetDeptMailCount数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
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
                                    int mailcount = 0;
                                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                    {
                                        DataRow dr = ds.Tables[1].Rows[i];
                                        if (i < topcount)
                                        {
                                            deptMailCounts.Add(new DeptMailCountInfo { DEPTID = Convert.ToInt32(dr["DEPTID"]), DEPT_DESCR = Convert.ToString(dr["DEPT_DESCR"]), COMPANY_DESCR = Convert.ToString(dr["HPS_WORK_COMP_DESC"]), MailCount = Convert.ToInt32(dr["MailCount"]) });
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        //else
                                        //{
                                        //    mailcount += Convert.ToInt32(dr["MailCount"]);
                                        //}
                                    }
                                    //deptMailCounts.Add(new DeptMailCountInfo { DEPTID = 0, DEPT_DESCR = "其他", MailCount = mailcount });
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotLoginRole;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SystemReportDBProvider调用GetDeptMailCount异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("SystemReportDBProvider调用GetDeptMailCount异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用GetDeptMailCount异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetUserMailCount(Guid transactionid, AdminInfo admin, out List<UserMailCountInfo> userMailCounts, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            userMailCounts = new List<UserMailCountInfo>();
            int topcount = Convert.ToInt32(Common.ConfigHelper.ConfigInstance["TopCount"]);
            string strError = string.Empty;
            string paramstr = string.Empty;
            bool bResult = true;
            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetUserMailCount]", out ds, out strError))
                    {
                        strError = "prc_GetUserMailCount数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
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
                                    int mailcount = 0;
                                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                    {
                                        DataRow dr = ds.Tables[1].Rows[i];
                                        if (i < topcount)
                                        {
                                            userMailCounts.Add(new UserMailCountInfo { EMPLID = Convert.ToInt32(dr["EMPLID"]), MailAddress = Convert.ToString(dr["MailAddress"]), NAME = Convert.ToString(dr["NAME"]), MailCount = Convert.ToInt32(dr["MailCount"]) });
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        //else
                                        //{
                                        //    mailcount += Convert.ToInt32(dr["MailCount"]);
                                        //}
                                    }
                                    //userMailCounts.Add(new UserMailCountInfo { EMPLID = 0, MailAddress = string.Empty, NAME = "其他", MailCount = mailcount });
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotLoginRole;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SystemReportDBProvider调用GetUserMailCount异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("SystemReportDBProvider调用GetUserMailCount异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReportDBProvider调用GetUserMailCount异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }
    }
}
