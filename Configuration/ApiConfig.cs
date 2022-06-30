using System.Text.Json.Serialization;
using PosStoneNfce.API.Portal.App.Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;



namespace PosStoneNfce.API.Portal.Configuration
{
    public static class ApiConfig
    {

        public static IConfiguration Configuration;

        public static IServiceCollection AddApiConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllers();

            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.AddMvc().AddJsonOptions(op =>
            {
                op.JsonSerializerOptions.IgnoreNullValues = true;
                op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddCors(options =>
            {
                options.AddPolicy("Development",
                    builder =>
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

                options.AddPolicy("Staging",
                    builder =>
                        builder
                            .WithOrigins("https://*.posstonenfce.linx.com.br")
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .AllowAnyMethod()
                            .AllowAnyHeader());

                options.AddPolicy("Production",
                    builder =>
                        builder                            
                            .WithOrigins("https://*.posstonenfce.linx.com.br")
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(env.EnvironmentName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}