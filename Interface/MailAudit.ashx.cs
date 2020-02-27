using Common;
using Entity;
using Manager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Interface
{
    /// <summary>
    /// MailAudit 的摘要说明
    /// </summary>
    public class MailAudit : PageBase, IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string strResData = Operate(context);
            context.Response.Write(strResData);
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
                    case "GetMailAuditList":
                        strJsonResult = GetMailAuditList(context);
                        break;
                    case "GetMailAuditInfo":
                        strJsonResult = GetMailAuditInfo(context);
                        break;
                    case "AddMailAudit":
                        strJsonResult = AddMailAudit(context);
                        break;
                    case "DeleteMailAudit":
                        strJsonResult = DeleteMailAudit(context);
                        break;
                    case "ModifyMailAudit":
                        strJsonResult = ModifyMailAudit(context);
                        break;
                    case "GetMailAuditInfoByExchange":
                        strJsonResult = GetMailAuditInfoByExchange(context);
                        break;
                    default:
                        break;
                }
            }
            while (false);
            return strJsonResult;
        }

        private string GetMailAuditList(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetMailAuditList";
            try
            {
                do
                {
                    string strAccesstoken = context.Request["accessToken"];
                    //判断AccessToken
                    if (string.IsNullOrEmpty(strAccesstoken))
                    {
                        error.Code = ErrorCode.TokenEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    AdminInfo admin = new AdminInfo();
                    if (!TokenManager.ValidateUserToken(transactionid, strAccesstoken, out admin, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    userAccount = admin.UserAccount;
                    string pagesizeStr = context.Request["PageSize"];
                    if (string.IsNullOrEmpty(pagesizeStr))
                    {
                        error.Code = ErrorCode.PageSizeEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }
                    int pagesize = 0;
                    if (!Int32.TryParse(pagesizeStr, out pagesize))
                    {
                        error.Code = ErrorCode.PageSizeIllegal;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }
                    else
                    {
                        pagesize = Convert.ToInt32(pagesizeStr);
                    }

                    string curpageStr = context.Request["CurPage"];
                    if (string.IsNullOrEmpty(curpageStr))
                    {
                        error.Code = ErrorCode.CurPageEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }
                    int curpage = 0;
                    if (!Int32.TryParse(curpageStr, out curpage))
                    {
                        error.Code = ErrorCode.CurPageIllegal;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }
                    else
                    {
                        curpage = Convert.ToInt32(curpageStr);
                    }

                    string searchstr = context.Request["Searchstr"];

                    MailAuditManager manager = new MailAuditManager(ClientIP);
                    manager.GetMailAuditList(transactionid, admin, curpage, pagesize, searchstr, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAudit.ashx调用接口GetMailAuditList异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetMailAuditInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetMailAuditInfo";
            try
            {
                do
                {
                    string strAccesstoken = context.Request["accessToken"];
                    //判断AccessToken
                    if (string.IsNullOrEmpty(strAccesstoken))
                    {
                        error.Code = ErrorCode.TokenEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    AdminInfo admin = new AdminInfo();
                    if (!TokenManager.ValidateUserToken(transactionid, strAccesstoken, out admin, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    Stream str = context.Request.InputStream;
                    // Find number of bytes in stream.
                    Int32 strLen = Convert.ToInt32(str.Length);
                    // Create a byte array.
                    byte[] strArr = new byte[strLen];
                    // Read stream into byte array.
                    str.Read(strArr, 0, strLen);
                    string body = System.Text.Encoding.UTF8.GetString(strArr);

                    MailAuditInfo info = JsonConvert.DeserializeObject<MailAuditInfo>(body);

                    MailAuditManager manager = new MailAuditManager(ClientIP);
                    manager.GetMailAuditInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAudit.ashx调用接口GetMailAuditInfo异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string AddMailAudit(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "AddMailAudit";
            try
            {
                do
                {
                    string strAccesstoken = context.Request["accessToken"];
                    //判断AccessToken
                    if (string.IsNullOrEmpty(strAccesstoken))
                    {
                        error.Code = ErrorCode.TokenEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    AdminInfo admin = new AdminInfo();
                    if (!TokenManager.ValidateUserToken(transactionid, strAccesstoken, out admin, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    Stream str = context.Request.InputStream;
                    // Find number of bytes in stream.
                    Int32 strLen = Convert.ToInt32(str.Length);
                    // Create a byte array.
                    byte[] strArr = new byte[strLen];
                    // Read stream into byte array.
                    str.Read(strArr, 0, strLen);
                    string body = System.Text.Encoding.UTF8.GetString(strArr);

                    MailAuditInfo info = JsonConvert.DeserializeObject<MailAuditInfo>(body);

                    MailAuditManager manager = new MailAuditManager(ClientIP);
                    manager.AddMailAudit(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAudit.ashx调用接口AddMailAudit异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DeleteMailAudit(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "DeleteMailAudit";
            try
            {
                do
                {
                    string strAccesstoken = context.Request["accessToken"];
                    //判断AccessToken
                    if (string.IsNullOrEmpty(strAccesstoken))
                    {
                        error.Code = ErrorCode.TokenEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    AdminInfo admin = new AdminInfo();
                    if (!TokenManager.ValidateUserToken(transactionid, strAccesstoken, out admin, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    Stream str = context.Request.InputStream;
                    // Find number of bytes in stream.
                    Int32 strLen = Convert.ToInt32(str.Length);
                    // Create a byte array.
                    byte[] strArr = new byte[strLen];
                    // Read stream into byte array.
                    str.Read(strArr, 0, strLen);
                    string body = System.Text.Encoding.UTF8.GetString(strArr);

                    MailAuditInfo info = JsonConvert.DeserializeObject<MailAuditInfo>(body);

                    MailAuditManager manager = new MailAuditManager(ClientIP);
                    manager.DeleteMailAudit(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAudit.ashx调用接口DeleteMailAudit异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ModifyMailAudit(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ModifyMailAudit";
            try
            {
                do
                {
                    string strAccesstoken = context.Request["accessToken"];
                    //判断AccessToken
                    if (string.IsNullOrEmpty(strAccesstoken))
                    {
                        error.Code = ErrorCode.TokenEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    AdminInfo admin = new AdminInfo();
                    if (!TokenManager.ValidateUserToken(transactionid, strAccesstoken, out admin, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    Stream str = context.Request.InputStream;
                    // Find number of bytes in stream.
                    Int32 strLen = Convert.ToInt32(str.Length);
                    // Create a byte array.
                    byte[] strArr = new byte[strLen];
                    // Read stream into byte array.
                    str.Read(strArr, 0, strLen);
                    string body = System.Text.Encoding.UTF8.GetString(strArr);

                    MailAuditInfo info = JsonConvert.DeserializeObject<MailAuditInfo>(body);

                    MailAuditManager manager = new MailAuditManager(ClientIP);
                    manager.ModifyMailAudit(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAudit.ashx调用接口ModifyMailAudit异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetMailAuditInfoByExchange(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetMailAuditInfoByExchange";
            try
            {
                do
                {
                    string strAccesstoken = context.Request["accessToken"];
                    //判断AccessToken
                    if (string.IsNullOrEmpty(strAccesstoken))
                    {
                        error.Code = ErrorCode.TokenEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    AdminInfo admin = new AdminInfo();
                    if (!TokenManager.ValidateUserToken(transactionid, strAccesstoken, out admin, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                        break;
                    }

                    Stream str = context.Request.InputStream;
                    // Find number of bytes in stream.
                    Int32 strLen = Convert.ToInt32(str.Length);
                    // Create a byte array.
                    byte[] strArr = new byte[strLen];
                    // Read stream into byte array.
                    str.Read(strArr, 0, strLen);
                    string body = System.Text.Encoding.UTF8.GetString(strArr);

                    MailAuditInfo info = JsonConvert.DeserializeObject<MailAuditInfo>(body);

                    MailAuditManager manager = new MailAuditManager(ClientIP);
                    manager.GetMailAuditInfoByExchange(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MailAudit.ashx调用接口GetMailAuditInfoByExchange异常", context.Request.RawUrl, ex.ToString(), transactionid);
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