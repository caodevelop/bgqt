using Entity;
using Manager;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interface
{
    /// <summary>
    /// ShowImg 的摘要说明
    /// </summary>
    public class ShowImg : PageBase,IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strJsonResult = string.Empty;
            AdminInfo admin = new AdminInfo();
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            try
            {
                string showType = context.Request["type"].ToLower();

                byte[] photo = null;
                switch (showType)
                {
                    case "codeimg":
                        //bool result = false;
                        //string errorinfo = string.Empty;
                        UserManager cmdll = new UserManager(ClientIP);
                        string keystr = context.Request["Key"];
                        string code = string.Empty;

                        Guid key = new Guid();
                        if (!Guid.TryParse(keystr, out key))
                        {
                            error.Code = ErrorCode.ValKeyEmpty;
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            context.Response.Write(strJsonResult);
                            return;
                        }

                        if (HttpContext.Current.Cache[key.ToString()] == null)
                        {
                            cmdll.GetValCode(transactionid, out code, out strJsonResult);
                            HttpContext.Current.Cache.Insert(key.ToString(), code, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(300));
                        }
                        else
                        {
                            cmdll.GetValCode(transactionid, out code, out strJsonResult);
                            HttpContext.Current.Cache.Insert(key.ToString(), code, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(300));
                            //code = HttpContext.Current.Cache[key.ToString()].ToString();
                        }


                        cmdll.GetValImg(transactionid, code, out photo, out strJsonResult);
                        if (photo != null)
                        {
                            context.Response.ContentType = "image/jpeg";
                            context.Response.BinaryWrite(photo);//读取
                            context.Response.Flush();
                        }
                        else
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write("用户没有权限");
                            return;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("ShowImg.ashx调用接口ProcessRequest异常", context.Request.RawUrl, ex.ToString(), transactionid);
            }
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