using PosStoneNfce.API.Portal.App.Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PosStoneNfce.API.Portal.Configuration
{
    public static class JsonWebTokenConfig
    {
        public static void AddJsonWebTokenConfiguration(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JsonWebToken");
            services.Configure<JsonWebToken>(jwtSection);
            var jwtSettings = jwtSection.Get<JsonWebToken>();

            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearerOptions =>
            {
                bearerOptions.RequireHttpsMetadata = true;
                bearerOptions.SaveToken = true;
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false                    
                };
            });
        }
    }
}