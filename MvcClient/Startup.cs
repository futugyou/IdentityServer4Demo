using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MvcClient
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            foreach (var item in JwtSecurityTokenHandler.DefaultInboundClaimTypeMap)
            {
                Console.WriteLine(item.Key + "========" + item.Value);
            }
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            //authenticationScheme "Cookies" must be same with 'options.DefaultScheme = "Cookies"'
            .AddCookie("Cookies", options =>
            {
                options.AccessDeniedPath = "/authorization/accessDenied";
            })
            //authenticationScheme "oidc" must be same with 'options.DefaultChallengeScheme = "oidc"'
            .AddOpenIdConnect("oidc", options =>
            {
                //must be same with 'options.DefaultScheme = "Cookies"'
                options.SignInScheme = "Cookies";
                //identity server ip address
                options.Authority = "http://localhost:5000";
                //must be same with 'ClientId' in identity server
                options.ClientId = "mvc";
                //must be same with 'ClientSecret' in identity server
                options.ClientSecret = "secret";
                //false in dev evn.
                options.RequireHttpsMetadata = false;
                options.ResponseType = "code id_token";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");//must be scope in identity server
                options.Scope.Add("roles");
                options.ClaimActions.MapUniqueJsonKey("role", "role");//把role claim 映射到User.Claims里
                // role claim映射成ASP.NET Core MVC可以识别的角色Roles。
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role",
                };

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
            })
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            //app.UseMvc();
            app.UseMvcWithDefaultRoute();
        }
    }
}
