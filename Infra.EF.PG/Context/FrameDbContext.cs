using Infra.Core.Abstract;
using Infra.Core.Attributes;
using Infra.Core.Extensions;
using Infra.Core.Extensions.Entities;
using Infra.Core.Models;
using Infra.EF.PG.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infra.EF.PG.Context
{
    public class FrameDbContext : DbContext
    {
        private readonly IServiceInfo serviceInfo;
        private readonly Assembly assembly;
        public FrameDbContext(DbContextOptions dbContextOptions, IServiceInfo serviceInfo) : base(dbContextOptions)
        {
            this.serviceInfo = serviceInfo;
            this.assembly=serviceInfo.GetDomainAssembly();
        }
        private void IgnoreTypes(ModelBuilder modelBuilder)
        {

            var ignoreTypes=assembly.GetExportedTypes()
                .Where(x => x.IsAssignableTo(typeof(EntityBase)))
                .Where(x=>x.IsAbstract||x.GetCustomAttribute<NotMappedAttribute>()!=null)
                .ToArray();
            ignoreTypes.ForEach(x=>modelBuilder.Ignore(x));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            IgnoreTypes(modelBuilder);
            var entities = this.assembly.GetExportedTypes()
                .Where(x => x.IsAssignableTo(typeof(EntityBase)))
                .Where(x=>x.IsNotAbstractClass(true))
                .Where(x=>x.IsPublic)
                .ToArray();
            foreach (var info in entities)
            {
                modelBuilder.Entity(info);
            }

            //这里做两件事情
            //1、统一把表名，列名转换成小写。
            //2、读取实体的注释<Summary>部分填充Comment
            var entityTypes = modelBuilder.Model.GetEntityTypes().Where(x => entities.Contains(x.ClrType)).ToList();
            entityTypes.ForEach(entityType =>
            {
                modelBuilder.Entity(entityType.Name, buider =>
                {
                    var typeSummary = entityType.ClrType.GetSummary();
                    buider.ToTable(entityType.ClrType.Name.ToLower(), t => t.HasComment(typeSummary));

                    var properties = entityType.GetProperties().ToList();
                    properties.ForEach(property =>
                    {
                        var memberSummary = entityType.ClrType.GetMember(property.Name).FirstOrDefault()?.GetSummary();
                        buider.Property(property.Name)
                            .HasColumnName(property.Name.ToLower())
                            .HasComment(memberSummary);
                    });
                });
            });
            
            modelBuilder.AddJsonFields();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<object> GetValueAsync(long id, Type type, bool isTracking)
        {
            var frameDbType = typeof(FrameDbContextType<>).MakeGenericType(type);
            return await Task.Run(() =>
            {
                return frameDbType.InvokeMethod("GetValue", [id, this, isTracking]);
            });
        }

        public async Task<object[]> QueryAsync(IQueryDTO queryDTO,Type queryType)
        {
            var frameDbType = typeof(FrameDbContextType<>).MakeGenericType(queryType);
            return await Task.Run<object[]>(() =>
            {
                var result= frameDbType.InvokeMethod("Query", [queryDTO, this]);
                return result as object[];
            });
        }

        public async Task<int> CountAsync(IQueryDTO queryDTO, Type queryType)
        {
            var frameDbType = typeof(FrameDbContextType<>).MakeGenericType(queryType);
            return await Task.Run(() =>
            {
                var result = frameDbType.InvokeMethod("Count", [queryDTO, this]);
                return (int)result;
            });
        }

        public async Task<IPagedList<object>> PageQueryAsync(IPageQueryDTO queryDTO, Type queryType)
        {
            var frameDbType = typeof(FrameDbContextType<>).MakeGenericType(queryType);
            return await Task.Run<IPagedList<object>>(() =>
            {
                var result= frameDbType.InvokeMethod("PageQuery", [queryDTO, this]);
                return result as IPagedList<object>;
            });
        }
    }

    
}
