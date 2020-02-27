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
    public class MailDataBaseDBProvider
    {
        public bool GetMailDataBaseList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out BaseListInfo lists, out ErrorCodeInfo error)
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
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetMailDataBaseList]", out ds, out strError))
                    {
                        strError = "prc_GetMailDataBaseList数据库执行失败,Error:" + strError;
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
                            MailDataBaseInfo info = new MailDataBaseInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.OuID = Guid.Parse(Convert.ToString(sdr["OuID"]));
                            info.OuName = Convert.ToString(sdr["OuName"]);
                            info.OUdistinguishedName = Convert.ToString(sdr["OUdistinguishedName"]);
                            info.MailboxDBID = Guid.Parse(Convert.ToString(sdr["MailboxDBID"]));
                            info.MailboxDB = Convert.ToString(sdr["MailboxDB"]);
                            info.MailboxServer = Convert.ToString(sdr["MailboxServer"]);
                            info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
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

        public bool GetMailDataBaseInfo(Guid transactionid, AdminInfo admin, ref MailDataBaseInfo maildb, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"ID:{maildb.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraID = new SqlParameter("@ID", maildb.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetMailDataBaseInfo]", out ds, out strError))
                    {
                        strError = "GetMailDataBaseInfo异常,Error:" + strError;
                        LoggerHelper.Error("MailDataBaseDBProvider调用GetMailDataBaseInfo异常", paramstr, strError, transactionid);
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
                                        maildb.MailboxDBID = Guid.Parse(Convert.ToString(sdr["MailboxDBID"]));
                                        maildb.MailboxDB = Convert.ToString(sdr["MailboxDB"]);
                                        maildb.MailboxServer = Convert.ToString(sdr["MailboxServer"]);
                                        maildb.OuID = Guid.Parse(Convert.ToString(sdr["OuID"]));
                                        maildb.OuName = Convert.ToString(sdr["OuName"]);
                                        maildb.OUdistinguishedName = Convert.ToString(sdr["OUdistinguishedName"]);
                                        maildb.Status = (State)Convert.ToInt32(sdr["Status"]);
                                        maildb.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                                        maildb.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.IdEmpty;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("MailDataBaseDBProvider调用GetMailDataBaseInfo异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("MailDataBaseDBProvider调用GetMailDataBaseInfo异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailDataBaseDBProvider调用GetMailDataBaseInfo异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddMailDataBase(Guid transactionid, AdminInfo admin, ref MailDataBaseInfo maildb, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"ID:{maildb.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraOuID = new SqlParameter("@OuID", maildb.OuID);
                paras.Add(paraOuID);
                SqlParameter paraOuName = new SqlParameter("@OuName", maildb.OuName);
                paras.Add(paraOuName);
                SqlParameter paraOUdistinguishedName = new SqlParameter("@OUdistinguishedName", maildb.OUdistinguishedName);
                paras.Add(paraOUdistinguishedName);
                SqlParameter paraMailboxDB = new SqlParameter("@MailboxDB", maildb.MailboxDB);
                paras.Add(paraMailboxDB);
                SqlParameter paraMailboxServer = new SqlParameter("@MailboxServer", maildb.MailboxServer);
                paras.Add(paraMailboxServer);
                SqlParameter paraRoleID = new SqlParameter("@RoleID", admin.RoleID);
                paras.Add(paraRoleID);
                SqlParameter paraMailboxDBID = new SqlParameter("@MailboxDBID", maildb.MailboxDBID);
                paras.Add(paraMailboxDBID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddMailDataBase]", out ds, out strError))
                    {
                        strError = "AddMailDataBase异常,Error:" + strError;
                        LoggerHelper.Error("MailDataBaseDBProvider调用AddMailDataBase异常", paramstr, strError, transactionid);
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
                                        maildb.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameOu;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("MailDataBaseDBProvider调用AddMailDataBase异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("MailDataBaseDBProvider调用AddMailDataBase异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailDataBaseDBProvider调用AddMailDataBase异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool DeleteMailDataBase(Guid transactionid, AdminInfo admin, MailDataBaseInfo maildb, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"ID:{maildb.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraID = new SqlParameter("@ID", maildb.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DeleteMailDataBase]", out ds, out strError))
                    {
                        strError = "DeleteMailDataBase异常,Error:" + strError;
                        LoggerHelper.Error("MailDataBaseDBProvider调用DeleteMailDataBase异常", paramstr, strError, transactionid);
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
                                    error.Code = ErrorCode.IdEmpty;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("MailDataBaseDBProvider调用DeleteMailDataBase异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("MailDataBaseDBProvider调用DeleteMailDataBase异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailDataBaseDBProvider调用DeleteMailDataBase异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool ChangeMailDataBase(Guid transactionid, AdminInfo admin, MailDataBaseInfo maildb, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"ID:{maildb.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraID = new SqlParameter("@ID", maildb.ID);
                paras.Add(paraID);
                SqlParameter paraOuID = new SqlParameter("@OuID", maildb.OuID);
                paras.Add(paraOuID);
                SqlParameter paraOuName = new SqlParameter("@OuName", maildb.OuName);
                paras.Add(paraOuName);
                SqlParameter paraOUdistinguishedName = new SqlParameter("@OUdistinguishedName", maildb.OUdistinguishedName);
                paras.Add(paraOUdistinguishedName);
                SqlParameter paraMailboxDBID = new SqlParameter("@MailboxDBID", maildb.MailboxDBID);
                paras.Add(paraMailboxDBID);
                SqlParameter paraMailboxDB = new SqlParameter("@MailboxDB", maildb.MailboxDB);
                paras.Add(paraMailboxDB);
                SqlParameter paraMailboxServer = new SqlParameter("@MailboxServer", maildb.MailboxServer);
                paras.Add(paraMailboxServer);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_ModifyMailDataBase]", out ds, out strError))
                    {
                        strError = "ChangeMailDataBase异常,Error:" + strError;
                        LoggerHelper.Error("MailDataBaseDBProvider调用ChangeMailDataBase异常", paramstr, strError, transactionid);
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
                                        maildb.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.OuHaveMailBoxDB;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("MailDataBaseDBProvider调用ChangeMailDataBase异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("MailDataBaseDBProvider调用ChangeMailDataBase异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailDataBaseDBProvider调用ChangeMailDataBase异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetMailDataBaseBydistinguishedName(Guid transactionid, string distinguishedName, out List<MailDataBaseInfo> list, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            list = new List<MailDataBaseInfo>();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraDistinguishedName = new SqlParameter("@distinguishedName", distinguishedName);
                paras.Add(paraDistinguishedName);
              
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetMailboxDBBydistinguishedName]", out ds, out strError))
                    {
                        strError = "prc_GetMailboxDBBydistinguishedName数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }
                    
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            MailDataBaseInfo info = new MailDataBaseInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.OuID = Guid.Parse(Convert.ToString(sdr["OuID"]));
                            info.OuName = Convert.ToString(sdr["OuName"]);
                            info.OUdistinguishedName = Convert.ToString(sdr["OUdistinguishedName"]);
                            info.MailboxDBID = Guid.Parse(Convert.ToString(sdr["MailboxDBID"]));
                            info.MailboxDB = Convert.ToString(sdr["MailboxDB"]);
                            info.MailboxServer = Convert.ToString(sdr["MailboxServer"]);
                            info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                            list.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetMailboxDBBydistinguishedName失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailDataBaseDBProvider调用prc_GetMailboxDBBydistinguishedName异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetMailDataBaseTreeList(Guid transactionid, AdminInfo admin, out List<string> ouPaths, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            ouPaths = new List<string>();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction("dbo.[prc_GetMailDataBaseTreeList]", out ds, out strError))
                    {
                        strError = "prc_GetMailDataBaseTreeList数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string OuPath = Convert.ToString(ds.Tables[0].Rows[i]["OudistinguishedName"]);
                            ouPaths.Add(OuPath);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("prc_GetMailDataBaseTreeList数据库执行失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailDataBaseDBProvider调用prc_GetMailDataBaseTreeList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }
    }
}
