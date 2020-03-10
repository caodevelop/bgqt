using Common;
using Entity;
using Manager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AttachforOutlookWeb.api
{
    /// <summary>
    /// Summary description for File
    /// </summary>
    public class File : PageBase, IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string strResData = Operate(context);
        }

        private string Operate(HttpContext context)
        {
            string strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();

            do
            {
                string strOp = context.Request["op"];
                //判断json参数是否为空
                if (string.IsNullOrEmpty(strOp))
                {
                    error.Code = ErrorCode.JsonRequestEmpty;
                    strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                    break;
                }
                switch (strOp)
                {
                    case "GetFileList":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetFileList(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "Search":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = Search(context);
                        context.Response.Write(strJsonResult);
                        break;
                        
                    case "QuickUpload":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = QuickUpload(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "Upload":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = Upload(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "CancelUpload":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = CancelUpload(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "RenameFile":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = RenameFile(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "DeleteFile":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = DeleteFile(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "Share":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = Share(context);
                        context.Response.Write(strJsonResult);
                        break;
                        
                    default:
                        context.Response.Write(strJsonResult);
                        break;
                }
            }
            while (false);
            return strJsonResult;
        }

        private string GetFileList(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetFileList";
            try
            {
                do
                {                  
                    //pageIndex = 1 & pageSize = 30 & orderbyField = LastUpdateTime & orderbyType = DESC & _ = 1582705328638
                    int pageIndex = Convert.ToInt32(context.Request["pageIndex"]);
                    int pageSize = Convert.ToInt32(context.Request["pageSize"]);
                    string orderbyField = context.Request["orderbyField"];
                    string orderbyType = context.Request["orderbyType"];

                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.GetFileList(transactionid, userid, pageIndex, pageSize, out strJsonResult);
                    //AttachResultInfo resultinfo = new AttachResultInfo();
                    //BigFileListInfo bfli = new BigFileListInfo();
                    //bfli.pageCount = 1;
                    //bfli.recordCount = 1;
                    //bfli.files = new List<BigFileItemInfo>();
                    //BigFileItemInfo fi = new BigFileItemInfo();
                    //fi.FileName = "test.txt";
                    //fi.FileSize = 354.0;
                    //bfli.files.Add(fi);
                    //resultinfo.data = bfli;
                    //string json = JsonConvert.SerializeObject(resultinfo);
                    //strJsonResult = json;
                    //{"error":null,"data":{"files":[{"ID":"92eb58d7-f042-4c1c-8f59-50d68b6dfdec","FileName":"versions.json","ExtensionName":".json","FileSize":357.0,"UserID":"9d3ecb19-d692-47cb-b8e5-73c01b8a125c","FolderID":"230cf621-1011-4921-8896-edd973fbda73","UploadTime":"2020-02-26T16:20:50","LastUpdateTime":"2020-02-26T16:20:50","DisplayFileSize":"1 KB"}],"pageCount":1,"recordCount":1}}

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("File.ashx调用接口GetFileList异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string Search(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "Search";
            try
            {
                do
                {                    
                    int top = Convert.ToInt32(context.Request["Top"]);
                    string keyword = context.Request["keyword"];

                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.Search(transactionid, userid, keyword, top, out strJsonResult);
                    
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error($"File.ashx调用接口{funname}异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string RenameFile(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "RenameFile";
            try
            {
                do
                {
                    string id = context.Request.Form["fileID"];
                    string newname = context.Request.Form["newName"];

                    BigFileItemInfo info = new BigFileItemInfo();
                    info.ID = Guid.Parse(id);
                    info.FileName = newname;

                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.RenameFile(transactionid, userid, info, out strJsonResult);
                 
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error($"File.ashx调用接口{funname}异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DeleteFile(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "DeleteFile";
            try
            {
                do
                {
                    string ids = context.Request.Form["fileID"];
                    string[] idarr = ids.Split(',');
                    List<BigFileItemInfo> infolist = new List<BigFileItemInfo>();
                    for(int i = 0;i< idarr.Length;i++)
                    {
                        BigFileItemInfo info = new BigFileItemInfo();
                        info.ID = Guid.Parse(idarr[i]);
                        infolist.Add(info);
                    }
                            

                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.DeleteFile(transactionid, userid, infolist, out strJsonResult);
                   
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error($"File.ashx调用接口{funname}异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string Share(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "Share";
            try
            {
                do
                {
                    string ids = context.Request.Form["fileID"];
                    string[] idarr = ids.Split(',');
                    List<BigFileItemInfo> infolist = new List<BigFileItemInfo>();
                    for (int i = 0; i < idarr.Length; i++)
                    {
                        BigFileItemInfo info = new BigFileItemInfo();
                        info.ID = Guid.Parse(idarr[i]);
                        infolist.Add(info);
                    }


                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.Share(transactionid, userid, infolist, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error($"File.ashx调用接口{funname}异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string QuickUpload(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "QuickUpload";
            try
            {
                do
                {
                    string filename = context.Request.Form["FileName"];
                    string hashcode = context.Request.Form["HashCode"];

                    BigFileItemInfo info = new BigFileItemInfo();
                    info.FileName = filename;
                    info.HashCode = hashcode;

                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.QuickUpload(transactionid, userid, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error($"File.ashx调用接口{funname}异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string CancelUpload(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "CancelUpload";
            try
            {
                do
                {
                    string tempid = context.Request.Form["Identifier"];

                    BigFileItemInfo info = new BigFileItemInfo();
                    info.TempID = Guid.Parse(tempid);

                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.CancelUpload(transactionid, userid, info, out strJsonResult);
                   
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error($"File.ashx调用接口{funname}异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string Upload(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "Upload";
            try
            {
                do
                {
                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    if(context.Request.Files.Count <= 0)
                    {
                        //error?
                        break;
                    }

                    HttpPostedFile _file = context.Request.Files[0];
                    Stream stream = _file.InputStream;
                    if (stream.Length <= 0)
                    {
                        //error?
                        break;
                    }
                    byte[] filedata = new byte[stream.Length];
                    stream.Read(filedata, 0, filedata.Length);
                


                    string tempid = context.Request.Form["Identifier"];
                    string HashCode = context.Request.Form["HashCode"];
                    string TotalChunks = context.Request.Form["TotalChunks"];
                    string ChunkIndex = context.Request.Form["ChunkIndex"];
                    string FileName = context.Request.Form["FileName"];
                    string FileSize = context.Request.Form["FileSize"];
                    string Position = context.Request.Form["Position"];
                    UploadFileItemInfo info = new UploadFileItemInfo();
                    info.TempID = Guid.Parse(tempid);
                    info.TotalChunks = Convert.ToInt32(TotalChunks);
                    info.ChunkIndex = Convert.ToInt32(ChunkIndex);
                    info.FileName = FileName;
                    info.HashCode = HashCode;
                    info.FileSizeInt = Convert.ToInt32(FileSize);
                    info.Position = Convert.ToInt32(Position);
                    info.Data = filedata;
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.Upload(transactionid, userid, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error($"File.ashx调用接口{funname}异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}