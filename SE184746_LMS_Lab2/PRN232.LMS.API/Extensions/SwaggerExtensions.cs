using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace PRN232.LMS.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddLmsSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token here. Example: Bearer {your_token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            IncludeXmlComments(options, Assembly.GetExecutingAssembly());

            var servicesAssembly = typeof(PRN232.LMS.Services.Interfaces.IStudentService).Assembly;
            IncludeXmlComments(options, servicesAssembly);
        });

        return services;
    }

    public static IApplicationBuilder UseLmsSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"PRN232 LMS API {description.GroupName.ToUpperInvariant()}");
            }
        });

        return app;
    }

    public static void ConfigureSwaggerDocs(this Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options, IApiVersionDescriptionProvider provider)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "PRN232 LMS API",
                Version = description.GroupName,
                Description = $"Learning Management System REST API (Lab 2) - {description.GroupName.ToUpperInvariant()}"
                              + (description.IsDeprecated ? " [DEPRECATED]" : string.Empty)
            });
        }
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
