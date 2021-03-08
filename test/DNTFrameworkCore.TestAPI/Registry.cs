using System;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using DNTFrameworkCore.Localization;
using DNTFrameworkCore.TestAPI.Authentication;
using DNTFrameworkCore.TestAPI.Infrastructure.Context;
using DNTFrameworkCore.Web.EFCore.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace DNTFrameworkCore.TestAPI
{
    public static class Registry
    {
        public static void AddWebApp(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddLocalization(setup => setup.ResourcesPath = "Resources");
            services.AddControllers(options =>
                {
                    //options.UseFilteredPagedRequestModelBinder();
                    //options.UseExceptionHandling();
                    
                    // remove formatter that turns nulls into 204 - No Content responses
                    // this formatter breaks SPA's Http response JSON parsing
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                    options.OutputFormatters.Insert(0, new HttpNoContentOutputFormatter
                    {
                        TreatNullValueAsNoContent = false
                    });
                }).AddDataAnnotationsLocalization(o =>
                {
                    o.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var localizationResource = type.GetTypeAttribute<LocalizationResourceAttribute>();
                        return localizationResource == null
                            ? factory.Create(type)
                            : factory.Create(localizationResource.Name, localizationResource.Location);
                    };
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory =
                        context => new BadRequestObjectResult(context.ModelState);
                });

            services.AddDataProtection().PersistKeysToDbContext<ProjectDbContext>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed(host => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            
            services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");
            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API Documentation",
                    Description = "DNTFrameworkCore API Documentation",
                    Contact = new OpenApiContact
                    {
                        Email = "gholamrezarabbal@gmail.com",
                        Name = "GholamReza Rabbal",
                        Url = new Uri(
                            "https://www.dotnettips.info/user/%D8%BA%D9%84%D8%A7%D9%85%D8%B1%D8%B6%D8%A7%20%D8%B1%D8%A8%D8%A7%D9%84")
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
            });
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authentication = configuration.GetSection("Authentication");
            services.Configure<TokenOptions>(options => authentication.Bind(options));

            var signingKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authentication[nameof(TokenOptions.SigningKey)]));

            var tokenValidationParameters = new TokenValidationParameters
            {
                // Ensure the token was issued by a trusted authorization server (default true):
                ValidIssuer = authentication[nameof(TokenOptions.Issuer)], // site that make the token
                ValidateIssuer = true,

                // Ensure the token audience matches our audience value (default true):
                ValidAudience =
                    authentication[nameof(TokenOptions.Audience)], // site that consumes the token
                ValidateAudience = true,

                RequireSignedTokens = true,
                IssuerSigningKey = signingKey,
                ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                // Ensure the token hasn't expired:
                ValidateLifetime = true,
                RequireExpirationTime = true,

                // tolerance for the expiration date (compensates for server time drift).
                // We recommend 5 minutes or less:
                ClockSkew = TimeSpan.Zero
            };

            services
                .AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.ClaimsIssuer = authentication[nameof(TokenOptions.Issuer)];
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                                .CreateLogger(nameof(JwtBearerEvents));
                            logger.LogError("Authentication Failed", context.Exception);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async context =>
                        {
                            var validator = context.HttpContext.RequestServices.GetRequiredService<ITokenValidator>();
                            await validator.ValidateAsync(context);
                            //context.Principal.AddIdentity(new ClaimsIdentity());
                        },
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationhub"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                                .CreateLogger(nameof(JwtBearerEvents));
                            logger.LogError("OnChallenge Error", context.Error, context.ErrorDescription);
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}