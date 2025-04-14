
using Infra.EF.Attributes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtension
    {
        private static bool HasJsonAttribute(PropertyInfo propertyInfo)
        {
            return propertyInfo != null && propertyInfo.CustomAttributes.Any(a => a.AttributeType == typeof(FrameJsonAttribute));
        }

        public static void AddJsonFields(this ModelBuilder modelBuilder, bool skipConventionalEntities = true)
        {

            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {

                var typeBase = typeof(TypeBase);
                if (skipConventionalEntities)
                {
                    var typeConfigurationSource = typeBase.GetField("_configurationSource", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(entityType)?.ToString();
                    if (Enum.TryParse(typeConfigurationSource, out ConfigurationSource typeSource) && typeSource == ConfigurationSource.Convention) continue;
                }

                var ignoredMembers = typeBase.GetField("_ignoredMembers", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(entityType) as Dictionary<string, ConfigurationSource>;
                bool NotIgnored(PropertyInfo property) =>
                  property != null && !ignoredMembers.ContainsKey(property.Name) && !property.CustomAttributes.Any(a => a.AttributeType == typeof(NotMappedAttribute));

                foreach (var clrProperty in entityType.ClrType.GetProperties().Where(x => NotIgnored(x) && HasJsonAttribute(x)))
                {
                    var property = modelBuilder.Entity(entityType.ClrType).Property(clrProperty.PropertyType, clrProperty.Name);
                    var modelType = clrProperty.PropertyType;

                    var converterType = typeof(JsonValueConverter<>).MakeGenericType(modelType);
                    var converter = (ValueConverter)Activator.CreateInstance(converterType, new object[] { null });
                    property.Metadata.SetValueConverter(converter);

                    var valueComparer = typeof(JsonValueComparer<>).MakeGenericType(modelType);
                    property.Metadata.SetValueComparer((ValueComparer)Activator.CreateInstance(valueComparer, new object[0]));

                }
            }
        }
    }
}
