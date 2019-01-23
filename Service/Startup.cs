using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using IdentityServer4;
using Microsoft.IdentityModel.Tokens;

namespace Service
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
            var connectionString = Configuration["ConnectionString"];
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1"
                });
                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Service.xml");
                c.IncludeXmlComments(xmlPath);
                //  c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
            });

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddTestUsers(Config.GetUsers())
                .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>()
                .AddProfileService<CustomProfileService>()
                .AddConfigurationStore(options =>
                {
                    // this adds the config data from DB (clients, resources)
                    //dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb  --project Service
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(connectionString, sql =>
                        {
                            sql.MigrationsAssembly(migrationsAssembly);
                            sql.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                    };
                })
                .AddOperationalStore(options =>
                {
                    // this adds the operational data from DB (codes, tokens, consents)
                    //dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb  --project Service
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(connectionString, sql =>
                        {
                            sql.MigrationsAssembly(migrationsAssembly);
                            sql.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                    };
                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddOidcStateDataFormatterCache();

            services.AddAuthentication(options =>
            { 
                options.DefaultChallengeScheme = "Bearer"; 
            })
            .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "api1"; 
                });
            //        services.AddAuthentication("MyCookie")
            //.AddCookie("MyCookie", options =>
            //{

            //});
            
            services.AddAuthentication()
                .AddGoogle("Google", "Google!!!", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";
                    options.ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo";
                })
                .AddOpenIdConnect("oidc", "OpenID Connect", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "implicit";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            InitializeDatabase.InitAsync(app);
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TwBusManagement API V1");
            });

            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            //app.UseHttpsRedirection();
            //app.UseAuthentication();

            app.UseStaticFiles();
            app.UseIdentityServer();
            //app.UseMvc();
            app.UseMvcWithDefaultRoute();
        }
    }
}
