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
    /// ADManagerInterface 的摘要说明
    /// </summary>
    public class ADManagerInterface : PageBase, IHttpHandler
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
                    case "GetAccessToken":
                        strJsonResult = GetAccessToken(context);
                        break;
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
                    case "DisableAndMoveUser":
                        strJsonResult = DisableAndMoveUser(context);
                        break;
                    case "DeleteUser":
                        strJsonResult = DeleteUser(context);
                        break;
                    case "ResumeUser":
                        strJsonResult = ResumeUser(context);
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
                    case "MoveOu":
                        strJsonResult = MoveOu(context);
                        break;
                    case "SendMeeting":
                        strJsonResult = SendMeeting(context);
                        break;
                    default:
                        break;
                }
            }
            while (false);
            return strJsonResult;
        }

        private string GetAccessToken(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string appid = string.Empty;
            string secret = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "GetAccessToken";
            try
            {
                do
                {
                    appid = context.Request["appid"];
                    secret = context.Request["secret"];
                    if (string.IsNullOrEmpty(appid))
                    {
                        error.Code = ErrorCode.AppIDEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        break;
                    }
                    else if (string.IsNullOrEmpty(secret))
                    {
                        error.Code = ErrorCode.SecretEmpty;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        break;
                    }
                    RoleManager dll = new RoleManager(ClientIP);
                    dll.GetAppAccessToken(transactionid, appid, secret, out strJsonResult);
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口GetAccessToken异常", context.Request.RawUrl, ex.ToString(), transactionid);
                LoggerHelper.Info(appid, funname, context.Request.RawUrl, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }
            return strJsonResult;
        }

        #region Ou
        private string AddOu(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    OuInfo info = JsonConvert.DeserializeObject<OuInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.AddOuByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口AddOu异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }
            return strJsonResult;
        }

        private string ModifyOu(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    OuInfo info = JsonConvert.DeserializeObject<OuInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.ModifyOuByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口ModifyOuByInterface异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DeleteOu(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "DeleteOu";
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    OuInfo info = JsonConvert.DeserializeObject<OuInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.DeleteOuByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口DeleteOuByInterface异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetOuInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    OuInfo info = JsonConvert.DeserializeObject<OuInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.GetOuInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口GetOuInfo异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        #endregion

        #region User
        private string AddUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.AddUserByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口AddUser异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ChangeUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.ChangeUserByInterface(transactionid, admin, info,out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口ChangeUser异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ResetUserPassword(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.ResetUserPasswordByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口ResetUserPassword异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetUserInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.GetUserInfoByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口GetUserInfo异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }
        
        private string ChangeUserExchangeInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.ChangeUserExchangeInfoByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口ChangeUserExchangeInfo异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DisableUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.DisableUserByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口DisableUser异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string EnableUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.EnableUserByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口EnableUser异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ChangeUserMemberof(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.ChangeUserMemberofByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口ChangeUserMemberof异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DisableAndMoveUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
            string funname = "DisableAndMoveUser";
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.DisableAndMoveUserByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口DisableAndMoveUser异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DeleteUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.DeleteUserByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口DeleteUser异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ResumeUser(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
            string funname = "ResumeUser";
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    UserInfo info = JsonConvert.DeserializeObject<UserInfo>(body);

                    UserManager manager = new UserManager(ClientIP);
                    manager.ResumeUserByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口ResumeUser异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }
        #endregion

        #region Group
        private string AddGroup(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    GroupInfo info = JsonConvert.DeserializeObject<GroupInfo>(body);

                    GroupManager manager = new GroupManager(ClientIP);
                    manager.AddGroupByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口AddGroup异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string ModifyGroup(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    GroupInfo info = JsonConvert.DeserializeObject<GroupInfo>(body);

                    GroupManager manager = new GroupManager(ClientIP);
                    manager.ModifyGroupByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口ModifyGroup异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string DeleteGroup(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    GroupInfo info = JsonConvert.DeserializeObject<GroupInfo>(body);

                    GroupManager manager = new GroupManager(ClientIP);
                    manager.DeleteGroupByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口DeleteGroup异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string GetGroupInfo(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    GroupInfo info = JsonConvert.DeserializeObject<GroupInfo>(body);

                    GroupManager manager = new GroupManager(ClientIP);
                    manager.GetGroupInfo(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口GetGroupInfo异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        #endregion

        #region move
        private string MoveNodes(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "MoveNodes";
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    MoveNodeInfo info = JsonConvert.DeserializeObject<MoveNodeInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.MoveNodesByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口MoveNodes异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }

        private string MoveOu(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string funname = "MoveOu";
            string body = string.Empty;
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    MoveNodeInfo info = JsonConvert.DeserializeObject<MoveNodeInfo>(body);

                    OuManager manager = new OuManager(ClientIP);
                    manager.MoveOuByInterface(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口MoveOuByInterface异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
            }

            return strJsonResult;
        }
        #endregion

        #region SendMessage
        private string SendMeeting(HttpContext context)
        {
            string strJsonResult = string.Empty;
            string userAccount = "Interface";
            ErrorCodeInfo error = new ErrorCodeInfo();
            Guid transactionid = Guid.NewGuid();
            string body = string.Empty;
            string funname = "SendMeeting";
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
                    if (!TokenManager.ValidateAppToken(transactionid, strAccesstoken, out admin, out error))
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
                    body = System.Text.Encoding.UTF8.GetString(strArr);

                    MeetingInfo info = JsonConvert.DeserializeObject<MeetingInfo>(body);

                    SendMessageManager manager = new SendMessageManager(ClientIP);
                    manager.SendMeeting(transactionid, admin, info, out strJsonResult);

                } while (false);
            }
            catch (Exception ex)
            {
                string paramstr = string.Empty;
                paramstr += $"Url：{context.Request.RawUrl}";
                paramstr += $"||Body：{body}";
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("ADManagerInterface.ashx调用接口SendMeeting异常", paramstr, ex.ToString(), transactionid);
                LoggerHelper.Info(userAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
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