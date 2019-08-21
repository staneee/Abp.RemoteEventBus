using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestCommon
{
    /// <summary>
    /// 公用测试数据
    /// </summary>
    public static class Data
    {
        public static string BasePath { get; private set; }

        /// <summary>
        /// 8kb数据量
        /// </summary>
        public static byte[] Msg001 { get; set; }
        /// <summary>
        /// 8kb数据量
        /// </summary>
        public static string Msg001Str { get; set; }
        /// <summary>
        /// 40kb数据量
        /// </summary>
        public static byte[] Msg002 { get; set; }
        /// <summary>
        /// 40kb数据量
        /// </summary>
        public static string Msg002Str { get; set; }

        static Data()
        {
            BasePath = Path.GetDirectoryName(typeof(Data).Assembly.Location);
            Msg001Str = ReadFile(nameof(Msg001));
            Msg001 = GetBytes(Msg001Str);

            Msg002Str = ReadFile(nameof(Msg002));
            Msg002 = GetBytes(Msg002Str);
        }


        static string ReadFile(string fileName)
        {
            var filePath = Path.Combine(BasePath, $"{fileName}.txt");
            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        static byte[] GetBytes(string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }
    }
}
