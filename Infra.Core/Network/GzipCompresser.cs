using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Network
{
    public class GzipCompresser
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="sCon">字符串内容</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public byte[] Compress(string sCon, Encoding encoding)
        {
            if (sCon == null || sCon.Length == 0)
            {
                return new byte[0];
            }

            var bCon = encoding.GetBytes(sCon);
            using (var ms = new MemoryStream())
            {
                using (var mStream = new MemoryStream(bCon))
                {
                    using (GZipStream stream = new GZipStream(ms, CompressionMode.Compress))
                    {
                        mStream.CopyTo(stream);
                        mStream.Flush();
                    }
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="sCon">字符串内容</param>
        /// <returns></returns>
        public byte[] Compress(string sCon)
        {
            return Compress(sCon, Encoding.UTF8);
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="cbCon">被压缩的内容</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public string Decompress(byte[] cbCon, Encoding encoding)
        {
            if (cbCon == null || cbCon.Length == 0)
            {
                return string.Empty;
            }

            using (var mStream = new MemoryStream(cbCon))
            {
                using (var ms = new MemoryStream())
                {
                    using (var gzip = new GZipStream(mStream, CompressionMode.Decompress))
                    {
                        mStream.Flush();
                        gzip.CopyTo(ms);
                    }
                    var bCon = ms.ToArray();
                    return encoding.GetString(bCon);
                }
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="cbCon">被压缩的内容</param>
        /// <returns></returns>
        public string Decompress(byte[] cbCon)
        {
            return Decompress(cbCon, Encoding.UTF8);
        }
    }
}
