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
    /// HabBook 的摘要说明
    /// </summary>
    public class HabBook : PageBase, IHttpHandler
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
                    case "validateToken":
                        strJsonResult = validateToken(context);
                        break;
                    case "getGroupsByParentDn":
                        strJsonResult = getGroupsByParentDn(context);
                        break;
                    case "searchADObject":
                        strJsonResult = searchADObject(context);
                        break;
                    default:
                        break;
                }
            }
            while (false);
            return strJsonResult;
        }

        private string validateToken(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "validateToken";
            try
            {
                do
                {
                    userAccount = context.Request["emailAddress"];

                    UserManager manager = new UserManager(ClientIP);
                    manager.validateToken(transactionid, userAccount, out strJsonResult);
                    
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("HabBook.ashx调用接口validateToken异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string getGroupsByParentDn(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "getGroupsByParentDn";
            try
            {
                do
                {
                    string DistinguishedName = context.Request["DistinguishedName"];
                    string IsOrganizational = context.Request["IsOrganizational"];

                    GroupManager manager = new GroupManager(ClientIP);
                    manager.getGroupsByParentDn(transactionid, DistinguishedName, IsOrganizational, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("HabBook.ashx调用接口getGroupsByParentDn异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string searchADObject(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "searchADObject";
            try
            {
                do
                {
                    string searchkey = context.Request["key"];
                    string dn = context.Request["dn"];
                    int pagesize = Convert.ToInt32(context.Request["pagesize"]);
                    int pageindex = Convert.ToInt32(context.Request["pageindex"]);

                    UserManager manager = new UserManager(ClientIP);
                    manager.searchADObject(transactionid, searchkey, dn, pagesize, pageindex, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("HabBook.ashx调用接口searchADObject异常", context.Request.RawUrl, ex.ToString(), transactionid);
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