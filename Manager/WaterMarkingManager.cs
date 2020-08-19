using Common;
using Entity;
using Newtonsoft.Json;
using Provider.DBProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class WaterMarkingManager
    {
        private string _clientip = string.Empty;
        public WaterMarkingManager(string ip)
        {
            _clientip = ip;
        }

        #region PDF加水印
        public bool GetPDFWaterMakingList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "GetPDFWaterMakingList";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    WaterMakingDBProvider Provider = new WaterMakingDBProvider();
                    if (!Provider.GetPDFWaterMakingList(transactionid, admin, curpage, pagesize, searchstr, out lists, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(lists);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用GetPDFWaterMakingList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetPDFWaterMakingInfo(Guid transactionid, AdminInfo admin, PDFWaterMakingInfo waterMakingInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"ID:{waterMakingInfo.ID}";
            string funname = "GetPDFWaterMakingInfo";

            try
            {
                do
                {
                    WaterMakingDBProvider Provider = new WaterMakingDBProvider();
                    if (!Provider.GetPDFWaterMakingInfo(transactionid, admin, ref waterMakingInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(waterMakingInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用GetPDFWaterMakingInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool AddPDFWaterMaking(Guid transactionid, AdminInfo admin, PDFWaterMakingInfo waterMakingInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||Name:{waterMakingInfo.Name}";
            paramstr += $"||From:{waterMakingInfo.PDFCondition.From}";
            paramstr += $"||Recipients:{waterMakingInfo.PDFCondition.Recipients}";
            paramstr += $"||Subject:{waterMakingInfo.PDFCondition.Subject}";
            paramstr += $"||PDFName:{waterMakingInfo.PDFCondition.PDFName}";
            paramstr += $"||IsAllRecipients:{waterMakingInfo.WaterMakingContent.IsAllRecipients}";
            paramstr += $"||Content:{waterMakingInfo.WaterMakingContent.Content}";

            string funname = "AddPDFWaterMaking";

            try
            {
                do
                {
                    error = waterMakingInfo.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    WaterMakingDBProvider provider = new WaterMakingDBProvider();
                    if (!provider.AddPDFWaterMaking(transactionid, admin, ref waterMakingInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(waterMakingInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加PDF打水印规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加PDF打水印规则。名称：{waterMakingInfo.Name}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用AddPDFWaterMaking异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ModifyPDFWaterMaking(Guid transactionid, AdminInfo admin, PDFWaterMakingInfo waterMakingInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            paramstr += $"||Name:{waterMakingInfo.Name}";
            paramstr += $"||From:{waterMakingInfo.PDFCondition.From}";
            paramstr += $"||Recipients:{waterMakingInfo.PDFCondition.Recipients}";
            paramstr += $"||Subject:{waterMakingInfo.PDFCondition.Subject}";
            paramstr += $"||PDFName:{waterMakingInfo.PDFCondition.PDFName}";
            paramstr += $"||IsAllRecipients:{waterMakingInfo.WaterMakingContent.IsAllRecipients}";
            paramstr += $"||Content:{waterMakingInfo.WaterMakingContent.Content}";

            string funname = "ModifyPDFWaterMaking";

            try
            {
                do
                {
                    error = waterMakingInfo.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    WaterMakingDBProvider provider = new WaterMakingDBProvider();
                    if (!provider.ModifyPDFWaterMaking(transactionid, admin, ref waterMakingInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(waterMakingInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "修改PDF打水印规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改PDF打水印规则。名称：{waterMakingInfo.Name}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用ModifyPDFWaterMaking异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeletePDFWaterMaking(Guid transactionid, AdminInfo admin, PDFWaterMakingInfo waterMakingInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{waterMakingInfo.ID}";

            string funname = "DeletePDFWaterMaking";

            try
            {
                do
                {
                    WaterMakingDBProvider Provider = new WaterMakingDBProvider();
                    if (!Provider.DeletePDFWaterMaking(transactionid, admin, waterMakingInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "删除PDF打水印规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除PDF打水印规则。名称：{waterMakingInfo.Name}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用DeletePDFWaterMaking异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        #endregion

        #region 正文加水印
        public bool GetBodyWaterMakingList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "GetBodyWaterMakingList";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    WaterMakingDBProvider Provider = new WaterMakingDBProvider();
                    if (!Provider.GetBodyWaterMakingList(transactionid, admin, curpage, pagesize, searchstr, out lists, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(lists);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用GetBodyWaterMakingList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetBodyWaterMakingInfo(Guid transactionid, AdminInfo admin, BodyWaterMakingInfo waterMakingInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"ID:{waterMakingInfo.ID}";
            string funname = "GetBodyWaterMakingInfo";

            try
            {
                do
                {
                    WaterMakingDBProvider Provider = new WaterMakingDBProvider();
                    if (!Provider.GetBodyWaterMakingInfo(transactionid, admin, ref waterMakingInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(waterMakingInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用GetBodyWaterMakingInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool AddBodyWaterMaking(Guid transactionid, AdminInfo admin, BodyWaterMakingInfo waterMakingInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||Name:{waterMakingInfo.Name}";
            paramstr += $"||From:{waterMakingInfo.BodyCondition.From}";
            paramstr += $"||Recipients:{waterMakingInfo.BodyCondition.Recipients}";
            paramstr += $"||Subject:{waterMakingInfo.BodyCondition.Subject}";
            paramstr += $"||PDFName:{waterMakingInfo.BodyCondition.IsContainsAttachment}";
            paramstr += $"||PDFName:{waterMakingInfo.BodyCondition.AttachmentName}";
            paramstr += $"||IsAllRecipients:{waterMakingInfo.WaterMakingContent.IsAllRecipients}";
            paramstr += $"||Content:{waterMakingInfo.WaterMakingContent.Content}";

            string funname = "AddBodyWaterMaking";

            try
            {
                do
                {
                    error = waterMakingInfo.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    WaterMakingDBProvider provider = new WaterMakingDBProvider();
                    if (!provider.AddBodyWaterMaking(transactionid, admin, ref waterMakingInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(waterMakingInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加正文打水印规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加正文打水印规则。名称：{waterMakingInfo.Name}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用AddBodyWaterMaking异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ModifyBodyWaterMaking(Guid transactionid, AdminInfo admin, BodyWaterMakingInfo waterMakingInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{waterMakingInfo.ID}";
            paramstr += $"||Name:{waterMakingInfo.Name}";
            paramstr += $"||From:{waterMakingInfo.BodyCondition.From}";
            paramstr += $"||Recipients:{waterMakingInfo.BodyCondition.Recipients}";
            paramstr += $"||Subject:{waterMakingInfo.BodyCondition.Subject}";
            paramstr += $"||IsContainsAttachment:{waterMakingInfo.BodyCondition.IsContainsAttachment}";
            paramstr += $"||AttachmentName:{waterMakingInfo.BodyCondition.AttachmentName}";
            paramstr += $"||IsAllRecipients:{waterMakingInfo.WaterMakingContent.IsAllRecipients}";
            paramstr += $"||Content:{waterMakingInfo.WaterMakingContent.Content}";

            string funname = "ModifyBodyWaterMaking";

            try
            {
                do
                {
                    error = waterMakingInfo.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    WaterMakingDBProvider provider = new WaterMakingDBProvider();
                    if (!provider.ModifyBodyWaterMaking(transactionid, admin, ref waterMakingInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(waterMakingInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "修改正文打水印规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改正文打水印规则。名称：{waterMakingInfo.Name}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用ModifyBodyWaterMaking异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteBodyWaterMaking(Guid transactionid, AdminInfo admin, BodyWaterMakingInfo waterMakingInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{waterMakingInfo.ID}";

            string funname = "DeleteBodyWaterMaking";

            try
            {
                do
                {
                    WaterMakingDBProvider Provider = new WaterMakingDBProvider();
                    if (!Provider.DeleteBodyWaterMaking(transactionid, admin, waterMakingInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "删除正文打水印规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除正文打水印规则。名称：{waterMakingInfo.Name}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("WaterMarkingManager调用DeleteBodyWaterMaking异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        #endregion

    }
}
