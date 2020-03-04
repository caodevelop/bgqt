using Common;
using Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttachforOutlookWeb.api
{
    /// <summary>
    /// Summary description for File
    /// </summary>
    public class File : IHttpHandler
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
                    Guid userid = Guid.Empty;
                    CheckCookie(context);
                    //pageIndex = 1 & pageSize = 30 & orderbyField = LastUpdateTime & orderbyType = DESC & _ = 1582705328638
                    int pageIndex = Convert.ToInt32(context.Request["pageIndex"]);
                    int pageSize = Convert.ToInt32(context.Request["pageSize"]);
                    string orderbyField = context.Request["orderbyField"];
                    string orderbyType = context.Request["orderbyType"];
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    BigFileListInfo bfli = new BigFileListInfo();
                    
                    resultinfo.data = bfli;
                    string json = JsonConvert.SerializeObject(resultinfo);
                    strJsonResult = json;
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


        private string CheckCookie(HttpContext context)
        {
            string userid = string.Empty;
            if (context.Request.Cookies["BGQTUserToken"] != null)
            {
                // return error?
            }
            else if (String.IsNullOrEmpty(context.Request.Cookies["BGQTUserToken"]["Token"]) == true)
            {
                // return error?
            }
            else
            {
                userid = context.Request.Cookies["BGQTUserToken"]["Token"];
            }
            return userid;
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