using Common;
using Entity;
using Manager;
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
    public class Setting : PageBase, IHttpHandler
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
                    case "GetUploadPar":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetUploadPar(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "GetShareSettings":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetShareSettings(context);
                        context.Response.Write(strJsonResult);
                        break;
                        
                    case "AutoLogin":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = AutoLogin(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "GetServerTime":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetServerTime(context);
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

                    //AttachResultInfo resultinfo = new AttachResultInfo();
                    //AttachSettingItem asi = new AttachSettingItem();
                    //asi.MaxFileSize = 6442450944.0;
                    //asi.MaxUploads = 5;
                    //asi.FileUploadBlackList = "ade; adp; app; asa; ashx; asmx; asp; bas; bat; cdx; cer; chm;class; cmd;com;config;cpl;crt;csh;dll;fxp;hlp;hta;htr;htw;ida;idc;idq;inf;ins;isp;its;jar;js;jse;ksh;lnk;mad;maf;mag;mam;maq;mar;mas;mat;mau;mav;maw;mda;mdb;mde;mdt;mdw;mdz;msc;msh;msh1;msh1xml;msh2;msh2xml;mshxml;msi;msp;mst;ops;pcd;pif;prf;prg;printer;pst;reg;rem;scf;scr;sct;shb;shs;shtm;shtml;soap;stm;tmp;url;vb;vbe;vbs;vsmacros;vss;vst;vsw;ws;wsc;wsf;wsh;";
                    //asi.ChunkSize = 1048576;
                    //asi.AllowDrop = true;
                    //asi.DefaultExtension = ".xdrv";
                    //resultinfo.data = asi;
                    //string json = JsonConvert.SerializeObject(resultinfo);
                    //strJsonResult = json;
                    //"{"error":null,"data":{"MaxFileSize":6442450944.0,"MaxUploads":5,"FileUploadBlackList":"ade; adp; app; asa; ashx; asmx; asp; bas; bat; cdx; cer; chm;class; cmd;com;config;cpl;crt;csh;dll;fxp;hlp;hta;htr;htw;ida;idc;idq;inf;ins;isp;its;jar;js;jse;ksh;lnk;mad;maf;mag;mam;maq;mar;mas;mat;mau;mav;maw;mda;mdb;mde;mdt;mdw;mdz;msc;msh;msh1;msh1xml;msh2;msh2xml;mshxml;msi;msp;mst;ops;pcd;pif;prf;prg;printer;pst;reg;rem;scf;scr;sct;shb;shs;shtm;shtml;soap;stm;tmp;url;vb;vbe;vbs;vsmacros;vss;vst;vsw;ws;wsc;wsf;wsh;","ChunkSize":1048576,"AllowDrop":true,"DefaultExtension":".xdrv"}}";
                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.GetGlobalUploadSetting(transactionid, userid, out strJsonResult);

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

        private string GetUploadPar(HttpContext context)
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
                    //AttachResultInfo resultinfo = new AttachResultInfo();
                    //GetUploadParItem gupi = new GetUploadParItem();
                    //gupi.OutlookFolderID = "";
                    //gupi.StorageID = "";
                    //gupi.StorageRelativePath = "";
                    //gupi.StorageUri = "";
                    //gupi.UserQuota = 2147483648.0;
                    //gupi.UserUsedQuota = 648.0;

                    //resultinfo.data = gupi;
                    //string json = JsonConvert.SerializeObject(resultinfo);
                    //strJsonResult = json;

                    //{"error":null,"data":{"StorageID":"8522ed80-7edc-4b30-9456-cd8edac0684e","StorageRelativePath":"2020-02-26","StorageUri":"\\\\172.16.6.12\\e","UserQuota":2147483648.0,"UserUsedQuota":357.0,"OutlookFolderID":"230cf621-1011-4921-8896-edd973fbda73"}}

                    Guid userid = this.CheckCookie(context);
                    if (userid == Guid.Empty)
                    {
                        //error?
                        break;
                    }
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.GetUploadParItem(transactionid, userid, out strJsonResult);
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
        private string GetShareSettings(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetShareSettings";
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
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    dll.GetShareSettings(transactionid, userid, out strJsonResult);
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("Setting.ashx调用接口GetShareSettings异常", context.Request.RawUrl, ex.ToString(), transactionid);
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
                    string email = context.Request["mailAddress"];
                    BigAttachManager dll = new BigAttachManager(ClientIP);
                    Guid userid = Guid.Empty;
                    bool result = dll.GetUserIDByEmail(transactionid, email, out userid, out strJsonResult);
                    if (result == true)
                    {
                        HttpCookie myCookie = new HttpCookie("BGQTUserToken");
                        myCookie["Token"] = userid.ToString();
                        myCookie["Account"] = email;
                        myCookie.Expires = DateTime.Now.AddHours(1);                        
                        context.Response.Cookies.Add(myCookie);
                    }
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

        private string GetServerTime(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetServerTime";
            try
            {
                do
                {
                    AttachResultInfo ar = new AttachResultInfo();
                    ar.data = DateTime.Now.ToString("yyyy-MM-dd");
                    strJsonResult = JsonHelper.Obj2Json<AttachResultInfo>(ar);// JsonConvert.SerializeObject(ar);
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