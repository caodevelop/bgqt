using Common;
using DBUtility;
using Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.DBProvider
{
    public class UserDBProvider
    {
        public bool ModifyUser(Guid transactionid, AdminInfo admin, UserInfo user, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||UserDisplayName:{user.DisplayName}";
            paramstr += $"||UserFirstName:{user.FirstName}";
            paramstr += $"||UserLastName:{user.LastName}";
            paramstr += $"||DistinguishedName:{user.DistinguishedName}";

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", user.UserID);
                paras.Add(paraUserID);
                SqlParameter paraUserAccount = new SqlParameter("@UserAccount", user.UserAccount);
                paras.Add(paraUserAccount);
                SqlParameter paraDisplayName = new SqlParameter("@DisplayName", user.DisplayName);
                paras.Add(paraDisplayName);
                SqlParameter paraFirstName = new SqlParameter("@FirstName", user.FirstName);
                paras.Add(paraFirstName);
                SqlParameter paraLastName = new SqlParameter("@LastName", user.LastName);
                paras.Add(paraLastName);
                SqlParameter paraDistinguishedName = new SqlParameter("@DistinguishedName", user.DistinguishedName);
                paras.Add(paraDistinguishedName);
               
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_ModifyUser]", out ds, out strError))
                    {
                        strError = "ModifyUser异常,Error:" + strError;
                        LoggerHelper.Error("UserDBProvider调用ModifyUser异常", paramstr, strError, transactionid);
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int iResult = 0;
                            iResult = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            switch (iResult)
                            {
                                case 1:
                                    bResult = true;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("UserDBProvider调用ModifyUser异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("UserDBProvider调用ModifyUser异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("UserDBProvider调用ModifyUser异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool DeleteUser(Guid transactionid, AdminInfo admin, UserInfo user, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
         
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", user.UserID);
                paras.Add(paraUserID);
              
                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DeleteUser]", out ds, out strError))
                    {
                        strError = "DeleteUser异常,Error:" + strError;
                        LoggerHelper.Error("UserDBProvider调用DeleteUser异常", paramstr, strError, transactionid);
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int iResult = 0;
                            iResult = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            switch (iResult)
                            {
                                case 1:
                                    bResult = true;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("UserDBProvider调用DeleteUser异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("UserDBProvider调用DeleteUser异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("UserDBProvider调用DeleteUser异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool SendFUserMobileCode(Guid transactionid, Guid userid, string mobile, string code, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||mobile:{mobile}";
            paramstr += $"||code:{code}";
            string strError = string.Empty;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", userid);
                SqlParameter paraMobile = new SqlParameter("@Mobile", mobile);
                SqlParameter paraCode = new SqlParameter("@Code", code);
                paras.Add(paraUserID);
                paras.Add(paraMobile);
                paras.Add(paraCode);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_SendFUserMobileCode]", out ds, out strError))
                    {
                        strError = "prc_SendFUserMobileCode数据库执行失败,Error:" + strError;
                        LoggerHelper.Error("UserDBProvider调用SendFUserMobileCode异常", paramstr, strError, transactionid);
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }
                    else
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int iResult = 0;
                            iResult = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            switch (iResult)
                            {
                                case 0:
                                    bResult = true;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("UserDBProvider调用SendFUserMobileCode异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                    }
                } while (false);

            }
            catch (Exception ex)
            {
                bResult = false;
                LoggerHelper.Error("UserDBProvider调用SendFUserMobileCode异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
            }
            return bResult;
        }

        public bool CheckFUserSmsCode(Guid transactionid, Guid userid, string code, out Guid codeid, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            paramstr += $"||code:{code}";
            string strError = string.Empty;
            codeid = Guid.Empty;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", userid);
                SqlParameter paraCode = new SqlParameter("@Code", code);
                paras.Add(paraUserID);
                paras.Add(paraCode);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_CheckFUserSmsCode]", out ds, out strError))
                    {
                        strError = "prc_CheckFUserSmsCode数据库执行失败,Error:" + strError;
                        LoggerHelper.Error("UserDBProvider调用CheckFUserSmsCode异常", paramstr, strError, transactionid);
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }
                    else
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int iResult = 0;
                            iResult = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            switch (iResult)
                            {
                                case 0:
                                    bResult = true;
                                    break;
                                case -3:
                                    bResult = false;
                                    error.Code = ErrorCode.SmsCodeWrong;
                                    break;
                                case -4:
                                    bResult = false;
                                    error.Code = ErrorCode.SmsCodeExpired;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("UserDBProvider调用CheckFUserSmsCode异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                        if (ds.Tables.Count > 1)
                        {
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {
                                DataRow sdr = ds.Tables[1].Rows[i];
                                codeid = Guid.Parse(Convert.ToString(sdr["ID"]));
                            }
                        }
                    }
                } while (false);

            }
            catch (Exception ex)
            {
                bResult = false;
                LoggerHelper.Error("UserDBProvider调用CheckFUserSmsCode异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
            }
            return bResult;
        }

        public bool CheckFUserSmsByCodeID(Guid transactionid, Guid codeid, out Guid userid, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"codeid:{codeid}";
            string strError = string.Empty;
            userid = Guid.Empty;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraCodeID = new SqlParameter("@CodeID", codeid);
                paras.Add(paraCodeID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_CheckFUserSmsByCodeID]", out ds, out strError))
                    {
                        strError = "prc_CheckFUserSmsByCodeID数据库执行失败,Error:" + strError;
                        LoggerHelper.Error("UserDBProvider调用CheckFUserSmsByCodeID异常", paramstr, strError, transactionid);
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }
                    else
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int iResult = 0;
                            iResult = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            switch (iResult)
                            {
                                case 0:
                                    bResult = true;
                                    break;
                                case -3:
                                    bResult = false;
                                    error.Code = ErrorCode.SmsCodeWrong;
                                    break;
                                case -4:
                                    bResult = false;
                                    error.Code = ErrorCode.SmsCodeExpired;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("UserDBProvider调用CheckFUserSmsByCodeID异常", paramstr, "-9999", transactionid);
                                    break;
                                default:
                                    bResult = false;
                                    error.Code = ErrorCode.Exception;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            error.Code = ErrorCode.Exception;
                            break;
                        }
                        if (ds.Tables.Count > 1)
                        {
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {
                                DataRow sdr = ds.Tables[1].Rows[i];
                                userid = Guid.Parse(Convert.ToString(sdr["UserID"]));
                            }
                        }
                    }
                } while (false);

            }
            catch (Exception ex)
            {
                bResult = false;
                LoggerHelper.Error("UserDBProvider调用CheckFUserSmsByCodeID异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
            }
            return bResult;
        }
    }
}
