using Agy.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;// nuget - IdentityServer4.AccessTokenValidation cant find Duende equivilent on nuget yet.
using Microsoft.IdentityModel.Tokens;

namespace SyncHub
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5001";
              
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Is token in Querystring?
                        var accessToken = context.Request.Query["access_token"];
                        if(!string.IsNullOrWhiteSpace(accessToken))
                        {
                            Debug.WriteLine("Sync Hub.......");
                            Debug.WriteLine($"Event... OnMessageReceived... Token found in Querystring : {ShorterJWT(accessToken)}");

                            var path = context.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/sighub"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;

                                Debug.WriteLine($"Path: {path} - Token: {accessToken}");
                                Debug.WriteLine("");
                            }
                        }

                        // Is token in Authorization Header?
                        var headers = context.Request.Headers;
                        var token = context.Request.Headers.FirstOrDefault(o => o.Key == "Authorization").Value.ToString();
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            //token = token.Replace("Bearer ", "");
                            accessToken = token;
                            Debug.WriteLine($"Sync Hub.......");
                            Debug.WriteLine($"Event... OnMessageReceived... Token fouund in Headers[Authorization] : {ShorterJWT(accessToken)}");
                            Debug.WriteLine("");

                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

             
            // Check the token for scope
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SigScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "sig1");
                });

                #region old policies
                //options.AddPolicy("MyAuthPolicy", policy => policy.RequireAssertion(httpCtx =>
                //{
                //    policy.RequireAuthenticatedUser();
                //    policy.RequireClaim("scope", "sig1");
                //});

                //options.AddPolicy("SigScope", policy => policy.RequireAssertion(httpCtx =>
                //{
                //    return true; // This obviously worked
                //}));
                #endregion
            });
           

            services.AddCors(options =>
            {
                // Duende Identity Server https://localhost:5001/
                // Sync Hub server http://localhost:5050/
                options.AddPolicy("AllowClients",
                                  p => p.WithOrigins("https://localhost:5001/", "http://localhost:5050/")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials());
            });

            services.AddControllersWithViews();

            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            services.AddSignalR();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowClients");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.Use(next =>
            {
                return context =>
                {
                    return next(context);// there are no claims! 
                };
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<SignalrHub>("sighub");// .RequireAuthorization("SigScope"); // will this will always cause a 401??
            });

        }

        // helpers
        //

        // Notes: 
        // HubConnectionContext doesnt have any claims!
        //
        //
        public class NameUserIdProvider : IUserIdProvider
        {
            public virtual string GetUserId(HubConnectionContext connection)
            {
                var name = connection.User?.Identity?.Name;// always null, try email
                //var email = connection.User?.FindFirst(ClaimTypes.Email)?.Value;// null too!!! Why???

                Debug.WriteLine($"Sync Hub.......");
                Debug.WriteLine($"Name UserId Provider: {name}");
                Debug.WriteLine("");

                return name;
            }
        }



        private string ShorterJWT(string token)
        {
            // Pass back the first & last 10 digits of the Token
            if (!string.IsNullOrWhiteSpace(token))
                return $"{token.Substring(0, 10)}...{token.Substring(token.Length - 10, 10)}";

            return "no token!";
        }


    }
}
