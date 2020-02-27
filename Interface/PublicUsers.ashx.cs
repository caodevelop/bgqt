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
    /// PublicUsers 的摘要说明
    /// </summary>
    public class PublicUsers : PageBase,IHttpHandler
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
                    case "AddOu":
                        strJsonResult = AddOu(context);
                        break;
                    case "ModifyOu":
                        strJsonResult = ModifyOu(context);
                        break;
                    case "DeleteOu":
                        strJsonResult = DeleteOu(context);
                        break;
                    case "GetOuInfo":
                        strJsonResult = GetOuInfo(context);
                        break;
                    case "MoveOu":
                        strJsonResult = MoveOu(context);
                        break;
                    case "AddUser":
                        strJsonResult = AddUser(context);
                        break;
                    case "ChangeUser":
                        strJsonResult = ChangeUser(context);
                        break;
                    case "ResetUserPassword":
                        strJsonResult = ResetUserPassword(context);
                        break;
                    case "GetUserInfo":
                        strJsonResult = GetUserInfo(context);
                        break;
                    case "GetUserExchangeInfo":
                        strJsonResult = GetUserExchangeInfo(context);
                        break;
                    case "ChangeUserExchangeInfo":
                        strJsonResult = ChangeUserExchangeInfo(context);
                        break;
                    case "DisableUser":
                        strJsonResult = DisableUser(context);
                        break;
                    case "EnableUser":
                        strJsonResult = EnableUser(context);
                        break;
                    case "ChangeUserMemberof":
                        strJsonResult = ChangeUserMemberof(context);
                        break;
                    case "DeleteUser":
                        strJsonResult = DeleteUser(context);
                        break;
                    case "AddGroup":
                        strJsonResult = AddGroup(context);
                        break;
                    case "ModifyGroup":
                        strJsonResult = ModifyGroup(context);
                        break;
                    case "DeleteGroup":
                        strJsonResult = DeleteGroup(context);
                        break;
                    case "GetGroupInfo":
                        strJsonResult = GetGroupInfo(context);
                        break;
                    case "MoveNodes":
                        strJsonResult = MoveNodes(context);
                        break;
                    default:
                        break;
                }
            }
            while (false);
            return strJsonResult;
        }
        #region ou
        private string AddOu(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "AddOu";
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
                    if (!TokenManager.ValidateUserToken1(transactionid, strAccesstoken, out admin, out error))
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

                    OuInfo info = JsonConvert.DeserializeObject<OuInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.AddOuNoHab(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口AddOu异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ModifyOu(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ModifyOu";
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

                    OuInfo info = JsonConvert.DeserializeObject<OuInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.ModifyOuNoHab(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口ModifyOu异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DeleteOu(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "DeleteOu";
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
                    if (!TokenManager.ValidateUserToken1(transactionid, strAccesstoken, out admin, out error))
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

                    OuInfo info = JsonConvert.DeserializeObject<OuInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.DeleteOuNoHab(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口DeleteOuNoHab异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetOuInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetOuInfo";
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

                    OuInfo info = JsonConvert.DeserializeObject<OuInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.GetOuInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口GetOuInfo异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string MoveOu(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "MoveOu";
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

                    MoveNodeInfo info = JsonConvert.DeserializeObject<MoveNodeInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.MoveOuNoHab(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口MoveOu异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string MoveNodes(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "MoveNodes";
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

                    MoveNodeInfo info = JsonConvert.DeserializeObject<MoveNodeInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.MoveNodesNoHab(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口MoveNodes异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }
        #endregion
        #region user
        public string AddUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "AddUser";
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
                    if (!TokenManager.ValidateUserToken1(transactionid, strAccesstoken, out admin, out error))
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.AddUserNoHab(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口AddUser异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ChangeUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ChangeUser";
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.ChangeUser(transactionid, admin, info, true, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ChangeUser.ashx调用接口ChangeUser异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ResetUserPassword(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ResetUserPassword";
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.ResetUserPassword(transactionid, admin, info, true, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口ResetUserPassword异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetUserInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetUserInfo";
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.GetUserInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口GetUserInfo异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetUserExchangeInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetUserExchangeInfo";
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.GetUserExchangeInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口GetUserExchangeInfo异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ChangeUserExchangeInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ChangeUserExchangeInfo";
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.ChangeUserExchangeInfo(transactionid, admin, info, true, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口ChangeUserExchangeInfo异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DisableUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "DisableUser";
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);
                    info.UserStatus = false;

                    UserManager manager = new UserManager(ClientIP);
                    manager.DisableUser(transactionid, admin, info, true, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口DisableUser异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string EnableUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "EnableUser";
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);
                    info.UserStatus = true;

                    UserManager manager = new UserManager(ClientIP);
                    manager.EnableUser(transactionid, admin, info, true, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("EnableUser.ashx调用接口EnableUser异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ChangeUserMemberof(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ChangeUserMemberof";
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.ChangeUserMemberof(transactionid, admin, info, true, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口ChangeUserMemberof异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DeleteUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "DeleteUser";
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
                    if (!TokenManager.ValidateUserToken1(transactionid, strAccesstoken, out admin, out error))
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

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.DeleteUserNoHab(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口DeleteUser异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        #endregion
        #region group
        private string AddGroup(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "AddGroup";
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

                    GroupManager manager = new GroupManager(ClientIP);
                    manager.AddGroupNoHab(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口AddGroup异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ModifyGroup(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "ModifyGroup";
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

                    GroupManager manager = new GroupManager(ClientIP);
                    manager.ModifyGroup(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口ModifyGroup异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DeleteGroup(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "DeleteGroup";
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

                    GroupManager manager = new GroupManager(ClientIP);
                    manager.DeleteGroup(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口DeleteGroup异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetGroupInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetGroupInfo";
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

                    GroupManager manager = new GroupManager(ClientIP);
                    manager.GetGroupInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("PublicUsers.ashx调用接口GetGroupInfo异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }
        #endregion
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}