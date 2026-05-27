using System.Reflection;
using Microsoft.OpenApi.Models;

namespace PRN232.LMS.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddLmsSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PRN232 LMS API",
                Version = "v1",
                Description = "Learning Management System REST API (Lab 1)."
            });

            options.EnableAnnotations();

            IncludeXmlComments(options, Assembly.GetExecutingAssembly());

            var servicesAssembly = typeof(PRN232.LMS.Services.Models.Common.ApiResponse<>).Assembly;
            IncludeXmlComments(options, servicesAssembly);
        });

        return services;
    }

    private static void IncludeXmlComments(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options, Assembly assembly)
    {
        var xmlFile = $"{assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            return;
        }

        var locationXml = Path.ChangeExtension(assembly.Location, ".xml");
        if (File.Exists(locationXml))
        {
            options.IncludeXmlComments(locationXml, includeControllerXmlComments: true);
        }
    }
}
