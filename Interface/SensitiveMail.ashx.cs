using Entity;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Manager;
using System.IO;
using Newtonsoft.Json;

namespace Interface
{
    /// <summary>
    /// SensitiveMail 的摘要说明
    /// </summary>
    public class SensitiveMail : PageBase, IHttpHandler
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
                    case "GetSensitiveMailList":
                        strJsonResult = GetSensitiveMailList(context);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(strJsonResult);
                        break;
                    case "GetSensitiveMailInfo":
                        strJsonResult = GetSensitiveMailInfo(context);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(strJsonResult);
                        break;
                    case "AddSensitiveMail":
                        strJsonResult = AddSensitiveMail(context);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(strJsonResult);
                        break;
                    case "DeleteSensitiveMail":
                        strJsonResult = DeleteSensitiveMail(context);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(strJsonResult);
                        break;
                    case "ExecuteSensitiveMail":
                        strJsonResult = ExecuteSensitiveMail(context);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(strJsonResult);
                        break;
                    case "ModifySensitiveMail":
                        strJsonResult = ModifySensitiveMail(context);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(strJsonResult);
                        break;
                    case "Test":
                        strJsonResult = Test(context);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(strJsonResult);
                        break;
                    case "ExportToExcel":
                        strJsonResult = ExportToExcel(context);
                        break;
                    default:
                        break;
                }
            }
            while (false);
            return strJsonResult;
        }

        private string GetSensitiveMailList(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetSensitiveMailList";
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

                    string sstartTime = Convert.ToString(context.Request["startTime"]);

                    DateTime startTime = new DateTime(1900, 1, 1);
                    if (sstartTime != string.Empty)
                    {
                        startTime = Convert.ToDateTime(sstartTime).Date;
                    }

                    string sendTime = Convert.ToString(context.Request["endTime"]);

                    DateTime endTime = DateTime.MaxValue;
                    if (sendTime != string.Empty)
                    {
                        endTime = Convert.ToDateTime(sendTime).Date.AddDays(1).AddSeconds(-1);
                    }

                    string searchstr = context.Request["Searchstr"];

                    SensitiveMailManager manager = new SensitiveMailManager(ClientIP);
                    manager.GetSensitiveMailList(transactionid, admin, curpage, pagesize,startTime,endTime, searchstr, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMail.ashx调用接口GetSensitiveMailList异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetSensitiveMailInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetSensitiveMailInfo";
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

                    SensitiveMailInfo info = JsonConvert.DeserializeObject<SensitiveMailInfo>(body);

                    SensitiveMailManager manager = new SensitiveMailManager(ClientIP);
                    manager.GetSensitiveMailInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMail.ashx调用接口GetSensitiveMailInfo异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string AddSensitiveMail(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "AddSensitiveMail";
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

                    SensitiveMailInfo info = JsonConvert.DeserializeObject<SensitiveMailInfo>(body);

                    SensitiveMailManager manager = new SensitiveMailManager(ClientIP);
                    manager.AddSensitiveMail(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMail.ashx调用接口AddSensitiveMail异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DeleteSensitiveMail(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "DeleteSensitiveMail";
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

                    SensitiveMailInfo info = JsonConvert.DeserializeObject<SensitiveMailInfo>(body);

                    SensitiveMailManager manager = new SensitiveMailManager(ClientIP);
                    manager.DeleteSensitiveMail(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMail.ashx调用接口DeleteSensitiveMail异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ExecuteSensitiveMail(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ExecuteSensitiveMail";
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

                    SensitiveMailInfo info = JsonConvert.DeserializeObject<SensitiveMailInfo>(body);

                    SensitiveMailManager manager = new SensitiveMailManager(ClientIP);
                    manager.ExecuteSensitiveMail(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMail.ashx调用接口ExecuteSensitiveMail异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ModifySensitiveMail(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ModifySensitiveMail";
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

                    SensitiveMailInfo info = JsonConvert.DeserializeObject<SensitiveMailInfo>(body);

                    SensitiveMailManager manager = new SensitiveMailManager(ClientIP);
                    manager.ModifySensitiveMail(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMail.ashx调用接口ModifySensitiveMail异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ExportToExcel(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ExportToExcel";
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
                    Guid sensitiveMailID = Guid.Parse(context.Request["ID"]);

                    byte[] content = null;
                    string filename = "SensitiveMail_" + DateTime.Now.Ticks + ".xlsx";
                    SensitiveMailManager manager = new SensitiveMailManager(ClientIP);
                    manager.ExportToExcel(transactionid, admin, sensitiveMailID, out content, out strJsonResult);

                    context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    context.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);
                    context.Response.BinaryWrite(content);
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMail.ashx调用接口ExportToExcel异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }
            return strJsonResult;
        }

        private string Test(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "Test";
            try
            {
                do
                {
                    //string strCommand = "set-Mailbox -Identity \"CN=cs6121,OU=B集团控股,OU=佳兆业集团,DC=bnc2,DC=cn\" -ExtensionCustomAttribute3 \"11111\"";
                    string strCommand = $"Search-Mailbox -Identity \"CN=cs6121,OU=B集团控股,OU=佳兆业集团,DC=bnc2,DC=cn\" " +
                        $"-SearchQuery 'subject:\"test111\"  AND sent>=\"2019-07-04 00:00:01\" AND sent<=\"2019-07-06 00:00:01\"'  -DeleteContent -Force";
                    Log4netHelper.Info(strCommand);
                    SensitiveMailManager manager = new SensitiveMailManager(ClientIP);
                    manager.test(transactionid, strCommand, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SensitiveMail.ashx调用接口test异常", context.Request.RawUrl, ex.ToString(), transactionid);
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