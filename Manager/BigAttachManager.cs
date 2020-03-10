using Common;
using Entity;
using Newtonsoft.Json;
using Provider.DBProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class BigAttachManager
    {
        private string _clientip = string.Empty;
        public BigAttachManager(string ip)
        {
            _clientip = ip;
        }

        public bool GetUserIDByEmail(
            Guid transactionid,
            string email,
            out Guid userid,
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            userid = Guid.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"email:{email}";
            string funname = "GetUserIDByEmail";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    //AttachSettingItem asi = new AttachSettingItem();
                    AttatchUserInfo aui = new AttatchUserInfo();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.GetUserIDByEmail(transactionid, email, out userid, out error);
                    if (result == true)
                    {
                        aui.currentEmailAddress = email;
                        resultinfo.data = aui;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error($"BigAttachManager调用{funname}异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetFileList(
            Guid transactionid, 
            Guid userid, 
            int curpage, 
            int pagesize, 
            //string orderbyField, 
            //string orderbyType, 
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";            
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            //paramstr += $"||orderbyField:{orderbyField}";
            //paramstr += $"||orderbyType:{orderbyType}";
            string funname = "GetFileList";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    BigFileListInfo bfli = new BigFileListInfo();

               
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.GetFileList(transactionid, userid, curpage, pagesize, out bfli, out error);
                    if (result == true)
                    {
                        resultinfo.data = bfli;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("BigAttachManager调用GetFileList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool RenameFile(
            Guid transactionid,
            Guid userid,
            BigFileItemInfo info,
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||ID:{info.ID}";
            paramstr += $"||FileName:{info.FileName}";
            string funname = "RenameFile";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.RenameFile(transactionid, userid, ref info, out error);
                    if (result == true)
                    {
                        resultinfo.data = info;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("BigAttachManager调用GetGlobalUploadSetting异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteFile(
            Guid transactionid,
            Guid userid,
            BigFileItemInfo info,
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||ID:{info.ID}";
            string funname = "DeleteFile";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.DeleteFile(transactionid, userid, ref info, out error);
                    if (result == true)
                    {
                        //delete temp file 
                        if (info.FilePath != string.Empty)
                        {
                            FileInfo fi = new FileInfo(info.FilePath);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }
                        }
                        // to do
                        resultinfo.data = true;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        resultinfo.data = false;
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error($"BigAttachManager调用{funname}异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }


        public bool QuickUpload(
            Guid transactionid,
            Guid userid,
            BigFileItemInfo info,
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||FileName:{info.FileName}";
            paramstr += $"||HashCode:{info.HashCode}";
            string funname = "QuickUpload";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.QuickUpload(transactionid, userid, ref info, out error);
                    if (result == true)
                    {
                        resultinfo.data = info;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error($"BigAttachManager调用{funname}异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool CancelUpload(
            Guid transactionid,
            Guid userid,
            BigFileItemInfo info,
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||TempID:{info.TempID}";
            string funname = "CancelUpload";

            try
            {
                do
                {
                  
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.CancelUpload(transactionid, userid, ref info, out error);
                    if (result == true)
                    {
                        //delete temp file 
                        if (info.FilePath != string.Empty)
                        {
                            FileInfo fi = new FileInfo(info.FilePath);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }
                        }
                        // to do
                        resultinfo.data = true;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        resultinfo.data = false;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error($"BigAttachManager调用{funname}异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool Upload(
            Guid transactionid,
            Guid userid,
            UploadFileItemInfo info,          
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||FileName:{info.FileName}";
            paramstr += $"||HashCode:{info.HashCode}";
            paramstr += $"||TempID:{info.TempID}";
            paramstr += $"||TotalChunks:{info.TotalChunks}";
            paramstr += $"||ChunkIndex:{info.ChunkIndex}";
            paramstr += $"||Position:{info.Position}";
            paramstr += $"||FileSizeInt:{info.FileSizeInt}";
            string funname = "Upload";

            try
            {
                do
                {
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    UploadFileItemInfo infonow = new UploadFileItemInfo();
                    infonow.TempID = info.TempID;
                    infonow.FileName = info.FileName;
                    infonow.FileSize = info.FileSizeInt;
                    infonow.ChunkIndex = info.ChunkIndex;
                    infonow.HashCode = info.HashCode;

                    if (info.FileName.IndexOf('.') > 0)
                    {
                        info.ExtensionName = info.FileName.Substring(info.FileName.LastIndexOf('.'));
                    }

                    result = Provider.CheckFile(transactionid, userid, ref infonow, out error);
                    if(result == false)
                    {
                        //error
                        //?
                    }
                    
                    //check file exists and create
                    FileInfo finfo = new FileInfo(infonow.FilePath);
                    if (finfo.Exists == false && infonow.ChunkIndex == 0)
                    {
                        finfo.Create();
                    }
                    long sizenow = finfo.Length;
                    
                    FileStream fs = null;
                    try
                    {             
                        if(sizenow != info.Position)
                        {
                            //delete file
                            finfo.Create();
                            BigFileItemInfo deleteinfo = new BigFileItemInfo();
                            deleteinfo.TempID = infonow.TempID;
                            Provider.CancelUpload(transactionid, userid, ref deleteinfo, out error);
                            //error
                            // to do
                        }
                        else
                        {
                            fs = new FileStream(infonow.FilePath, FileMode.Append, FileAccess.Write, FileShare.Read, 1024);
                            fs.Write(info.Data, 0, info.Data.Length);
                            fs.Close();
                        }                      
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Error($"BigAttachManager调用{funname}异常", paramstr, ex.ToString(), transactionid);
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }

                    finfo.Refresh();
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    info.TempID = infonow.TempID;
                    if (finfo.Length == infonow.FileSizeInt)
                    {
                        // upload finish
                        // move file to new folder
                        string newpath = infonow.FilePath.Substring(0, infonow.FilePath.LastIndexOf("\\temp\\")) +
                            @"\" + DateTime.Now.ToString("yyyy-MM-dd") + 
                            infonow.FilePath.Substring(infonow.FilePath.LastIndexOf('\\'));

                        finfo.MoveTo(newpath);
                        info.FilePath = newpath;
                        result = Provider.UploadFinish(transactionid, userid, ref info, out error);
                        if (result == true)
                        {
                            info.Succeed = true;
                            resultinfo.data = info;
                            strJsonResult = JsonConvert.SerializeObject(resultinfo);
                            //LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);

                            result = true;
                            break;
                        }
                    }
                    else
                    {                       
                        result = Provider.Upload(transactionid, userid, ref info, out error);
                        if (result == true)
                        {
                            info.Succeed = false;
                            resultinfo.data = info;
                            strJsonResult = JsonConvert.SerializeObject(resultinfo);
                            result = true;
                            break;
                        }
                    }                   
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error($"BigAttachManager调用{funname}异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetGlobalUploadSetting(
            Guid transactionid,
            Guid userid,          
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            string funname = "GetGlobalUploadSetting";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    AttachSettingItem asi = new AttachSettingItem();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.GetGlobalUploadSetting(transactionid, userid, ref asi, out error);
                    if (result == true)
                    {
                        resultinfo.data = asi;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);                        
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("BigAttachManager调用GetGlobalUploadSetting异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetUploadParItem(
            Guid transactionid,
            Guid userid,
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            string funname = "GetUploadParItem";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    UploadParItemInfo upi = new UploadParItemInfo();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.GetUploadParItem(transactionid, userid, ref upi, out error);
                    if (result == true)
                    {
                        resultinfo.data = upi;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("BigAttachManager调用GetUploadParItem异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetShareSettings(
            Guid transactionid,
            Guid userid,
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            string funname = "GetShareSettings";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    ShareSettingsInfo ssi = new ShareSettingsInfo();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.GetShareSettings(transactionid, userid, ref ssi, out error);
                    if (result == true)
                    {
                        resultinfo.data = ssi;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error($"BigAttachManager调用{funname}异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

    }


}
