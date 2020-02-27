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
    /// SystemReport 的摘要说明
    /// </summary>
    public class SystemReport : PageBase,IHttpHandler
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
                    case "GetNoLoginUsers":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetNoLoginUsers(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "ExportNoLoginUsersToExcel":
                        strJsonResult = ExportNoLoginUsersToExcel(context);
                        break;
                    case "GetPasswordStateUsers":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetPasswordStateUsers(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "ExportPasswordStateUsersToExcel":
                        strJsonResult = ExportPasswordStateUsersToExcel(context);
                        break;
                    case "GetDisableUsers":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetDisableUsers(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "ExportDisableUsersToExcel":
                        strJsonResult = ExportDisableUsersToExcel(context);
                        break;
                    case "GetUserCreateTime":
                        context.Response.ContentType = "text/plain";
                        strJsonResult = GetUserCreateTime(context);
                        context.Response.Write(strJsonResult);
                        break;
                    case "ExportUserCreateTimeToExcel":
                        strJsonResult = ExportUserCreateTimeToExcel(context);
                        break;
                    default:
                        break;
                }
            }
            while (false);
            return strJsonResult;
        }

        private string GetNoLoginUsers(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetNoLoginUsers";
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
                    if (!string.IsNullOrEmpty(sstartTime))
                    {
                        startTime = Convert.ToDateTime(sstartTime).Date;
                    }
                    
                    string sendTime = Convert.ToString(context.Request["endTime"]);

                    DateTime endTime = DateTime.MaxValue;
                    if (!string.IsNullOrEmpty(sendTime))
                    {
                        endTime = Convert.ToDateTime(sendTime).Date.AddDays(1).AddSeconds(-1);
                    }
                    
                    string searchstr = context.Request["Searchstr"];

                    SystemReportManager manager = new SystemReportManager(ClientIP);
                    manager.GetNoLoginUsers(transactionid, admin, curpage, pagesize, startTime, endTime, searchstr, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReport.ashx调用接口GetNoLoginUsers异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetPasswordStateUsers(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetPasswordStateUsers";
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

                    PasswordType type = (PasswordType)Convert.ToInt32(context.Request["Type"]);
                    string searchstr = context.Request["Searchstr"];

                    SystemReportManager manager = new SystemReportManager(ClientIP);
                    manager.GetPasswordStateUsers(transactionid, admin, curpage, pagesize, searchstr, type, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReport.ashx调用接口GetPasswordStateUsers异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetDisableUsers(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetDisableUsers";
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

                    SystemReportManager manager = new SystemReportManager(ClientIP);
                    manager.GetDisableUsers(transactionid, admin, curpage, pagesize, searchstr, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReport.ashx调用接口GetDisableUsers异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetUserCreateTime(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetUserCreateTime";
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

                    SystemReportManager manager = new SystemReportManager(ClientIP);
                    manager.GetUserCreateTime(transactionid, admin, curpage, pagesize, startTime, endTime, searchstr, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReport.ashx调用接口GetUserCreateTime异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ExportNoLoginUsersToExcel(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ExportNoLoginUsersToExcel";
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

                    string searchstr = context.Request["Searchstr"];

                    byte[] content = null;
                    string filename = "NoLoginUsers_" + DateTime.Now.Ticks + ".xlsx";
                    SystemReportManager manager = new SystemReportManager(ClientIP);
                    manager.ExportNoLoginUsersToExcel(transactionid, admin, curpage, pagesize, startTime, endTime, searchstr, out content, out strJsonResult);
                    context.Response.BinaryWrite(content);
                    context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    context.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReport.ashx调用接口ExportNoLoginUsersToExcel异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ExportPasswordStateUsersToExcel(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ExportPasswordStateUsersToExcel";
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

                    PasswordType type = (PasswordType)Convert.ToInt32(context.Request["Type"]);
                    string searchstr = context.Request["Searchstr"];

                    byte[] content = null;
                    string filename = "PasswordStateUsers_" + DateTime.Now.Ticks + ".xlsx";
                    SystemReportManager manager = new SystemReportManager(ClientIP);
                    manager.ExportPasswordStateUsersToExcel(transactionid, admin, curpage, pagesize, searchstr, type,out content, out strJsonResult);
                    context.Response.BinaryWrite(content);
                    context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    context.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReport.ashx调用接口ExportPasswordStateUsersToExcel异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ExportDisableUsersToExcel(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ExportDisableUsersToExcel";
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

                    byte[] content = null;
                    string filename = "DisableUsers_" + DateTime.Now.Ticks + ".xlsx";
                    SystemReportManager manager = new SystemReportManager(ClientIP);
                    manager.ExportDisableUsersToExcel(transactionid, admin, curpage, pagesize, searchstr, out content, out strJsonResult);
                    context.Response.BinaryWrite(content);
                    context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    context.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReport.ashx调用接口ExportDisableUsersToExcel异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ExportUserCreateTimeToExcel(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ExportUserCreateTimeToExcel";
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

                    byte[] content = null;
                    string filename = "UserCreateTime_" + DateTime.Now.Ticks + ".xlsx";
                    SystemReportManager manager = new SystemReportManager(ClientIP);
                    manager.ExportUserCreateTimeToExcel(transactionid, admin, curpage, pagesize, startTime, endTime, searchstr,out content, out strJsonResult);
                    context.Response.BinaryWrite(content);
                    context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    context.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("SystemReport.ashx调用接口ExportUserCreateTimeToExcel异常", context.Request.RawUrl, ex.ToString(), transactionid);
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