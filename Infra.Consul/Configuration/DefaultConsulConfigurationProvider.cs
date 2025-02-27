﻿
using Consul;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Infra.Consul.Configuration
{
    /// <summary>
    /// Consul配置默认提供者
    /// </summary>
    public class DefaultConsulConfigurationProvider : ConfigurationProvider
    {
        #region <变量>

        private readonly ConsulClient _consulClient;
        private readonly string _path="test";
        private readonly Int32 _waitMillisecond;
        private readonly bool _reloadOnChange;
        private ulong _currentIndex;
        private Task _pollTask;

        #endregion <变量>

        #region <构造方法和析构方法>

        public DefaultConsulConfigurationProvider(ConsulConfig config, bool reloadOnChanges)
        {
            _consulClient = new ConsulClient(x =>
            {
                // consul 服务地址
                x.Address = new Uri(config.Address);
            });
            //_path = config.ConsulKeyPath;
            _waitMillisecond = 3;
            _reloadOnChange = reloadOnChanges;
        }

        #endregion <构造方法和析构方法>

        #region <方法>

        public override void Load()
        {
            if (_pollTask != null)
            {
                return;
            }
            //加载数据
            LoadData(GetData().GetAwaiter().GetResult());
            //处理数据变更
            PollReaload();
        }

        //设置数据
        private void LoadData(QueryResult<KVPair> queryResult)
        {
            _currentIndex = queryResult.LastIndex;
            if (queryResult.Response == null
                || queryResult.Response.Value == null
                || !queryResult.Response.Value.Any())
            {
                return;
            }
            Stream stream = new MemoryStream(queryResult.Response.Value);
            Data = JsonConfigurationFileParser.Parse(stream);

        }

        //获取consul配置中心数据
        private async Task<QueryResult<KVPair>> GetData()
        {
            var res = await _consulClient.KV.Get(_path);
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.NotFound)
            {
                return res;
            }
            throw new Exception($"Error loading configuration from consul. Status code: {res.StatusCode}.");
        }

        //处理数据变更
        private void PollReaload()
        {
            if (_reloadOnChange)
            {
                _pollTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        QueryResult<KVPair> queryResult = await GetData();
                        if (queryResult.LastIndex != _currentIndex)
                        {
                            LoadData(queryResult);
                            OnReload();
                        }
                        await Task.Delay(_waitMillisecond);
                    }
                });
            }
        }

        #endregion <方法>
    }
}
