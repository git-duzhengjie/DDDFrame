using Infra.Cache;
using Microsoft.Extensions.Logging;
using Infra.Core.Extensions;
using Infra.Core.System.Extensions;

namespace Infra.IdGenerater.Yitter
{
    public sealed class WorkerNode(ILogger<WorkerNode> logger
           , IRedisProvider redisProvider
           , IDistributedLocker distributedLocker)
    {
        private readonly ILogger<WorkerNode> logger = logger;
        private readonly IRedisProvider redisProvider = redisProvider;
        private readonly IDistributedLocker distributedLocker = distributedLocker;

        internal async Task InitWorkerNodesAsync(string serviceName)
        {
            var workerIdSortedSetCacheKey = GetWorkerIdCacheKey(serviceName);

            if (!redisProvider.KeyExists(workerIdSortedSetCacheKey))
            {
                logger.LogInformation("Starting InitWorkerNodes:{workerIdSortedSetCacheKey}", workerIdSortedSetCacheKey);

                var (Success, LockValue) = await distributedLocker.LockAsync(workerIdSortedSetCacheKey);

                if (!Success)
                {
                    await Task.Delay(300);
                    await InitWorkerNodesAsync(serviceName);
                }

                long count = 0;
                try
                {
                    var set = new Dictionary<long, double>();
                    for (long index = 0; index <= IdGenerater.MaxWorkerId; index++)
                    {
                        set.Add(index, DateTime.Now.GetTotalMilliseconds());
                    }
                    count = await redisProvider.ZAddAsync(workerIdSortedSetCacheKey, set);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                finally
                {
                    await distributedLocker.SafedUnLockAsync(workerIdSortedSetCacheKey, LockValue);
                }

                logger.LogInformation("Finlished InitWorkerNodes:{workerIdSortedSetCacheKey}:{count}"
                    , workerIdSortedSetCacheKey, count);
            }
            else
                logger.LogInformation("Exists WorkerNodes:{workerIdSortedSetCacheKey}", workerIdSortedSetCacheKey);
        }

        internal async Task<long> GetWorkerIdAsync(string serviceName)
        {
            var workerIdSortedSetCacheKey = GetWorkerIdCacheKey(serviceName);

            var scirpt = @"local workerids = redis.call('ZRANGE', @key, @start,@stop)
                                    redis.call('ZADD',@key,@score,workerids[1])
                                    return workerids[1]";

            var parameters = new { key = workerIdSortedSetCacheKey, start = 0, stop = 0, score = DateTime.Now.GetTotalMilliseconds() };
            var luaResult = (byte[])await redisProvider.ScriptEvaluateAsync(scirpt, parameters);
            var workerId = redisProvider.Serializer.Deserialize<long>(luaResult);

            logger.LogInformation("Get WorkerNodes:{workerId}", workerId);

            return workerId;
        }

        internal async Task RefreshWorkerIdScoreAsync(string serviceName, long workerId, double? workerIdScore = null)
        {
            if (workerId < 0 || workerId > IdGenerater.MaxWorkerId)
                throw new Exception(string.Format("worker Id can't be greater than {0} or less than 0", IdGenerater.MaxWorkerId));

            var workerIdSortedSetCacheKey = GetWorkerIdCacheKey(serviceName);

            var score = workerIdScore == null ? DateTime.Now.GetTotalMilliseconds() : workerIdScore.Value;
            await redisProvider.ZAddAsync(workerIdSortedSetCacheKey, new Dictionary<long, double> { { workerId, score } });
            logger.LogDebug("Refresh WorkerNodes:{workerId}:{score}", workerId, score);
        }

        internal static string GetWorkerIdCacheKey(string serviceName) => $"frame:{serviceName}:workids";
    }
}
