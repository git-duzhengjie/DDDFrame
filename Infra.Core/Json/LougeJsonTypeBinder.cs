using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Infra.Core.Json
{
    public class LougeJsonTypeBinder : ISerializationBinder
    {
        static IDictionary<string, Type> typeMap = new Dictionary<string, Type>();

        static LougeJsonTypeBinder()
        {
            try
            {
                var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var lougeFiles = GetLougeAssemblyFiles(currentDirectory);
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x.FullName.Contains("Contract")
                || x.FullName.Contains("Louge.Geometry")
                || x.FullName.Contains("Domain")
                || x.FullName.Contains("Louge.Plugin"));
                var pluginAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x.FullName.Contains("Louge.Plugin"))
                    .Where(x => x.ManifestModule.Name.EndsWith(".rhp"));
                foreach (var pluginAssembly in pluginAssemblies)
                {
                    var directory = Path.GetDirectoryName(pluginAssembly.Location);
                    lougeFiles = lougeFiles.Union(GetLougeAssemblyFiles(directory));
                }
                if (lougeFiles.Any())
                {
                    foreach (var rhinoFile in lougeFiles)
                    {
                        var rhinoAssembly = Assembly.LoadFrom(rhinoFile);
                        if (!assemblies.Contains(rhinoAssembly))
                        {
                            assemblies = assemblies.Append(rhinoAssembly);
                        }
                    }
                }
                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.ExportedTypes)
                    {
                        if (type.Namespace == null)
                        {
                            continue;
                        }
                        if (!typeMap.ContainsKey(type.FullName))
                        {
                            typeMap.Add(type.FullName, type);
                        }

                    }

                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        /// <summary>
        /// 添加类型映射
        /// </summary>
        /// <param name="type">类型</param>
        public static void AddTypeMap(Type type)
        {
            if (type != null && (type.IsClass || type.IsValueType))
            {
                typeMap[type.FullName] = type;
            }
        }

        private static IEnumerable<string> GetLougeAssemblyFiles(string directory)
        {
            return Directory.GetFiles(directory).Where(f => f.EndsWith(".dll") &&
            (f.Contains("RhinoCommon")
            || f.Contains("Rhino3dm")
            || f.Contains("Contract")
            || f.Contains("Louge.Abstract")
            || f.Contains("Louge.Geometry")
            || f.Contains("Domain")));
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.GetName().Name;
            typeName = serializedType.FullName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Type BindToType(string assemblyName, string typeName)
        {
            try
            {
                switch (typeName)
                {
                    case "System.Byte[]":
                        return typeof(byte[]);
                }
                if (typeName.StartsWith("System.ValueTuple"))
                {
                    typeName = typeName.Replace("System.Private.CoreLib", "mscorlib");
                    return Type.GetType(typeName);
                }
                if (typeName.Contains("System.Private.CoreLib"))
                {
                    typeName = typeName.Replace("System.Private.CoreLib", "mscorlib");
                    return Type.GetType(typeName);
                }
                if (typeName.Contains("Louge.Sketch.Domain.Aggregates.AnnotationAggregate"))
                {
                    typeName = typeName.Replace("Louge.Sketch.Domain.Aggregates.AnnotationAggregate", "Louge.Geometry.Annotation");
                }
                if (typeMap.TryGetValue(typeName,out var type))
                {
                    return type;
                }
                else
                {
                    return Type.GetType(typeName);
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
