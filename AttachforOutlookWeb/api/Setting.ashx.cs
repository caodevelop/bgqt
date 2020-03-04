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
    /// Summary description for Setting
    /// </summary>
    public class Setting : IHttpHandler
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
                    case "getGlobalUploadSetting":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetGlobalUploadSetting(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "GetUploadBar":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetUploadBar(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "AutoLogin":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = AutoLogin(context);
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

        private string GetGlobalUploadSetting(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetGlobalUploadSetting";
            try
            {
                do
                {

                    AttachResultInfo resultinfo = new AttachResultInfo();
                    AttachSettingItem asi = new AttachSettingItem();
                    asi.MaxFileSize = 6442450944.0;
                    asi.MaxUploads = 5;
                    asi.FileUploadBlackList = "ade; adp; app; asa; ashx; asmx; asp; bas; bat; cdx; cer; chm;class; cmd;com;config;cpl;crt;csh;dll;fxp;hlp;hta;htr;htw;ida;idc;idq;inf;ins;isp;its;jar;js;jse;ksh;lnk;mad;maf;mag;mam;maq;mar;mas;mat;mau;mav;maw;mda;mdb;mde;mdt;mdw;mdz;msc;msh;msh1;msh1xml;msh2;msh2xml;mshxml;msi;msp;mst;ops;pcd;pif;prf;prg;printer;pst;reg;rem;scf;scr;sct;shb;shs;shtm;shtml;soap;stm;tmp;url;vb;vbe;vbs;vsmacros;vss;vst;vsw;ws;wsc;wsf;wsh;";
                    asi.ChunkSize = 1048576;
                    asi.AllowDrop = true;
                    asi.DefaultExtension = ".xdrv";
                    resultinfo.data = asi;
                    string json = JsonConvert.SerializeObject(resultinfo);
                    strJsonResult = json;
                    //"{"error":null,"data":{"MaxFileSize":6442450944.0,"MaxUploads":5,"FileUploadBlackList":"ade; adp; app; asa; ashx; asmx; asp; bas; bat; cdx; cer; chm;class; cmd;com;config;cpl;crt;csh;dll;fxp;hlp;hta;htr;htw;ida;idc;idq;inf;ins;isp;its;jar;js;jse;ksh;lnk;mad;maf;mag;mam;maq;mar;mas;mat;mau;mav;maw;mda;mdb;mde;mdt;mdw;mdz;msc;msh;msh1;msh1xml;msh2;msh2xml;mshxml;msi;msp;mst;ops;pcd;pif;prf;prg;printer;pst;reg;rem;scf;scr;sct;shb;shs;shtm;shtml;soap;stm;tmp;url;vb;vbe;vbs;vsmacros;vss;vst;vsw;ws;wsc;wsf;wsh;","ChunkSize":1048576,"AllowDrop":true,"DefaultExtension":".xdrv"}}";

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("Setting.ashx调用接口GetGlobalUploadSetting异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetUploadBar(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetUploadBar";
            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    GetUploadParItem gupi = new GetUploadParItem();
                    gupi.OutlookFolderID = "";
                    gupi.StorageID = "";
                    gupi.StorageRelativePath = "";
                    gupi.StorageUri = "";
                    gupi.UserQuota = 2147483648.0;
                    gupi.UserUsedQuota = 648.0;

                    resultinfo.data = gupi;
                    string json = JsonConvert.SerializeObject(resultinfo);
                    strJsonResult = json;
                    
                    //{"error":null,"data":{"StorageID":"8522ed80-7edc-4b30-9456-cd8edac0684e","StorageRelativePath":"2020-02-26","StorageUri":"\\\\172.16.6.12\\e","UserQuota":2147483648.0,"UserUsedQuota":357.0,"OutlookFolderID":"230cf621-1011-4921-8896-edd973fbda73"}}
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("Setting.ashx调用接口GetUploadBar异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string AutoLogin(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "AutoLogin";
            try
            {
                do
                {
                    HttpCookie myCookie = new HttpCookie("BGQTUserToken");
                    myCookie["Token"] = Guid.NewGuid().ToString();
                    myCookie.Expires = DateTime.Now.AddHours(1);
                    context.Response.Cookies.Add(myCookie);
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("Setting.ashx调用接口AutoLogin异常", context.Request.RawUrl, ex.ToString(), transactionid);
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