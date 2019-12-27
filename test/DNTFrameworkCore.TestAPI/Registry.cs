using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Castle.Core.Internal;
using DNTFrameworkCore.Localization;
using DNTFrameworkCore.TestAPI.Application.Tasks.Models;
using DNTFrameworkCore.TestAPI.Authentication;
using DNTFrameworkCore.Web.Cryptography;
using DNTFrameworkCore.Web.Filters;
using DNTFrameworkCore.Web.ModelBinders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


namespace DNTFrameworkCore.TestAPI
{
    public static class Registry
    {
        public static void AddWebAPI(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddLocalization(setup => setup.ResourcesPath = "Resources");
            services.AddHttpContextAccessor();
            services.AddControllers(options =>
                {
                    options.UseDefaultFilteredPagedQueryModelBinder();
                    options.UseFilteredPagedQueryModelBinder<TaskFilteredPagedQueryModel>();
                    options.Filters.Add<HandleExceptionFilter>();
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
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressMapClientErrors = true;
                });

            services.AddDataProtection().PersistKeysToStore();
            
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
                        Url =new Uri("https://www.dotnettips.info/user/%D8%BA%D9%84%D8%A7%D9%85%D8%B1%D8%B6%D8%A7%20%D8%B1%D8%A8%D8%A7%D9%84")
                            
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
                            logger.LogError("Authentication failed.", context.Exception);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async context =>
                        {
                            var validator = context.HttpContext.RequestServices.GetRequiredService<ITokenValidator>();
                            await validator.ValidateAsync(context);

                            context.Principal.AddIdentity(new ClaimsIdentity());
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
                            logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}