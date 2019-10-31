using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TopherAPI.Services
{
    public static class SwaggerConfiguration
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Info { Title = "Topher API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
                config.DescribeAllParametersInCamelCase();
                config.DescribeAllEnumsAsStrings();

                config.TagActionsBy(apiDescription => new string[] { "Actions" });
                config.OrderActionsBy(apiDescription => apiDescription.RelativePath);

                //config.AddSecurityDefinition("Bearer", new ApiKeyScheme
                //{
                //    Type = "apiKey",
                //    Description = "JWT Authorization header using the Bearer scheme",
                //    Name = "Authorization",
                //    In = "header"
                //});

                //config.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                //{
                //    {"Bearer", new string[] { }},
                //});
            });
        }

        public static void ConfigureSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("../swagger/v1/swagger.json", "Topher API v1");

                config.DefaultModelsExpandDepth(-1); // hide models section
            });
        }
    }
}
