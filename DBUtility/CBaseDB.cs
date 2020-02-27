using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtility
{
    public class CBaseDB
    {
        //Construct
        public CBaseDB(string p_strConnection)
        {
            m_strConnection = p_strConnection;
        }


        /// <summary>
        /// 功能: ExcuteTransaction 
        /// 作者:wangguan
        /// 创建日期:2008-10-10
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="p_parameters">ParameterCollection</param>
        /// <param name="p_transactionName">TransactionName</param>
        /// <param name="strError">Error Discription</param>
        /// <returns>Success or fail</returns>
        public bool ExcuteByTransaction(CParameters p_parameters, string p_transactionName, out string strError)
        {
            bool result = false;
            strError = string.Empty;

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_transactionName || 0 == p_transactionName.Length)
            {
                strError = "parameter p_transactionName can not be emply";
                return result;
            }
            #endregion

            result = true;
            #region connect
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = new SqlCommand();

                cmd.CommandTimeout = 60000;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = p_transactionName;
                cmd.Connection = con;
                foreach (SqlParameter para in p_parameters)
                {
                    cmd.Parameters.Add(para);
                }


                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 功能: ExcuteTransaction 
        /// 作者:wangguan
        /// 创建日期:2008-10-10
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="p_parameters">ParameterCollection</param>
        /// <param name="p_transactionName">TransactionName</param>
        /// <param name="strError">Error Discription</param>
        /// <param name="ReturnValue">Transaction Code</param>
        /// <returns>Success or fail</returns>
        public bool ExcuteByTransaction(CParameters p_parameters, string p_transactionName, out int ReturnValue, out string strError)
        {
            bool result = false;
            strError = string.Empty;
            ReturnValue = 0;

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_transactionName || 0 == p_transactionName.Length)
            {
                strError = "parameter p_transactionName can not be emply";
                return result;
            }
            #endregion

            result = true;
            #region connect
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = new SqlCommand();

                cmd.CommandTimeout = 60000;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = p_transactionName;
                cmd.Connection = con;
                foreach (SqlParameter para in p_parameters)
                {
                    cmd.Parameters.Add(para);
                }

                cmd.Parameters.Add("O_RETURN", SqlDbType.Int).Direction = ParameterDirection.Output;
                con.Open();
                cmd.ExecuteNonQuery();
                ReturnValue = Convert.ToInt32(cmd.Parameters["O_RETURN"].Value);
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }




        /// <summary>
        /// 功能: ExcuteTransaction return DataSet
        /// 作者:wangguan
        /// 创建日期:2008-10-10
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="p_paramters">ParameterCollection</param>
        /// <param name="strError">Error Discription</param>
        /// <returns></returns>
        public bool ExcuteByTransaction(CParameters p_paramters, string p_transactionName, out DataSet ds, out string strError)
        {
            bool result = false;
            strError = string.Empty;
            ds = new DataSet();

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_transactionName || 0 == p_transactionName.Length)
            {
                strError = "parameter p_transactionName can not be emply";
                return result;
            }
            #endregion

            result = true;
            #region connect
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = new SqlCommand();
                cmd.CommandTimeout = 60000;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = p_transactionName;
                cmd.Connection = con;
                foreach (SqlParameter para in p_paramters)
                {
                    cmd.Parameters.Add(para);
                }

                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                con.Open();
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (adapter != null)
                        adapter.Dispose();
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 功能: ExcuteTransaction return DataSet & PageParas
        /// 作者: LD
        /// 创建日期:2008-11-9
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="p_paramters">ParameterCollection</param>
        /// <param name="strError">Error Discription</param>
        /// <returns></returns>
        public bool ExcuteByTransaction(CParameters p_paramters, string p_transactionName, out int iRecordCount,
            out int iPageCount, out DataSet ds, out string strError)
        {
            iRecordCount = 0;
            iPageCount = 0;
            bool result = false;
            strError = string.Empty;
            ds = new DataSet();

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_transactionName || 0 == p_transactionName.Length)
            {
                strError = "parameter p_transactionName can not be emply";
                return result;
            }
            #endregion

            result = true;
            #region connect
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = new SqlCommand();
                cmd.CommandTimeout = 60000;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = p_transactionName;
                cmd.Connection = con;
                foreach (SqlParameter para in p_paramters)
                {
                    cmd.Parameters.Add(para);
                }

                SqlParameter RecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);//总记录数
                RecordCount.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(RecordCount);

                SqlParameter PageCount = new SqlParameter("@PageCount", SqlDbType.Int);//总页数
                PageCount.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(PageCount);

                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                con.Open();
                adapter.Fill(ds);

                iRecordCount = (int)RecordCount.Value;
                iPageCount = (int)PageCount.Value;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (adapter != null)
                        adapter.Dispose();
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 功能: ExcuteTransaction return DataSet & PageParas
        /// 作者: LD
        /// 创建日期:2008-11-9
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="p_paramters">ParameterCollection</param>
        /// <param name="strError">Error Discription</param>
        /// <returns></returns>
        public bool ExcuteByTransaction(CParameters p_paramters, string p_transactionName, out decimal minPrice,
            out decimal maxPrice, out DataSet ds, out string strError)
        {
            maxPrice = 0;
            minPrice = 0;
            bool result = false;
            strError = string.Empty;
            ds = new DataSet();

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_transactionName || 0 == p_transactionName.Length)
            {
                strError = "parameter p_transactionName can not be emply";
                return result;
            }
            #endregion

            result = true;
            #region connect
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = new SqlCommand();
                cmd.CommandTimeout = 60000;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = p_transactionName;
                cmd.Connection = con;
                foreach (SqlParameter para in p_paramters)
                {
                    cmd.Parameters.Add(para);
                }

                SqlParameter paraMinPrice = new SqlParameter("@minPrice", SqlDbType.Decimal);
                paraMinPrice.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraMinPrice);

                SqlParameter paraMaxPrice = new SqlParameter("@maxPrice", SqlDbType.Decimal);
                paraMaxPrice.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraMaxPrice);

                SqlParameter paraReturnvalue = new SqlParameter("@O_RETURN", SqlDbType.Int);
                paraReturnvalue.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraReturnvalue);

                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                con.Open();
                adapter.Fill(ds);

                minPrice = (decimal)paraMinPrice.Value;
                maxPrice = (decimal)paraMaxPrice.Value;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (adapter != null)
                        adapter.Dispose();
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 功能: ExcuteTransaction return DataSet with on Parameters
        /// 作者:dk_eric
        /// 创建日期:2009年12月15日
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="p_paramters">ParameterCollection</param>
        /// <param name="strError">Error Discription</param>
        /// <returns></returns>
        public bool ExcuteByTransaction(string p_transactionName, out DataSet ds, out string strError)
        {
            bool result = false;
            strError = string.Empty;
            ds = new DataSet();

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_transactionName || 0 == p_transactionName.Length)
            {
                strError = "parameter p_transactionName can not be emply";
                return result;
            }
            #endregion

            result = true;
            #region connect
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = new SqlCommand();
                cmd.CommandTimeout = 60000;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = p_transactionName;
                cmd.Connection = con;

                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                con.Open();
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (adapter != null)
                        adapter.Dispose();
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 功能: Excute Sql
        /// 作者:wangguan
        /// 创建日期:2008-10-10
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="sqlString">Sql string</param>
        /// <param name="strError">Error Discription</param>
        /// <returns>Success or Fail</returns>
        public bool ExcuteBySql(string p_sqlString, out string strError)
        {
            bool result = false;
            strError = string.Empty;

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_sqlString || 0 == p_sqlString.Length)
            {
                strError = "parameter sqlString can not be emply";
                return result;
            }
            #endregion

            #region connect
            result = true;
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = new SqlCommand(p_sqlString, con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }


        /// <summary>
        /// 功能: Excute Sql
        /// 作者:wangguan
        /// 创建日期:2009-02-06
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="sqlString">Sql string</param>
        /// <param name="strError">Error Discription</param>
        /// <returns>Success or Fail</returns>
        public bool ExcuteBySql(string p_sqlString, CParameters p_paramters, out string strError)
        {
            bool result = false;
            strError = string.Empty;

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_sqlString || 0 == p_sqlString.Length)
            {
                strError = "parameter sqlString can not be emply";
                return result;
            }
            #endregion

            #region connect
            result = true;
            try
            {
                con = new SqlConnection(m_strConnection);
                con.Open();
                cmd = con.CreateCommand();

                foreach (SqlParameter para in p_paramters)
                {
                    cmd.Parameters.Add(para);
                }

                cmd.CommandText = p_sqlString;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }


        /// <summary>
        /// 功能: ExcuteByArrayList
        /// 作者:wangguan
        /// 创建日期:2008-10-10
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool ExcuteByArrayList(ArrayList arr, out string strError)
        {
            bool result = false;
            strError = string.Empty;

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (arr == null || 0 == arr.Count)
            {
                strError = "parameter sqlString can not be emply";
                return result;
            }
            #endregion

            #region connect
            result = true;
            int i = 0;
            SqlTransaction ot = null;
            try
            {
                con = new SqlConnection(m_strConnection);
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandTimeout = 60000;
                ot = con.BeginTransaction();

                cmd.Transaction = ot;

                for (; i < arr.Count; i++)
                {
                    cmd.CommandText = (string)arr[i];
                    cmd.ExecuteNonQuery();
                }
                ot.Commit();
            }
            catch (Exception ex)
            {
                if (ot != null)
                    ot.Rollback();
                strError = ex.Message + " index:" + i.ToString();
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 功能: ExcuteByArraySql
        /// 作者:wangguan
        /// 创建日期:2008-10-10
        /// 最后修改日期:
        /// Review:*********
        /// </summary>
        /// <param name="p_sqlStrings"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool ExcuteByArraySql(string[] p_sqlStrings, out string strError)
        {
            bool result = false;
            strError = string.Empty;

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (0 == p_sqlStrings.Length)
            {
                strError = "parameter sqlString can not be emply";
                return result;
            }
            #endregion

            #region connect
            result = true;
            int i = 0;
            SqlTransaction ot = null;
            try
            {
                con = new SqlConnection(m_strConnection);
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandTimeout = 60000;
                ot = con.BeginTransaction();

                cmd.Transaction = ot;

                for (; i < p_sqlStrings.Length; i++)
                {
                    cmd.CommandText = p_sqlStrings[i];
                    cmd.ExecuteNonQuery();
                }
                ot.Commit();
            }
            catch (Exception ex)
            {
                if (ot != null)
                    ot.Rollback();
                strError = ex.Message + " index:" + i.ToString();
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        public bool ExecuteScalar(string[] p_sqlString, out int Num, out string strError)
        {
            bool result = false;
            strError = string.Empty;
            Num = 1;

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (p_sqlString == null || 0 == p_sqlString.Length)
            {
                strError = "parameter sqlString's length is lawless";
                return result;
            }
            #endregion

            #region connect
            result = true;
            int i = 1;
            SqlTransaction ot = null;
            try
            {
                con = new SqlConnection(m_strConnection);
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandTimeout = 60000;
                ot = con.BeginTransaction();

                cmd.Transaction = ot;

                cmd.CommandText = (string)p_sqlString[0];
                Num = (int)cmd.ExecuteScalar();

                if (p_sqlString.Length > 1)
                {
                    cmd.CommandText = (string)p_sqlString[1];
                    cmd.ExecuteNonQuery();
                }
                ot.Commit();
            }
            catch (Exception ex)
            {
                if (ot != null)
                    ot.Rollback();
                strError = ex.Message + " index:" + i.ToString();
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        public bool ExecuteScalar(string p_sqlString, out object Num, out string strError)
        {
            bool result = false;
            strError = string.Empty;
            Num = null;

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (p_sqlString == null || 0 == p_sqlString.Length)
            {
                strError = "parameter sqlString's length is lawless";
                return result;
            }
            #endregion

            #region connect
            result = true;
            int i = 1;
            SqlTransaction ot = null;
            try
            {
                con = new SqlConnection(m_strConnection);
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandTimeout = 60000;
                ot = con.BeginTransaction();

                cmd.Transaction = ot;

                cmd.CommandText = p_sqlString;
                Num = cmd.ExecuteScalar();

                ot.Commit();
            }
            catch (Exception ex)
            {
                if (ot != null)
                    ot.Rollback();
                strError = ex.Message + " index:" + i.ToString();
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (cmd != null)
                        cmd.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 功能: Get DataSet
        /// 作者:wangguan
        /// 创建日期:2008-10-10
        /// 最后修改日期:
        /// </summary>
        /// <param name="p_sqlString">Sql String</param>
        /// <param name="ds">DataSet</param>
        /// <param name="error">Error Discription</param>
        /// <returns>Success or Fail</returns>
        public bool ExcuteByDataAdapter(string p_sqlString, out DataSet ds, out string strError)
        {
            bool result = false;
            strError = string.Empty;
            ds = new DataSet();

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_sqlString || 0 == p_sqlString.Length)
            {
                strError = "parameter sqlString can not be emply";
                return result;
            }
            #endregion

            #region connect
            result = true;
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = new SqlCommand(p_sqlString, con);
                cmd.CommandTimeout = 60000;

                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                con.Open();
                adapter.Fill(ds);
                if (null == ds)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (adapter != null)
                        adapter.Dispose();

                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 功能: Get DataSet
        /// 作者:wangguan
        /// 创建日期:2008-10-10
        /// 最后修改日期:
        /// </summary>
        /// <param name="p_sqlString">Sql String</param>
        /// <param name="ds">DataSet</param>
        /// <param name="error">Error Discription</param>
        /// <returns>Success or Fail</returns>
        public bool ExcuteByDataAdapterWithRefDataSet(string p_sqlString, ref DataSet ds, out string strError)
        {
            bool result = false;
            strError = string.Empty;
            //ds = new DataSet();

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_sqlString || 0 == p_sqlString.Length)
            {
                strError = "parameter sqlString can not be emply";
                return result;
            }
            #endregion

            #region connect
            result = true;
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = new SqlCommand(p_sqlString, con);
                cmd.CommandTimeout = 60000;

                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                con.Open();
                adapter.Fill(ds);
                if (null == ds)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (adapter != null)
                        adapter.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 功能: Get DataSet
        /// 作者:wangguan
        /// 创建日期:2008-10-10
        /// 最后修改日期:
        /// </summary>
        /// <param name="p_sqlString">Sql String</param>
        /// <param name="ds">DataSet</param>
        /// <param name="error">Error Discription</param>
        /// <returns>Success or Fail</returns>
        public bool ExcuteByDataAdapter(string p_sqlString, CParameters p_paramters, out DataSet ds, out string strError)
        {
            bool result = false;
            strError = string.Empty;
            ds = new DataSet();

            #region check strConnection
            if (null == m_strConnection || 0 == m_strConnection.Length)
            {
                strError = "construct's parameter can not be emply";
                return result;
            }
            if (null == p_sqlString || 0 == p_sqlString.Length)
            {
                strError = "parameter sqlString can not be emply";
                return result;
            }
            #endregion

            #region connect
            result = true;
            try
            {
                con = new SqlConnection(m_strConnection);
                cmd = con.CreateCommand();

                foreach (SqlParameter para in p_paramters)
                {
                    cmd.Parameters.Add(para);
                }

                cmd.CommandText = p_sqlString;
                cmd.CommandTimeout = 60000;

                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;

                con.Open();
                adapter.Fill(ds);
                if (null == ds)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                result = false;
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    if (adapter != null)
                        adapter.Dispose();
                    con.Close();
                }
            }
            #endregion

            return result;
        }


        /// <summary>
        /// 高级查询
        /// </summary>
        /// <param name="ServerID">0珠海GPS1深圳GPS2珠海Customer</param>
        /// <param name="iPageIndex">页索引</param>
        /// <param name="iPageSize">每页显示记录数</param>
        /// <param name="strFields">要显示的字段列表</param>
        /// <param name="strFrom">要查询的表名</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="strOrderBy">排序字段</param>
        /// <param name="iRecordCount">总记录数</param>
        /// <param name="iPageCount">总页数</param>
        /// <param name="ds">DataSet控件ID</param>
        public bool DataSelect(int iPageIndex, int iPageSize, string strFields, string strFrom,
            string strWhere, string strOrderBy, out int iRecordCount, out int iPageCount, out System.Data.DataSet ds, out string strError)
        {
            iRecordCount = 0;
            iPageCount = 0;
            ds = new DataSet();
            strError = string.Empty;
            bool result = false;

            SqlConnection myConn = new SqlConnection(m_strConnection);
            SqlCommand myComm = new SqlCommand("prDefiPager", myConn);
            myComm.CommandType = CommandType.StoredProcedure;

            SqlParameter PageIndex = new SqlParameter("@PageIndex", SqlDbType.Int, 20);//页索引
            PageIndex.Value = iPageIndex;
            myComm.Parameters.Add(PageIndex);

            SqlParameter PageSize = new SqlParameter("@PageSize", SqlDbType.Int, 20);//每页显示记录数
            PageSize.Value = iPageSize;
            myComm.Parameters.Add(PageSize);

            SqlParameter Fields = new SqlParameter("@Fields", SqlDbType.VarChar, 500);//要显示的字段列表
            Fields.Value = strFields;
            myComm.Parameters.Add(Fields);

            SqlParameter From = new SqlParameter("@From", SqlDbType.VarChar, 700);//要查询的表名
            From.Value = strFrom;
            myComm.Parameters.Add(From);

            SqlParameter Where = new SqlParameter("@Where", SqlDbType.VarChar, 500);//查询条件
            Where.Value = strWhere;
            myComm.Parameters.Add(Where);

            SqlParameter OrderBy = new SqlParameter("@OrderBy", SqlDbType.VarChar, 100);//排序字段
            OrderBy.Value = strOrderBy;
            myComm.Parameters.Add(OrderBy);

            SqlParameter RecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);//总记录数
            RecordCount.Direction = ParameterDirection.Output;
            myComm.Parameters.Add(RecordCount);

            SqlParameter PageCount = new SqlParameter("@PageCount", SqlDbType.Int);//总页数
            PageCount.Direction = ParameterDirection.Output;
            myComm.Parameters.Add(PageCount);

            try
            {
                myComm.CommandTimeout = 6000;
                myConn.Open();
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = myComm;
                sda.Fill(ds);

                iRecordCount = (int)RecordCount.Value;
                iPageCount = (int)PageCount.Value;
                result = true;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 高级查询
        /// </summary>
        /// <param name="ServerID">0珠海GPS1深圳GPS2珠海Customer</param>
        /// <param name="iPageIndex">页索引</param>
        /// <param name="iPageSize">每页显示记录数</param>
        /// <param name="strFields">要显示的字段列表</param>
        /// <param name="strFrom">要查询的表名</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="strOrderBy">排序字段</param>
        /// <param name="iRecordCount">总记录数</param>
        /// <param name="iPageCount">总页数</param>
        /// <param name="ds">DataSet控件ID</param>
        public bool DataSelect1(int iPageIndex, int iPageSize, string strFields, string strFrom,
            string strWhere, string strOrderBy, out int iRecordCount, out int iPageCount, out System.Data.DataSet ds, out string strError)
        {
            iRecordCount = 0;
            iPageCount = 0;
            ds = new DataSet();
            strError = string.Empty;
            bool result = false;

            SqlConnection myConn = new SqlConnection(m_strConnection);
            SqlCommand myComm = new SqlCommand("prDefiPager1", myConn);
            myComm.CommandType = CommandType.StoredProcedure;

            SqlParameter PageIndex = new SqlParameter("@PageIndex", SqlDbType.Int, 20);//页索引
            PageIndex.Value = iPageIndex;
            myComm.Parameters.Add(PageIndex);

            SqlParameter PageSize = new SqlParameter("@PageSize", SqlDbType.Int, 20);//每页显示记录数
            PageSize.Value = iPageSize;
            myComm.Parameters.Add(PageSize);

            SqlParameter Fields = new SqlParameter("@Fields", SqlDbType.VarChar, 500);//要显示的字段列表
            Fields.Value = strFields;
            myComm.Parameters.Add(Fields);

            SqlParameter From = new SqlParameter("@From", SqlDbType.VarChar, 700);//要查询的表名
            From.Value = strFrom;
            myComm.Parameters.Add(From);

            SqlParameter Where = new SqlParameter("@Where", SqlDbType.VarChar, 500);//查询条件
            Where.Value = strWhere;
            myComm.Parameters.Add(Where);

            SqlParameter OrderBy = new SqlParameter("@OrderBy", SqlDbType.VarChar, 100);//排序字段
            OrderBy.Value = strOrderBy;
            myComm.Parameters.Add(OrderBy);

            SqlParameter RecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);//总记录数
            RecordCount.Direction = ParameterDirection.Output;
            myComm.Parameters.Add(RecordCount);

            SqlParameter PageCount = new SqlParameter("@PageCount", SqlDbType.Int);//总页数
            PageCount.Direction = ParameterDirection.Output;
            myComm.Parameters.Add(PageCount);

            try
            {
                myConn.Open();
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = myComm;
                sda.Fill(ds);

                iRecordCount = (int)RecordCount.Value;
                iPageCount = (int)PageCount.Value;
                result = true;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }
            return result;
        }

        //public
        private SqlConnection con;
        private SqlCommand cmd;
        private SqlDataReader reader;
        private SqlDataAdapter adapter;

        //private
        private string m_strConnection;
    }
}
