using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Store.Api.Swagger;

/// <summary>Registers one Swagger document per supported API version (v1, v2).</summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Title = "Store API",
                    Version = description.GroupName,
                    Description = description.GroupName == "v2"
                        ? "v2 adds CategoryName to the product list and details contracts."
                        : "v1 of the Store API (products and categories).",
                });
        }
    }
}
