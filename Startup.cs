using PosStoneNfce.API.Portal.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using PosStoneNfce.API.Portal.Infrastructure;
using PosStoneNfce.API.Portal.Services;

namespace PosStoneNfce.API.Portal
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostEnvironment hostEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            ApiConfig.Configuration = Configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddApiConfiguration(Configuration);
            services.AddJsonWebTokenConfiguration(Configuration);
            services.AddSwaggerConfiguration();
            
            services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("PosStoneLogger"));
            services.AddSingleton<IDBRepository<PosStone>, DBRepository<PosStone>>();

            services.AddSingleton<IServiceEmpresa<ServiceEmpresa, EmpresaEntity>, ServiceEmpresa<ServiceEmpresa>>();
            services.AddSingleton<IServiceGeneral<ServiceGeneral, GeneralEntity>, ServiceGeneral<ServiceGeneral>>();
            services.AddSingleton<IServiceNfe<ServiceNfe, NfeEntity>, ServiceNfe<ServiceNfe>>();
            services.AddSingleton<IServiceNfeItens<ServiceNfeItens, NfeItensEntity>, ServiceNfeItens<ServiceNfeItens>>();
            services.AddSingleton<IServiceToken<ServiceToken, TokenEntity>, ServiceToken<ServiceToken>>();

            services.RegisterServices(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {            
            app.UseSwaggerConfiguration();
            app.UseApiConfiguration(env);
        }
    }
}