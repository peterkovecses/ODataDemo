using Microsoft.AspNetCore.OData.Query;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ODataDemo.Swagger;

public class OperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        context.ApiDescription.ParameterDescriptions
            .Where(apiParameterDescription => apiParameterDescription.ParameterDescriptor?.ParameterType.BaseType == typeof(ODataQueryOptions))
            .ToList()
            .ForEach(apiParameterDescription =>
            {
                var removable = operation.Parameters.SingleOrDefault(openApiParameter => openApiParameter.Name == apiParameterDescription.Name);
                if (removable is not null)
                {
                    _ = operation.Parameters.Remove(removable);
                }
            });
    }
}
