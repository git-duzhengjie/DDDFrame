﻿
using System;
using System.Collections.Generic;

namespace Infra.Cache.Configurations
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisDBOptions
    {
        /// <summary>
        /// Gets or sets the username to be used to connect to the Redis server.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; } = null;

        /// <summary>
        /// Gets or sets the password to be used to connect to the Redis server.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether to use SSL encryption.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is SSL; otherwise, <c>false</c>.
        /// </value>
        public bool IsSsl { get; set; } = false;

        /// <summary>
        /// Gets or sets the SSL Host.
        /// If set, it will enforce this particular host on the server's certificate.
        /// </summary>
        /// <value>
        /// The SSL host.
        /// </value>
        public string SslHost { get; set; } = null;

        /// <summary>
        /// Gets or sets the timeout for any connect operations.
        /// </summary>
        /// <value>
        /// The connection timeout.
        /// </value>
        public Int32 ConnectionTimeout { get; set; } = 5000;

        /// <summary>
        /// Gets the list of endpoints to be used to connect to the Redis server.
        /// </summary>
        /// <value>
        /// The endpoints.
        /// </value>
        public IList<ServerEndPoint> Endpoints { get; } = new List<ServerEndPoint>();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Infra.Cache.Core.Internal.BaseRedisOptions"/>
        /// allow admin.
        /// </summary>
        /// <value><c>true</c> if allow admin; otherwise, <c>false</c>.</value>
        public bool AllowAdmin { get; set; } = false;

        /// <summary>
        /// Gets or sets the string configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public string ConnectionString { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Infra.Cache.Core.Internal.BaseRedisOptions"/>
        /// abort on connect fail.
        /// </summary>
        /// <value><c>true</c> if abort on connect fail; otherwise, <c>false</c>.</value>
        public bool AbortOnConnectFail { get; set; } = false;

        /// <summary>
        /// Gets or sets the Redis database index the cache will use.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public Int32 Database { get; set; } = 0;
    }
}
