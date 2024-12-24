using Infra.Core.Abstract;
using Infra.Core.Extensions;

namespace Infra.EF.PG.Service
{
    public class EntityFactory
    {
        private readonly Dictionary<int,Type> entityTypeMap = [];
        private readonly Dictionary<int, Type> outputTypeMap = [];
        public EntityFactory() {
            var assemblies=AppDomain.CurrentDomain.GetAssemblies()
                .Where(x=>x.FullName.Contains(".Domain")||x.FullName.Contains(".Contract"));
            foreach(var assembly in assemblies)
            {
                var types=assembly.GetExportedTypes();
                foreach(var type in types)
                {
                    if (type.IsAssignableTo(typeof(IEntity)))
                    {
                        var object=Activator.CreateInstance(type) as IEntity;
                        if (entityTypeMap.ContainsKey(object.objectType))
                        {
                            throw new Exception($"当前Entity类型{object.objectName}已经存在");
                        }
                        entityTypeMap.Add(object.objectType, type);
                    }
                    if (type.IsAssignableTo(typeof(IOutputDTO)))
                    {
                        var object = Activator.CreateInstance(type) as IOutputDTO;
                        if (outputTypeMap.ContainsKey(object.objectType))
                        {
                            throw new Exception($"当前类型{object.objectName}已经存在");
                        }
                        outputTypeMap.Add(object.objectType, type);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IEntity GetEntity(IInputDTO input)
        {
            if(entityTypeMap.TryGetValue(input.objectType,out var type))
            {
                var mapType = typeof(MapTo<>).MakeGenericType(type);
                return mapType.InvokeMethod("GetValue", [input]) as IEntity;
            }
            throw new Exception($"未找到类型{input.objectName}对应的Entity");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IOutputDTO GetOutput(IEntity entity)
        {
            if (outputTypeMap.TryGetValue(entity.objectType, out var type))
            {
                var mapType = typeof(MapTo<>).MakeGenericType(type);
                return mapType.InvokeMethod("GetValue", [entity]) as IOutputDTO;
            }
            throw new Exception($"未找到类型{entity.objectName}对应的Entity");
        }
    }

   
}
