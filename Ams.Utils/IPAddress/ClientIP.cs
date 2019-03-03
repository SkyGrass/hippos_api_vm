using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Web;

namespace Ams.Utils.IPAddress
{
    public static class ClientIP
    {
        //public static string GetWebClientIp()
        //{
        //    string userIP = "未获取用户IP";

        //    try
        //    {
        //        if (System.Web.HttpContext.Current == null
        //    || System.Web.HttpContext.Current.Request == null
        //    || System.Web.HttpContext.Current.Request.ServerVariables == null)
        //            return "";

        //        string CustomerIP = "";

        //        //CDN加速后取到的IP 
        //        CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
        //        if (!string.IsNullOrEmpty(CustomerIP))
        //        {
        //            return CustomerIP;
        //        }

        //        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


        //        if (!String.IsNullOrEmpty(CustomerIP))
        //            return CustomerIP;

        //        if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
        //        {
        //            CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //            if (CustomerIP == null)
        //                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //        }
        //        else
        //        {
        //            CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

        //        }

        //        if (string.Compare(CustomerIP, "unknown", true) == 0)
        //            return System.Web.HttpContext.Current.Request.UserHostAddress;
        //        return CustomerIP;
        //    }
        //    catch { }

        //    return userIP;
        //}

        [DllImport("Iphlpapi.dll")]
        static extern int SendARP(Int32 DestIP, Int32 SrcIP, ref Int64 MacAddr, ref Int32 PhyAddrLen);
        [DllImport("Ws2_32.dll")]
        static extern Int32 inet_addr(string ipaddr);
        ///<summary>  
        /// SendArp获取MAC地址  
        ///</summary>  
        ///<param name="RemoteIP">目标机器的IP地址如(192.168.1.1)</param>  
        ///<returns>目标机器的mac 地址</returns>  
        public static string GetMacAddress(string RemoteIP)
        {

            StringBuilder macAddress = new StringBuilder();

            try
            {
                Int32 remote = inet_addr(RemoteIP);
                Int64 macInfo = new Int64();
                Int32 length = 6;
                SendARP(remote, 0, ref macInfo, ref length);
                string temp = Convert.ToString(macInfo, 16).PadLeft(12, '0').ToUpper();
                int x = 12;
                for (int i = 0; i < 6; i++)
                {
                    if (i == 5)
                    {
                        macAddress.Append(temp.Substring(x - 2, 2));
                    }
                    else
                    {
                        macAddress.Append(temp.Substring(x - 2, 2) + "-");
                    }

                    x -= 2;
                }
                return macAddress.ToString();
            }
            catch
            {
                return macAddress.ToString();
            }
        }

              /// <summary>
     /// 获取客户端IP地址（无视代理）
     /// </summary>
     /// <returns>若失败则返回回送地址</returns>
     public static string GetWebClientIp()
     {
          string userHostAddress = HttpContext.Current.Request.UserHostAddress;
  
          if (string.IsNullOrEmpty(userHostAddress))
         {
             userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
         }
 
         //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
         if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
         {
             return userHostAddress;
         }
         return "127.0.0.1";
     }
 
     /// <summary>
     /// 检查IP地址格式
     /// </summary>
     /// <param name="ip"></param>
     /// <returns></returns>
     public static bool IsIP(string ip)
     {
         return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
     }
    }
}
