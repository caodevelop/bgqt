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
    public class BigAttachManager
    {
        private string _clientip = string.Empty;
        public BigAttachManager(string ip)
        {
            _clientip = ip;
        }

        public bool GetFileList(
            Guid transactionid, 
            Guid userid, 
            int curpage, 
            int pagesize, 
            //string orderbyField, 
            //string orderbyType, 
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";            
            paramstr += $"||curpage:{curpage}";
            paramstr += $"||pagesize:{pagesize}";
            //paramstr += $"||orderbyField:{orderbyField}";
            //paramstr += $"||orderbyType:{orderbyType}";
            string funname = "GetFileList";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    BigFileListInfo bfli = new BigFileListInfo();

               
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.GetFileList(transactionid, userid, curpage, pagesize, out bfli, out error);
                    if (result == true)
                    {
                        resultinfo.data = bfli;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("BigAttachManager调用GetFileList异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetGlobalUploadSetting(
            Guid transactionid,
            Guid userid,          
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            string funname = "GetGlobalUploadSetting";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    AttachSettingItem asi = new AttachSettingItem();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.GetGlobalUploadSetting(transactionid, userid, ref asi, out error);
                    if (result == true)
                    {
                        resultinfo.data = asi;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);                        
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("BigAttachManager调用GetGlobalUploadSetting异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }

        public bool GetUploadParItem(
            Guid transactionid,
            Guid userid,
            out string strJsonResult)
        {
            bool result = true;
            strJsonResult = string.Empty;
            ErrorCodeInfo error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"userid:{userid}";
            string funname = "GetUploadParItem";

            try
            {
                do
                {
                    AttachResultInfo resultinfo = new AttachResultInfo();
                    UploadParItemInfo upi = new UploadParItemInfo();
                    BigAttachDBProvider Provider = new BigAttachDBProvider();
                    result = Provider.GetUploadParItem(transactionid, userid, ref upi, out error);
                    if (result == true)
                    {
                        resultinfo.data = upi;
                        strJsonResult = JsonConvert.SerializeObject(resultinfo);
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), true, transactionid);
                        result = true;
                        break;
                    }
                    else
                    {
                        LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                        result = false;
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Info(userid.ToString(), funname, paramstr, Convert.ToString(error.Code), false, transactionid);
                LoggerHelper.Error("BigAttachManager调用GetUploadParItem异常", paramstr, ex.ToString(), transactionid);
                strJsonResult = JsonHelper.ReturnJson(false, Convert.ToInt32(error.Code), error.Info);
                result = false;
            }
            return result;
        }
        
    }


}
