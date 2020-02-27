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
    }
}
