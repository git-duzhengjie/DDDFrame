
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Infra.Cache.Configurations
{
    /// <summary>
    /// Redis database provider.
    /// </summary>
    public interface IRedisDatabaseProvider
    {
        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <returns>The database.</returns>
        IDatabase GetDatabase();

        /// <summary>
        /// Gets the server list.
        /// </summary>
        /// <returns>The server list.</returns>
        IEnumerable<IServer> GetServerList();

        String DBProviderName { get; }
    }
}
