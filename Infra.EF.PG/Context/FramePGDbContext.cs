using Infra.Core.Abstract;
using Infra.Core.Attributes;
using Infra.Core.Extensions;
using Infra.EF.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infra.EF.Context
{
    public class FramePGDbContext(DbContextOptions<FramePGDbContext> dbContextOptions, IServiceInfo serviceInfo) : FrameDbContextBase(dbContextOptions,serviceInfo),IFrameDbContext
    {
        public override bool RelationDatabase => true;

        /// <summary>
        /// 是否开启事务
        /// </summary>
        public override bool Transaction => true;


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
    }

    
}
