using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Common
{
    public class JsonHelper
    {
        /// <summary>
        /// 去掉特殊字符
        /// </summary>
        public static string string2Json(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// json 序列化
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ToJson(object item)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(item.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, item);
                string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                //替换Json的Date字符串
                string p = @"\\/Date\((\d+)\+\d+\)\\/";
                MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
                Regex reg = new Regex(p);
                jsonString = reg.Replace(jsonString, matchEvaluator);
                return jsonString;
            }
        }


        /// <summary>
        ///json 反序列化
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T FromJsonTo<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T jsonObject = (T)ser.ReadObject(ms);
                return jsonObject;
            }
        }


        /// <summary>
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        public static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>
        /// 将时间字符串转为Json时间
        /// </summary>
        public static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }

        public static DataSet ConvertToDataSetFromJson(string strJson)
        {
            DataSet ds = null;
            try
            {
                ds = new DataSet();
                JavaScriptSerializer JSS = new JavaScriptSerializer();


                object obj = JSS.DeserializeObject(strJson);
                Dictionary<string, object> datajson = (Dictionary<string, object>)obj;

                DataTable dt = new DataTable();

                foreach (var item in datajson)
                {
                    dt.Columns.Add(item.Key);
                }
                DataRow dr = dt.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dr[dt.Columns[i].ColumnName] = Convert.ToString(datajson[dt.Columns[i].ColumnName]);
                }
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);
            }
            catch
            {
                ds = null;
            }

            return ds;
        }

        public static string ReturnstrResult(bool bResult, string strErrorMsg)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("([{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\"", Convert.ToString(bResult).ToLower(), strErrorMsg) + "}])");
            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrJson(bool bResult, string strErrorMsg)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{" + string.Format("result:'{0}',errMsg:'{1}'", Convert.ToString(bResult).ToLower(), strErrorMsg) + "}");
            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrResult(bool bResult, string strErrorMsg, string json)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("([{");
            sb.Append(string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\", ", Convert.ToString(bResult).ToLower(), strErrorMsg));
            sb.Append(string.Format("\"data\":{0}", json));
            sb.Append("}])");

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrJson(bool bResult, string strErrorMsg, string json)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");
            sb.Append(string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\", ", Convert.ToString(bResult).ToLower(), strErrorMsg));
            sb.Append(string.Format("\"data\":{0}", json));
            sb.Append("}");

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrResult(bool bResult, string strErrorMsg, DataSet ds)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("([{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\",\"data\":[", Convert.ToString(bResult).ToLower(), strErrorMsg));
            try
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("{");
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            sb.Append(string.Format("\"{0}\":\"{1}\",", ds.Tables[0].Columns[j].ColumnName, Convert.ToString(ds.Tables[0].Rows[i][j])));

                            if (j == ds.Tables[0].Columns.Count - 1)
                            {
                                sb.Remove(sb.Length - 1, 1);
                            }
                        }
                        sb.Append("},");

                        if (i == ds.Tables[0].Rows.Count - 1)
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                    }
                }
                sb.Append("]}])");
            }
            catch
            {
                sb.Remove(0, sb.Length);
                sb.Append("([{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\",\"data\":[]}])", Convert.ToString(bResult), strErrorMsg));
            }

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrJson(bool bResult, string strErrorMsg, DataSet ds)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\",\"data\":[", Convert.ToString(bResult).ToLower(), strErrorMsg));
            try
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("{");
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            sb.Append(string.Format("\"{0}\":\"{1}\",", ds.Tables[0].Columns[j].ColumnName, Convert.ToString(ds.Tables[0].Rows[i][j])));

                            if (j == ds.Tables[0].Columns.Count - 1)
                            {
                                sb.Remove(sb.Length - 1, 1);
                            }
                        }
                        sb.Append("},");

                        if (i == ds.Tables[0].Rows.Count - 1)
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                    }
                }
                sb.Append("]}");
            }
            catch
            {
                sb.Remove(0, sb.Length);
                sb.Append("{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\",\"data\":[]}", Convert.ToString(bResult), strErrorMsg));
            }

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrResult(bool bResult, string strErrorMsg, int iRecordCount, int iPageCount, DataSet ds)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{" + string.Format("result:'{0}',errMsg:'{1}',recordcount:'{2}',pagecount:'{3}',data:[", Convert.ToString(bResult).ToLower(), strErrorMsg, Convert.ToString(iRecordCount), Convert.ToString(iPageCount)));
            try
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("{");
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            sb.Append(string.Format("{0}:'{1}',", ds.Tables[0].Columns[j].ColumnName, Convert.ToString(ds.Tables[0].Rows[i][j])));

                            if (j == ds.Tables[0].Columns.Count - 1)
                            {
                                sb.Remove(sb.Length - 1, 1);
                            }
                        }
                        sb.Append("},");

                        if (i == ds.Tables[0].Rows.Count - 1)
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                    }
                }
                sb.Append("]}");
            }
            catch
            {
                sb.Remove(0, sb.Length);
                sb.Append("{" + string.Format("result:'{0}',errMsg:'{1}',recordcount:'0',pagecount:'0',data:[]}", Convert.ToString(bResult), strErrorMsg));
            }
            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrResult(bool bResult, string strErrorMsg, int iRecordCount, DataSet ds)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{" + string.Format("result:'{0}',errMsg:'{1}',totalCount:'{2}',data:[", Convert.ToString(bResult).ToLower(), strErrorMsg, Convert.ToString(iRecordCount)));
            try
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("{");
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            sb.Append(string.Format("{0}:'{1}',", ds.Tables[0].Columns[j].ColumnName, Convert.ToString(ds.Tables[0].Rows[i][j])));

                            if (j == ds.Tables[0].Columns.Count - 1)
                            {
                                sb.Remove(sb.Length - 1, 1);
                            }
                        }
                        sb.Append("},");

                        if (i == ds.Tables[0].Rows.Count - 1)
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                    }
                }
                sb.Append("]}");
            }
            catch
            {
                sb.Remove(0, sb.Length);
                sb.Append("{" + string.Format("result:'{0}',errMsg:'{1}',totalCount:'0',data:[]}", Convert.ToString(bResult), strErrorMsg));
            }
            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrResult(bool bResult, string strErrorMsg, Dictionary<string, object> Dictionary)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("([{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\"", Convert.ToString(bResult).ToLower(), strErrorMsg) + ",\"data\":[{");
            try
            {
                if (Dictionary != null && Dictionary.Keys.Count > 0)
                {
                    foreach (KeyValuePair<string, object> Temp in Dictionary)
                    {
                        sb.Append(string.Format("\"{0}\":\"{1}\",", Temp.Key, Convert.ToString(Temp.Value)));
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("}]}])");
            }
            catch
            {
                sb.Remove(0, sb.Length);
                sb.Append("([{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\"", Convert.ToString(bResult), strErrorMsg) + ",\"data\":[]}])");
            }

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrJson(bool bResult, string strErrorMsg, Dictionary<string, object> Dictionary)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\"", Convert.ToString(bResult).ToLower(), strErrorMsg) + ",\"data\":[{");
            try
            {
                if (Dictionary != null && Dictionary.Keys.Count > 0)
                {
                    foreach (KeyValuePair<string, object> Temp in Dictionary)
                    {
                        sb.Append(string.Format("\"{0}\":\"{1}\",", Temp.Key, Convert.ToString(Temp.Value)));
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("}]}");
            }
            catch
            {
                sb.Remove(0, sb.Length);
                sb.Append("{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\"", Convert.ToString(bResult), strErrorMsg) + ",\"data\":[]}");
            }

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnstrResult(DataSet ds)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("[");
            try
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("     {");
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            sb.Append(string.Format("{0}:'{1}',", ds.Tables[0].Columns[j].ColumnName, Convert.ToString(ds.Tables[0].Rows[i][j])));

                            if (j == ds.Tables[0].Columns.Count - 1)
                            {
                                sb.Remove(sb.Length - 1, 1);
                            }
                        }
                        sb.Append("},");

                        if (i == ds.Tables[0].Rows.Count - 1)
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                    }
                }
                sb.Append("]");
            }
            catch
            {
                sb.Remove(0, sb.Length);
                sb.Append("[]");
            }

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnJson(bool bResult, string strErrorMsg)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\"", Convert.ToString(bResult).ToLower(), strErrorMsg) + "}");
            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnJson(bool bResult, int strErrorCode, string strErrorMsg)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\",\"errCode\":\"{2}\"", Convert.ToString(bResult).ToLower(), strErrorMsg, strErrorCode) + "}");
            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnJson(bool bResult, int strErrorCode, string strErrorMsg, string json)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");
            sb.Append(string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\", \"errCode\":\"{2}\",", Convert.ToString(bResult).ToLower(), strErrorMsg, strErrorCode));
            sb.Append(string.Format("\"data\":{0}", json));
            sb.Append("}");

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnJson(bool bResult, int strErrorCode, string strErrorMsg, Dictionary<string, object> Dictionary)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");
            sb.Append(string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\", \"errCode\":\"{2}\",", Convert.ToString(bResult).ToLower(), strErrorMsg, strErrorCode));
            sb.Append("\"data\":[{");
            try
            {
                if (Dictionary != null && Dictionary.Keys.Count > 0)
                {
                    foreach (KeyValuePair<string, object> Temp in Dictionary)
                    {
                        sb.Append(string.Format("\"{0}\":\"{1}\",", Temp.Key, Convert.ToString(Temp.Value)));
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("}]}");
            }
            catch
            {
                sb.Remove(0, sb.Length);
                sb.Append("{");
                sb.Append(string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\", \"errCode\":\"{2}\",", Convert.ToString(bResult).ToLower(), strErrorMsg, strErrorCode));
                sb.Append("\"data\":[]");
                sb.Append("}");
            }

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }
        public static string ReturnJson(bool bResult, string strErrorMsg, Dictionary<string, object> Dictionary)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\"", Convert.ToString(bResult).ToLower(), strErrorMsg) + ",\"data\":[{");
            try
            {
                if (Dictionary != null && Dictionary.Keys.Count > 0)
                {
                    foreach (KeyValuePair<string, object> Temp in Dictionary)
                    {
                        sb.Append(string.Format("\"{0}\":\"{1}\",", Temp.Key, Convert.ToString(Temp.Value)));
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("}]}");
            }
            catch
            {
                sb.Remove(0, sb.Length);
                sb.Append("{" + string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\"", Convert.ToString(bResult), strErrorMsg) + ",\"data\":[]}");
            }

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string ReturnJson(bool bResult, string strErrorMsg, string json)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");
            sb.Append(string.Format("\"result\":\"{0}\",\"errMsg\":\"{1}\", ", Convert.ToString(bResult).ToLower(), strErrorMsg));
            sb.Append(string.Format("\"data\":{0}", json));
            sb.Append("}");

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        public static string Obj2Json<T>(T data)
        {
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(data.GetType());

                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, data);

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                return null;

            }

        }
        /// <summary>
        /// 将datatable转换为json 
        /// </summary>
        /// <param name="dtb">Dt</param>
        /// <returns>JSON字符串</returns>
        public static string Dtb2Json(DataTable dtb)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            System.Collections.ArrayList dic = new System.Collections.ArrayList();
            foreach (DataRow dr in dtb.Rows)
            {
                System.Collections.Generic.Dictionary<string, object> drow = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn dc in dtb.Columns)
                {
                    drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                dic.Add(drow);

            }
            //序列化 
            return jss.Serialize(dic);
        }

        #region 根据提交数据,api接口,动作调用接口
        /// <summary>
        /// 根据提交数据,api接口,动作调用接口
        /// </summary>
        /// <param name="postString">提交的数据</param>
        /// <param name="apiUrl">api接口</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static string CallInterface(string postString, string apiUrl, string action)
        {
            string json = string.Empty;
            byte[] postData = Encoding.UTF8.GetBytes(postString);//编码，尤其是汉字
            string url = apiUrl;
            if (!string.IsNullOrEmpty(action))
                url = string.Format("{0}?action={1}", apiUrl, action);//地址
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
            byte[] responseData = webClient.UploadData(url, "POST", postData);//得到返回字符流  
            //json = Encoding.GetEncoding("GBK").GetString(responseData);//解码
            json = Encoding.UTF8.GetString(responseData);
            return json;
        }
        #endregion

        public static byte[] SerializeObject(object pObj)
        {
            if (pObj == null)
                return null;
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, pObj);
            memoryStream.Position = 0;
            byte[] read = new byte[memoryStream.Length];
            memoryStream.Read(read, 0, read.Length);
            memoryStream.Close();
            return read;
        }

        public static object DeserializeObject(byte[] pBytes)
        {
            object newOjb = null;
            if (pBytes == null)
            {
                return newOjb;
            }
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(pBytes);
            memoryStream.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            newOjb = formatter.Deserialize(memoryStream);
            memoryStream.Close();
            return newOjb;
        }
    }
}
