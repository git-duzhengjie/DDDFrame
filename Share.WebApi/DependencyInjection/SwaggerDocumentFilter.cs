using Infra.Core.Abstract;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Infra.Core.Extensions;
namespace Infra.WebApi.DependInjection
{
    internal class SwaggerDocumentFilter : IDocumentFilter
    {
        private IServiceInfo serviceInfo;

        public SwaggerDocumentFilter(IServiceInfo serviceInfo)
        {
            this.serviceInfo = serviceInfo;
        }
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var contractAssembly= serviceInfo.GetApplicationContractAssembly();
            if (contractAssembly != null)
            {
                var exprotedTypes = contractAssembly.GetExportedTypes();
                var types = exprotedTypes
                    .Where(x=>x.IsAssignableTo(typeof(IInputDTO))
                    || x.IsAssignableTo(typeof(IOutputDTO))
                    ||x.IsAssignableTo(typeof(IQueryDTO)))
                    .ToArray();
                foreach (var item in types)
                {
                    context.SchemaGenerator.GenerateSchema(item, context.SchemaRepository);
                }
            }
        }
    }
}