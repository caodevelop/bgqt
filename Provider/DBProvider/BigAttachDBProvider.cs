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
    public class BigAttachDBProvider
    {
        public bool GetFileList(
            Guid transactionid,
            Guid userid, 
            int curpage, 
            int pagesize, 
            out BigFileListInfo lists, 
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BigFileListInfo();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", userid);
                paras.Add(paraUserID);
                SqlParameter paraPageIndex = new SqlParameter("@PageIndex", curpage);
                paras.Add(paraPageIndex);
                SqlParameter paraPageSize = new SqlParameter("@PageSize", pagesize);
                paras.Add(paraPageSize);              
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
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetFileList]", out ds, out strError))
                    {
                        strError = "prc_GetFileList数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    lists.recordCount = (int)paraRecordCount.Value;
                    lists.pageCount = (int)paraPageCount.Value;

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            BigFileItemInfo info = new BigFileItemInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));                           
                            info.UserID = Guid.Parse(Convert.ToString(sdr["UserID"]));
                            info.FileName = Convert.ToString(sdr["AdminAccount"]);
                            info.ExtensionName = Convert.ToString(sdr["ClientIP"]);
                            info.FileSize = Convert.ToDouble(sdr["FileSize"]);
                            info.LastUpdateTime = Convert.ToDateTime(sdr["LastUpdateTime"]);
                            info.UploadTime = Convert.ToDateTime(sdr["UploadTime"]);
                          
                            lists.files.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("BigAttachDBProvider数据库执行prc_GetFileList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用prc_GetFileList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetGlobalUploadSetting(
            Guid transactionid, 
            Guid userid, 
            ref AttachSettingItem info, 
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", userid);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetGlobalUploadSetting]", out ds, out strError))
                    {
                        strError = "GetGlobalUploadSetting异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用GetGlobalUploadSetting异常", paramstr, strError, transactionid);
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
                                    bResult = true;
                                    if (ds.Tables.Count > 1)
                                    {
                                        DataRow sdr = ds.Tables[1].Rows[0];
                                        info.ChunkSize = Convert.ToInt32(sdr["MaxUploads"]);
                                        info.AllowDrop = true;
                                        info.DefaultExtension = Convert.ToString(sdr["DefaultExtension"]);
                                        info.MaxFileSize = Convert.ToDouble(sdr["MaxFileSize"]);
                                        info.MaxUploads = Convert.ToInt32(sdr["MaxUploads"]);
                                        info.FileUploadBlackList = Convert.ToString(sdr["FileUploadBlackList"]);
                                    }
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用GetGlobalUploadSetting异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用GetGlobalUploadSetting异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用GetGlobalUploadSetting异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetUploadParItem(
            Guid transactionid,
            Guid userid,
            ref UploadParItemInfo info,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", userid);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetUploadParItem]", out ds, out strError))
                    {
                        strError = "GetUploadParItem异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用GetUploadParItem异常", paramstr, strError, transactionid);
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
                                    bResult = true;
                                    if (ds.Tables.Count > 1)
                                    {
                                        DataRow sdr = ds.Tables[1].Rows[0];
                                        info.OutlookFolderID = "";
                                        info.StorageID = "";
                                        info.StorageRelativePath = "";
                                        info.StorageUri = "";
                                        info.UserQuota = Convert.ToDouble(sdr["UserQuota"]);
                                        info.UserUsedQuota = Convert.ToDouble(sdr["UserUsedQuota"]);
                                    }
                                    break;                              
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用GetUploadParItem异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用GetUploadParItem异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用GetUploadParItem异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool DeleteFile(
            Guid transactionid,
            Guid userid,
            ref BigFileItemInfo info,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||ID:{info.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", userid);
                SqlParameter paraFileID = new SqlParameter("@FileID", info.ID);
                paras.Add(paraUserID);
                paras.Add(paraFileID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DeleteFile]", out ds, out strError))
                    {
                        strError = "DeleteFile异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用DeleteFile异常", paramstr, strError, transactionid);
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
                                    bResult = true;                                  
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameOu;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用DeleteFile异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用DeleteFile异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用DeleteFile异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool RenameFile(
            Guid transactionid,
            Guid userid,
            ref BigFileItemInfo info,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||ID:{info.ID}";
            paramstr += $"||FileName:{info.FileName}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", userid);
                SqlParameter paraFileName = new SqlParameter("@FileName", info.FileName);
                paras.Add(paraUserID);
                paras.Add(paraFileName);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_RenameFile]", out ds, out strError))
                    {
                        strError = "RenameFile异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用RenameFile异常", paramstr, strError, transactionid);
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
                                    bResult = true;
                                    break;                            
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用RenameFile异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用RenameFile异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用RenameFile异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }
    }
}
