using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ValidatorHelper
    {
        public const int INT_CONST_DECODEKEY = 5;

        public const int INT_CONST_PASSWORDKEY = 4;

        public ValidatorHelper()
        {
        }

        //将图片以二进制流
        public static byte[] SaveImage(String path)
        {
            byte[] imgBytesIn;

            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
                BinaryReader br = new BinaryReader(fs);
                imgBytesIn = br.ReadBytes((int)fs.Length);  //将流读入到字节数组中
                fs.Close();
            }
            catch
            {
                imgBytesIn = null;
            }
            return imgBytesIn;
        }
        //现实二进制流代表的图片
        public void ShowImgByByte(byte[] imgBytesIn)
        {
            MemoryStream ms = new MemoryStream(imgBytesIn);
            //pictureBox1.Image = Image.FromStream(ms);
        }
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string path = this.textBox1.Text;
        //    byte[] imgBytesIn = SaveImage(path);
        //    ShowImgByByte(imgBytesIn);
        //    //Parameters.Add("@Photo", SqlDbType.Binary).Value = imgBytesIn;

        //}

        public static string EncodeString(string str)
        {
            string enstr = Encode(str, INT_CONST_DECODEKEY);
            byte[] bytes = Encoding.Default.GetBytes(enstr);
            string base64str = Convert.ToBase64String(bytes).Replace('=', '*').Replace('+', '!');
            return base64str;
        }


        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encode(string str, int key)
        {
            int i;
            char[] arr = str.ToCharArray();

            for (i = 0; i < arr.Length; i++)
            {
                arr[i] = Convert.ToChar(Convert.ToInt32(arr[i]) + key);
            }

            return new string(arr);
        }

        /// <summary>
        /// 解码    (1)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string Decode(string str, int key)
        {
            int i;
            char[] arr = str.ToCharArray();

            for (i = 0; i < arr.Length; i++)
            {
                arr[i] = Convert.ToChar(Convert.ToInt32(arr[i]) - key);
            }

            return new string(arr);
        }

        /// <summary>
        /// 计算相差时间    (3)
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <returns></returns>
        public static bool IsOverTime(string strDateTime, int vaTime)
        {
            bool bResult = false;

            try
            {
                DateTime dtStr = Convert.ToDateTime(strDateTime);
                TimeSpan ts = DateTime.Now.Subtract(dtStr).Duration();
                if (ts.TotalSeconds <= vaTime)
                {
                    bResult = true;
                }
            }
            catch (Exception)
            { }

            return bResult;
        }

        /// <summary>
        /// 验证是不是时间类型   (2)
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <returns></returns>
        public static bool CheckTimeType(string strDateTime)
        {
            bool bResult = false;
            try
            {
                DateTime dt = Convert.ToDateTime(strDateTime);
                bResult = true;
            }
            catch (Exception)
            { }

            return bResult;
        }


        /// <summary>
        /// 解码时间戳   (1)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeString(string str)
        {
            byte[] outputb = Convert.FromBase64String(str.Replace('*', '=').Replace('!', '+'));
            string orgStr = Encoding.Default.GetString(outputb);
            int i;
            char[] arr = orgStr.ToCharArray();
            for (i = 0; i < arr.Length; i++)
            {
                arr[i] = Convert.ToChar(Convert.ToInt32(arr[i]) - INT_CONST_DECODEKEY);
            }
            return new string(arr);
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="tokenArray"></param>
        /// <returns></returns>
        public static string GenToken(params string[] tokenArray)
        {
            string token = string.Empty;
            foreach (string arr in tokenArray)
            {
                token += arr + ";";
            }
            return EncodeString(token.Trim(';'));
        }

        /// <summary>
        /// 验证token
        /// 0：合法
        /// 1：token不合法
        /// 2：token超时
        /// </summary>
        /// <param name="token"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ValToken(string token, out ArrayList array)
        {
            return ValToken(token, 7200, out array);
        }

        /// <summary>
        /// 0：合法
        /// 1：token不合法
        /// 2：token超时
        /// </summary>
        /// <param name="token"></param>
        /// <param name="valTime"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ValToken(string token, int valTime, out ArrayList array)
        {
            array = new ArrayList();
            try
            {
                string[] tokenArray = DecodeString(token).Split(';');
                for (int i = 0; i < tokenArray.Length; i++)
                {
                    string a = tokenArray[i];
                    if (i == 0)
                    {
                        if (!CheckTimeType(a))
                        {
                            return 1;
                        }
                        if (!IsOverTime(a, valTime))
                        {
                            return 2;
                        }
                    }
                    array.Add(a);
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        public static bool CheckSNKey(string str)
        {
            bool result = false;
            DateTime datetime = new DateTime(2000, 1, 1);
            try
            {
                if (str.Length < 74)
                {
                    return result;
                }
                else
                {
                    string str1 = str.Substring(5, 5);
                    string str2 = str.Substring(15, 5);
                    string str3 = str.Substring(25, 5);
                    string str4 = str.Substring(35, 5);
                    string str5 = str.Substring(45, 4);

                    string snencode = str1 + str2 + str3 + str4 + str5;
                    datetime = Convert.ToDateTime(DecodeString(snencode));
                    if (DateTime.Now < datetime)
                    {
                        result = true;
                    }
                }
            }
            catch
            {

            }
            return result;
        }
    }
}
