using Infra.Core.Abstract;
using Infra.Core.Attributes;
using Infra.Core.Extensions;
using System.Reflection;

namespace Infra.EF.Service
{
    public class EntityFactory:IFactory
    {
        private readonly Dictionary<int,Type> entityTypeMap = [];
        private readonly Dictionary<int, Type> outputTypeMap = [];
        public EntityFactory(IServiceInfo serviceInfo) {
            var contractAssemblyName = serviceInfo.AssemblyName + ".Application.Contract";
            var contractAssemblyFullName = serviceInfo.AssemblyFullName.Replace(serviceInfo.AssemblyName, contractAssemblyName);
            var assemblies=AppDomain.CurrentDomain.GetAssemblies()
                .Where(x=>x.FullName.Contains(".Domain")||x.FullName== contractAssemblyFullName);
            foreach(var assembly in assemblies)
            {
                var types=assembly.GetExportedTypes().Where(x=>x.IsNotAbstractClass(true)).ToArray();
                foreach(var type in types)
                {
                    if (type.IsAssignableTo(typeof(IEntity)))
                    {
                        var obj = Activator.CreateInstance(type) as FrameObjectBase;
                        if (entityTypeMap.ContainsKey(obj.ObjectType))
                        {
                            throw new Exception($"当前Entity类型{obj.ObjectName}已经存在");
                        }
                        entityTypeMap.Add(obj.ObjectType, type);
                    }
                    if (type.IsAssignableTo(typeof(IOutputDTO)))
                    {
                        var obj = Activator.CreateInstance(type) as IOutputDTO;
                        if (outputTypeMap.ContainsKey(obj.ObjectType))
                        {
                            throw new Exception($"当前类型{obj.ObjectName}已经存在");
                        }
                        outputTypeMap.Add(obj.ObjectType, type);
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
            if(entityTypeMap.TryGetValue(input.ObjectType,out var type))
            {
                var mapType = typeof(MapTo<>).MakeGenericType(type);
                return mapType.InvokeMethod("GetValue", [input]) as IEntity;
            }
            throw new Exception($"未找到类型{input.ObjectName}对应的Entity");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Type GetEntityType(int objectType)
        {
            if (entityTypeMap.TryGetValue(objectType, out var type))
            {
                return type;
            }
            throw new Exception($"未找到类型{objectType}对应的Entity");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IOutputDTO GetOutput(IEntity entity)
        {
            if (outputTypeMap.TryGetValue(entity.ObjectType, out var type))
            {
                var mapType = typeof(MapTo<>).MakeGenericType(type);
                return mapType.InvokeMethod("GetValue", [entity]) as IOutputDTO;
            }
            throw new Exception($"未找到类型{entity.ObjectName}对应的Entity");
        }
    }

   
}
