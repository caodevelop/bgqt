using DBUtility;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.DOMConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace Common
{
    public class Log4netHelper
    {
        /// <summary>
        /// 使用文本记录异常日志
        /// </summary>
        /// <Author>Ryanding</Author>
        /// <date>2011-05-01</date>
        public static void LoadFileAppender()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string txtLogPath = string.Empty;
            string iisBinPath = AppDomain.CurrentDomain.RelativeSearchPath;

            if (!string.IsNullOrEmpty(iisBinPath))
                txtLogPath = Path.Combine(iisBinPath, "ErrorLog.txt");
            else
                txtLogPath = Path.Combine(currentPath, "ErrorLog.txt");

            log4net.Repository.Hierarchy.Hierarchy hier =
             log4net.LogManager.GetLoggerRepository() as log4net.Repository.Hierarchy.Hierarchy;

            FileAppender fileAppender = new FileAppender();
            fileAppender.Name = "LogFileAppender";
            fileAppender.File = txtLogPath;
            fileAppender.AppendToFile = true;

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "记录时间：%date 线程ID:[%thread] 日志级别：%-5level 出错类：%logger property:[%property{NDC}] - 错误描述：%message%newline";
            patternLayout.ActivateOptions();
            fileAppender.Layout = patternLayout;

            //选择UTF8编码，确保中文不乱码。
            fileAppender.Encoding = Encoding.UTF8;

            fileAppender.ActivateOptions();
            BasicConfigurator.Configure(fileAppender);

        }


        /// <summary>
        /// 对外信息接口
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(message);
        }

        /// <summary>
        /// 对外错误接口
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Error(message);
        }

        /// <summary>
        /// 对外警告接口
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(string message)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Warn(message);
        }

        /// <summary>
        /// 对外调试接口
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Debug(message);
        }

        public static void Error(
          String OperationType,
          String ParamInfo,
          String ErrorInfo,
          Guid TransactionID)
        {
            String msg = "OperationType:" + OperationType + "||ParamInfo:" + ParamInfo + "||ErrorInfo:" + ErrorInfo + "||TransactionID:" + TransactionID;
            Log4netHelper.Error(msg);
        }

        public static void Info(
            Guid OrgID,
            Guid OperatorID,
            String OperationName,
            String ParamInfo,
            String DetailInfo,
            bool Result,
            Guid TransactionID)
        {
            String msg = "OrgID:" + OrgID + "||OperatorID:" + OperatorID + "||OperationName:" + OperationName + "||DetailInfo:" + DetailInfo + "||Result:" + Result + "||TransactionID:" + TransactionID;
            if (Result)
            {
                Log4netHelper.Info(msg);
            }
            else
            {
                Log4netHelper.Error(msg);
            }
        }
    }

    public class LogDBHelper
    {
        public static bool AddErrorLog(
            String OperationType,
            String ParamInfo,
            String ErrorInfo,
            Guid TransactionID
            )
        {
            bool bResult = true;
            string strError = string.Empty;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraOperationType = new SqlParameter("@OperationType", OperationType);
                SqlParameter paraParamInfo = new SqlParameter("@ParamInfo", ParamInfo);
                SqlParameter paraErrorInfo = new SqlParameter("@ErrorInfo", ErrorInfo);
                SqlParameter paraTransactionID = new SqlParameter("@TransactionID", TransactionID);


                paras.Add(paraOperationType);
                paras.Add(paraParamInfo);
                paras.Add(paraErrorInfo);
                paras.Add(paraTransactionID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    int iResult = 0;
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddErrorLog]", out iResult, out strError))
                    {
                        strError = "prc_AddErrorLog数据库执行失败,Error:" + strError;
                        bResult = false;
                        break;
                    }
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //iResult = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            switch (iResult)
                            {
                                case 0:
                                    bResult = true;
                                    break;
                                case -9999:
                                    bResult = false;
                                    break;
                                default:
                                    bResult = false;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            break;
                        }
                    }
                } while (false);

            }
            catch (Exception ex)
            {
                bResult = false;
            }
            return bResult;
        }

        public static bool AddSysLog(
            string UserAccount,
            String OperationName,
            String ParamInfo,
            String DetailInfo,
            String Result,
            Guid TransactionID
            )
        {
            bool bResult = true;
            string strError = string.Empty;
            try
            {
                CParameters paras = new CParameters();
                SqlParameter paraUserAccount = new SqlParameter("@UserAccount", UserAccount);
                SqlParameter paraOperationName = new SqlParameter("@OperationName", OperationName);
                SqlParameter paraParamInfo = new SqlParameter("@ParamInfo", ParamInfo);
                SqlParameter paraDetailInfo = new SqlParameter("@DetailInfo", DetailInfo);
                SqlParameter paraResult = new SqlParameter("@Result", Result);
                SqlParameter paraTransactionID = new SqlParameter("@TransactionID", TransactionID);

                paras.Add(paraUserAccount);
                paras.Add(paraOperationName);
                paras.Add(paraParamInfo);
                paras.Add(paraDetailInfo);
                paras.Add(paraResult);
                paras.Add(paraTransactionID);

                CBaseDB _db = new CBaseDB(Conntection.strConnection);
                do
                {
                    int iResult = 0;
                    DataSet ds = new DataSet();
                    if (!_db.ExcuteByTransaction(paras, "dbo.[prc_AddSysLog]", out iResult, out strError))
                    {
                        strError = "prc_AddSysLog数据库执行失败,Error:" + strError;
                        Log4netHelper.Error(strError);
                        bResult = false;
                        break;
                    }
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            switch (iResult)
                            {
                                case 0:
                                    bResult = true;
                                    break;
                                case -9999:
                                    bResult = false;
                                    break;
                                default:
                                    bResult = false;
                                    break;
                            }
                        }
                        else
                        {
                            bResult = false;
                            break;
                        }
                    }
                } while (false);

            }
            catch (Exception ex)
            {
                Log4netHelper.Error(ex.ToString());
                bResult = false;
            }
            return bResult;
        }
    }

    public class LoggerHelper
    {
        public static void Error(
            String OperationType,
            String ParamInfo,
            String ErrorInfo,
            Guid TransactionID)
        {
            LogDBHelper.AddErrorLog(OperationType, ParamInfo, ErrorInfo, TransactionID);
            String msg = "OperationType:" + OperationType + "||ParamInfo:" + ParamInfo + "||ErrorInfo:" + ErrorInfo + "||TransactionID:" + TransactionID;
            Log4netHelper.Error(msg);
        }

        public static void Info(
            string account,
            String OperationName,
            String ParamInfo,
            String DetailInfo,
            bool Result,
            Guid TransactionID)
        {
            LogDBHelper.AddSysLog(account, OperationName, ParamInfo, DetailInfo, Result.ToString(), TransactionID);
            String msg = "UserAccount:" + account + "||OperationName:" + OperationName + "||DetailInfo:" + DetailInfo + "||Result:" + Result + "||TransactionID:" + TransactionID;
            if (Result)
            {
                Log4netHelper.Info(msg);
            }
            else
            {
                Log4netHelper.Error(msg);
            }
        }
    }
}
