using PosStoneNfce.API.Autenticacao.App.Common.Services;
using PosStoneNfce.API.Portal.App.Common.Services;
using PosStoneNfce.API.Portal.App.Modules.Autenticacao.Services;
using PosStoneNfce.Common.Modules.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data.SqlClient;

namespace PosStoneNfce.API.Portal.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            var configEmailSection = configuration.GetSection("EmailService");
            services.Configure<EmailServiceConfig>(configEmailSection);
            var emailSettings = configEmailSection.Get<EmailServiceConfig>();

            // HTTP Services
            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>();        
            
            // Email Service With MIDe ServiceBus Transporter
            services.AddSingleton<IEmailService>(factory => {
                var emailService = new EmailService(
                        factory.GetRequiredService<IHostEnvironment>()
                    );

                emailService.SetTransporter(
                    new MIDeServiceBusEmailTransporter(emailSettings.Connection, emailSettings.QueueName));

                return emailService;
            });

            services.AddScoped(factory =>
            {
                var queryFactory = new QueryFactory
                {
                    Compiler = new SqlServerCompiler(),
                    Connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")),
                    Logger = compiled => Console.WriteLine(compiled)
                };

                queryFactory.Connection.Open();

                return queryFactory;
            });      

            services.AddTransient<ServiceResolver>(serviceProvider => key =>
            {
                QueryFactory queryFactory; 
                switch (key)
                {
                    case "MIDe":
                        queryFactory = new QueryFactory
                        {
                            Compiler = new SqlServerCompiler(),
                            Connection = new SqlConnection(configuration.GetConnectionString("MIDe")),
                            Logger = compiled => Console.WriteLine(compiled)
                        };
                        break;

                    case "PosStoneNfce":
                    default:
                        queryFactory = new QueryFactory
                        {
                            Compiler = new SqlServerCompiler(),
                            Connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")),
                            Logger = compiled => Console.WriteLine(compiled)
                        };
                        break;
                }

                queryFactory.Connection.Open();
                return queryFactory;
            });
        }
    }
}