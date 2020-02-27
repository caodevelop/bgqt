using DBUtility;
using Entity;
using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provider.ADProvider;

namespace Provider.DBProvider
{
    public class RoleDBProvider
    {
        public bool GetUserRole(Guid transactionid, Guid userID, ref AdminInfo info, out ErrorCodeInfo error)
        {
            bool bResult = true;

            error = new ErrorCodeInfo();
            string strError = string.Empty;

            string paramstr = string.Empty;
            paramstr = "userID:" + userID;

            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", userID);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetUserRole]", out ds, out strError))
                    {
                        LoggerHelper.Error("RoleDBProvider执行prc_GetUserRole异常", paramstr, strError, transactionid);
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
                                    DataRow dr = ds.Tables[1].Rows[0];
                                    info.RoleName = Convert.ToString(dr["RoleName"]);
                                    //info.ControlLimitPath = Convert.ToString(dr["ControlLimitPath"]);
                                    info.RoleID = Guid.Parse(Convert.ToString(dr["RoleID"]));

                                    for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                                    {
                                        RoleModuleList rolemodule = new RoleModuleList();
                                        rolemodule.ModuleType = (RoleModuleType)Convert.ToInt32(ds.Tables[2].Rows[i]["Type"]);
                                        rolemodule.ModuleTypeName = Convert.ToString(ds.Tables[2].Rows[i]["Name"]);

                                        DataRow[] datarows = ds.Tables[3].Select("Type = " + ds.Tables[2].Rows[i]["Type"]);

                                        foreach (DataRow sdr in datarows)
                                        {
                                            RoleParam param = new RoleParam();
                                            param.ParamID = Guid.Parse(Convert.ToString(sdr["ParamID"]));
                                            param.ParamValue = Convert.ToString(sdr["ParamValue"]);
                                            param.ParamCode = Convert.ToString(sdr["ParamCode"]);
                                            if (!info.ParamList.Contains(param.ParamCode))
                                            {
                                                info.ParamList.Add(param.ParamCode, (RoleParamCode)Enum.Parse(typeof(RoleParamCode), param.ParamCode));
                                            }
                                            rolemodule.RoleParamList.Add(param);
                                            
                                        }
                                        info.RoleList.Add(rolemodule);
                                    }
                                    
                                    foreach (DataRow sdr in ds.Tables[4].Rows)
                                    {
                                        SameLevelOuInfo ou = new SameLevelOuInfo();
                                        ou.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        ou.SamelevelOuPath = Convert.ToString(sdr["OuPath"]);
                                        info.SameLevelOuList.Add(ou);
                                    }

                                    foreach (DataRow sdr in ds.Tables[5].Rows)
                                    {
                                        ControlLimitOuInfo ou = new ControlLimitOuInfo();
                                        ou.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        ou.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                                        ou.OuID = Guid.Parse(Convert.ToString(sdr["OuID"]));
                                        ou.OUdistinguishedName = Convert.ToString(sdr["OUdistinguishedName"]);
                                        if (ou.OUdistinguishedName == ConfigADProvider.GetCompanyOuDistinguishedName())
                                        {
                                            info.IsCompanyAdmin = true;
                                        }
                                        info.ControlLimitOuList.Add(ou);
                                    }

                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotLoginRole;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用GetUserRole异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用GetUserRole异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                LoggerHelper.Error("RoleDBProvider调用GetUserRole异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
            }
            return bResult;
        }

        public bool GetRoleList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, out BaseListInfo lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BaseListInfo();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraPageIndex = new SqlParameter("@PageIndex", curpage);
                paras.Add(paraPageIndex);
                SqlParameter paraPageSize = new SqlParameter("@PageSize", pagesize);
                paras.Add(paraPageSize);
                SqlParameter paraRecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);
                paraRecordCount.Direction = ParameterDirection.Output;
                paras.Add(paraRecordCount);
                SqlParameter paraPageCount = new SqlParameter("@PageCount", SqlDbType.Int);
                paraPageCount.Direction = ParameterDirection.Output;
                paras.Add(paraPageCount);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetRoleList]", out ds, out strError))
                    {
                        strError = "prc_GetRoleList数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    lists.RecordCount = (int)paraRecordCount.Value;
                    lists.PageCount = (int)paraPageCount.Value;

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        UserProvider provider = new UserProvider();
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            RoleInfo info = new RoleInfo();
                            info.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                            info.RoleName = Convert.ToString(sdr["RoleName"]);
                            info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                            info.CreateUser = Convert.ToString(sdr["CreateUserName"]);
                            info.IsDefault = Convert.ToInt32(sdr["IsDefault"]);

                            DataRow[] datarows = ds.Tables[1].Select("RoleID='" + info.RoleID + "'");
                            foreach (DataRow dr in datarows)
                            {
                                ControlLimitOuInfo controlLimitOu = new ControlLimitOuInfo();
                                controlLimitOu.ID = Guid.Parse(Convert.ToString(dr["ID"]));
                                controlLimitOu.RoleID = Guid.Parse(Convert.ToString(dr["RoleID"]));
                                controlLimitOu.OuID = Guid.Parse(Convert.ToString(dr["OuID"]));
                                controlLimitOu.OUdistinguishedName = Convert.ToString(dr["OUdistinguishedName"]);
                                info.ControlLimitPath += controlLimitOu.OUdistinguishedName + ";";
                                info.ControlLimitOuList.Add(controlLimitOu);
                            }

                            DataRow[] data2rows = ds.Tables[2].Select("RoleID='" + info.RoleID + "'");
                            foreach (DataRow dr in data2rows)
                            {
                                UserInfo user = new UserInfo();
                                user.UserID = Guid.Parse(Convert.ToString(dr["UserID"]));
                                if (provider.GetUserInfo(transactionid, ref user, out error))
                                {
                                    info.UserNameList += user.DisplayName + "(" + user.UserAccount + ")，";
                                    info.UserList.Add(user);
                                }
                            }
                            info.UserNameList = string.IsNullOrEmpty(info.UserNameList) ? string.Empty : info.UserNameList.Remove(info.UserNameList.LastIndexOf('，'), 1);
                            info.ControlLimitPath = string.IsNullOrEmpty(info.ControlLimitPath) ? string.Empty : info.ControlLimitPath.Remove(info.ControlLimitPath.LastIndexOf(';'), 1);
                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetRoleList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用prc_GetRoleList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetRoleInfo(Guid transactionid, AdminInfo admin, Guid roleID, out RoleInfo info, out ErrorCodeInfo error)
        {
            bool bResult = true;
            info = new RoleInfo();
            error = new ErrorCodeInfo();
            string strError = string.Empty;

            string paramstr = string.Empty;
            paramstr = "userID:" + admin.UserID;
            paramstr = "||roleID:" + roleID;

            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraRoleID = new SqlParameter("@RoleID", roleID);
                paras.Add(paraRoleID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetRoleInfo]", out ds, out strError))
                    {
                        LoggerHelper.Error("RoleDBProvider执行prc_GetRoleInfo异常", paramstr, strError, transactionid);
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
                                    DataRow dr = ds.Tables[1].Rows[0];
                                    info.RoleName = Convert.ToString(dr["RoleName"]);
                                    info.RoleID = Guid.Parse(Convert.ToString(dr["RoleID"]));
                                    //info.Count = Convert.ToInt32(dr["Count"]);
                                    //info.ControlLimit = (ControlLimitType)Convert.ToInt32(dr["ControlLimit"]);
                                    //info.ControlLimitID = Guid.Parse(Convert.ToString(dr["ControlLimitID"]));
                                    //info.ControlLimitPath = Convert.ToString(dr["ControlLimitPath"]);
                                    info.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                                    info.CreateUser = Convert.ToString(dr["CreateUserName"]);
                                    info.IsDefault = Convert.ToInt32(dr["IsDefault"]);

                                    for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                                    {
                                        RoleModuleList rolemodule = new RoleModuleList();
                                        rolemodule.ModuleType = (RoleModuleType)Convert.ToInt32(ds.Tables[2].Rows[i]["Type"]);
                                        rolemodule.ModuleTypeName = Convert.ToString(ds.Tables[2].Rows[i]["Name"]);

                                        DataRow[] datarows = ds.Tables[3].Select("Type = " + ds.Tables[2].Rows[i]["Type"]);

                                        foreach (DataRow sdr in datarows)
                                        {
                                            RoleParam param = new RoleParam();
                                            param.ParamID = Guid.Parse(Convert.ToString(sdr["ParamID"]));
                                            param.ParamValue = Convert.ToString(sdr["ParamValue"]);
                                            param.ParamCode = Convert.ToString(sdr["ParamCode"]);
                                            rolemodule.RoleParamList.Add(param);
                                        }
                                        info.RoleList.Add(rolemodule);
                                    }

                                    foreach (DataRow sdr in ds.Tables[4].Rows)
                                    {
                                        UserInfo user = new UserInfo();
                                        user.UserID = Guid.Parse(Convert.ToString(sdr["UserID"]));
                                        info.UserList.Add(user);
                                    }
                                    foreach (DataRow sdr in ds.Tables[5].Rows)
                                    {
                                        SameLevelOuInfo ou = new SameLevelOuInfo();
                                        ou.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        ou.SamelevelOuPath = Convert.ToString(sdr["OuPath"]);
                                        info.SameLevelOuList.Add(ou);
                                    }
                                    foreach (DataRow sdr in ds.Tables[6].Rows)
                                    {
                                        ControlLimitOuInfo ou = new ControlLimitOuInfo();
                                        ou.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        ou.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                                        ou.OuID = Guid.Parse(Convert.ToString(sdr["OuID"]));
                                        ou.OUdistinguishedName = Convert.ToString(sdr["OUdistinguishedName"]);
                                        info.ControlLimitOuList.Add(ou);
                                    }
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotRole;
                                    break;
                                case -2:
                                    bResult = false;
                                    error.Code = ErrorCode.RoleNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用GetRoleInfo异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用GetRoleInfo异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                LoggerHelper.Error("RoleDBProvider调用GetRoleInfo异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
            }
            return bResult;
        }

        public bool GetRoleModuleList(Guid transactionid, AdminInfo admin, out List<RoleModuleList> lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new List<RoleModuleList>();
            string strError = string.Empty;
            bool bResult = true;

            string paramstr = string.Empty;
            paramstr = "userID:" + admin.UserID;

            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetRoleModuleList]", out ds, out strError))
                    {
                        strError = "prc_GetRoleModuleList数据库执行失败,Error:" + strError;
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
                                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                    {
                                        RoleModuleList roleModuleList = new RoleModuleList();

                                        roleModuleList.ModuleType = (RoleModuleType)Convert.ToInt32(ds.Tables[1].Rows[i]["Type"]);
                                        roleModuleList.ModuleTypeName = Convert.ToString(ds.Tables[1].Rows[i]["Name"]);

                                        DataRow[] datarows = ds.Tables[2].Select("ParamType = " + ds.Tables[1].Rows[i]["Type"]);

                                        foreach (DataRow sdr in datarows)
                                        {
                                            RoleParam param = new RoleParam();
                                            param.ParamID = Guid.Parse(Convert.ToString(sdr["ParamID"]));
                                            param.ParamValue = Convert.ToString(sdr["ParamValue"]);
                                            param.ParamCode = Convert.ToString(sdr["ParamCode"]);
                                           
                                            roleModuleList.RoleParamList.Add(param);
                                        }

                                        lists.Add(roleModuleList);
                                    }
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用GetRoleModuleList异常", paramstr, "-9999", transactionid);
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
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用prc_GetRoleModuleList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddRole(Guid transactionid, AdminInfo admin, ref RoleInfo role, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||RoleName:{role.RoleName}";
            paramstr += $"||ControlLimit:{role.ControlLimit.ToString()}";
            paramstr += $"||ControlLimitID:{role.ControlLimitID}";
            paramstr += $"||Members:";
            for (int i = 0; i < role.UserList.Count; i++)
            {
                paramstr += role.UserList[i].UserID + ",";
            }

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraRoleName = new SqlParameter("@RoleName", role.RoleName);
                paras.Add(paraRoleName);
                SqlParameter paraCount = new SqlParameter("@Count", role.UserList.Count);
                paras.Add(paraCount);
                SqlParameter paraCreateUserID = new SqlParameter("@CreateUserID", admin.UserID);
                paras.Add(paraCreateUserID);
                SqlParameter paraCreateUserAccount = new SqlParameter("@CreateUserName", admin.UserAccount);
                paras.Add(paraCreateUserAccount);
                SqlParameter paraControlLimit = new SqlParameter("@ControlLimit", role.ControlLimit);
                paras.Add(paraControlLimit);
                SqlParameter paraControlLimitID = new SqlParameter("@ControlLimitID", role.ControlLimitID);
                paras.Add(paraControlLimitID);
                SqlParameter paraControlLimitPath = new SqlParameter("@ControlLimitPath", role.ControlLimitPath);
                paras.Add(paraControlLimitPath);
                SqlParameter paraIsDefault = new SqlParameter("@IsDefault", false);
                paras.Add(paraIsDefault);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddRole]", out ds, out strError))
                    {
                        strError = "AddRole异常,Error:" + strError;
                        LoggerHelper.Error("RoleDBProvider调用AddRole异常", paramstr, strError, transactionid);
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
                                    if (ds.Tables.Count > 1)
                                    {
                                        DataRow sdr = ds.Tables[1].Rows[0];
                                        role.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameName;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用AddRole异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用AddRole异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用AddRole异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddRoleModuleParam(Guid transactionid, Guid roleID, RoleParam roleParam, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"RoleID:{roleID}";
            paramstr += $"||ParamID:{roleParam.ParamID}";

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraRoleID = new SqlParameter("@RoleID", roleID);
                paras.Add(paraRoleID);
                SqlParameter paraParamID = new SqlParameter("@ParamID", roleParam.ParamID);
                paras.Add(paraParamID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddRoleModuleParam]", out ds, out strError))
                    {
                        strError = "AddRoleModuleParam异常,Error:" + strError;
                        LoggerHelper.Error("RoleDBProvider调用AddRoleModuleParam异常", paramstr, strError, transactionid);
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
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.RoleNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用AddRoleModuleParam异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用AddRoleModuleParam异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用AddRoleModuleParam异常", string.Empty, ex.ToString(), transactionid);
            }

            return bResult;
        }

        public bool AddRoleMembers(Guid transactionid, Guid roleID, UserInfo user, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"RoleID:{roleID}";
            paramstr += $"||UserID:{user.UserID}";

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraRoleID = new SqlParameter("@RoleID", roleID);
                paras.Add(paraRoleID);
                SqlParameter paraUserID = new SqlParameter("@UserID", user.UserID);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddRoleMembers]", out ds, out strError))
                    {
                        strError = "AddRoleMembers异常,Error:" + strError;
                        LoggerHelper.Error("RoleDBProvider调用AddRoleMembers异常", paramstr, strError, transactionid);
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
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.RoleNotExist;
                                    break;
                                case -2:
                                    bResult = false;
                                    error.Code = ErrorCode.UserHaveRole;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用AddRoleMembers异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用AddRoleMembers异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用AddRoleMembers异常", string.Empty, ex.ToString(), transactionid);
            }

            return bResult;
        }

        public bool DeleteRole(Guid transactionid, AdminInfo admin, RoleInfo role, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||RoleID:{role.RoleID}";

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraRoleID = new SqlParameter("@RoleID", role.RoleID);
                paras.Add(paraRoleID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DeleteRole]", out ds, out strError))
                    {
                        strError = "DeleteRole异常,Error:" + strError;
                        LoggerHelper.Error("RoleDBProvider调用DeleteRole异常", paramstr, strError, transactionid);
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
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.RoleNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用DeleteRole异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用DeleteRole异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用DeleteRole异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool ChangeRole(Guid transactionid, AdminInfo admin, RoleInfo role, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||RoleID:{role.RoleID}";
            paramstr += $"||RoleName:{role.RoleName}";
            paramstr += $"||ControlLimit:{role.ControlLimit.ToString()}";
            paramstr += $"||ControlLimitID:{role.ControlLimitID}";

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraRoleID = new SqlParameter("@RoleID", role.RoleID);
                paras.Add(paraRoleID);
                SqlParameter paraRoleName = new SqlParameter("@RoleName", role.RoleName);
                paras.Add(paraRoleName);
                SqlParameter paraCount = new SqlParameter("@Count", role.UserList.Count);
                paras.Add(paraCount);
                SqlParameter paraControlLimit = new SqlParameter("@ControlLimit", role.ControlLimit);
                paras.Add(paraControlLimit);
                SqlParameter paraControlLimitID = new SqlParameter("@ControlLimitID", role.ControlLimitID);
                paras.Add(paraControlLimitID);
                SqlParameter paraControlLimitPath = new SqlParameter("@ControlLimitPath", role.ControlLimitPath);
                paras.Add(paraControlLimitPath);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_ModifyRole]", out ds, out strError))
                    {
                        strError = "ModifyRole异常,Error:" + strError;
                        LoggerHelper.Error("RoleDBProvider调用ModifyRole异常", paramstr, strError, transactionid);
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
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameGroup;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用ModifyRole异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用ModifyRole异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用AddRole异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetSameLevelOuList(Guid transactionid, AdminInfo admin, out List<SameLevelOuInfo> lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new List<SameLevelOuInfo>();
            string strError = string.Empty;
            bool bResult = true;

            string paramstr = string.Empty;
            paramstr = "userID:" + admin.UserID;

            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetSameLevelOuList]", out ds, out strError))
                    {
                        strError = "prc_GetSameLevelOuList数据库执行失败,Error:" + strError;
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
                                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                    {
                                        SameLevelOuInfo info = new SameLevelOuInfo();
                                        info.ID = Guid.Parse(Convert.ToString(ds.Tables[1].Rows[i]["ID"]));
                                        info.SamelevelOuPath = Convert.ToString(ds.Tables[1].Rows[i]["OuPath"]);
                                        lists.Add(info);
                                    }
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用GetSameLevelOuList异常", paramstr, "-9999", transactionid);
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
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用prc_GetRoleModuleList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetRoleSameLevelOuList(Guid transactionid, AdminInfo admin, out List<SameLevelOuInfo> lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new List<SameLevelOuInfo>();
            string strError = string.Empty;
            bool bResult = true;

            string paramstr = string.Empty;
            paramstr = "userID:" + admin.UserID;

            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserID = new SqlParameter("@UserID", admin.UserID);
                paras.Add(paraUserID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetRoleSameLevelOuList]", out ds, out strError))
                    {
                        strError = "prc_GetRoleSameLevelOuList数据库执行失败,Error:" + strError;
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
                                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                    {
                                        SameLevelOuInfo info = new SameLevelOuInfo();
                                        info.ID = Guid.Parse(Convert.ToString(ds.Tables[1].Rows[i]["ID"]));
                                        info.SamelevelOuPath = Convert.ToString(ds.Tables[1].Rows[i]["OuPath"]);
                                        lists.Add(info);
                                    }
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.UserNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用GetSameLevelOuList异常", paramstr, "-9999", transactionid);
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
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用prc_GetRoleModuleList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }
        public bool GetRoleParam(Guid transactionid, Guid ParamID, out RoleParam param, out ErrorCodeInfo error)
        {
            bool bResult = true;
            param = new RoleParam();
            error = new ErrorCodeInfo();
            string strError = string.Empty;

            string paramstr = string.Empty;
            paramstr = "ParamID:" + ParamID;

            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraParamID = new SqlParameter("@ParamID", ParamID);
                paras.Add(paraParamID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetRoleParam]", out ds, out strError))
                    {
                        LoggerHelper.Error("RoleDBProvider执行prc_GetRoleParam异常", paramstr, strError, transactionid);
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
                                    DataRow sdr = ds.Tables[1].Rows[0];
                                    param.ParamID = Guid.Parse(Convert.ToString(sdr["ParamID"]));
                                    param.ParamValue = Convert.ToString(sdr["ParamValue"]);
                                    param.ParamCode = Convert.ToString(sdr["ParamCode"]);
                                    bResult = true;
                                    break;
                               case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用GetRoleParam异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用GetRoleParam异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                LoggerHelper.Error("RoleDBProvider调用GetRoleParam异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
            }
            return bResult;
        }

        public bool AddSameLevelOu(Guid transactionid, Guid roleID, SameLevelOuInfo sameLevelOu, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"RoleID:{roleID}";
            paramstr += $"||SameLevelOuID:{sameLevelOu.ID}";

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraRoleID = new SqlParameter("@RoleID", roleID);
                paras.Add(paraRoleID);
                SqlParameter paraSameLevelOuID = new SqlParameter("@SameLevelOuID", sameLevelOu.ID);
                paras.Add(paraSameLevelOuID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddSameLevelOu]", out ds, out strError))
                    {
                        strError = "AddSameLevelOu异常,Error:" + strError;
                        LoggerHelper.Error("RoleDBProvider调用AddSameLevelOu异常", paramstr, strError, transactionid);
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
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.RoleNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用AddSameLevelOu异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用AddSameLevelOu异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用AddSameLevelOu异常", string.Empty, ex.ToString(), transactionid);
            }

            return bResult;
        }

        public bool AddControlLimitOu(Guid transactionid, Guid roleID, ControlLimitOuInfo controlLimitOu, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"RoleID:{roleID}";
            paramstr += $"||controlLimitOuID:{controlLimitOu.OuID}";

            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraRoleID = new SqlParameter("@RoleID", roleID);
                paras.Add(paraRoleID);
                SqlParameter paraControlLimitOuID = new SqlParameter("@ControlLimitOuID", controlLimitOu.OuID);
                paras.Add(paraControlLimitOuID);
                SqlParameter paraOUdistinguishedName = new SqlParameter("@OUdistinguishedName", controlLimitOu.OUdistinguishedName);
                paras.Add(paraOUdistinguishedName);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddControlLimitOu]", out ds, out strError))
                    {
                        strError = "AddControlLimitOu异常,Error:" + strError;
                        LoggerHelper.Error("RoleDBProvider调用AddControlLimitOu异常", paramstr, strError, transactionid);
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
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.RoleNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用AddControlLimitOu异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用AddControlLimitOu异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("RoleDBProvider调用AddControlLimitOu异常", string.Empty, ex.ToString(), transactionid);
            }

            return bResult;
        }

        public bool GetAppInfo(Guid transactionid, string appid, string secret, ref AdminInfo info, out ErrorCodeInfo error)
        {
            bool bResult = true;

            error = new ErrorCodeInfo();
            string strError = string.Empty;

            string paramstr = string.Empty;
            paramstr = "appid:" + appid;
            paramstr = "||secret:" + secret;

            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAppid = new SqlParameter("@appid", appid);
                paras.Add(paraAppid);
                SqlParameter paraSecret = new SqlParameter("@secret", secret);
                paras.Add(paraSecret);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetAPIInfo]", out ds, out strError))
                    {
                        LoggerHelper.Error("RoleDBProvider执行prc_GetAPIInfo异常", paramstr, strError, transactionid);
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
                                    DataRow dr = ds.Tables[1].Rows[0];
                                    info.UserID = Guid.Parse(Convert.ToString(dr["ID"]));
                                    info.DisplayName = appid;
                                    info.UserAccount = appid;
                                    bResult = true;
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.AppIDNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("RoleDBProvider调用GetAppInfo异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("RoleDBProvider调用GetAppInfo异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                LoggerHelper.Error("RoleDBProvider调用GetAppInfo异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
            }
            return bResult;
        }
    }
}
