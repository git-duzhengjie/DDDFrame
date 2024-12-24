using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Network
{
    public class HttpErrorEventArgs : EventArgs
    {
        /// <summary>
        /// 请求状态码
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// 请求的API
        /// </summary>
        public string API { get; }

        public HttpErrorEventArgs(HttpStatusCode statusCode, string error, string api)
        {
            HttpStatusCode = statusCode;
            Error = error;
            API = api;
        }
    }
}
