
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Infra.Cache.StackExchange
{
    public partial class DefaultRedisProvider : Infra.Cache.IRedisProvider
    {
        public async Task<dynamic> ScriptEvaluateAsync(String script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            return await _redisDb.ScriptEvaluateAsync(script, keys, values, flags);
        }

        public async Task<dynamic> ScriptEvaluateAsync(String script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            var prepared = LuaScript.Prepare(script);
            var result = await _redisDb.ScriptEvaluateAsync(prepared, parameters, flags);
            return result;
        }
    }
}
