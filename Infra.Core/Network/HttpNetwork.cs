using MapsterMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Infra.Core.Network
{
    public class HttpNetwork
    {
        #region <常量>

        #endregion <常量>

        #region <变量>
        private Dictionary<string, string> _headers;

        #endregion <变量>

        #region <属性>
        /// <summary>
        /// 
        /// </summary>
        public static string GatewayHost { get; set; }

        /// <summary>
        /// 前缀
        /// </summary>
        public static string Prefix { get; set; }
        #endregion <属性>

        /// <summary>
        /// 请求错误事件处理句柄
        /// </summary>
        internal event EventHandler<HttpErrorEventArgs> ErrorHandler;

        #region <构造方法和析构方法>
        public HttpNetwork(Dictionary<string, string> headers,string host=null)
        {
            _headers = headers;
            if (host.IsNotNullOrEmpty())
            {
                GatewayHost = host;
            }
        }
        #endregion <构造方法和析构方法>

        #region <方法>
        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(GatewayHost);
            httpClient.Timeout = TimeSpan.FromMinutes(60);

            if (_headers.Count > 0)
            {
                foreach (var item in _headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            return httpClient;
        }

        public string HttpPost(string api, object obj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var httpClient = GetHttpClient();
            var body = obj == null ? null : jsonSerializerSettings == null ? JsonConvert.SerializeObject(obj) : JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            var responseMessage = body == null ? httpClient.PostAsync(Prefix + api, null).Result : httpClient.PostAsync(Prefix + api, new StringContent(body, Encoding.UTF8, "application/json")).Result;
            var responseResult = responseMessage.Content.ReadAsStringAsync().Result;
            if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
            {
                return responseResult;
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("登录已失效，请求重新登录");
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("无权操作");
            }
            else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("请求地址没找到");
            }
            else
            {
                throw new Exception(responseMessage.StatusCode.ToString());
            }
        }

        public T HttpPost<T>(string api, object obj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var responseResult = HttpPost(api, obj, jsonSerializerSettings);
            return jsonSerializerSettings == null ? JsonConvert.DeserializeObject<T>(responseResult) : JsonConvert.DeserializeObject<T>(responseResult, jsonSerializerSettings);
        }

        public async Task<T> HttpPostAsync<T>(string api, object obj, CancellationToken cancellationToken, JsonSerializerSettings jsonSerializerSettings = null, bool useResponseCompress = false)
        {
            var httpClient = GetHttpClient();


            var body = jsonSerializerSettings == null ? JsonConvert.SerializeObject(obj) : JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            var responseMessage = await httpClient.PostAsync(Prefix + api, new StringContent(body, Encoding.UTF8, "application/json"), cancellationToken);

            if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
            {
                string responseResult;
                if (responseMessage.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    var cCon = await responseMessage.Content.ReadAsByteArrayAsync();
                    var compresser = new GzipCompresser();
                    responseResult = compresser.Decompress(cCon);
                }
                else
                {
                    responseResult = await responseMessage.Content.ReadAsStringAsync();
                }
                var responseResultobject = jsonSerializerSettings == null ? JsonConvert.DeserializeObject<T>(responseResult) : JsonConvert.DeserializeObject<T>(responseResult, jsonSerializerSettings);
                return responseResultobject;
            }
            else
            {
                if (ErrorHandler != null)
                {
                    ErrorHandler.Invoke(this, await GetErrorEventArgsAsync(responseMessage, api));
                    return default(T);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
                    throw new Exception(err?.detail);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("登录已失效，请求重新登录");
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception("无权操作");
                }
                else
                {
                    throw new Exception($"请求错误，请稍后再试");
                }
            }
        }


        public async Task<T> HttpPostAsync<T>(string api, HttpContent content, JsonSerializerSettings jsonSerializerSettings)
        {
            var httpClient = GetHttpClient();

            var responseMessage = await httpClient.PostAsync(Prefix + api, content);

            if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
            {
                string responseResult;
                if (responseMessage.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    var cCon = await responseMessage.Content.ReadAsByteArrayAsync();
                    var compresser = new GzipCompresser();
                    responseResult = compresser.Decompress(cCon);
                }
                else
                {
                    responseResult = await responseMessage.Content.ReadAsStringAsync();
                }
                var responseResultobject = jsonSerializerSettings == null ? JsonConvert.DeserializeObject<T>(responseResult) : JsonConvert.DeserializeObject<T>(responseResult, jsonSerializerSettings);
                return responseResultobject;
            }
            else if (ErrorHandler != null)
            {
                ErrorHandler.Invoke(this, await GetErrorEventArgsAsync(responseMessage, api));
                return default(T);
            }
            else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
                throw new Exception(err?.detail);
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("登录已失效，请求重新登录");
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("无权操作");
            }
            else
            {
                throw new Exception($"请求错误，请稍后再试");
            }
        }

        public async Task<T> HttpPostAsync<T>(string api, object obj, JsonSerializerSettings jsonSerializerSettings = null, bool useResponseCompress = false)
        {
            return await HttpPostAsync<T>(api, obj, CancellationToken.None, jsonSerializerSettings, useResponseCompress);
        }

        public async Task<string> HttpPostAsync(string api, object obj, CancellationToken cancellationToken, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var httpClient = GetHttpClient();
            var body = obj == null ? null : jsonSerializerSettings == null ? JsonConvert.SerializeObject(obj) : JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            var responseMessage = body == null ? await httpClient.PostAsync(Prefix + api, null) : await httpClient.PostAsync(Prefix + api, new StringContent(body, Encoding.UTF8, "application/json"), cancellationToken);
            var responseResult = await responseMessage.Content.ReadAsStringAsync();
            if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
            {
                return responseResult;
            }
            else if (ErrorHandler != null)
            {
                ErrorHandler.Invoke(this, await GetErrorEventArgsAsync(responseMessage, api));
                return default;
            }
            else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
                throw new Exception(err?.detail);
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("登录已失效，请求重新登录");
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("无权操作");
            }
            else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("请求地址没找到");
            }
            else
            {
                throw new Exception(responseMessage.StatusCode.ToString());
            }
        }

        public async Task<string> HttpPostAsync(string api, object obj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return await HttpPostAsync(api, obj, CancellationToken.None, jsonSerializerSettings);
        }

        public async Task HttpPutAsync(string api, object obj, CancellationToken? cancellationToken=null, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var httpClient = GetHttpClient();
            var body = obj == null ? null : jsonSerializerSettings == null ? JsonConvert.SerializeObject(obj) : JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            HttpResponseMessage responseMessage;
            if (cancellationToken != null)
            {
                responseMessage = await httpClient.PutAsync(Prefix + api, new StringContent(body, Encoding.UTF8, "application/json"), cancellationToken.Value);
            }
            else
            {
                responseMessage = await httpClient.PutAsync(Prefix + api, new StringContent(body, Encoding.UTF8, "application/json"));
            }

            if (responseMessage.StatusCode != HttpStatusCode.OK && responseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
                    throw new Exception(err?.detail);
                }
                else if (ErrorHandler != null)
                {
                    ErrorHandler.Invoke(this, await GetErrorEventArgsAsync(responseMessage, api));
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("登录已失效，请求重新登录");
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception("无权操作");
                }
                else
                {
                    throw new Exception($"请求错误，请稍后再试");
                }
            }
        }

        public async Task HttpPutAsync(string api, object obj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            await HttpPutAsync(api, obj, CancellationToken.None, jsonSerializerSettings);
        }

        //public async Task<T> HttpPutAsync<T>(string api, object object, JsonSerializerSettings jsonSerializerSettings = null) where T : class
        //{
        //    var httpClient = GetHttpClient();
        //    var body = object == null ? null : jsonSerializerSettings == null ? JsonConvert.SerializeObject(object) : JsonConvert.SerializeObject(object, jsonSerializerSettings);
        //    var responseMessage = await httpClient.PutAsync(Prefix+api, new StringContent(body, Encoding.UTF8, "application/json"));
        //    if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
        //    {
        //        var responseResult = await responseMessage.Content.ReadAsStringAsync();
        //        var responseResultobject = jsonSerializerSettings == null ? JsonConvert.DeserializeObject<T>(responseResult) : JsonConvert.DeserializeObject<T>(responseResult, jsonSerializerSettings);
        //        return responseResultobject;
        //    }
        //    else if (responseMessage.StatusCode == HttpStatusCode.NoContent)
        //    {
        //        return null;
        //    }
        //    else
        //    {

        //        if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
        //        {
        //            var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
        //            throw new Exception(err?.detail);
        //        }
        //        else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        //        {
        //            throw new Exception("登录已失效，请求重新登录");
        //        }
        //        else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
        //        {
        //            throw new Exception("无权操作");
        //        }
        //        else
        //        {
        //            throw new Exception($"请求错误，请稍后再试");
        //        }
        //    }
        //}

        public async Task<T> HttpPutAsync<T>(string api, object obj, CancellationToken cancellationToken, JsonSerializerSettings jsonSerializerSettings = null, bool useResponseCompress = true) where T : class
        {
            var httpClient = GetHttpClient();
            if (useResponseCompress)
            {
                httpClient.UseResponseGzipCompress();
            }
            var body = obj == null ? null : jsonSerializerSettings == null ? JsonConvert.SerializeObject(obj) : JsonConvert.SerializeObject(obj, jsonSerializerSettings);

            HttpResponseMessage responseMessage = await httpClient.PutAsync(Prefix + api, new StringContent(body, Encoding.UTF8, "application/json"), cancellationToken);

            if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                string responseResult;
                if (responseMessage.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    var cCon = await responseMessage.Content.ReadAsByteArrayAsync();
                    var compresser = new GzipCompresser();
                    responseResult = compresser.Decompress(cCon);
                }
                else
                {
                    responseResult = await responseMessage.Content.ReadAsStringAsync();
                }
                var lenItem = responseMessage.Content.Headers.FirstOrDefault(h => h.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase));
                stringBuilder.AppendLine($"响应长度(字节)：{lenItem.Value?.FirstOrDefault()}");
                Stopwatch stopwatch = Stopwatch.StartNew();
                var responseResultobject = jsonSerializerSettings == null ? JsonConvert.DeserializeObject<T>(responseResult) : JsonConvert.DeserializeObject<T>(responseResult, jsonSerializerSettings);
                stopwatch.Stop();
                stringBuilder.AppendLine($"反序列化耗时(毫秒)： {stopwatch.ElapsedMilliseconds.ToString()}");
                using (var fs = File.Open(DateTime.Now.Ticks + ".txt", FileMode.OpenOrCreate))
                {
                    byte[] bCon = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    fs.Write(bCon, 0, bCon.Length);
                    fs.Flush();
                }
                return responseResultobject;
            }
            else if (responseMessage.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }
            else
            {
                if (ErrorHandler != null)
                {
                    ErrorHandler.Invoke(this, await GetErrorEventArgsAsync(responseMessage, api));
                    return default(T);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
                    throw new Exception(err?.detail);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("登录已失效，请求重新登录");
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception("无权操作");
                }
                else
                {
                    throw new Exception($"请求错误，请稍后再试");
                }
            }
        }

        public async Task<T> HttpPutAsync<T>(string api, object obj, JsonSerializerSettings jsonSerializerSettings = null, bool useResponseCompress = true) where T : class
        {
            return await HttpPutAsync<T>(api, obj, CancellationToken.None, jsonSerializerSettings, useResponseCompress);
        }

        public async Task HttpDeleteAsync(string api)
        {
            var httpClient = GetHttpClient();
            var responseMessage = await httpClient.DeleteAsync(Prefix + api);
            if (responseMessage.StatusCode != HttpStatusCode.OK && responseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                if (ErrorHandler != null)
                {
                    ErrorHandler.Invoke(this, await GetErrorEventArgsAsync(responseMessage, api));
                }
                else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
                    throw new Exception(err?.detail);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("登录已失效，请求重新登录");
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception("无权操作");
                }
                else
                {
                    throw new Exception($"请求错误，请稍后再试");
                }
            }
        }

        public async Task<T> HttpDeleteAsync<T>(string api, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var httpClient = GetHttpClient();
            var responseMessage = await httpClient.DeleteAsync(Prefix + api);
            if (responseMessage.StatusCode != HttpStatusCode.OK && responseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                if (ErrorHandler != null)
                {
                    ErrorHandler.Invoke(this, await GetErrorEventArgsAsync(responseMessage, api));
                    return default(T);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
                    throw new Exception(err?.detail);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("登录已失效，请求重新登录");
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception("无权操作");
                }
                else
                {
                    throw new Exception($"请求错误，请稍后再试");
                }
            }
            if (responseMessage.StatusCode == HttpStatusCode.OK && responseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                var responseResult = await responseMessage.Content.ReadAsStringAsync();
                var responseResultobject = jsonSerializerSettings == null ? JsonConvert.DeserializeObject<T>(responseResult) : JsonConvert.DeserializeObject<T>(responseResult, jsonSerializerSettings);
                return responseResultobject;
            }
            return default(T);
        }

        private string objectToStr(object args)
        {
            if (args == null)
                return "";
            StringBuilder sb = new StringBuilder();
            Type type = args.GetType();
            var propertyInfos = type.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                sb.Append(propertyInfos[i].Name + "=" + propertyInfos[i].GetValue(args, null) + "&");
            }
            return "?" + sb.ToString().TrimEnd(new char[] { '&' });
        }

        //public async Task<string> HttpGetAsync(string api, object args = null)
        //{

        //    var httpClient = GetHttpClient();
        //    var argsStr = objectToStr(args);
        //    var responseMessage = await httpClient.GetAsync($"{Prefix+api}{argsStr}");
        //    if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
        //    {
        //        var content = await responseMessage.Content.ReadAsStringAsync();
        //        return content;
        //    }
        //    else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
        //    {
        //        var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
        //        throw new Exception(err?.detail);
        //    }
        //    else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        //    {
        //        throw new Exception("登录已失效，请求重新登录");
        //    }
        //    else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
        //    {
        //        throw new Exception("无权操作");
        //    }
        //    else if (responseMessage.StatusCode == HttpStatusCode.NoContent)
        //    {
        //        return null;
        //    }
        //    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
        //    {
        //        throw new Exception("请求地址没有找到");
        //    }
        //    else
        //    {
        //        throw new Exception($"请求错误，请稍后再试");
        //    }
        //}

        public async Task<string> HttpGetAsync(string api, CancellationToken cancellationToken, object args = null)
        {

            var httpClient = GetHttpClient();
            var argsStr = objectToStr(args);
            var responseMessage = await httpClient.GetAsync($"{Prefix + api}{argsStr}", cancellationToken);
            if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
            {
                var content = await responseMessage.Content.ReadAsStringAsync();
                return content;
            }
            else if (ErrorHandler != null)
            {
                ErrorHandler.Invoke(this, await GetErrorEventArgsAsync(responseMessage, api));
                return default;
            }
            else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
                throw new Exception(err?.detail);
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("登录已失效，请求重新登录");
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("无权操作");
            }
            else if (responseMessage.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }
            else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("请求地址没有找到");
            }
            else
            {
                throw new Exception($"请求错误，请稍后再试");
            }
        }

        public async Task<string> HttpGetAsync(string api, object args = null)
        {
            return await HttpGetAsync(api, CancellationToken.None, args);
        }

        public async Task<T> HttpGetAsync<T>(string api, CancellationToken cancellationToken, JsonSerializerSettings jsonSerializerSettings = null, bool useResponseCompress = false)
        {
            var httpClient = GetHttpClient();
            if (useResponseCompress)
            {
                httpClient.UseResponseGzipCompress();
            }
            var responseMessage = await httpClient.GetAsync(Prefix + api, cancellationToken);
            if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
            {
                string content;
                if (responseMessage.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    var cCon = await responseMessage.Content.ReadAsByteArrayAsync();
                    var compresser = new GzipCompresser();
                    content = compresser.Decompress(cCon);
                }
                else
                {
                    content = await responseMessage.Content.ReadAsStringAsync();
                }
                var result = jsonSerializerSettings == null ? JsonConvert.DeserializeObject<T>(content) : JsonConvert.DeserializeObject<T>(content, jsonSerializerSettings);
                return result;

            }
            else
            {
                if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(T);
                }
                else if (ErrorHandler != null)
                {
                    ErrorHandler.Invoke(this, await GetErrorEventArgsAsync(responseMessage, api));
                    return default(T);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    var err = JsonConvert.DeserializeObject<AppErr>(await responseMessage.Content.ReadAsStringAsync());
                    throw new Exception(err?.detail);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("登录已失效，请求重新登录");
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception("无权操作");
                }
                else
                {
                    throw new Exception($"{api}请求出错：{responseMessage.StatusCode}");
                }

            }
        }

        public async Task<T> HttpGetAsync<T>(string api, JsonSerializerSettings jsonSerializerSettings = null, bool useResponseCompress = false)
        {
            return await HttpGetAsync<T>(api, CancellationToken.None, jsonSerializerSettings, useResponseCompress);
        }

        public T HttpGet<T>(string api, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var httpClient = GetHttpClient();
            var responseMessage = httpClient.GetAsync(Prefix + api).Result;
            if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
            {
                var content = responseMessage.Content.ReadAsStringAsync().Result;
                var result = jsonSerializerSettings == null ? JsonConvert.DeserializeObject<T>(content) : JsonConvert.DeserializeObject<T>(content, jsonSerializerSettings);
                return result;

            }
            else
            {
                if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(T);
                }
                else if (ErrorHandler != null)
                {
                    ErrorHandler.Invoke(this, GetErrorEventArgsAsync(responseMessage, api).Result);
                    return default(T);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    var content = responseMessage.Content.ReadAsStringAsync().Result;
                    var err = JsonConvert.DeserializeObject<AppErr>(content);
                    throw new Exception(err?.detail);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("登录已失效，请求重新登录");
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception("无权操作");
                }
                else
                {
                    throw new Exception($"{api}请求出错：{responseMessage.StatusCode}");
                }
            }


        }

        public string HttpGet(string api, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var httpClient = GetHttpClient();
            var responseMessage = httpClient.GetAsync(Prefix + api).Result;
            if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created)
            {
                var content = responseMessage.Content.ReadAsStringAsync().Result;
                return content;

            }
            else
            {
                if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                {
                    return "";
                }
                else if (ErrorHandler != null)
                {
                    ErrorHandler.Invoke(this, GetErrorEventArgsAsync(responseMessage, api).Result);
                    return "";
                }
                else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    var content = responseMessage.Content.ReadAsStringAsync().Result;
                    var err = JsonConvert.DeserializeObject<AppErr>(content);
                    throw new Exception(err?.detail);
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("登录已失效，请求重新登录");
                }
                else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception("无权操作");
                }
                else
                {
                    throw new Exception($"{api}请求出错：{responseMessage.StatusCode}");
                }
            }


        }

        private async Task<HttpErrorEventArgs> GetErrorEventArgsAsync(HttpResponseMessage httpResponse, string api)
        {
            string err;
            if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = await httpResponse.Content.ReadAsStringAsync();
                err = JsonConvert.DeserializeObject<AppErr>(content)?.detail;
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                err = "登录已失效，请求重新登录";
            }
            else if (httpResponse.StatusCode == HttpStatusCode.Forbidden)
            {
                err = "无权操作";
            }
            else
            {
                err = $"请求出错：{httpResponse.StatusCode}";
            }
            return new HttpErrorEventArgs(httpResponse.StatusCode, err, api);
        }
        #endregion <方法>
    }
}
