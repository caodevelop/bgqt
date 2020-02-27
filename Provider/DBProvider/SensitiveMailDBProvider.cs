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
    public class SensitiveMailDBProvider
    {
        public bool AddSensitiveMail(Guid transactionid, AdminInfo admin, ref SensitiveMailInfo sensitiveMailInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||Name:{sensitiveMailInfo.Name}";
            paramstr += $"||Keywords:{sensitiveMailInfo.Keywords}";
            paramstr += $"||StartTime:{sensitiveMailInfo.StartTime}";
            paramstr += $"||EndTime:{sensitiveMailInfo.EndTime}";
            paramstr += $"||Type:{sensitiveMailInfo.MailType}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraName = new SqlParameter("@Name", sensitiveMailInfo.Name);
                paras.Add(paraName);
                SqlParameter paraKeywords = new SqlParameter("@Keywords", sensitiveMailInfo.Keywords);
                paras.Add(paraKeywords);
                SqlParameter paraStartTime = new SqlParameter("@StartTime", sensitiveMailInfo.StartTime);
                paras.Add(paraStartTime);
                SqlParameter paraEndTime = new SqlParameter("@EndTime", sensitiveMailInfo.EndTime);
                paras.Add(paraEndTime);
                SqlParameter paraRoleID = new SqlParameter("@RoleID", admin.RoleID);
                paras.Add(paraRoleID);
                SqlParameter paraUserID = new SqlParameter("@CreateUserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraType = new SqlParameter("@Type", sensitiveMailInfo.MailType);
                paras.Add(paraType);
                SqlParameter paraStatus = new SqlParameter("@Status", sensitiveMailInfo.Status);
                paras.Add(paraStatus);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddSensitiveMail]", out ds, out strError))
                    {
                        strError = "AddSensitiveMail异常,Error:" + strError;
                        LoggerHelper.Error("SensitiveMailDBProvider调用AddSensitiveMail异常", paramstr, strError, transactionid);
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
                                        sensitiveMailInfo.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameRule;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SensitiveMailDBProvider调用AddSensitiveMail异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("SensitiveMailDBProvider调用AddSensitiveMail异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用AddSensitiveMail异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddSensitiveMailObjects(Guid transactionid, AdminInfo admin, SensitiveMailObject member, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"SensitiveMailID:{member.SensitiveMailID}";
            paramstr += $"||ObjectID:{member.ObjectID}";
            paramstr += $"||ObjectName:{member.ObjectName}";
            paramstr += $"||ObjectType:{member.ObjectType}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraSensitiveMailID = new SqlParameter("@SensitiveMailID", member.SensitiveMailID);
                paras.Add(paraSensitiveMailID);
                SqlParameter paraObjectID = new SqlParameter("@ObjectID", member.ObjectID);
                paras.Add(paraObjectID);
                SqlParameter paraObjectName = new SqlParameter("@ObjectName", member.ObjectName);
                paras.Add(paraObjectName);
                SqlParameter paraObjectType = new SqlParameter("@ObjectType", member.ObjectType);
                paras.Add(paraObjectType);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddSensitiveMailObject]", out ds, out strError))
                    {
                        strError = "AddSensitiveMailObjects异常,Error:" + strError;
                        LoggerHelper.Error("SensitiveMailDBProvider调用AddSensitiveMailObjects异常", paramstr, strError, transactionid);
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
                                    error.Code = ErrorCode.HaveSameRule;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SensitiveMailDBProvider调用AddSensitiveMailObjects异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("SensitiveMailDBProvider调用AddSensitiveMailObjects异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用AddSensitiveMailObjects异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetSensitiveMailList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, DateTime startTime, DateTime endTime, string searchstr, out BaseListInfo lists, out ErrorCodeInfo error)
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
                SqlParameter paraStartTime = new SqlParameter("@StartTime", startTime);
                paras.Add(paraStartTime);
                SqlParameter paraEndTime = new SqlParameter("@EndTime", endTime);
                paras.Add(paraEndTime);
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
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetSensitiveMailList]", out ds, out strError))
                    {
                        strError = "prc_GetSensitiveMailList数据库执行失败,Error:" + strError;
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
                            SensitiveMailInfo info = new SensitiveMailInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.Name = Convert.ToString(sdr["Name"]);
                            info.Keywords = Convert.ToString(sdr["Keywords"]);
                            info.StartTime = Convert.ToDateTime(sdr["StartTime"]);
                            info.EndTime = Convert.ToDateTime(sdr["EndTime"]);
                            info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                            info.UpdateTime = Convert.ToDateTime(sdr["UpdateTime"]);
                            info.CreateUserID = Guid.Parse(Convert.ToString(sdr["CreateUserID"]));
                            info.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                            info.MailType = (SensitiveMailType)Convert.ToInt32(sdr["Type"]);
                            info.ExecuteTime = Convert.ToDateTime(sdr["ExecuteTime"]);
                            info.ExecuteResult = Convert.ToString(sdr["ExecuteResult"]);
                            info.Status = (SensitiveMailStatus)Convert.ToInt32(sdr["Status"]);
                            info.ExecuteID = Guid.Parse(Convert.ToString(sdr["ExecuteID"]));
                            DataRow[] drArr = ds.Tables[1].Select("SensitiveMailID='" + info.ID + "'");
                            foreach (DataRow dr in drArr)
                            {
                                SensitiveMailObject member = new SensitiveMailObject();
                                member.ID = Guid.Parse(Convert.ToString(dr["ID"]));
                                member.SensitiveMailID = Guid.Parse(Convert.ToString(dr["SensitiveMailID"]));
                                member.ObjectID = Guid.Parse(Convert.ToString(dr["ObjectID"]));
                                member.ObjectName = Convert.ToString(dr["ObjectName"]);
                                member.ObjectType = (NodeType)Convert.ToInt32(dr["ObjectType"]);
                                info.ObjectNames += member.ObjectName + "，";
                                info.Objects.Add(member);
                            }

                            DataRow[] datarows = ds.Tables[2].Select("SensitiveMailID = '" + info.ID + "' and ExecuteID = '" + info.ExecuteID + "'");
                            foreach (DataRow dr in datarows)
                            {
                                UserSensitiveMailQueueInfo queueinfo = new UserSensitiveMailQueueInfo();
                                queueinfo.ID = Guid.Parse(Convert.ToString(dr["ID"]));
                                queueinfo.SensitiveMailID = Guid.Parse(Convert.ToString(dr["SensitiveMailID"]));
                                queueinfo.ExecuteID = Guid.Parse(Convert.ToString(dr["ExecuteID"]));
                                queueinfo.UserID = Guid.Parse(Convert.ToString(dr["UserID"]));
                                queueinfo.Status = (SensitiveMailStatus)Convert.ToInt32(dr["Status"]);
                                info.QueueLists.Add(queueinfo);
                            }

                            if (info.QueueLists.Count > 0)
                            {
                                decimal ExecutedCount = info.QueueLists.Count(p => p.Status == SensitiveMailStatus.Success || p.Status == SensitiveMailStatus.Failed);
                                info.PercentageComplete = (ExecutedCount / info.QueueLists.Count).ToString("0.00%");
                            }
                            else
                            {
                                info.PercentageComplete = "0.00%";
                            }

                            info.ObjectNames = string.IsNullOrEmpty(info.ObjectNames) ? "" : info.ObjectNames.Remove(info.ObjectNames.LastIndexOf('，'), 1);
                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetSensitiveMailList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用prc_GetSensitiveMailList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetSensitiveMailInfo(Guid transactionid, AdminInfo admin, ref SensitiveMailInfo sensitiveMailInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{sensitiveMailInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", sensitiveMailInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetSensitiveMailInfo]", out ds, out strError))
                    {
                        strError = "GetSensitiveMailInfo异常,Error:" + strError;
                        LoggerHelper.Error("SensitiveMailDBProvider调用GetSensitiveMailInfo异常", paramstr, strError, transactionid);
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
                                        sensitiveMailInfo.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        sensitiveMailInfo.Name = Convert.ToString(sdr["Name"]);
                                        sensitiveMailInfo.Keywords = Convert.ToString(sdr["Keywords"]);
                                        sensitiveMailInfo.StartTime = Convert.ToDateTime(sdr["StartTime"]);
                                        sensitiveMailInfo.EndTime = Convert.ToDateTime(sdr["EndTime"]);
                                        sensitiveMailInfo.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                                        sensitiveMailInfo.UpdateTime = Convert.ToDateTime(sdr["UpdateTime"]);
                                        sensitiveMailInfo.CreateUserID = Guid.Parse(Convert.ToString(sdr["CreateUserID"]));
                                        sensitiveMailInfo.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                                        sensitiveMailInfo.MailType = (SensitiveMailType)Convert.ToInt32(sdr["Type"]);
                                        sensitiveMailInfo.ExecuteTime = Convert.ToDateTime(sdr["ExecuteTime"]);
                                        sensitiveMailInfo.ExecuteResult = Convert.ToString(sdr["ExecuteResult"]);
                                        sensitiveMailInfo.Status = (SensitiveMailStatus)Convert.ToInt32(sdr["Status"]);
                                        foreach (DataRow dr in ds.Tables[2].Rows)
                                        {
                                            SensitiveMailObject member = new SensitiveMailObject();
                                            member.ID = Guid.Parse(Convert.ToString(dr["ID"]));
                                            member.SensitiveMailID = Guid.Parse(Convert.ToString(dr["SensitiveMailID"]));
                                            member.ObjectID = Guid.Parse(Convert.ToString(dr["ObjectID"]));
                                            member.ObjectName = Convert.ToString(dr["ObjectName"]);
                                            member.ObjectType = (NodeType)Convert.ToInt32(dr["ObjectType"]);
                                            sensitiveMailInfo.ObjectNames += member.ObjectName + "，";
                                            sensitiveMailInfo.Objects.Add(member);
                                        }

                                        sensitiveMailInfo.ObjectNames = string.IsNullOrEmpty(sensitiveMailInfo.ObjectNames) ? "" : sensitiveMailInfo.ObjectNames.Remove(sensitiveMailInfo.ObjectNames.LastIndexOf('，'), 1);
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameOu;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SensitiveMailDBProvider调用GetSensitiveMailInfo异常", paramstr, "-9999", transactionid);
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
                LoggerHelper.Error("SensitiveMailDBProvider调用GetSensitiveMailInfo异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool DeleteSensitiveMail(Guid transactionid, AdminInfo admin, SensitiveMailInfo sensitiveMailInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{sensitiveMailInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", sensitiveMailInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DeleteSensitiveMail]", out ds, out strError))
                    {
                        strError = "DeleteSensitiveMail异常,Error:" + strError;
                        LoggerHelper.Error("SensitiveMailDBProvider调用DeleteSensitiveMail异常", paramstr, strError, transactionid);
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
                                    error.Code = ErrorCode.HaveSameOu;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SensitiveMailDBProvider调用DeleteSensitiveMail异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("SensitiveMailDBProvider调用DeleteSensitiveMail异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用DeleteSensitiveMail异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool ModifySensitiveMail(Guid transactionid, AdminInfo admin, SensitiveMailInfo sensitiveMailInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{sensitiveMailInfo.ID}";
            paramstr += $"||Keywords:{sensitiveMailInfo.Keywords}";
            paramstr += $"||StartTime:{sensitiveMailInfo.StartTime}";
            paramstr += $"||EndTime:{sensitiveMailInfo.EndTime}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraID = new SqlParameter("@ID", sensitiveMailInfo.ID);
                paras.Add(paraID);
                SqlParameter paraKeywords = new SqlParameter("@Keywords", sensitiveMailInfo.Keywords);
                paras.Add(paraKeywords);
                SqlParameter paraStartTime = new SqlParameter("@StartTime", sensitiveMailInfo.StartTime);
                paras.Add(paraStartTime);
                SqlParameter paraEndTime = new SqlParameter("@EndTime", sensitiveMailInfo.EndTime);
                paras.Add(paraEndTime);
                SqlParameter paraStatus = new SqlParameter("@Status", sensitiveMailInfo.Status);
                paras.Add(paraStatus);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_ModfiySensitiveMail]", out ds, out strError))
                    {
                        strError = "ModifySensitiveMail异常,Error:" + strError;
                        LoggerHelper.Error("SensitiveMailDBProvider调用ModifySensitiveMail异常", paramstr, strError, transactionid);
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
                                    error.Code = ErrorCode.HaveSameRule;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SensitiveMailDBProvider调用ModifySensitiveMail异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("SensitiveMailDBProvider调用ModifySensitiveMail异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用ModifySensitiveMail异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool UpdateSensitiveMailStatus(Guid transactionid, AdminInfo admin, SensitiveMailInfo sensitiveMailInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"||ID:{sensitiveMailInfo.ID}";
            paramstr += $"||Status:{sensitiveMailInfo.Status}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraID = new SqlParameter("@ID", sensitiveMailInfo.ID);
                paras.Add(paraID);
                SqlParameter paraStatus = new SqlParameter("@Status", sensitiveMailInfo.Status);
                paras.Add(paraStatus);
                SqlParameter paraExecuteID = new SqlParameter("@ExecuteID", sensitiveMailInfo.ExecuteID);
                paras.Add(paraExecuteID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_UpdateSensitiveMailStatus]", out ds, out strError))
                    {
                        strError = "UpdateSensitiveMailStatus异常,Error:" + strError;
                        LoggerHelper.Error("SensitiveMailDBProvider调用UpdateSensitiveMailStatus异常", paramstr, strError, transactionid);
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
                                    error.Code = ErrorCode.IdNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SensitiveMailDBProvider调用UpdateSensitiveMailStatus异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("SensitiveMailDBProvider调用UpdateSensitiveMailStatus异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用UpdateSensitiveMailStatus异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool UpdateSensitiveMailExecuteResult(Guid transactionid, SensitiveMailInfo sensitiveMailInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"||ID:{sensitiveMailInfo.ID}";
            paramstr += $"||ExecuteResult:{sensitiveMailInfo.ExecuteResult}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraID = new SqlParameter("@ID", sensitiveMailInfo.ID);
                paras.Add(paraID);
                SqlParameter paraExecuteResult = new SqlParameter("@ExecuteResult", sensitiveMailInfo.ExecuteResult);
                paras.Add(paraExecuteResult);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_UpdateSensitiveMailExecuteResult]", out ds, out strError))
                    {
                        strError = "UpdateSensitiveMailExecuteResult异常,Error:" + strError;
                        LoggerHelper.Error("SensitiveMailDBProvider调用UpdateSensitiveMailExecuteResult异常", paramstr, strError, transactionid);
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
                                    error.Code = ErrorCode.IdNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SensitiveMailDBProvider调用UpdateSensitiveMailExecuteResult异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("SensitiveMailDBProvider调用UpdateSensitiveMailExecuteResult异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用UpdateSensitiveMailExecuteResult异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetSubmitSensitiveMailQueueList(Guid transactionid, out List<SensitiveMailInfo> lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new List<SensitiveMailInfo>();
            string strError = string.Empty;
            bool bResult = true;
            try
            {

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetSubmitSensitiveMailQueueList]", out ds, out strError))
                    {
                        strError = "prc_GetSubmitSensitiveMailQueueList数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            SensitiveMailInfo info = new SensitiveMailInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.Name = Convert.ToString(sdr["Name"]);
                            info.Keywords = Convert.ToString(sdr["Keywords"]);
                            info.StartTime = Convert.ToDateTime(sdr["StartTime"]);
                            info.EndTime = Convert.ToDateTime(sdr["EndTime"]);
                            info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                            info.UpdateTime = Convert.ToDateTime(sdr["UpdateTime"]);
                            info.CreateUserID = Guid.Parse(Convert.ToString(sdr["CreateUserID"]));
                            info.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                            info.MailType = (SensitiveMailType)Convert.ToInt32(sdr["Type"]);
                            info.ExecuteTime = Convert.ToDateTime(sdr["ExecuteTime"]);
                            info.ExecuteResult = Convert.ToString(sdr["ExecuteResult"]);
                            info.Status = (SensitiveMailStatus)Convert.ToInt32(sdr["Status"]);
                            info.ExecuteID = Guid.Parse(Convert.ToString(sdr["ExecuteID"]));
                            DataRow[] drArr = ds.Tables[1].Select("SensitiveMailID='" + info.ID + "'");
                            foreach (DataRow dr in drArr)
                            {
                                SensitiveMailObject member = new SensitiveMailObject();
                                member.ID = Guid.Parse(Convert.ToString(dr["ID"]));
                                member.SensitiveMailID = Guid.Parse(Convert.ToString(dr["SensitiveMailID"]));
                                member.ObjectID = Guid.Parse(Convert.ToString(dr["ObjectID"]));
                                member.ObjectName = Convert.ToString(dr["ObjectName"]);
                                member.ObjectType = (NodeType)Convert.ToInt32(dr["ObjectType"]);
                                info.ObjectNames += member.ObjectName + "，";
                                info.Objects.Add(member);
                            }

                            info.ObjectNames = string.IsNullOrEmpty(info.ObjectNames) ? "" : info.ObjectNames.Remove(info.ObjectNames.LastIndexOf('，'), 1);
                            lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetSubmitSensitiveMailQueueList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用prc_GetSubmitSensitiveMailQueueList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddUserSensitiveMailQueue(Guid transactionid, SensitiveMailInfo sensitiveMailInfo, UserInfo user, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"||ID:{sensitiveMailInfo.ID}";
            paramstr += $"||UserID:{user.UserID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraSensitiveMailID = new SqlParameter("@SensitiveMailID", sensitiveMailInfo.ID);
                paras.Add(paraSensitiveMailID);
                SqlParameter paraUserID = new SqlParameter("@UserID", user.UserID);
                paras.Add(paraUserID);
                SqlParameter paraStatus = new SqlParameter("@Status", SensitiveMailStatus.Enable);
                paras.Add(paraStatus);
                SqlParameter paraExecuteID = new SqlParameter("@ExecuteID", sensitiveMailInfo.ExecuteID);
                paras.Add(paraExecuteID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddSensitiveMailUserQueue]", out ds, out strError))
                    {
                        strError = "AddUserSensitiveMailQueue异常,Error:" + strError;
                        LoggerHelper.Error("SensitiveMailDBProvider调用AddUserSensitiveMailQueue异常", paramstr, strError, transactionid);
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
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SensitiveMailDBProvider调用AddUserSensitiveMailQueue异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("SensitiveMailDBProvider调用AddUserSensitiveMailQueue异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用AddUserSensitiveMailQueue异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetUserSensitiveMailQueueList(Guid transactionid, out List<UserSensitiveMailQueueInfo> lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new List<UserSensitiveMailQueueInfo>();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CBaseDB _db = new CBaseDB(Conntection.strConnection);

                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetUserSensitiveMailQueueList]", out ds, out strError))
                    {
                        strError = "prc_GetUserSensitiveMailQueueList数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            UserSensitiveMailQueueInfo info = new UserSensitiveMailQueueInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.SensitiveMailID = Guid.Parse(Convert.ToString(sdr["SensitiveMailID"]));
                            info.Keywords = Convert.ToString(sdr["Keywords"]);
                            info.StartTime = Convert.ToDateTime(sdr["StartTime"]);
                            info.EndTime = Convert.ToDateTime(sdr["EndTime"]);
                            info.UserID = Guid.Parse(Convert.ToString(sdr["UserID"]));
                            info.ExecuteID = Guid.Parse(Convert.ToString(sdr["ExecuteID"]));
                            lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetSubmitSensitiveMailQueueList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用prc_GetSubmitSensitiveMailQueueList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool UpdateUserSensitiveMailQueue(Guid transactionid, UserSensitiveMailQueueInfo info,string resultmessage, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"||ID:{info.ID}";
            paramstr += $"||Status:{info.Status}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraID = new SqlParameter("@ID", info.ID);
                paras.Add(paraID);
                SqlParameter paraExecuteResult = new SqlParameter("@Status", info.Status);
                paras.Add(paraExecuteResult);
                SqlParameter paraResultMessage = new SqlParameter("@ResultMessage", resultmessage);
                paras.Add(paraResultMessage);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_UpdateUserSensitiveMailQueue]", out ds, out strError))
                    {
                        strError = "UpdateUserSensitiveMailQueue异常,Error:" + strError;
                        LoggerHelper.Error("SensitiveMailDBProvider调用UpdateUserSensitiveMailQueue异常", paramstr, strError, transactionid);
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
                                    error.Code = ErrorCode.IdNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("SensitiveMailDBProvider调用UpdateUserSensitiveMailQueue异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("SensitiveMailDBProvider调用UpdateUserSensitiveMailQueue异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用UpdateUserSensitiveMailQueue异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetSensitiveMailReport(Guid transactionid, AdminInfo admin, Guid sensitiveMailID, out List<UserSensitiveMailQueueInfo> queueInfos, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            queueInfos = new List<UserSensitiveMailQueueInfo>();
            string strError = string.Empty;
            bool bResult = true;
            try
            {

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                CParameters paras = new CParameters();
                SqlParameter paraID = new SqlParameter("@ID", sensitiveMailID);
                paras.Add(paraID);

                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras,"dbo.[prc_GetSensitiveMailReport]", out ds, out strError))
                    {
                        strError = "prc_GetSensitiveMailReport数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            UserSensitiveMailQueueInfo info = new UserSensitiveMailQueueInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.SensitiveMailID = Guid.Parse(Convert.ToString(sdr["SensitiveMailID"]));
                            info.Keywords = Convert.ToString(sdr["Keywords"]);
                            info.StartTime = Convert.ToDateTime(sdr["StartTime"]);
                            info.EndTime = Convert.ToDateTime(sdr["EndTime"]);
                            info.UserID = Guid.Parse(Convert.ToString(sdr["UserID"]));
                            info.ExecuteID = Guid.Parse(Convert.ToString(sdr["ExecuteID"]));
                            info.ExecuteStartTime = Convert.ToDateTime(sdr["ExecuteStartTime"]);
                            info.ExecuteEndTime = Convert.ToDateTime(sdr["ExecuteEndTime"]);
                            info.ExecuteResult = Convert.ToString(sdr["ResultMessage"]);
                            info.Status = (SensitiveMailStatus)Convert.ToInt32(sdr["Status"]);
                            queueInfos.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetSensitiveMailReport失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMailDBProvider调用prc_GetSensitiveMailReport异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }
    }
}
