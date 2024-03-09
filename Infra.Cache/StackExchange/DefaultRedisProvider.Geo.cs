
using Infra.Cache.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Cache.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : IRedisProvider
    {
        public long GeoAdd(String cacheKey, List<(Double longitude, Double latitude, String member)> values)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(values, nameof(values));

            var list = new List<GeoEntry>();

            foreach (var item in values)
            {
                list.Add(new GeoEntry(item.longitude, item.latitude, item.member));
            }

            var res = _redisDb.GeoAdd(cacheKey, list.ToArray());
            return res;
        }

        public async Task<Int64> GeoAddAsync(String cacheKey, List<(Double longitude, Double latitude, String member)> values)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(values, nameof(values));

            var list = new List<GeoEntry>();

            foreach (var item in values)
            {
                list.Add(new GeoEntry(item.longitude, item.latitude, item.member));
            }

            var res = await _redisDb.GeoAddAsync(cacheKey, list.ToArray());
            return res;
        }

        public Double? GeoDist(String cacheKey, String member1, String member2, String unit = "m")
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(member1, nameof(member1));
            ArgumentCheck.NotNullOrWhiteSpace(member2, nameof(member2));
            ArgumentCheck.NotNullOrWhiteSpace(unit, nameof(unit));

            var res = _redisDb.GeoDistance(cacheKey, member1, member2, GetGeoUnit(unit));
            return res;
        }

        public async Task<Double?> GeoDistAsync(String cacheKey, String member1, String member2, String unit = "m")
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(member1, nameof(member1));
            ArgumentCheck.NotNullOrWhiteSpace(member2, nameof(member2));
            ArgumentCheck.NotNullOrWhiteSpace(unit, nameof(unit));

            var res = await _redisDb.GeoDistanceAsync(cacheKey, member1, member2, GetGeoUnit(unit));
            return res;
        }

        public List<String> GeoHash(String cacheKey, List<String> members)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(members, nameof(members));

            var list = new List<RedisValue>();
            foreach (var item in members)
            {
                list.Add(item);
            }

            var res = _redisDb.GeoHash(cacheKey, list.ToArray());
            return res.ToList();
        }

        public async Task<List<String>> GeoHashAsync(String cacheKey, List<String> members)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(members, nameof(members));

            var list = new List<RedisValue>();
            foreach (var item in members)
            {
                list.Add(item);
            }

            var res = await _redisDb.GeoHashAsync(cacheKey, list.ToArray());
            return res.ToList();
        }

        public List<(Double longitude, Double latitude)?> GeoPos(String cacheKey, List<String> members)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(members, nameof(members));

            var list = new List<RedisValue>();
            foreach (var item in members)
            {
                list.Add(item);
            }

            var res = _redisDb.GeoPosition(cacheKey, list.ToArray());

            var tuple = new List<(Double longitude, Double latitude)?>();

            foreach (var item in res)
            {
                if (item.HasValue)
                {
                    tuple.Add((item.Value.Longitude, item.Value.Latitude));
                }
                else
                {
                    tuple.Add(null);
                }
            }

            return tuple;
        }

        public async Task<List<(Double longitude, Double latitude)?>> GeoPosAsync(String cacheKey, List<String> members)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(members, nameof(members));

            var list = new List<RedisValue>();
            foreach (var item in members)
            {
                list.Add(item);
            }

            var res = await _redisDb.GeoPositionAsync(cacheKey, list.ToArray());

            var tuple = new List<(Double longitude, Double latitude)?>();

            foreach (var item in res)
            {
                if (item.HasValue)
                {
                    tuple.Add((item.Value.Longitude, item.Value.Latitude));
                }
                else
                {
                    tuple.Add(null);
                }
            }

            return tuple;
        }

        private GeoUnit GetGeoUnit(String unit)
        {
            GeoUnit geoUnit;
            switch (unit)
            {
                case "km":
                    geoUnit = GeoUnit.Kilometers;
                    break;

                case "ft":
                    geoUnit = GeoUnit.Feet;
                    break;

                case "mi":
                    geoUnit = GeoUnit.Miles;
                    break;

                default:
                    geoUnit = GeoUnit.Meters;
                    break;
            }
            return geoUnit;
        }
    }
}
