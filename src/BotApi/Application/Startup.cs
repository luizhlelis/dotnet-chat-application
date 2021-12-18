using System;
using BotApi.Application.Filters;
using BotApi.Application.Settings;
using BotApi.Application.Swagger;
using BotApi.Domain.Notifications;
using BotApi.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BotApi.Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // API versioning
            services.AddApiVersioning(options => {
                options.ReportApiVersions = true;

                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Swagger
            var openApiInfo = new OpenApiInfo();
            Configuration.Bind("OpenApiInfo", openApiInfo);
            services.AddSingleton(openApiInfo);
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();
            });

            // Authentication
            var tokenCredentials = new TokenCredentials();
            Configuration.Bind("TokenCredentials", tokenCredentials);
            services.AddSingleton(tokenCredentials);

            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    sharedOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = tokenCredentials.Issuer,
                        ValidAudience = tokenCredentials.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(tokenCredentials.HmacSecretKey))
                    }
                );

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<NotificationContext>();

            // API Validators
            services
                .AddMvc(options => {
                    options.Filters.Add(new ModelStateFilter());
                    options.Filters.Add<NotificationFilter>();
                })
                .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Startup>());

            // RabbitMq
            var raabitConnection = new ConnectionFactory
            {
                HostName = Configuration["RabbitMq:HostName"],
                Port = Configuration.GetValue<int>("RabbitMq:Port"),
                UserName = Configuration["RabbitMq:UserName"],
                Password = Configuration["RabbitMq:Password"],
                VirtualHost = Configuration["RabbitMq:VirtualHost"]
            }.CreateConnection();

            services.AddSingleton(raabitConnection);
            services.AddScoped<IMessageBroker, MessageBroker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                        c.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName);
                }
            );

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
