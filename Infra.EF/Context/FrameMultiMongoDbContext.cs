using Infra.Core.Abstract;
using Infra.EF.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Reflection;

namespace Infra.EF.Context
{
    public class FrameMultiMongoDbContext(DbContextOptions<FrameMongoDbContext> dbContextOptions, IServiceInfo serviceInfo) : FrameMongoDbContext(dbContextOptions, serviceInfo)
    {
        /// <summary>
        /// 是否开启事务
        /// </summary>
        public override bool Transaction => true;
    }
}
