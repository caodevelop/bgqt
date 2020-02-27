using Entity;
using Common;
using Provider.DBProvider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Provider.ADProvider;
using System.DirectoryServices;

namespace Manager
{
    public class TokenManager
    {
        public static bool ValidateToken(string strToken, out AdminInfo admin, out ErrorCodeInfo err)
        {
            bool result = true;
            admin = new AdminInfo();
            Guid userid = Guid.Empty;
            err = new ErrorCodeInfo();

            ArrayList tokenList = new ArrayList();
            int valresult = ValidatorHelper.ValToken(strToken, out tokenList);
            if (valresult == 0)
            {
                if (tokenList.Count == 2)
                {
                    if (Guid.TryParse(Convert.ToString(tokenList[1]), out userid))
                    {
                        admin.UserID = Guid.Parse(Convert.ToString(tokenList[1]));
                    }
                    else
                    {
                        err.Code = ErrorCode.TokenIllegal;
                        result = false;
                    }
                }
                else
                {
                    err.Code = ErrorCode.TokenIllegal;
                    result = false;
                }
            }
            else if (valresult == 1)
            {
                err.Code = ErrorCode.TokenIllegal;
                result = false;
            }
            else if (valresult == 2)
            {
                err.Code = ErrorCode.TokenExpire;
                result = false;
            }

            return result;
        }

        public static bool ValidateUserToken(Guid transactionid, string strToken, out AdminInfo admin, out ErrorCodeInfo error)
        {
            bool result = true;
            admin = new AdminInfo();
            string message = string.Empty;
            Guid userid = Guid.Empty;
            error = new ErrorCodeInfo();

            ArrayList tokenList = new ArrayList();
            do
            {
                int valresult = ValidatorHelper.ValToken(strToken,3600*24, out tokenList);
                if (valresult == 0)
                {
                    if (tokenList.Count == 2)
                    {
                        if (Guid.TryParse(Convert.ToString(tokenList[1]), out userid))
                        {
                            admin.UserID = Guid.Parse(Convert.ToString(tokenList[1]));

                            DirectoryEntry entry = new DirectoryEntry();
                            CommonProvider commonProvider = new CommonProvider();
                            if (!commonProvider.GetADEntryByGuid(admin.UserID, out entry, out message))
                            {
                                result = false;
                                break;
                            }

                            admin.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                            admin.SAMAccountName = entry.Properties["sAMAccountName"].Value == null ? "" : Convert.ToString(entry.Properties["sAMAccountName"].Value);
                            admin.DisplayName = entry.Properties["cn"].Value == null ? "" : Convert.ToString(entry.Properties["cn"].Value);

                            RoleDBProvider roleDBProvider = new RoleDBProvider();
                            if (!roleDBProvider.GetUserRole(transactionid, admin.UserID, ref admin, out error))
                            {
                                result = false;
                                break;
                            }
                        }
                        else
                        {
                            error.Code = ErrorCode.TokenIllegal;
                            result = false;
                            break;
                        }
                    }
                    else
                    {
                        error.Code = ErrorCode.TokenIllegal;
                        result = false;
                        break;
                    }
                }
                else if (valresult == 1)
                {
                    error.Code = ErrorCode.TokenIllegal;
                    result = false;
                    break;
                }
                else if (valresult == 2)
                {
                    error.Code = ErrorCode.TokenExpire;
                    result = false;
                    break;
                }
            } while (false);
            return result;
        }

        public static bool ValidateUserToken1(Guid transactionid, string strToken, out AdminInfo admin, out ErrorCodeInfo error)
        {
            bool result = true;
            admin = new AdminInfo();
            string message = string.Empty;
            Guid userid = Guid.Empty;
            error = new ErrorCodeInfo();

            ArrayList tokenList = new ArrayList();
            do
            {
                int valresult = ValidatorHelper.ValToken(strToken, 3600 * 24, out tokenList);
                string snkey = ConfigHelper.ConfigInstance["SNKey"];
                if (valresult == 0)
                {
                    if (tokenList.Count == 2)
                    {
                        if (Guid.TryParse(Convert.ToString(tokenList[1]), out userid))
                        {
                            admin.UserID = Guid.Parse(Convert.ToString(tokenList[1]));

                            DirectoryEntry entry = new DirectoryEntry();
                            CommonProvider commonProvider = new CommonProvider();
                            if (!commonProvider.GetADEntryByGuid(admin.UserID, out entry, out message))
                            {
                                result = false;
                                break;
                            }

                            admin.UserAccount = entry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(entry.Properties["userPrincipalName"].Value);
                            admin.SAMAccountName = entry.Properties["sAMAccountName"].Value == null ? "" : Convert.ToString(entry.Properties["sAMAccountName"].Value);
                            admin.DisplayName = entry.Properties["cn"].Value == null ? "" : Convert.ToString(entry.Properties["cn"].Value);

                            RoleDBProvider roleDBProvider = new RoleDBProvider();
                            if (!roleDBProvider.GetUserRole(transactionid, admin.UserID, ref admin, out error))
                            {
                                result = false;
                                break;
                            }
                        }
                        else
                        {
                            error.Code = ErrorCode.TokenIllegal;
                            result = false;
                            break;
                        }
                    }
                    else
                    {
                        error.Code = ErrorCode.TokenIllegal;
                        result = false;
                        break;
                    }
                }
                else if (valresult == 1)
                {
                    error.Code = ErrorCode.TokenIllegal;
                    result = false;
                    break;
                }
                else if (valresult == 2)
                {
                    error.Code = ErrorCode.TokenExpire;
                    result = false;
                    break;
                }
                if (!ValidatorHelper.CheckSNKey(snkey))
                {
                    error.Code = ErrorCode.SNKeyExpire;
                    result = false;
                    break;
                }

            } while (false);
            return result;
        }

        public static bool ValidateAppToken(Guid transactionid, string strToken, out AdminInfo admin, out ErrorCodeInfo error)
        {
            bool result = true;
            admin = new AdminInfo();
            string message = string.Empty;
            error = new ErrorCodeInfo();

            ArrayList tokenList = new ArrayList();
            do
            {
                int valresult = ValidatorHelper.ValToken(strToken, out tokenList);
                string snkey = ConfigHelper.ConfigInstance["SNKey"];
                if (valresult == 0)
                {
                    if (tokenList.Count == 3)
                    {
                        string appid = Convert.ToString(tokenList[1]);
                        string secret = Convert.ToString(tokenList[2]);
                        RoleDBProvider roleDBProvider = new RoleDBProvider();
                        if (!roleDBProvider.GetAppInfo(transactionid, appid, secret, ref admin, out error))
                        {
                            result = false;
                            break;
                        }
                    }
                    else
                    {
                        error.Code = ErrorCode.TokenIllegal;
                        result = false;
                        break;
                    }
                }
                else if (valresult == 1)
                {
                    error.Code = ErrorCode.TokenIllegal;
                    result = false;
                    break;
                }
                else if (valresult == 2)
                {
                    error.Code = ErrorCode.TokenExpire;
                    result = false;
                    break;
                }
                if (!ValidatorHelper.CheckSNKey(snkey))
                {
                    error.Code = ErrorCode.SNKeyExpire;
                    result = false;
                    break;
                }
            } while (false);
            return result;
        }

        public static string GenToken(Guid userid)
        {
            return ValidatorHelper.GenToken(DateTime.Now.ToString(), userid.ToString());
        }

        public static string GenToken(string appid,string secret)
        {
            return ValidatorHelper.GenToken(DateTime.Now.ToString(), appid, secret);
        }
    }
}