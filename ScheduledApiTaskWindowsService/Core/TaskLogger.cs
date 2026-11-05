using System;
using System.IO;

namespace ScheduledApiTaskWindowsService.Core
{
    /// <summary>
    /// 按“日期_任务名.log”写日志，每行格式：HH:mm:ss + Tab + 内容
    /// </summary>
    public static class TaskLogger
    {
        private static readonly object SyncRoot = new object();

        public static string LogDirectory
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"); }
        }

        public static void Log(string taskName, string message)
        {
            try
            {
                string dir = LogDirectory;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "_" + SanitizeFileName(taskName) + ".log";
                string path = Path.Combine(dir, fileName);

                // 保证每条日志只占一行
                string oneLine = (message ?? string.Empty).Replace("\r", " ").Replace("\n", " ");
                string line = DateTime.Now.ToString("HH:mm:ss") + "\t" + oneLine + Environment.NewLine;

                lock (SyncRoot)
                {
                    File.AppendAllText(path, line);
                }
            }
            catch
            {
                // 日志写失败不影响主流程
            }
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Unknown";
            char[] invalid = Path.GetInvalidFileNameChars();
            char[] chars = name.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (Array.IndexOf(invalid, chars[i]) >= 0)
                {
                    chars[i] = '_';
                }
            }
            return new string(chars);
        }
    }
}
