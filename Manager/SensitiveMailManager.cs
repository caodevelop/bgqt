using Common;
using Entity;
using Newtonsoft.Json;
using OfficeOpenXml;
using Provider.ADProvider;
using Provider.DBProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manager
{
    public class SensitiveMailManager
    {
        private string _clientip = string.Empty;
        public SensitiveMailManager(string ip)
        {
            _clientip = ip;
        }
        public bool AddSensitiveMail(Guid transactionid, AdminInfo admin, SensitiveMailInfo sensitiveMailInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||Name:{sensitiveMailInfo.Name}";
            paramstr += $"||Keywords:{sensitiveMailInfo.Keywords}";
            paramstr += $"||StartTime:{sensitiveMailInfo.StartTime}";
            paramstr += $"||EndTime:{sensitiveMailInfo.EndTime}";
            paramstr += $"||Type:{sensitiveMailInfo.MailType}";

            string funname = "AddSensitiveMail";

            try
            {
                do
                {
                    error = sensitiveMailInfo.AddCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    List<SensitiveMailObject> members = new List<SensitiveMailObject>();
                    List<string> distinguishedNames = new List<string>();
                    for (int i = 0; i < sensitiveMailInfo.Objects.Count; i++)
                    {
                        if (!commonProvider.GetADEntryByGuid(sensitiveMailInfo.Objects[i].ObjectID, out entry, out message))
                        {
                            LoggerHelper.Error("AddSensitiveMail调用GetADEntryByGuid异常", paramstr, message, transactionid);
                            continue;
                        }
                        SensitiveMailObject mailObject = new SensitiveMailObject();
                        mailObject.ObjectID = sensitiveMailInfo.Objects[i].ObjectID;
                        mailObject.ObjectName = Convert.ToString(entry.Properties["name"].Value);
                        mailObject.ObjectType = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                        members.Add(mailObject);
                        distinguishedNames.Add(Convert.ToString(entry.Properties["distinguishedName"].Value));
                    }

                    if (!CheckdistinguishedNames(transactionid, distinguishedNames, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("SensitiveMailManager调用AddSensitiveMail异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }

                    //AD添加User
                    sensitiveMailInfo.Status = SensitiveMailStatus.Enable;
                    SensitiveMailDBProvider provider = new SensitiveMailDBProvider();
                    if (!provider.AddSensitiveMail(transactionid, admin, ref sensitiveMailInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    for (int i = 0; i < members.Count; i++)
                    {
                        members[i].SensitiveMailID = sensitiveMailInfo.ID;
                        if (!provider.AddSensitiveMailObjects(transactionid, admin, members[i], out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(sensitiveMailInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);

                    #region 操作日志
                    LogInfo operateLog = new LogInfo();
                    operateLog.AdminID = admin.UserID;
                    operateLog.AdminAccount = admin.UserAccount;
                    operateLog.RoleID = admin.RoleID;
                    operateLog.ClientIP = _clientip;
                    operateLog.OperateResult = true;
                    operateLog.OperateType = "添加敏感邮件规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}添加敏感邮件规则。名称：{sensitiveMailInfo.Name}，" +
                        $"关键字：{sensitiveMailInfo.Keywords}，开始时间：{sensitiveMailInfo.StartTime}，结束时间：{sensitiveMailInfo.EndTime}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("SensitiveMailManager调用AddSensitiveMail异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetSensitiveMailList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, DateTime start, DateTime end, string searchstr, out string strJsonResult)
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
            paramstr += $"||start:{start}";
            paramstr += $"||end:{end}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "GetSensitiveMailList";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    SensitiveMailDBProvider Provider = new SensitiveMailDBProvider();
                    if (!Provider.GetSensitiveMailList(transactionid, admin, curpage, pagesize, start, end, searchstr, out lists, out error))
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
                LoggerHelper.Error("SensitiveMailManager调用GetSensitiveMailList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetSensitiveMailInfo(Guid transactionid, AdminInfo admin, SensitiveMailInfo sensitiveMailInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"ID:{sensitiveMailInfo.ID}";
            string funname = "GetSensitiveMailInfo";

            try
            {
                do
                {
                    SensitiveMailDBProvider Provider = new SensitiveMailDBProvider();
                    if (!Provider.GetSensitiveMailInfo(transactionid, admin, ref sensitiveMailInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    error.Code = ErrorCode.None;
                    string json = JsonConvert.SerializeObject(sensitiveMailInfo);
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info, json);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("SensitiveMailManager调用GetSensitiveMailInfo异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool DeleteSensitiveMail(Guid transactionid, AdminInfo admin, SensitiveMailInfo sensitiveMailInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{sensitiveMailInfo.ID}";

            string funname = "DeleteSensitiveMail";

            try
            {
                do
                {
                    SensitiveMailDBProvider Provider = new SensitiveMailDBProvider();
                    if (!Provider.GetSensitiveMailInfo(transactionid, admin, ref sensitiveMailInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (sensitiveMailInfo.Status == SensitiveMailStatus.Executing)
                    {
                        error.Code = ErrorCode.SensitiveMailIsExecuting;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (!Provider.DeleteSensitiveMail(transactionid, admin, sensitiveMailInfo, out error))
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
                    operateLog.OperateType = "删除敏感邮件规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}删除敏感邮件规则。名称：{sensitiveMailInfo.Name}，" +
                        $"关键字：{sensitiveMailInfo.Keywords}，开始时间：{sensitiveMailInfo.StartTime}，结束时间：{sensitiveMailInfo.EndTime}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("SensitiveMailManager调用DeleteSensitiveMail异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ModifySensitiveMail(Guid transactionid, AdminInfo admin, SensitiveMailInfo sensitiveMailInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{sensitiveMailInfo.ID}";
            paramstr += $"||Keywords:{sensitiveMailInfo.Keywords}";
            paramstr += $"||StartTime:{sensitiveMailInfo.StartTime}";
            paramstr += $"||EndTime:{sensitiveMailInfo.EndTime}";

            string funname = "ModifySensitiveMail";

            try
            {
                do
                {
                    error = sensitiveMailInfo.ChangeCheckProp();

                    if (error.Code != ErrorCode.None)
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    SensitiveMailDBProvider provider = new SensitiveMailDBProvider();
                    SensitiveMailInfo oldSensitiveMailInfo = new SensitiveMailInfo();
                    oldSensitiveMailInfo.ID = sensitiveMailInfo.ID;
                    if (!provider.GetSensitiveMailInfo(transactionid, admin, ref oldSensitiveMailInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (oldSensitiveMailInfo.Status == SensitiveMailStatus.Executing)
                    {
                        error.Code = ErrorCode.SensitiveMailIsExecuting;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    #region
                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    List<SensitiveMailObject> members = new List<SensitiveMailObject>();
                    List<string> distinguishedNames = new List<string>();
                    for (int i = 0; i < sensitiveMailInfo.Objects.Count; i++)
                    {
                        if (!commonProvider.GetADEntryByGuid(sensitiveMailInfo.Objects[i].ObjectID, out entry, out message))
                        {
                            LoggerHelper.Error("ModifiedSensitiveMail调用GetADEntryByGuid异常", paramstr, message, transactionid);
                            continue;
                        }
                        SensitiveMailObject mailObject = new SensitiveMailObject();
                        mailObject.ObjectID = sensitiveMailInfo.Objects[i].ObjectID;
                        mailObject.ObjectType = (NodeType)Enum.Parse(typeof(NodeType), entry.SchemaClassName);
                        mailObject.ObjectName = Convert.ToString(entry.Properties["name"].Value);
                        members.Add(mailObject);
                        distinguishedNames.Add(Convert.ToString(entry.Properties["distinguishedName"].Value));
                    }

                    if (!CheckdistinguishedNames(transactionid, distinguishedNames, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        LoggerHelper.Error("SensitiveMailManager调用ModifiedSensitiveMail异常", paramstr, error.Info, transactionid);
                        result = false;
                        break;
                    }
                    #endregion
                    sensitiveMailInfo.Status = SensitiveMailStatus.Enable;
                    if (!provider.ModifySensitiveMail(transactionid, admin, sensitiveMailInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    for (int i = 0; i < members.Count; i++)
                    {
                        members[i].SensitiveMailID = sensitiveMailInfo.ID;
                        if (!provider.AddSensitiveMailObjects(transactionid, admin, members[i], out error))
                        {
                            strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                            LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                            result = false;
                            break;
                        }
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
                    operateLog.OperateType = "修改敏感邮件规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}修改敏感邮件规则。" +
                        $"原关键字：{oldSensitiveMailInfo.Keywords}，现关键字：{sensitiveMailInfo.Keywords}；" +
                        $"原开始时间：{oldSensitiveMailInfo.StartTime}，现开始时间：{sensitiveMailInfo.StartTime}；" +
                        $"原结束时间：{oldSensitiveMailInfo.EndTime}，现结束时间：{sensitiveMailInfo.EndTime}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("SensitiveMailManager调用ModifySensitiveMail异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ExecuteSensitiveMail(Guid transactionid, AdminInfo admin, SensitiveMailInfo sensitiveMailInfo, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||ID:{sensitiveMailInfo.ID}";

            string funname = "ExecuteSensitiveMail";

            try
            {
                do
                {
                    SensitiveMailDBProvider provider = new SensitiveMailDBProvider();
                    if (!provider.GetSensitiveMailInfo(transactionid, admin, ref sensitiveMailInfo, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    if (sensitiveMailInfo.Status == SensitiveMailStatus.Executing || sensitiveMailInfo.Status == SensitiveMailStatus.Submit)
                    {
                        error.Code = ErrorCode.SensitiveMailExecute;
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    sensitiveMailInfo.Status = SensitiveMailStatus.Submit;
                    sensitiveMailInfo.ExecuteID = transactionid;
                    if (!provider.UpdateSensitiveMailStatus(transactionid, admin, sensitiveMailInfo, out error))
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
                    operateLog.OperateType = "执行敏感邮件规则";
                    operateLog.OperateLog = $"{admin.UserAccount}于{DateTime.Now}执行敏感邮件规则。" +
                        $"名称：{sensitiveMailInfo.Name}，" +
                        $"关键字：{sensitiveMailInfo.Keywords}，" +
                        $"开始时间：{sensitiveMailInfo.StartTime}，" +
                        $"结束时间：{sensitiveMailInfo.EndTime}";
                    LogManager.AddOperateLog(transactionid, operateLog);
                    #endregion

                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("SensitiveMailManager调用ExecuteSensitiveMail异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ExportToExcel(Guid transactionid, AdminInfo admin, Guid sensitiveMailID, out byte[] filebyte, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            filebyte = new byte[0];
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||ID:{sensitiveMailID}";
            string funname = "ExportToExcel";

            try
            {
                do
                {
                    List<UserSensitiveMailQueueInfo> queueInfos = new List<UserSensitiveMailQueueInfo>();
                    SensitiveMailDBProvider Provider = new SensitiveMailDBProvider();
                    if (!Provider.GetSensitiveMailReport(transactionid, admin, sensitiveMailID, out queueInfos, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        int successCount = queueInfos.Count(x => x.Status == SensitiveMailStatus.Success);
                        int failedCount = queueInfos.Count(x => x.Status == SensitiveMailStatus.Failed);
                        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("敏感邮件执行结果");
                        sheet.Cells[1, 1, 1, 5].Merge = true;
                        sheet.Cells[1, 1].Value = $"总执行数量：{queueInfos.Count} 已执行数量：{successCount + failedCount} 待执行数量：{queueInfos.Count - (successCount + failedCount) } 成功数量：{successCount } 失败数量：{failedCount}";
                        sheet.Cells[2, 1].Value = "关键字";
                        sheet.Column(1).Width = 50;//设置列宽
                        sheet.Cells[2, 2].Value = "开始时间";
                        sheet.Column(2).Width = 20;//设置列宽
                        sheet.Cells[2, 3].Value = "结束时间";
                        sheet.Column(3).Width = 20;//设置列宽
                        sheet.Cells[2, 4].Value = "执行时间";
                        sheet.Column(4).Width = 20;//设置列宽
                        sheet.Cells[2, 5].Value = "执行结果";
                        sheet.Column(5).Width = 150;//设置列宽
                        for (int i = 0; i < queueInfos.Count; i++)
                        {
                            sheet.Cells[i + 3, 1].Value = queueInfos[i].Keywords;
                            sheet.Cells[i + 3, 2].Value = queueInfos[i].StartTimeName;
                            sheet.Cells[i + 3, 3].Value = queueInfos[i].EndTimeName;
                            sheet.Cells[i + 3, 4].Value = queueInfos[i].ExecuteEndTimeName;
                            sheet.Cells[i + 3, 5].Value = queueInfos[i].ExecuteResult;
                        }
                        filebyte = package.GetAsByteArray();
                    }
                    error.Code = ErrorCode.None;
                    LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    result = true;
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("SensitiveMailManager调用ExportToExcel异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool test(Guid transactionid, string cmd, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;

            try
            {
                do
                {
                    //if (!ExchangeProvider.TestCommand(cmd, out message))
                    //{
                    //    strJsonResult = JsonHelper.ReturnJson(false, -9999, message);
                    //    result = false;
                    //    break;
                    //}

                    strJsonResult = JsonHelper.ReturnJson(true, Convert.ToInt32(error.Code), error.Info);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool CheckdistinguishedNames(Guid transactionid, List<string> distinguishedNames, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();

            try
            {
                do
                {
                    string rootOuPath = ConfigADProvider.GetCompanyOuDistinguishedName();
                    foreach (string distinguishedName in distinguishedNames)
                    {
                        string ouPath = string.Empty;
                        if (distinguishedName.ToLower().Contains("cn="))
                        {
                            ouPath = distinguishedName.Substring(ouPath.IndexOf(',') + 1);
                        }
                        else
                        {
                            ouPath = distinguishedName;
                        }
                        do
                        {
                            ouPath = ouPath.Substring(ouPath.IndexOf(',') + 1);
                            if (distinguishedNames.Contains(ouPath))
                            {
                                bResult = false;
                                error.Code = ErrorCode.HaveParentOu;
                                error.SetInfo(distinguishedName);
                                break;
                            }

                        } while (ouPath.ToLower().Contains("ou="));
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("CheckdistinguishedNames", string.Empty, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }

            return bResult;
        }

        public void RemoveSensitiveMailQueue()
        {
            Guid transactionid = new Guid();
            int removeSensitiveMailServiceSleepTime = Convert.ToInt32(Common.ConfigHelper.ConfigInstance["removeSensitiveMailServiceSleepTime"]);
            TimeSpan sensitiveMailQueueTimeOut = TimeSpan.FromSeconds(Convert.ToInt32(Common.ConfigHelper.ConfigInstance["sensitiveMailQueueTimeOut"]));
            int sensitiveMailQueueCount = Convert.ToInt32(Common.ConfigHelper.ConfigInstance["sensitiveMailQueueCount"]);

            AdminInfo adminInfo = new AdminInfo();
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;

            try
            {
                do
                {
                    string snkey = ConfigHelper.ConfigInstance["SNKey"];
                    //if (ValidatorHelper.CheckSNKey(snkey))
                    //{
                    List<SensitiveMailInfo> sensitiveMails = new List<SensitiveMailInfo>();
                    SensitiveMailDBProvider provider = new SensitiveMailDBProvider();
                    UserProvider userProvider = new UserProvider();
                    //1.先获取已提交的敏感邮件 2.更新子队列全部执行完成的主数据的状态 3.
                    if (provider.GetSubmitSensitiveMailQueueList(transactionid, out sensitiveMails, out error))
                    {
                        //拆分Ou在子队列总添加用户
                        for (int i = 0; i < sensitiveMails.Count; i++)
                        {
                            SensitiveMailInfo sensitiveMailInfo = sensitiveMails[i];
                            if (!AddUserSensitiveMailQueue(transactionid, sensitiveMailInfo, out error))
                            {
                                continue;
                            }
                            sensitiveMailInfo.Status = SensitiveMailStatus.Executing;
                            provider.UpdateSensitiveMailStatus(transactionid, adminInfo, sensitiveMailInfo, out error);
                        }
                    }

                    //根据线程数量获取需要提交队列的数据
                    List<UserSensitiveMailQueueInfo> queueInfos = new List<UserSensitiveMailQueueInfo>();
                    provider.GetUserSensitiveMailQueueList(transactionid, out queueInfos, out error);
                    if (queueInfos.Count > 0)
                    {
                        Task<string[]> parent = new Task<string[]>(state =>
                        {
                            string[] result = new string[queueInfos.Count];
                                //创建并启动子任务
                                for (int i = 0; i < queueInfos.Count; i++)
                            {
                                UserSensitiveMailQueueInfo queueInfo = queueInfos[i];
                                queueInfo.Status = SensitiveMailStatus.Executing;
                                provider.UpdateUserSensitiveMailQueue(transactionid, queueInfo, string.Empty, out error);
                                var cts = new CancellationTokenSource();
                                new Task(() => { WorkerOperation(queueInfo); }, TaskCreationOptions.AttachedToParent).Start();
                            }
                            return result;
                        }, "");
                        //任务处理完成后执行的操作
                        parent.ContinueWith(t =>
                        {
                        });
                        //启动父任务
                        parent.Start();
                    }
                    //}
                    //等待任务结束 Wait只能等待父线程结束,没办法等到父线程的ContinueWith结束
                    Thread.Sleep(TimeSpan.FromSeconds(removeSensitiveMailServiceSleepTime));
                } while (removeSensitiveMailServiceSleepTime > 0);
            }
            catch (Exception ex)
            {
                Log4netHelper.Error($"RemoveSensitiveMailQueue Exception: {ex.ToString()}");
            }
        }

        public bool AddUserSensitiveMailQueue(Guid transactionid, SensitiveMailInfo sensitiveMailInfo, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;

            try
            {
                do
                {
                    SensitiveMailDBProvider provider = new SensitiveMailDBProvider();
                    UserProvider userProvider = new UserProvider();
                    DirectoryEntry ouEntry = new DirectoryEntry();
                    DirectoryEntry item = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();

                    for (int j = 0; j < sensitiveMailInfo.Objects.Count; j++)
                    {
                        if (sensitiveMailInfo.Objects[j].ObjectType == NodeType.organizationalUnit)
                        {
                            if (!commonProvider.GetADEntryByGuid(sensitiveMailInfo.Objects[j].ObjectID, out ouEntry, out message))
                            {
                                Log4netHelper.Error($"ID：{sensitiveMailInfo.Objects[j].ObjectID}，ObjectName：{sensitiveMailInfo.Objects[j].ObjectName}，ObjectType：{sensitiveMailInfo.Objects[j].ObjectType.ToString()}，GetADEntryByGuid Error：{message}");
                                continue;
                            }

                            DirectoryEntry de = null;
                            de = new DirectoryEntry(ouEntry.Path);
                            DirectorySearcher deSearch = new DirectorySearcher(de);
                            deSearch.SearchRoot = de;
                            string strFilter = commonProvider.GetSearchType(SearchType.MailUser, string.Empty);
                            deSearch.Filter = strFilter;
                            deSearch.SearchScope = SearchScope.Subtree;
                            deSearch.SizeLimit = 20000;
                            deSearch.ServerTimeLimit = TimeSpan.FromSeconds(600);
                            deSearch.ClientTimeout = TimeSpan.FromSeconds(600);
                            SearchResultCollection results = deSearch.FindAll();

                            if (results != null && results.Count > 0)
                            {
                                foreach (SearchResult Result in results)
                                {
                                    item = Result.GetDirectoryEntry();
                                    UserInfo user = new UserInfo();
                                    user.UserID = item.Guid;
                                    user.UserAccount = item.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(item.Properties["userPrincipalName"].Value);
                                    user.SAMAccountName = item.Properties["sAMAccountName"].Value == null ? "" : Convert.ToString(item.Properties["sAMAccountName"].Value);
                                    provider.AddUserSensitiveMailQueue(transactionid, sensitiveMailInfo, user, out error);
                                }
                            }
                        }
                        else if (sensitiveMailInfo.Objects[j].ObjectType == NodeType.user)
                        {
                            if (!commonProvider.GetADEntryByGuid(sensitiveMailInfo.Objects[j].ObjectID, out item, out message))
                            {
                                Log4netHelper.Error($"ID：{sensitiveMailInfo.Objects[j].ObjectID}，ObjectName：{sensitiveMailInfo.Objects[j].ObjectName}，ObjectType：{sensitiveMailInfo.Objects[j].ObjectType.ToString()}，GetADEntryByGuid Error：{message}");
                                continue;
                            }
                            UserInfo user = new UserInfo();
                            user.UserID = item.Guid;
                            provider.AddUserSensitiveMailQueue(transactionid, sensitiveMailInfo, user, out error);
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                Log4netHelper.Error($"RemoveSensitiveMailQueue Exception: {ex.ToString()}");
            }
            return bResult;
        }

        public bool WorkerOperation(UserSensitiveMailQueueInfo info)
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            string message = string.Empty;
            string resultmessage = string.Empty;
            Guid transactionid = Guid.NewGuid();
            string paramstr = string.Empty;
            paramstr += $"SensitiveID:{info.ID}";
            paramstr += $"||Keywords:{info.Keywords}";
            paramstr += $"||StartTime:{info.StartTime}";
            paramstr += $"||EndTime:{info.EndTime}";
            paramstr += $"||UserID:{info.UserID}";
            bool bResult = true;

            try
            {
                do
                {
                    Log4netHelper.Info($"RemoveSensitiveMail Begin: {paramstr}");
                    CommonProvider commonProvider = new CommonProvider();
                    DirectoryEntry userEntry = new DirectoryEntry();
                    SensitiveMailDBProvider provider = new SensitiveMailDBProvider();
                    if (!commonProvider.GetADEntryByGuid(info.UserID, out userEntry, out message))
                    {
                        Log4netHelper.Error($"RemoveSensitiveMail GetADEntryByGuid ID：{info.UserID}， Error：{message}");
                        info.Status = SensitiveMailStatus.Failed;
                        resultmessage = "用户不存在。";
                        provider.UpdateUserSensitiveMailQueue(transactionid, info, resultmessage, out error);
                        bResult = false;
                        break;
                    }
                    string userMail = userEntry.Properties["mail"].Value == null ? "" : Convert.ToString(userEntry.Properties["mail"].Value);
                    ADManagerWebService.ManagerWebService webService = new ADManagerWebService.ManagerWebService();
                    webService.Timeout = -1;
                    if (!webService.RemoveSensitiveMail(transactionid, userMail, info.Keywords, info.StartTime, info.EndTime, out resultmessage, out message))
                    {
                        info.Status = SensitiveMailStatus.Failed;
                        provider.UpdateUserSensitiveMailQueue(transactionid, info, resultmessage, out error);
                        Log4netHelper.Error($"RemoveSensitiveMail ID：{info.ID}， Error：{message}");
                        bResult = false;
                        break;
                    }
                    //记录执行日志
                    info.Status = SensitiveMailStatus.Success;
                    provider.UpdateUserSensitiveMailQueue(transactionid, info, resultmessage, out error);
                    Log4netHelper.Info($"RemoveSensitiveMail End: {paramstr}");
                } while (false);
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("RemoveSensitiveMail异常", paramstr, ex.ToString(), transactionid);
                bResult = false;
            }
            return bResult;
        }

        static void WorkerOperationWait(CancellationTokenSource cts, bool isTimeOut, UserSensitiveMailQueueInfo info)
        {
            SensitiveMailDBProvider provider = new SensitiveMailDBProvider();
            Guid transactionid = Guid.NewGuid();
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (isTimeOut)
            {
                Log4netHelper.Error($"Thead TimeOut,ID: {info.ID}");
                info.Status = SensitiveMailStatus.Failed;
                provider.UpdateUserSensitiveMailQueue(transactionid, info,string.Empty, out error);
                cts.Cancel();
            }
        }
    }
}
