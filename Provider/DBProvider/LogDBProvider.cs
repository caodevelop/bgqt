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
    public class LogDBProvider
    {
        public bool GetLogList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string account, DateTime startTime, DateTime endTime, string searchstr, out BaseListInfo lists, out ErrorCodeInfo error)
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
                SqlParameter paraAccount = new SqlParameter("@account", $"%{account}%");
                paras.Add(paraAccount);
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
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetLogList]", out ds, out strError))
                    {
                        strError = "prc_GetLogList数据库执行失败,Error:" + strError;
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
                            LogInfo info = new LogInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.LogNum = Convert.ToString(sdr["LogNum"]);
                            info.AdminID = Guid.Parse(Convert.ToString(sdr["AdminID"]));
                            info.AdminAccount = Convert.ToString(sdr["AdminAccount"]);
                            info.ClientIP = Convert.ToString(sdr["ClientIP"]);
                            info.OperateLog = Convert.ToString(sdr["OperateLog"]);
                            info.OperateResult = Convert.ToBoolean(sdr["OperateResult"]);
                            info.OperateTime = Convert.ToDateTime(sdr["OperateTime"]);
                            info.OperateType = Convert.ToString(sdr["OperateType"]);
                            info.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetLogList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("LogDBProvider调用prc_GetLogList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddOperateLog(Guid transactionid, LogInfo log, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            bool bResult = true;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{log.AdminID}";
            paramstr += $"||AdminAccount:{log.AdminAccount}";
            paramstr += $"||RoleID:{log.RoleID}";
            paramstr += $"||OperateType:{log.OperateType}";
            paramstr += $"||OperateTimeName:{log.OperateTimeName}";
            paramstr += $"||OperateTime:{log.OperateTime}";
            paramstr += $"||OperateResult:{log.OperateResult}";
            paramstr += $"||OperateLog:{log.OperateLog}";

            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", log.AdminID);
                paras.Add(paraAdminID);
                SqlParameter paraAdminAccount = new SqlParameter("@AdminAccount", log.AdminAccount);
                paras.Add(paraAdminAccount);
                SqlParameter paraRoleID = new SqlParameter("@RoleID", log.RoleID);
                paras.Add(paraRoleID);
                SqlParameter paraOperateType = new SqlParameter("@OperateType", log.OperateType);
                paras.Add(paraOperateType);
                SqlParameter paraOperateLog = new SqlParameter("@OperateLog", log.OperateLog);
                paras.Add(paraOperateLog);
                SqlParameter paraOperateResult = new SqlParameter("@OperateResult", log.OperateResult);
                paras.Add(paraOperateResult);
                SqlParameter paraClientIP = new SqlParameter("@ClientIP", log.ClientIP);
                paras.Add(paraClientIP);


                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddOperateLog]", out ds, out strError))
                    {
                        strError = "prc_AddOperateLog数据库执行失败,Error:" + strError;
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
                                    LoggerHelper.Error("LogDBProvider调用AddOperateLog异常", "", "-9999", transactionid);
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
                        LoggerHelper.Error("LogDBProvider调用AddOperateLog异常", "", "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("LogDBProvider调用AddOperateLog异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }
    }
}
