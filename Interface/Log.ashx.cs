using Common;
using Entity;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interface
{
    /// <summary>
    /// Log 的摘要说明
    /// </summary>
    public class Log : PageBase, IHttpHandler
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
                    case "GetLogList":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetLogList(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "ExportLogListToExcel":
                        strJsonResult = ExportLogListToExcel(context);
                        break;
                    default:
                        context.Response.Write(strJsonResult);
                        break;
                }
            }
            while (false);
            return strJsonResult;
        }

        private string GetLogList(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetLogList";
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

                    DateTime startTime = new DateTime(2000, 1, 1);
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

                    string account = context.Request["account"];
                    string searchstr = context.Request["Searchstr"];

                    LogManager manager = new LogManager(ClientIP);
                    manager.GetLogList(transactionid, admin, curpage, pagesize, account,startTime,endTime, searchstr, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("Log.ashx调用接口GetLogList异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ExportLogListToExcel(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ExportLogListToExcel";
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

                    DateTime startTime = new DateTime(2000, 1, 1);
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

                    string account = context.Request["account"];
                    string searchstr = context.Request["Searchstr"];

                    byte[] content = null;
                    string filename = "Log_" + DateTime.Now.Ticks + ".xlsx";
                    LogManager manager = new LogManager(ClientIP);
                    manager.ExportLogListToExcel(transactionid, admin, curpage, pagesize, account, startTime, endTime, searchstr, out content, out strJsonResult);
                    context.Response.BinaryWrite(content);
                    context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    context.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);
                    context.Response.Flush();

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("Log.ashx调用接口ExportLogListToExcel异常", context.Request.RawUrl, ex.ToString(), transactionid);
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