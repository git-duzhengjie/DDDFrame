using Infra.Core.Abstract;
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
        public FrameDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            Console.WriteLine("sdfsdfsdf");
        }
        private void IgnoreTypes(ModelBuilder modelBuilder)
        {

            var ignoreTypes=GetType().Assembly.GetTypes()
                .Where(x => x.IsAssignableFrom(typeof(EntityBase)))
                .Where(x=>x.IsAbstract||x.GetCustomAttribute<NotMappedAttribute>()!=null)
                .ToArray();
            ignoreTypes.ForEach(x=>modelBuilder.Ignore(x));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            IgnoreTypes(modelBuilder);
            var entities = GetType().Assembly.GetTypes().Where(x => x.IsAssignableFrom(typeof(EntityBase)));
            foreach (var info in entities)
            {
                modelBuilder.Entity(info);
            }
            
            //从程序集加载fuluentapi加载配置文件
            var assembly = GetType().Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);

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
            var task = frameDbType.InvokeMethod("QueryAsync", [queryDTO, this]) as Task<object[]>;
            return await task;
        }

        public async Task<IPagedList<object>> PageQueryAsync(IPageQueryDTO queryDTO, Type queryType)
        {
            var frameDbType = typeof(FrameDbContextType<>).MakeGenericType(queryType);
            var task = frameDbType.InvokeMethod("PageQueryAsync", [queryDTO, this]) as Task<IPagedList<object>>;
            return await task;
        }
    }

    
}
