using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SignalR_Demo.SignalHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Demo
{
    public class Startup
    {
        IConfiguration Configuration { get; }
        readonly byte[] JwtKey = Encoding.ASCII.GetBytes(
            "put token secret key here"
        );
        const string MessengerHubPath = "/messenger-hub";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, MessengerUserIdProvider>();
            services.AddCors(options => options.AddPolicy(
                "AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            ));
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo {
                Title = "Demo",
                Version = "v1",
            }));

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(JwtKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
#if DEBUG
                    ValidateLifetime = false,
#endif
                };
                options.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        // In some cases of Signalr, token has to be taken from query string
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(MessengerHubPath))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo"));

            app.UseRouting();
            app.UseCors("AllowAll");

            // these are necessary
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<MessengerHub>(MessengerHubPath);
            });
        }
    }
}
