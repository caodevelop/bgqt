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
    public class WaterMakingDBProvider
    {
        #region PDF
        public bool GetPDFWaterMakingList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out BaseListInfo lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BaseListInfo();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraPageIndex = new SqlParameter("@PageIndex", curpage);
                paras.Add(paraPageIndex);
                SqlParameter paraPageSize = new SqlParameter("@PageSize", pagesize);
                paras.Add(paraPageSize);
                SqlParameter paraSearchstr = new SqlParameter("@Searchstr", $"%{searchstr}%");
                paras.Add(paraSearchstr);
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
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetPDFWaterMakingList]", out ds, out strError))
                    {
                        strError = "prc_GetPDFWaterMakingList数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    lists.RecordCount = (int)paraRecordCount.Value;
                    lists.PageCount = (int)paraPageCount.Value;

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            PDFWaterMakingInfo info = new PDFWaterMakingInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.Name = Convert.ToString(sdr["Name"]);
                           // info.Description = Convert.ToString(sdr["Description"]);
                            info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                            info.CreateUserID = Guid.Parse(Convert.ToString(sdr["CreateUserID"]));
                            info.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                            info.Status = (WaterMakingStatus)Convert.ToInt32(sdr["Status"]);
                            info.Priority = Convert.ToInt32(sdr["Priority"]);
                            info.PDFCondition.From = Convert.ToString(sdr["From"]);
                            info.PDFCondition.IsAllFrom = Convert.ToBoolean(sdr["IsAllFrom"]);
                            info.PDFCondition.ExcludeFroms = Convert.ToString(sdr["ExcludeFroms"]);
                            info.PDFCondition.Recipients = Convert.ToString(sdr["Recipients"]);
                            info.PDFCondition.Subject = Convert.ToString(sdr["Subject"]);
                            info.PDFCondition.PDFName = Convert.ToString(sdr["PDFName"]);
                            info.WaterMakingContent.IsAllRecipients = Convert.ToBoolean(sdr["IsAllRecipients"]);
                            info.WaterMakingContent.Content = Convert.ToString(sdr["Content"]);
                            info.WaterMakingContent.IsAddDate = Convert.ToBoolean(sdr["IsAddDate"]);
                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetPDFWaterMakingList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用prc_GetPDFWaterMakingList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetPDFWaterMakingInfo(Guid transactionid, AdminInfo admin, ref PDFWaterMakingInfo info, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{info.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", info.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetPDFWaterMakingInfo]", out ds, out strError))
                    {
                        strError = "GetPDFWaterMakingInfo异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用GetPDFWaterMakingInfo异常", paramstr, strError, transactionid);
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
                                        info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        info.Name = Convert.ToString(sdr["Name"]);
                                        //info.Description = Convert.ToString(sdr["Description"]);
                                        info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                                        info.CreateUserID = Guid.Parse(Convert.ToString(sdr["CreateUserID"]));
                                        info.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                                        info.Status = (WaterMakingStatus)Convert.ToInt32(sdr["Status"]);
                                        info.Priority = Convert.ToInt32(sdr["Priority"]);
                                        info.PDFCondition.From = Convert.ToString(sdr["From"]);
                                        info.PDFCondition.IsAllFrom = Convert.ToBoolean(sdr["IsAllFrom"]);
                                        info.PDFCondition.ExcludeFroms = Convert.ToString(sdr["ExcludeFroms"]);
                                        info.PDFCondition.Recipients = Convert.ToString(sdr["Recipients"]);
                                        info.PDFCondition.Subject = Convert.ToString(sdr["Subject"]);
                                        info.PDFCondition.PDFName = Convert.ToString(sdr["PDFName"]);
                                        info.WaterMakingContent.IsAllRecipients = Convert.ToBoolean(sdr["IsAllRecipients"]);
                                        info.WaterMakingContent.Content = Convert.ToString(sdr["Content"]);
                                        info.WaterMakingContent.IsAddDate = Convert.ToBoolean(sdr["IsAddDate"]);
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.WaterMarkingNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("WaterMakingDBProvider调用GetPDFWaterMakingInfo异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用GetPDFWaterMakingInfo异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用GetPDFWaterMakingInfo异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddPDFWaterMaking(Guid transactionid, AdminInfo admin, ref PDFWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||Name:{waterMakingInfo.Name}";
            paramstr += $"||From:{waterMakingInfo.PDFCondition.From}";
            paramstr += $"||IsAllFrom:{waterMakingInfo.PDFCondition.IsAllFrom}";
            paramstr += $"||ExcludeFroms:{waterMakingInfo.PDFCondition.ExcludeFroms}";
            paramstr += $"||Recipients:{waterMakingInfo.PDFCondition.Recipients}";
            paramstr += $"||Subject:{waterMakingInfo.PDFCondition.Subject}";
            paramstr += $"||PDFName:{waterMakingInfo.PDFCondition.PDFName}";
            paramstr += $"||IsAllRecipients:{waterMakingInfo.WaterMakingContent.IsAllRecipients}";
            paramstr += $"||Content:{waterMakingInfo.WaterMakingContent.Content}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraName = new SqlParameter("@Name", waterMakingInfo.Name);
                paras.Add(paraName);
                SqlParameter paraFrom = new SqlParameter("@From", waterMakingInfo.PDFCondition.From);
                paras.Add(paraFrom);
                SqlParameter paraIsAllFrom = new SqlParameter("@IsAllFrom", waterMakingInfo.PDFCondition.IsAllFrom);
                paras.Add(paraIsAllFrom);
                SqlParameter paraExcludeFroms = new SqlParameter("@ExcludeFroms", waterMakingInfo.PDFCondition.ExcludeFroms);
                paras.Add(paraExcludeFroms);
                SqlParameter paraRecipients = new SqlParameter("@Recipients", waterMakingInfo.PDFCondition.Recipients);
                paras.Add(paraRecipients);
                SqlParameter paraSubject = new SqlParameter("@Subject", waterMakingInfo.PDFCondition.Subject);
                paras.Add(paraSubject);
                SqlParameter paraPDFName = new SqlParameter("@PDFName", waterMakingInfo.PDFCondition.PDFName);
                paras.Add(paraPDFName);
                SqlParameter paraIsAllRecipients = new SqlParameter("@IsAllRecipients", waterMakingInfo.WaterMakingContent.IsAllRecipients);
                paras.Add(paraIsAllRecipients);
                SqlParameter paraContect = new SqlParameter("@Content", waterMakingInfo.WaterMakingContent.Content);
                paras.Add(paraContect);
                SqlParameter paraIsAddDate = new SqlParameter("@IsAddDate", waterMakingInfo.WaterMakingContent.IsAddDate);
                paras.Add(paraIsAddDate);
                SqlParameter paraUserID = new SqlParameter("@CreateUserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraRoleID = new SqlParameter("@RoleID", admin.RoleID);
                paras.Add(paraRoleID);
                SqlParameter paraPriority = new SqlParameter("@Priority", waterMakingInfo.Priority);
                paras.Add(paraPriority);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddPDFWaterMaking]", out ds, out strError))
                    {
                        strError = "AddPDFWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用AddPDFWaterMaking异常", paramstr, strError, transactionid);
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
                                        waterMakingInfo.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameWaterMarking;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("WaterMakingDBProvider调用AddPDFWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用AddPDFWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用AddPDFWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool ModifyPDFWaterMaking(Guid transactionid, AdminInfo admin, ref PDFWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            paramstr += $"||Name:{waterMakingInfo.Name}";
            paramstr += $"||From:{waterMakingInfo.PDFCondition.From}";
            paramstr += $"||IsAllFrom:{waterMakingInfo.PDFCondition.IsAllFrom}";
            paramstr += $"||ExcludeFroms:{waterMakingInfo.PDFCondition.ExcludeFroms}";
            paramstr += $"||Recipients:{waterMakingInfo.PDFCondition.Recipients}";
            paramstr += $"||Subject:{waterMakingInfo.PDFCondition.Subject}";
            paramstr += $"||PDFName:{waterMakingInfo.PDFCondition.PDFName}";
            paramstr += $"||IsAllRecipients:{waterMakingInfo.WaterMakingContent.IsAllRecipients}";
            paramstr += $"||Content:{waterMakingInfo.WaterMakingContent.Content}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraID = new SqlParameter("@ID", waterMakingInfo.ID);
                paras.Add(paraID);
                SqlParameter paraName = new SqlParameter("@Name", waterMakingInfo.Name);
                paras.Add(paraName);
                SqlParameter paraFrom = new SqlParameter("@From", waterMakingInfo.PDFCondition.From);
                paras.Add(paraFrom);
                SqlParameter paraIsAllFrom = new SqlParameter("@IsAllFrom", waterMakingInfo.PDFCondition.IsAllFrom);
                paras.Add(paraIsAllFrom);
                SqlParameter paraExcludeFroms = new SqlParameter("@ExcludeFroms", waterMakingInfo.PDFCondition.ExcludeFroms);
                paras.Add(paraExcludeFroms);
                SqlParameter paraRecipients = new SqlParameter("@Recipients", waterMakingInfo.PDFCondition.Recipients);
                paras.Add(paraRecipients);
                SqlParameter paraSubject = new SqlParameter("@Subject", waterMakingInfo.PDFCondition.Subject);
                paras.Add(paraSubject);
                SqlParameter paraPDFName = new SqlParameter("@PDFName", waterMakingInfo.PDFCondition.PDFName);
                paras.Add(paraPDFName);
                SqlParameter paraIsAllRecipients = new SqlParameter("@IsAllRecipients", waterMakingInfo.WaterMakingContent.IsAllRecipients);
                paras.Add(paraIsAllRecipients);
                SqlParameter paraContect = new SqlParameter("@Content", waterMakingInfo.WaterMakingContent.Content);
                paras.Add(paraContect);
                SqlParameter paraIsAddDate = new SqlParameter("@IsAddDate", waterMakingInfo.WaterMakingContent.IsAddDate);
                paras.Add(paraIsAddDate);
                SqlParameter paraPriority = new SqlParameter("@Priority", waterMakingInfo.Priority);
                paras.Add(paraPriority);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_ModifyPDFWaterMaking]", out ds, out strError))
                    {
                        strError = "ModifyPDFWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用ModifyPDFWaterMaking异常", paramstr, strError, transactionid);
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
                                    error.Code = ErrorCode.HaveSameRule;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("WaterMakingDBProvider调用ModifyPDFWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用ModifyPDFWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用ModifyPDFWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool DeletePDFWaterMaking(Guid transactionid, AdminInfo admin, PDFWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", waterMakingInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DeletePDFWaterMaking]", out ds, out strError))
                    {
                        strError = "DeletePDFWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用DeletePDFWaterMaking异常", paramstr, strError, transactionid);
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
                                    LoggerHelper.Error("WaterMakingDBProvider调用DeletePDFWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用DeletePDFWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用DeletePDFWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool DisablePDFWaterMaking(Guid transactionid, AdminInfo admin, PDFWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", waterMakingInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DisablePDFWaterMaking]", out ds, out strError))
                    {
                        strError = "DisablePDFWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用DisablePDFWaterMaking异常", paramstr, strError, transactionid);
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
                                    LoggerHelper.Error("WaterMakingDBProvider调用DisablePDFWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用DisablePDFWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用DisablePDFWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool EnablePDFWaterMaking(Guid transactionid, AdminInfo admin, PDFWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", waterMakingInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_EnablePDFWaterMaking]", out ds, out strError))
                    {
                        strError = "EnablePDFWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用EnablePDFWaterMaking异常", paramstr, strError, transactionid);
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
                                    LoggerHelper.Error("WaterMakingDBProvider调用EnablePDFWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用EnablePDFWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用EnablePDFWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }
        #endregion
        #region
        public bool GetBodyWaterMakingList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out BaseListInfo lists, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            lists = new BaseListInfo();
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraPageIndex = new SqlParameter("@PageIndex", curpage);
                paras.Add(paraPageIndex);
                SqlParameter paraPageSize = new SqlParameter("@PageSize", pagesize);
                paras.Add(paraPageSize);
                SqlParameter paraSearchstr = new SqlParameter("@Searchstr", $"%{searchstr}%");
                paras.Add(paraSearchstr);
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
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetBodyWaterMakingList]", out ds, out strError))
                    {
                        strError = "prc_GetBodyWaterMakingList数据库执行失败,Error:" + strError;
                        bResult = false;
                        error.Code = ErrorCode.SQLException;
                        break;
                    }

                    lists.RecordCount = (int)paraRecordCount.Value;
                    lists.PageCount = (int)paraPageCount.Value;

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow sdr = ds.Tables[0].Rows[i];
                            BodyWaterMakingInfo info = new BodyWaterMakingInfo();
                            info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                            info.Name = Convert.ToString(sdr["Name"]);
                            //info.Description = Convert.ToString(sdr["Description"]);
                            info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                            info.CreateUserID = Guid.Parse(Convert.ToString(sdr["CreateUserID"]));
                            info.Status = (WaterMakingStatus)Convert.ToInt32(sdr["Status"]);
                            info.Priority = Convert.ToInt32(sdr["Priority"]);
                            info.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                            info.BodyCondition.From = Convert.ToString(sdr["From"]);
                            info.BodyCondition.IsAllFrom = Convert.ToBoolean(sdr["IsAllFrom"]);
                            info.BodyCondition.ExcludeFroms = Convert.ToString(sdr["ExcludeFroms"]);
                            info.BodyCondition.Recipients = Convert.ToString(sdr["Recipients"]);
                            info.BodyCondition.Subject = Convert.ToString(sdr["Subject"]);
                            info.BodyCondition.IsContainsAttachment = Convert.ToBoolean(sdr["IsContainsAttachment"]);
                            info.BodyCondition.AttachmentName = Convert.ToString(sdr["AttachmentName"]);
                            info.WaterMakingContent.IsAllRecipients = Convert.ToBoolean(sdr["IsAllRecipients"]);
                            info.WaterMakingContent.Content = Convert.ToString(sdr["Content"]);
                            info.WaterMakingContent.IsAddDate = Convert.ToBoolean(sdr["IsAddDate"]);
                            lists.Lists.Add(info);
                        }
                    }
                    else
                    {
                        bResult = false;
                        error.Code = ErrorCode.Exception;
                        LoggerHelper.Error("数据库执行prc_GetBodyWaterMakingList失败", string.Empty, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用prc_GetBodyWaterMakingList异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool GetBodyWaterMakingInfo(Guid transactionid, AdminInfo admin, ref BodyWaterMakingInfo info, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{info.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", info.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_GetBodyWaterMakingInfo]", out ds, out strError))
                    {
                        strError = "GetBodyWaterMakingInfo异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用GetBodyWaterMakingInfo异常", paramstr, strError, transactionid);
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
                                        info.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                        info.Name = Convert.ToString(sdr["Name"]);
                                        //info.Description = Convert.ToString(sdr["Description"]);
                                        info.CreateTime = Convert.ToDateTime(sdr["CreateTime"]);
                                        info.CreateUserID = Guid.Parse(Convert.ToString(sdr["CreateUserID"]));
                                        info.RoleID = Guid.Parse(Convert.ToString(sdr["RoleID"]));
                                        info.Status = (WaterMakingStatus)Convert.ToInt32(sdr["Status"]);
                                        info.Priority = Convert.ToInt32(sdr["Priority"]);
                                        info.BodyCondition.From = Convert.ToString(sdr["From"]);
                                        info.BodyCondition.IsAllFrom = Convert.ToBoolean(sdr["IsAllFrom"]);
                                        info.BodyCondition.ExcludeFroms = Convert.ToString(sdr["ExcludeFroms"]);
                                        info.BodyCondition.Recipients = Convert.ToString(sdr["Recipients"]);
                                        info.BodyCondition.Subject = Convert.ToString(sdr["Subject"]);
                                        info.BodyCondition.IsContainsAttachment = Convert.ToBoolean(sdr["IsContainsAttachment"]);
                                        info.BodyCondition.AttachmentName = Convert.ToString(sdr["AttachmentName"]);
                                        info.WaterMakingContent.IsAllRecipients = Convert.ToBoolean(sdr["IsAllRecipients"]);
                                        info.WaterMakingContent.Content = Convert.ToString(sdr["Content"]);
                                        info.WaterMakingContent.IsAddDate = Convert.ToBoolean(sdr["IsAddDate"]);
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.WaterMarkingNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("WaterMakingDBProvider调用GetBodyWaterMakingInfo异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用GetBodyWaterMakingInfo异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用GetBodyWaterMakingInfo异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool AddBodyWaterMaking(Guid transactionid, AdminInfo admin, ref BodyWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||Name:{waterMakingInfo.Name}";
            paramstr += $"||From:{waterMakingInfo.BodyCondition.From}";
            paramstr += $"||IsAllFrom:{waterMakingInfo.BodyCondition.IsAllFrom}";
            paramstr += $"||ExcludeFroms:{waterMakingInfo.BodyCondition.ExcludeFroms}";
            paramstr += $"||Recipients:{waterMakingInfo.BodyCondition.Recipients}";
            paramstr += $"||Subject:{waterMakingInfo.BodyCondition.Subject}";
            paramstr += $"||IsContainsAttachment:{waterMakingInfo.BodyCondition.IsContainsAttachment}";
            paramstr += $"||AttachmentName:{waterMakingInfo.BodyCondition.AttachmentName}";
            paramstr += $"||IsAllRecipients:{waterMakingInfo.WaterMakingContent.IsAllRecipients}";
            paramstr += $"||Content:{waterMakingInfo.WaterMakingContent.Content}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraName = new SqlParameter("@Name", waterMakingInfo.Name);
                paras.Add(paraName);
                SqlParameter paraFrom = new SqlParameter("@From", waterMakingInfo.BodyCondition.From);
                paras.Add(paraFrom);
                SqlParameter paraIsAllFrom = new SqlParameter("@IsAllFrom", waterMakingInfo.BodyCondition.IsAllFrom);
                paras.Add(paraIsAllFrom);
                SqlParameter paraExcludeFroms = new SqlParameter("@ExcludeFroms", waterMakingInfo.BodyCondition.ExcludeFroms);
                paras.Add(paraExcludeFroms);
                SqlParameter paraRecipients = new SqlParameter("@Recipients", waterMakingInfo.BodyCondition.Recipients);
                paras.Add(paraRecipients);
                SqlParameter paraSubject = new SqlParameter("@Subject", waterMakingInfo.BodyCondition.Subject);
                paras.Add(paraSubject);
                SqlParameter paraIsContainsAttachment = new SqlParameter("@IsContainsAttachment", waterMakingInfo.BodyCondition.IsContainsAttachment);
                paras.Add(paraIsContainsAttachment);
                SqlParameter paraIsAttachmentName = new SqlParameter("@AttachmentName", waterMakingInfo.BodyCondition.AttachmentName);
                paras.Add(paraIsAttachmentName);
                SqlParameter paraIsAllRecipients = new SqlParameter("@IsAllRecipients", waterMakingInfo.WaterMakingContent.IsAllRecipients);
                paras.Add(paraIsAllRecipients);
                SqlParameter paraContect = new SqlParameter("@Content", waterMakingInfo.WaterMakingContent.Content);
                paras.Add(paraContect);
                SqlParameter paraIsAddDate = new SqlParameter("@IsAddDate", waterMakingInfo.WaterMakingContent.IsAddDate);
                paras.Add(paraIsAddDate);
                SqlParameter paraUserID = new SqlParameter("@CreateUserID", admin.UserID);
                paras.Add(paraUserID);
                SqlParameter paraRoleID = new SqlParameter("@RoleID", admin.RoleID);
                paras.Add(paraRoleID);
                SqlParameter paraPriority = new SqlParameter("@Priority", waterMakingInfo.Priority);
                paras.Add(paraPriority);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddBodyWaterMaking]", out ds, out strError))
                    {
                        strError = "AddBodyWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用AddBodyWaterMaking异常", paramstr, strError, transactionid);
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
                                        waterMakingInfo.ID = Guid.Parse(Convert.ToString(sdr["ID"]));
                                    }
                                    break;
                                case -1:
                                    bResult = false;
                                    error.Code = ErrorCode.HaveSameWaterMarking;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("WaterMakingDBProvider调用AddBodyWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用AddBodyWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用AddBodyWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool ModifyBodyWaterMaking(Guid transactionid, AdminInfo admin, ref BodyWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            paramstr += $"||Name:{waterMakingInfo.Name}";
            paramstr += $"||From:{waterMakingInfo.BodyCondition.From}";
            paramstr += $"||IsAllFrom:{waterMakingInfo.BodyCondition.IsAllFrom}";
            paramstr += $"||ExcludeFroms:{waterMakingInfo.BodyCondition.ExcludeFroms}";
            paramstr += $"||Recipients:{waterMakingInfo.BodyCondition.Recipients}";
            paramstr += $"||Subject:{waterMakingInfo.BodyCondition.Subject}";
            paramstr += $"||IsContainsAttachment:{waterMakingInfo.BodyCondition.IsContainsAttachment}";
            paramstr += $"||AttachmentName:{waterMakingInfo.BodyCondition.AttachmentName}";
            paramstr += $"||IsAllRecipients:{waterMakingInfo.WaterMakingContent.IsAllRecipients}";
            paramstr += $"||Content:{waterMakingInfo.WaterMakingContent.Content}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraID = new SqlParameter("@ID", waterMakingInfo.ID);
                paras.Add(paraID);
                SqlParameter paraName = new SqlParameter("@Name", waterMakingInfo.Name);
                paras.Add(paraName);
                SqlParameter paraFrom = new SqlParameter("@From", waterMakingInfo.BodyCondition.From);
                paras.Add(paraFrom);
                SqlParameter paraIsAllFrom = new SqlParameter("@IsAllFrom", waterMakingInfo.BodyCondition.IsAllFrom);
                paras.Add(paraIsAllFrom);
                SqlParameter paraExcludeFroms = new SqlParameter("@ExcludeFroms", waterMakingInfo.BodyCondition.ExcludeFroms);
                paras.Add(paraExcludeFroms);
                SqlParameter paraRecipients = new SqlParameter("@Recipients", waterMakingInfo.BodyCondition.Recipients);
                paras.Add(paraRecipients);
                SqlParameter paraSubject = new SqlParameter("@Subject", waterMakingInfo.BodyCondition.Subject);
                paras.Add(paraSubject);
                SqlParameter paraIsContainsAttachment = new SqlParameter("@IsContainsAttachment", waterMakingInfo.BodyCondition.IsContainsAttachment);
                paras.Add(paraIsContainsAttachment);
                SqlParameter paraIsAttachmentName = new SqlParameter("@AttachmentName", waterMakingInfo.BodyCondition.AttachmentName);
                paras.Add(paraIsAttachmentName);
                SqlParameter paraIsAllRecipients = new SqlParameter("@IsAllRecipients", waterMakingInfo.WaterMakingContent.IsAllRecipients);
                paras.Add(paraIsAllRecipients);
                SqlParameter paraContect = new SqlParameter("@Content", waterMakingInfo.WaterMakingContent.Content);
                paras.Add(paraContect);
                SqlParameter paraIsAddDate = new SqlParameter("@IsAddDate", waterMakingInfo.WaterMakingContent.IsAddDate);
                paras.Add(paraIsAddDate);
                SqlParameter paraPriority = new SqlParameter("@Priority", waterMakingInfo.Priority);
                paras.Add(paraPriority);


                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_ModifyBodyWaterMaking]", out ds, out strError))
                    {
                        strError = "ModifyBodyWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用ModifyBodyWaterMaking异常", paramstr, strError, transactionid);
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
                                    error.Code = ErrorCode.WaterMarkingNotExist;
                                    break;
                                case -9999:
                                    bResult = false;
                                    error.Code = ErrorCode.SQLException;
                                    LoggerHelper.Error("WaterMakingDBProvider调用ModifyBodyWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用ModifyBodyWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用ModifyBodyWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool DeleteBodyWaterMaking(Guid transactionid, AdminInfo admin, BodyWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", waterMakingInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DeleteBodyWaterMaking]", out ds, out strError))
                    {
                        strError = "DeleteBodyWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用DeleteBodyWaterMaking异常", paramstr, strError, transactionid);
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
                                    LoggerHelper.Error("WaterMakingDBProvider调用DeleteBodyWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用DeleteBodyWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用DeleteBodyWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool DisableBodyWaterMaking(Guid transactionid, AdminInfo admin, BodyWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", waterMakingInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_DisableBodyWaterMaking]", out ds, out strError))
                    {
                        strError = "DisableBodyWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用DisableBodyWaterMaking异常", paramstr, strError, transactionid);
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
                                    LoggerHelper.Error("WaterMakingDBProvider调用DisableBodyWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用DisableBodyWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用DisableBodyWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }

        public bool EnableBodyWaterMaking(Guid transactionid, AdminInfo admin, BodyWaterMakingInfo waterMakingInfo, out ErrorCodeInfo error)
        {
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            string strError = string.Empty;
            bool bResult = true;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraAdminID = new SqlParameter("@AdminID", admin.UserID);
                paras.Add(paraAdminID);
                SqlParameter paraID = new SqlParameter("@ID", waterMakingInfo.ID);
                paras.Add(paraID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_EnableBodyWaterMaking]", out ds, out strError))
                    {
                        strError = "EnableBodyWaterMaking异常,Error:" + strError;
                        LoggerHelper.Error("WaterMakingDBProvider调用EnableBodyWaterMaking异常", paramstr, strError, transactionid);
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
                                    LoggerHelper.Error("WaterMakingDBProvider调用EnableBodyWaterMaking异常", paramstr, "-9999", transactionid);
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
                        LoggerHelper.Error("WaterMakingDBProvider调用EnableBodyWaterMaking异常", paramstr, "ds = null 或者 ds.Tables.Count <= 0", transactionid);
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                bResult = false;
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("WaterMakingDBProvider调用EnableBodyWaterMaking异常", string.Empty, ex.ToString(), transactionid);
            }
            return bResult;
        }
        #endregion
    }
}
