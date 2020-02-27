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
    public class MailAuditDBProvider
    {
        public bool AddMailAudit(Guid transactionid, AdminInfo admin, ref MailAuditInfo mailAuditInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{mailAuditInfo.Group.GroupID}";

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraGroupID = new SqlParameter("@GroupID", mailAuditInfo.Group.GroupID);
                paras.Add(paraGroupID);
                SqlParameter paraGroupName = new SqlParameter("@GroupName", mailAuditInfo.Group.DisplayName);
                paras.Add(paraGroupName);
                SqlParameter paraGroupAccount = new SqlParameter("@GroupAccount", mailAuditInfo.Group.Account);
                paras.Add(paraGroupAccount);
                SqlParameter paraRoleID = new SqlParameter("@RoleID", admin.RoleID);
                paras.Add(paraRoleID);
                SqlParameter paraUserID = new SqlParameter("@CreateUserID", admin.UserID);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddMailAudit]", out ds, out strError))
                    {
                        strError = "AddMailAudit异常,Error:" + strError;
                        LoggerHelper.Error("MailAuditDBProvider调用AddMailAudit异常", paramstr, strError, transactionid);
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
                                case 1:
                                    bResult = true;
                                    if (ds.Tables.Count > 1)
                                    {
                                        DataRow sdr = ds.Tables[1].Rows[0];
                                        mailAuditInfo.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameGroup;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("MailAuditDBProvider调用AddMailAudit异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("MailAuditDBProvider调用AddMailAudit异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAuditDBProvider调用AddMailAudit异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddMailAuditUsers(Guid transactionid, MailAuditInfo mailAuditInfo, UserInfo user, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"||MailAuditID:{mailAuditInfo.ID}";
            paramstr += $"||UserID:{user.UserID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraMailAuditID = new SqlParameter("@MailAuditID", mailAuditInfo.ID);
                paras.Add(paraMailAuditID);
                SqlParameter paraUserID = new SqlParameter("@UserID", user.UserID);
                paras.Add(paraUserID);
                SqlParameter paraDisplayName = new SqlParameter("@DisplayName", user.DisplayName);
                paras.Add(paraDisplayName);
                SqlParameter paraAccount = new SqlParameter("@Account", user.UserAccount);
                paras.Add(paraAccount);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddMailAuditUsers]", out ds, out strError))
                    {
                        strError = "AddMailAuditUsers异常,Error:" + strError;
                        LoggerHelper.Error("MailAuditDBProvider调用AddMailAuditUsers异常", paramstr, strError, transactionid);
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
                                case 1:
                                    bResult = true;
                                    if (ds.Tables.Count > 1)
                                    {
                                        DataRow sdr = ds.Tables[1].Rows[0];
                                        mailAuditInfo.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameOu;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("MailAuditDBProvider调用AddMailAuditUsers异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("MailAuditDBProvider调用AddMailAuditUsers异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAuditDBProvider调用AddMailAuditUsers异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetMailAuditList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out BaseListInfo lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BaseListInfo();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
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
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetMailAuditList]", out ds, out strError))
                    {
                        strError = "prc_GetMailAuditList数据库执行失败,Error:" + strError;
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
                            MailAuditInfo info = new MailAuditInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.Group.GroupID = Guid.Parse(Convert.ToString(sdr["GroupID"]));
                            info.Group.DisplayName = Convert.ToString(sdr["GroupName"]);
                            info.Group.Account = Convert.ToString(sdr["GroupAccount"]);
                            info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                            info.UpdateTime = Convert.ToDateTime(sdr["UpdateTime"]);
                            info.CreateUserID = Guid.Parse(Convert.ToString(sdr["CreateUserID"]));
                            info.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));

                            DataRow[] drArr = ds.Tables[1].Select("MailAuditID='" + info.ID + "'");

                            foreach (DataRow dr in drArr)
                            {
                                UserInfo user = new UserInfo();
                                user.UserID = Guid.Parse(Convert.ToString(dr["UserID"]));
                                user.DisplayName = Convert.ToString(dr["DisplayName"]);
                                user.UserAccount = Convert.ToString(dr["Account"]);
                                info.Audits.Add(user);
                                info.AuditUsers += user.DisplayName + "(" + user.UserAccount + ")，";
                            }

                            info.AuditUsers = string.IsNullOrEmpty(info.AuditUsers) ? string.Empty : info.AuditUsers.Remove(info.AuditUsers.LastIndexOf('，'), 1); 
                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetMailAuditList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAuditDBProvider调用prc_GetMailAuditList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetMailAuditInfo(Guid transactionid, AdminInfo admin, ref MailAuditInfo mailAuditInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{mailAuditInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", mailAuditInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetMailAuditInfo]", out ds, out strError))
                    {
                        strError = "GetMailAuditInfo异常,Error:" + strError;
                        LoggerHelper.Error("MailAuditDBProvider调用GetMailAuditInfo异常", paramstr, strError, transactionid);
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
                                case 1:
                                    bResult = true;
                                    if (ds.Tables.Count > 1)
                                    {
                                        DataRow sdr = ds.Tables[1].Rows[0];
                                        mailAuditInfo.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        mailAuditInfo.Group.GroupID = Guid.Parse(Convert.ToString(sdr["GroupID"]));
                                        mailAuditInfo.Group.DisplayName = Convert.ToString(sdr["GroupName"]);
                                        mailAuditInfo.Group.Account = Convert.ToString(sdr["GroupAccount"]);
                                        mailAuditInfo.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                                        mailAuditInfo.UpdateTime = Convert.ToDateTime(sdr["UpdateTime"]);
                                        mailAuditInfo.CreateUserID = Guid.Parse(Convert.ToString(sdr["CreateUserID"]));
                                        mailAuditInfo.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));

                                        foreach (DataRow dr in ds.Tables[2].Rows)
                                        {
                                            UserInfo user = new UserInfo();
                                            user.UserID = Guid.Parse(Convert.ToString(dr["UserID"]));
                                            user.DisplayName = Convert.ToString(dr["DisplayName"]);
                                            user.UserAccount = Convert.ToString(dr["Account"]);
                                            mailAuditInfo.Audits.Add(user);
                                            mailAuditInfo.AuditUsers +=user.DisplayName + "(" + user.UserAccount + ")，";
                                        }

                                        mailAuditInfo.AuditUsers = mailAuditInfo.AuditUsers.Remove(mailAuditInfo.AuditUsers.LastIndexOf('，'), 1);
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameOu;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("MailAuditDBProvider调用GetMailAuditInfo异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("SensitiveMailDBProvider调用GetSensitiveMailInfo异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用GetSensitiveMailInfo异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool ModifyMailAudit(Guid transactionid, AdminInfo admin, MailAuditInfo mailAuditInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||GroupID:{mailAuditInfo.Group.GroupID}";

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraID = new SqlParameter("@ID", mailAuditInfo.ID);
                paras.Add(paraID);
                SqlParameter paraGroupID = new SqlParameter("@GroupID", mailAuditInfo.Group.GroupID);
                paras.Add(paraGroupID);
                SqlParameter paraGroupName = new SqlParameter("@GroupName", mailAuditInfo.Group.DisplayName);
                paras.Add(paraGroupName);
                SqlParameter paraGroupAccount = new SqlParameter("@GroupAccount", mailAuditInfo.Group.Account);
                paras.Add(paraGroupAccount);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_ModifyMailAudit]", out ds, out strError))
                    {
                        strError = "ModifyMailAudit异常,Error:" + strError;
                        LoggerHelper.Error("MailAuditDBProvider调用ModifyMailAudit异常", paramstr, strError, transactionid);
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
                                case 1:
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameGroup;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("MailAuditDBProvider调用ModifyMailAudit异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("MailAuditDBProvider调用ModifyMailAudit异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAuditDBProvider调用ModifyMailAudit异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool DeleteMailAudit(Guid transactionid, AdminInfo admin, MailAuditInfo mailAuditInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{mailAuditInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", mailAuditInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DeleteMailAudit]", out ds, out strError))
                    {
                        strError = "DeleteMailAudit异常,Error:" + strError;
                        LoggerHelper.Error("MailAuditDBProvider调用DeleteMailAudit异常", paramstr, strError, transactionid);
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
                                case 1:
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameGroup;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("MailAuditDBProvider调用DeleteMailAudit异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("MailAuditDBProvider调用DeleteMailAudit异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAuditDBProvider调用DeleteMailAudit异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }
    }
}
