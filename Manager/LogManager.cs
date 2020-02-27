using Common;
using Entity;
using Newtonsoft.Json;
using OfficeOpenXml;
using Provider.DBProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class LogManager
    {
        private string _clientip = string.Empty;
        public LogManager(string ip)
        {
            _clientip = ip;
        }

        public bool GetLogList(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string account, DateTime startTime, DateTime endTime, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||account:{account}";
            paramstr += $"||startTime:{startTime}";
            paramstr += $"||endTime:{endTime}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "GetLogList";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    LogDBProvider Provider = new LogDBProvider();
                    if (!Provider.GetLogList(transactionid, admin, curpage, pagesize, account, startTime, endTime, searchstr, out lists, out error))
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
                LoggerHelper.Error("LogManager调用GetLogList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ExportLogListToExcel(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string account, DateTime startTime, DateTime endTime, string searchstr, out byte[] filebyte, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            filebyte = new byte[0];
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||account:{account}";
            paramstr += $"||startTime:{startTime}";
            paramstr += $"||endTime:{endTime}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "ExportLogListToExcel";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    LogDBProvider Provider = new LogDBProvider();
                    if (!Provider.GetLogList(transactionid, admin, curpage, pagesize, account, startTime, endTime, searchstr, out lists, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("日志信息");
                        sheet.Cells[1, 1].Value = "日志编号";
                        sheet.Cells[1, 2].Value = "操作IP地址";
                        sheet.Cells[1, 3].Value = "操作账号";
                        sheet.Cells[1, 4].Value = "操作类型";
                        sheet.Cells[1, 5].Value = "操作时间";
                        sheet.Cells[1, 6].Value = "详细信息";

                        for (int i = 0; i < lists.Lists.Count; i++)
                        {
                            LogInfo info = (LogInfo)lists.Lists[i];
                            sheet.Cells[i + 2, 1].Value = info.LogNum;
                            sheet.Cells[i + 2, 2].Value = info.ClientIP;
                            sheet.Cells[i + 2, 3].Value = info.AdminAccount;
                            sheet.Cells[i + 2, 4].Value = info.OperateType;
                            sheet.Cells[i + 2, 5].Value = info.OperateTimeName;
                            sheet.Cells[i + 2, 6].Value = info.OperateLog;
                        }

                        filebyte = package.GetAsByteArray();
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
                LoggerHelper.Error("LogManager调用GetLogList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }


        public static bool AddOperateLog(Guid transactionid, LogInfo log)
        {
            bool result = true;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"AdminID:{log.AdminID}";
            paramstr += $"||AdminAccount:{log.AdminAccount}";
            paramstr += $"||RoleID:{log.RoleID}";
            paramstr += $"||OperateType:{log.OperateType}";
            paramstr += $"||OperateTimeName:{log.OperateTimeName}";
            paramstr += $"||OperateTime:{log.OperateTime}";
            paramstr += $"||OperateResult:{log.OperateResult}";
            paramstr += $"||OperateLog:{log.OperateLog}";

            string funname = "AddOperateLog";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    LogDBProvider Provider = new LogDBProvider();
                    if (!Provider.AddOperateLog(transactionid, log, out error))
                    {
                        LoggerHelper.Info(log.AdminAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    LoggerHelper.Info(log.AdminAccount, funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(log.AdminAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("LogManager调用AddOperateLog异常", paramstr, ex.ToString(), transactionid);

                result = false;
            }
            return result;
        }
    }
}
