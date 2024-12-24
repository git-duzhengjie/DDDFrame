using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Network
{
    public static class CompressExtension
    {
        /// <summary>
        /// 使服务器使用Gzip压缩
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        public static void UseResponseGzipCompress(this HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        }
    }
}
