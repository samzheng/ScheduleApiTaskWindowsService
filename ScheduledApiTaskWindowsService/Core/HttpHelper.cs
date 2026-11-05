using System;
using System.IO;
using System.Net;
using System.Text;

namespace ScheduledApiTaskWindowsService.Core
{
    /// <summary>
    /// 使用 HttpWebRequest 调用 API 接口（GET / POST）
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// 调用接口。返回是否成功（2xx 视为成功），responseText 为接口返回内容或错误信息。
        /// </summary>
        public static bool Call(ScheduledTask task, out int statusCode, out string responseText)
        {
            statusCode = 0;
            responseText = string.Empty;

            string method = (task.Config.Method ?? "GET").Trim().ToUpperInvariant();
            int timeout = task.Config.TimeoutSeconds > 0 ? task.Config.TimeoutSeconds * 1000 : 60000;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(task.Config.Url);
            request.Method = method;
            request.Timeout = timeout;
            request.ReadWriteTimeout = timeout;

            try
            {
                if (method == "POST")
                {
                    byte[] data = Encoding.UTF8.GetBytes(task.Config.PostData ?? string.Empty);
                    request.ContentType = string.IsNullOrEmpty(task.Config.ContentType) ? "application/json" : task.Config.ContentType;
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    statusCode = (int)response.StatusCode;
                    responseText = ReadBody(response);
                    return statusCode >= 200 && statusCode < 300;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                if (errorResponse != null)
                {
                    using (errorResponse)
                    {
                        statusCode = (int)errorResponse.StatusCode;
                        responseText = ReadBody(errorResponse);
                    }
                }
                else
                {
                    responseText = "请求异常：" + ex.Message;
                }
                return false;
            }
        }

        private static string ReadBody(HttpWebResponse response)
        {
            Encoding encoding = Encoding.UTF8;
            if (!string.IsNullOrEmpty(response.CharacterSet))
            {
                try { encoding = Encoding.GetEncoding(response.CharacterSet); }
                catch { encoding = Encoding.UTF8; }
            }

            using (Stream stream = response.GetResponseStream())
            {
                if (stream == null) return string.Empty;
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
