﻿using Common;
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
    public class SystemReportManager
    {
        private string _clientip = string.Empty;
        public SystemReportManager(string ip)
        {
            _clientip = ip;
        }

        public bool GetNoLoginUsers(Guid transactionid, AdminInfo admin, int curpage, int pagesize, DateTime start, DateTime end, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||start:{start}";
            paramstr += $"||end:{end}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "GetNoLoginUsers";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    SystemReportDBProvider Provider = new SystemReportDBProvider();
                    if (!Provider.GetNoLoginUsers(transactionid, admin, curpage, pagesize, start, end, searchstr, out lists, out error))
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
                LoggerHelper.Error("SystemReportManager调用GetNoLoginUsers异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ExportNoLoginUsersToExcel(Guid transactionid, AdminInfo admin, int curpage, int pagesize, DateTime start, DateTime end, string searchstr, out byte[] filebyte, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            filebyte = new byte[0];
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||start:{start}";
            paramstr += $"||end:{end}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "ExportNoLoginUsersToExcel";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    SystemReportDBProvider Provider = new SystemReportDBProvider();
                    if (!Provider.GetNoLoginUsers(transactionid, admin, curpage, pagesize, start, end, searchstr, out lists, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("从未登录用户统计");
                        sheet.Cells[1, 1].Value = "显示名称";
                        sheet.Column(1).Width = 30;//设置列宽
                        sheet.Cells[1, 2].Value = "邮箱";
                        sheet.Column(2).Width = 50;//设置列宽
                        sheet.Cells[1, 3].Value = "创建时间";
                        sheet.Column(3).Width = 30;//设置列宽
                        sheet.Cells[1, 4].Value = "所在路径";
                        sheet.Column(4).Width = 150;//设置列宽

                        for (int i = 0; i < lists.Lists.Count; i++)
                        {
                            SystemReportInfo info = (SystemReportInfo)lists.Lists[i];
                            sheet.Cells[i + 2, 1].Value = info.DisplayName;
                            sheet.Cells[i + 2, 2].Value = info.UserAccount;
                            sheet.Cells[i + 2, 3].Value = info.CreateTimeName;
                            sheet.Cells[i + 2, 4].Value = info.DistinguishedName;
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
                LoggerHelper.Error("SystemReportManager调用ExportNoLoginUsersToExcel异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetPasswordStateUsers(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, PasswordType type, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            paramstr += $"||PasswordType:{type}";
            string funname = "GetPasswordStateUsers";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    SystemReportDBProvider Provider = new SystemReportDBProvider();
                    if (!Provider.GetPasswordStateUsers(transactionid, admin, curpage, pagesize, searchstr, type, out lists, out error))
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
                LoggerHelper.Error("SystemReportManager调用GetPasswordStateUsers异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ExportPasswordStateUsersToExcel(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, PasswordType type, out byte[] filebyte, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            filebyte = new byte[0];
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            paramstr += $"||PasswordType:{type}";
            string funname = "ExportPasswordStateUsersToExcel";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    SystemReportDBProvider Provider = new SystemReportDBProvider();
                    if (!Provider.GetPasswordStateUsers(transactionid, admin, curpage, pagesize, searchstr, type, out lists, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("密码有效期统计统计");
                        sheet.Cells[1, 1].Value = "显示名称";
                        sheet.Column(1).Width = 30;//设置列宽
                        sheet.Cells[1, 2].Value = "邮箱";
                        sheet.Column(2).Width = 50;//设置列宽
                        sheet.Cells[1, 3].Value = "创建时间";
                        sheet.Column(3).Width = 30;//设置列宽
                        sheet.Cells[1, 4].Value = "密码过期时间";
                        sheet.Column(4).Width = 30;//设置列宽
                        sheet.Cells[1, 5].Value = "密码状态";
                        sheet.Column(5).Width = 30;//设置列宽
                        sheet.Cells[1, 6].Value = "所在路径";
                        sheet.Column(6).Width = 150;//设置列宽

                        for (int i = 0; i < lists.Lists.Count; i++)
                        {
                            SystemReportInfo info = (SystemReportInfo)lists.Lists[i];
                            sheet.Cells[i + 2, 1].Value = info.DisplayName;
                            sheet.Cells[i + 2, 2].Value = info.UserAccount;
                            sheet.Cells[i + 2, 3].Value = info.CreateTimeName;
                            if (info.Type != PasswordType.Expired && info.Type != PasswordType.NerverExpire)
                            {
                                sheet.Cells[i + 2, 4].Value = info.PasswordExpireTimeName;
                            }
                            else
                            {
                                sheet.Cells[i + 2, 4].Value = "-";
                            }
                            sheet.Cells[i + 2, 5].Value = info.PasswordTypeName;
                            sheet.Cells[i + 2, 6].Value = info.DistinguishedName;
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
                LoggerHelper.Error("SystemReportManager调用ExportPasswordStateUsersToExcel异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetDisableUsers(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "GetDisableUsers";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    SystemReportDBProvider Provider = new SystemReportDBProvider();
                    if (!Provider.GetDisableUsers(transactionid, admin, curpage, pagesize, searchstr, out lists, out error))
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
                LoggerHelper.Error("SystemReportManager调用GetDisableUsers异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ExportDisableUsersToExcel(Guid transactionid, AdminInfo admin, int curpage, int pagesize, string searchstr, out byte[] filebyte, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            filebyte = new byte[0];
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "ExportDisableUsersToExcel";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    SystemReportDBProvider Provider = new SystemReportDBProvider();
                    if (!Provider.GetDisableUsers(transactionid, admin, curpage, pagesize, searchstr, out lists, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("已停用用户统计");
                        sheet.Cells[1, 1].Value = "显示名称";
                        sheet.Column(1).Width = 30;//设置列宽
                        sheet.Cells[1, 2].Value = "邮箱";
                        sheet.Column(2).Width =50;//设置列宽
                        sheet.Cells[1, 3].Value = "创建时间";
                        sheet.Column(3).Width = 30;//设置列宽
                        sheet.Cells[1, 4].Value = "最近登录时间";
                        sheet.Column(4).Width = 30;//设置列宽
                        sheet.Cells[1, 5].Value = "所在路径";
                        sheet.Column(5).Width = 150;//设置列宽

                        for (int i = 0; i < lists.Lists.Count; i++)
                        {
                            SystemReportInfo info = (SystemReportInfo)lists.Lists[i];
                            sheet.Cells[i + 2, 1].Value = info.DisplayName;
                            sheet.Cells[i + 2, 2].Value = info.UserAccount;
                            sheet.Cells[i + 2, 3].Value = info.CreateTimeName;
                            sheet.Cells[i + 2, 4].Value = info.LastLoginTimeName;
                            sheet.Cells[i + 2, 5].Value = info.DistinguishedName;
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
                LoggerHelper.Error("SystemReportManager调用GetDisableUsers异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetUserCreateTime(Guid transactionid, AdminInfo admin, int curpage, int pagesize, DateTime start, DateTime end, string searchstr, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||start:{start}";
            paramstr += $"||end:{end}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "GetUserCreateTime";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    SystemReportDBProvider Provider = new SystemReportDBProvider();
                    if (!Provider.GetUserCreateTime(transactionid, admin, curpage, pagesize, start, end, searchstr, out lists, out error))
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
                LoggerHelper.Error("SystemReportManager调用GetUserCreateTime异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool ExportUserCreateTimeToExcel(Guid transactionid, AdminInfo admin, int curpage, int pagesize, DateTime start, DateTime end, string searchstr, out byte[] filebyte, out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            filebyte = new byte[0];
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            paramstr += $"||start:{start}";
            paramstr += $"||end:{end}";
            paramstr += $"||searchstr:{searchstr}";
            string funname = "ExportUserCreateTimeToExcel";

            try
            {
                do
                {
                    BaseListInfo lists = new BaseListInfo();
                    SystemReportDBProvider Provider = new SystemReportDBProvider();
                    if (!Provider.GetUserCreateTime(transactionid, admin, curpage, pagesize, start, end, searchstr, out lists, out error))
                    {
                        strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                        LoggerHelper.Info(admin.UserAccount, funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                        break;
                    }

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("用户创建时间统计");
                        sheet.Cells[1, 1].Value = "显示名称";
                        sheet.Column(1).Width = 30;//设置列宽
                        sheet.Cells[1, 2].Value = "邮箱";
                        sheet.Column(2).Width = 50;//设置列宽
                        sheet.Cells[1, 3].Value = "创建时间";
                        sheet.Column(3).Width = 30;//设置列宽
                        sheet.Cells[1, 4].Value = "最近登录时间";
                        sheet.Column(4).Width = 30;//设置列宽
                        sheet.Cells[1, 5].Value = "所在路径";
                        sheet.Column(5).Width = 150;//设置列宽

                        for (int i = 0; i < lists.Lists.Count; i++)
                        {
                            SystemReportInfo info = (SystemReportInfo)lists.Lists[i];
                            sheet.Cells[i + 2, 1].Value = info.DisplayName;
                            sheet.Cells[i + 2, 2].Value = info.UserAccount;
                            sheet.Cells[i + 2, 3].Value = info.CreateTimeName;
                            sheet.Cells[i + 2, 4].Value = info.LastLoginTimeName;
                            sheet.Cells[i + 2, 5].Value = info.DistinguishedName;
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
                LoggerHelper.Error("SystemReportManager调用ExportUserCreateTimeToExcel异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
    }
}
