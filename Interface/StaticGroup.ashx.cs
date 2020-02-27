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
    /// StaticGroup 的摘要说明
    /// </summary>
    public class StaticGroup : PageBase, IHttpHandler
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
                    case "GetStaticGroupList":
                        strJsonResult = GetStaticGroupList(context);
                        break;
                    case "GetStaticGroupInfo":
                        strJsonResult = GetStaticGroupInfo(context);
                        break;
                    case "ChangeStaticGroupInfo":
                        strJsonResult = ChangeStaticGroupInfo(context);
                        break;
                    default:
                        break;
                }
            }
            while (false);
            return strJsonResult;
        }

        private string GetStaticGroupList(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetStaticGroupList";
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

                    StaticGroupManager manager = new StaticGroupManager(ClientIP);
                    manager.GetStaticGroupList(transactionid, admin, curpage, pagesize,searchstr, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("StaticGroup.ashx调用接口GetStaticGroupList异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetStaticGroupInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetStaticGroupInfo";
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

                    GroupInfo info = JsonConvert.DeserializeObject<GroupInfo>(body);

                    StaticGroupManager manager = new StaticGroupManager(ClientIP);
                    manager.GetStaticGroupInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("StaticGroup.ashx调用接口GetStaticGroupInfo异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ChangeStaticGroupInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ChangeStaticGroupInfo";
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

                    GroupInfo info = JsonConvert.DeserializeObject<GroupInfo>(body);

                    StaticGroupManager manager = new StaticGroupManager(ClientIP);
                    manager.ChangeStaticGroupInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("StaticGroup.ashx调用接口ChangeStaticGroupInfo异常", context.Request.RawUrl, ex.ToString(), transactionid);
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