
using Infra.Cache.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Cache.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : Infra.Cache.IRedisProvider
    {
        private const int TimeoutMilliseconds = 5 * 1000;

        public bool StringGetBit(string cacheKey, long offset)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return redisDb.StringGetBit(cacheKey, offset);
        }

        public List<bool> StringGetBit(string cacheKey, IEnumerable<long> offsets)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var results = new Task<bool>[offsets.Count()];

            var batch = redisDb.CreateBatch();
            int index = 0;
            foreach (var position in offsets)
            {
                results[index] = batch.StringGetBitAsync(cacheKey, position);
                index++;
            }
            batch.Execute();

            if (!Task.WaitAll(results, TimeoutMilliseconds)) throw new TimeoutException();

            return results.Select(r => r.Result).ToList();
        }

        public bool StringSetBit(string cacheKey, long position, bool value)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return redisDb.StringSetBit(cacheKey, position, value);
        }

        public List<bool> StringSetBit(string cacheKey, IEnumerable<long> offsets, bool value)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var results = new Task<bool>[offsets.Count()];

            var batch = redisDb.CreateBatch();
            var index = 0;
            foreach (var position in offsets)
            {
                results[index] = batch.StringSetBitAsync(cacheKey, position, value);
                index++;
            }
            batch.Execute();
            if (!Task.WaitAll(results, TimeoutMilliseconds)) throw new TimeoutException();

            return results.Select(r => r.Result).ToList();
        }

        public async Task<bool> StringGetBitAsync(string cacheKey, long offset)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return await redisDb.StringGetBitAsync(cacheKey, offset);
        }

        public async Task<List<bool>> StringGetBitAsync(string cacheKey, IEnumerable<long> offsets)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var results = new bool[offsets.Count()];

            var batch = redisDb.CreateBatch();
            int index = 0;
            foreach (var position in offsets)
            {
                results[index] = await batch.StringGetBitAsync(cacheKey, position);
                index++;
            }
            batch.Execute();

            return results.ToList();
        }

        public async Task<bool> StringSetBitAsync(string cacheKey, long offset, bool value)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return await redisDb.StringSetBitAsync(cacheKey, offset, value);
        }

        public async Task<List<bool>> StringSetBitAsync(string cacheKey, IEnumerable<long> offsets, bool value)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var results = new bool[offsets.Count()];

            var batch = redisDb.CreateBatch();
            var index = 0;
            foreach (var position in offsets)
            {
                results[index] = await batch.StringSetBitAsync(cacheKey, position, value);
                index++;
            }
            batch.Execute();

            return results.ToList();
        }
    }
}
