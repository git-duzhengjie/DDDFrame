using Infra.Core.Abstract;
using Infra.EF.PG.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infra.EF.PG
{
    public class FrameDbContext:DbContext
    {
        public FrameDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        private static void IgnoreTypes(ModelBuilder modelBuilder)
        {
            var rhinoCommon = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName!=null&& x.FullName.Contains("Rhino3dm"));
            if (rhinoCommon != null)
            {
                foreach (var item in rhinoCommon.DefinedTypes)
                {
                    modelBuilder.Ignore(item);
                }
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            IgnoreTypes(modelBuilder);
            var entities=this.GetType().Assembly.GetTypes().Where(x=>x.IsAssignableFrom(typeof(EntityBase)));
            foreach (var info in entities)
            {
                modelBuilder.Entity(info);
            }

            //从程序集加载fuluentapi加载配置文件
            var assembly = this.GetType().Assembly;
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
                    buider.ToTable(entityType.ClrType.Name.ToLower(),t=>t.HasComment(typeSummary));

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
        public object GetValue(long id,Type type,bool isTracking=false)
        {
            var frameDbType = typeof(FrameDbContext<>).MakeGenericType(type);
            return frameDbType.InvokeMethod("GetValue", [id,isTracking]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<object> GetValueAsync(long id, Type type,bool isTracking)
        {
            var frameDbType = typeof(FrameDbContext<>).MakeGenericType(type);
            return await Task.Run(() =>
            {
                return frameDbType.InvokeMethod("GetValue", [id,isTracking]);
            });
        }
    }

    public class FrameDbContext<T> where T : EntityBase
    {
        public T GetValue(int id,FrameDbContext frameDbContext,bool isTracking=false)
        {
            if (isTracking)
            {
                return frameDbContext.Set<T>().FirstOrDefault(x => x.Id == id);
            }
            else
            {
                return frameDbContext.Set<T>().AsNoTracking().FirstOrDefault(x=>x.Id == id);
            }
        }
    }
}
