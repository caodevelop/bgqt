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
        public bool GetUserIDByEmail(
            Guid transactionid,
            string email,
            out Guid userid,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            userid = Guid.Empty;
            string paramstr = string.Empty;
            paramstr += $"email:{email}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@Email", email);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetUserIDByEmail]", out ds, out strError))
                    {
                        strError = "GetUserIDByEmail异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用GetUserIDByEmail异常", paramstr, strError, transactionid);
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
                                        userid = Guid.Parse(Convert.ToString(sdr["UserID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotExist;
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
            lists.files = new List<BigFileItemInfo>();
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
                            info.FileName = Convert.ToString(sdr["FileName"]);
                            info.ExtensionName = Convert.ToString(sdr["ExtensionName"]);
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

        public bool Search(
            Guid transactionid,
            Guid userid,
            string keyword,
            int top,
            out BigFileListInfo lists,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BigFileListInfo();
            lists.files = new List<BigFileItemInfo>();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", userid);
                paras.Add(paraUserID);
                SqlParameter paraKeyword = new SqlParameter("@Keyword", "%" + keyword + "%");
                paras.Add(paraKeyword);
                SqlParameter paraTop = new SqlParameter("@Top", top);
                paras.Add(paraTop);
                

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_Search]", out ds, out strError))
                    {
                        strError = "prc_Search数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }  

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            BigFileItemInfo info = new BigFileItemInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.UserID = Guid.Parse(Convert.ToString(sdr["UserID"]));
                            info.FileName = Convert.ToString(sdr["FileName"]);
                            info.ExtensionName = Convert.ToString(sdr["ExtensionName"]);
                            info.FileSize = Convert.ToDouble(sdr["FileSize"]);
                            info.LastUpdateTime = Convert.ToDateTime(sdr["LastUpdateTime"]);
                            info.UploadTime = Convert.ToDateTime(sdr["UploadTime"]);
                            
                            lists.files.Add(info);
                        }
                        lists.recordCount = ds.Tables[0].Rows.Count;
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("BigAttachDBProvider数据库执行prc_Search失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用prc_Search异常", string.Empty, ex.ToString(), transactionid);
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
                                        info.ChunkSize = Convert.ToInt32(sdr["ChunkSize"]);
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

        public bool GetShareSettings(
            Guid transactionid,
            Guid userid,
            ref ShareSettingsInfo info,
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
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetShareSettings]", out ds, out strError))
                    {
                        strError = "GetShareSettings异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用GetShareSettings异常", paramstr, strError, transactionid);
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
                                        info.DefaultValidateCode = null;
                                        int expireday = Convert.ToInt32(sdr["ExpireDay"]);
                                        info.ExpireTime = DateTime.Now.AddDays(expireday);
                                        info.ShareNotificationTemplate = Convert.ToString(sdr["ShareNotificationTemplate"]);
                                        info.FileShareLimit = Convert.ToInt32(sdr["FileShareLimit"]);
                                    }
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用GetShareSettings异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用GetShareSettings异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
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
                SqlParameter paraID = new SqlParameter("@ID", info.ID);
                SqlParameter paraFileName = new SqlParameter("@FileName", info.FileName);
                paras.Add(paraID);
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
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.FileNotExist;
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
                SqlParameter paraID = new SqlParameter("@ID", info.ID);
                paras.Add(paraID);

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
                                case -1:
                                    bResult = true;
                                    if (ds.Tables.Count > 1)
                                    {
                                        DataRow sdr = ds.Tables[1].Rows[0];
                                        info.FilePath = Convert.ToString(sdr["FilePath"]);
                                    }
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


        public bool QuickUpload(
            Guid transactionid,
            Guid userid,
            ref BigFileItemInfo info,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||HashCode:{info.HashCode}";
            info.Succeed = false;
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraHashCode = new SqlParameter("@HashCode", info.HashCode);
                SqlParameter paraUserID = new SqlParameter("@UserID", userid);
                paras.Add(paraHashCode);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_QuickUpload]", out ds, out strError))
                    {
                        strError = "QuickUpload异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用QuickUpload异常", paramstr, strError, transactionid);
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
                                        info.FileID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        info.FileName = Convert.ToString(sdr["FileName"]);
                                    }
                                    info.Succeed = true;
                                    break;
                                case -1:
                                    bResult = true;
                                    info.Succeed = false;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用QuickUpload异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用QuickUpload异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用QuickUpload异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool CancelUpload(
            Guid transactionid,
            Guid userid,
            ref BigFileItemInfo info,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||TempID:{info.TempID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraTempID = new SqlParameter("@TempID", info.TempID);
                paras.Add(paraTempID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_CancelUpload]", out ds, out strError))
                    {
                        strError = "CancelUpload异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用CancelUpload异常", paramstr, strError, transactionid);
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
                                case -1:
                                    bResult = true;
                                    if (ds.Tables.Count > 1)
                                    {
                                        DataRow sdr = ds.Tables[1].Rows[0];                                        
                                        info.FilePath = Convert.ToString(sdr["FilePath"]);
                                    }
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用CancelUpload异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用CancelUpload异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用QuickUpload异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool CheckFile(
            Guid transactionid,
            Guid userid,
            ref UploadFileItemInfo info,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||TempID:{info.TempID}";
            paramstr += $"||HashCode:{info.HashCode}";
            paramstr += $"||FileName:{info.FileName}";
            paramstr += $"||ExtensionName:{info.ExtensionName}";
            paramstr += $"||FileSize:{info.FileSize}";
            paramstr += $"||ChunkIndex:{info.ChunkIndex}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraTempID = new SqlParameter("@TempID", info.TempID);
                SqlParameter paraFileName = new SqlParameter("@FileName", info.FileName);
                SqlParameter paraExtensionName = new SqlParameter("@ExtensionName", info.ExtensionName);
                SqlParameter paraFileSize = new SqlParameter("@FileSize", info.FileSize);
                SqlParameter paraChunkIndex = new SqlParameter("@ChunkIndex", info.ChunkIndex);
                SqlParameter paraUserD = new SqlParameter("@UserD", userid);
                SqlParameter paraHashCode = new SqlParameter("@HashCode", info.HashCode);

                paras.Add(paraTempID);
                paras.Add(paraFileName);
                paras.Add(paraExtensionName);
                paras.Add(paraFileSize);
                paras.Add(paraChunkIndex);
                paras.Add(paraUserD);
                paras.Add(paraHashCode);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_CheckFile]", out ds, out strError))
                    {
                        strError = "CheckFile异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用CheckFile异常", paramstr, strError, transactionid);
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
                                case 1:
                                    bResult = true;
                                    if (ds.Tables.Count > 1)
                                    {
                                        DataRow sdr = ds.Tables[1].Rows[0];
                                        info.FilePath = Convert.ToString(sdr["FilePath"]);
                                        info.ChunkIndex = Convert.ToInt64(sdr["ChunkIndex"]);
                                        info.FileSizeInt = Convert.ToInt64(sdr["FileSize"]);
                                    }
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用CheckFile异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用CheckFile异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用CheckFile异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool Upload(
            Guid transactionid,
            Guid userid,
            ref UploadFileItemInfo info,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||ChunkIndex:{info.ChunkIndex}";
            paramstr += $"||TempID:{info.TempID}";
            info.Succeed = false;
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraTempID = new SqlParameter("@TempID", info.TempID);
                SqlParameter paraChunkIndex = new SqlParameter("@ChunkIndex", info.ChunkIndex);
                paras.Add(paraTempID);
                paras.Add(paraChunkIndex);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_Upload]", out ds, out strError))
                    {
                        strError = "Upload异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用Upload异常", paramstr, strError, transactionid);
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
                                    LoggerHelper.Error("BigAttachDBProvider调用Upload异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用Upload异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用Upload异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool UploadFinish(
            Guid transactionid,
            Guid userid,
            ref UploadFileItemInfo info,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||FilePath:{info.FilePath}";
            paramstr += $"||TempID:{info.TempID}";
            info.Succeed = false;
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraTempID = new SqlParameter("@TempID", info.TempID);
                SqlParameter paraFilePath = new SqlParameter("@FilePath", info.FilePath);
                paras.Add(paraTempID);
                paras.Add(paraFilePath);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_UploadFinish]", out ds, out strError))
                    {
                        strError = "UploadFinish异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用UploadFinish异常", paramstr, strError, transactionid);
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
                                        info.FileID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                    }
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用UploadFinish异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用UploadFinish异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用UploadFinish异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetFileByID(
            Guid transactionid,
            Guid userid,
            ref BigFileItemInfo info,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||ID:{info.ID}";
            info.Succeed = false;
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraID = new SqlParameter("@ID", info.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetFileByID]", out ds, out strError))
                    {
                        strError = "GetFileByID异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用GetFileByID异常", paramstr, strError, transactionid);
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
                                        info.FileID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        info.FileName = Convert.ToString(sdr["FileName"]);
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用GetFileByID异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用GetFileByID异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用AddShare异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddShare(
            Guid transactionid,
            Guid userid,
            ref ShareInfo info,
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
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddShare]", out ds, out strError))
                    {
                        strError = "AddShare异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用AddShare异常", paramstr, strError, transactionid);
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
                                        info.ShareID = Guid.Parse(Convert.ToString(sdr["ShareID"]));
                                        info.ShortUrl = Convert.ToString(sdr["ShortUrl"]);
                                        info.ValCode = Convert.ToString(sdr["ValCode"]);
                                        info.ExpireTime = Convert.ToDateTime(sdr["ExpireTime"]);
                                    }
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用AddShare异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用GetFileByID异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用QuickUpload异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddShareFile(
            Guid transactionid,
            Guid shareid,
            Guid fileid,
            out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"shareid:{shareid}";
            paramstr += $"||fileid:{fileid}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraShareID = new SqlParameter("@ShareID", shareid);
                SqlParameter paraFileID = new SqlParameter("@FileID", fileid);
                paras.Add(paraShareID);
                paras.Add(paraFileID);
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddShareFile]", out ds, out strError))
                    {
                        strError = "AddShareFile异常,Error:" + strError;
                        LoggerHelper.Error("BigAttachDBProvider调用AddShareFile异常", paramstr, strError, transactionid);
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
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("BigAttachDBProvider调用AddShareFile异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("BigAttachDBProvider调用AddShareFile异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("BigAttachDBProvider调用AddShareFile异常", paramstr, ex.ToString(), transactionid);
            }
            return bResult;
        }
    }
}
