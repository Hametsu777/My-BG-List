using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MyBGList.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyBGList.Swagger
{
    public class SortOrderFilter : IParameterFilter
    {
        // Need to create this filter class implementing the IParameterFilter interface so we can add our custom
        // validation attribute's info to the swagger JSon File (Custom Validators aren't supported by swagger).
        // Swashbuckle will call and execute this filter before creating the JSON block for all the parameters used
        // by our controllers action methods (and minimal API methods). IParameterFilter's Apply method is implemented so it detects
        // all parameters decorated with our custom validation attributes and adds relevant info to the swagger.json file for
        // each of them.
        // The var attributes block checks whether the parameter has the attribute.
        // See 393 for Union code.
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var attributes = context.ParameterInfo?
                .GetCustomAttributes(true)
                .Union(
                    context.ParameterInfo.ParameterType.GetProperties()
                    .Where(p => p.Name == parameter.Name)
                    .SelectMany(p => p.GetCustomAttributes(true))
                    )
                .OfType<SortOrderValidatorAttribute>();

            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    parameter.Schema.Extensions.Add(
                        "pattern", new OpenApiString(string.Join("|", attribute.AllowedValues.Select(v => $"^{v}$"))));
                }
            }
        }
    }
}
