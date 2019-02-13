using System;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using DNTFrameworkCore.Localization;
using DNTFrameworkCore.TestAPI.Application.Tasks.Models;
using DNTFrameworkCore.TestAPI.Authentication;
using DNTFrameworkCore.Web.Filters;
using DNTFrameworkCore.Web.ModelBinders;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DNTFrameworkCore.TestAPI
{
    public static class Registry
    {
        public static void AddWeb(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddLocalization(setup => setup.ResourcesPath = "Resources");
            services.AddHttpContextAccessor();
            services.AddMvcCore(options =>
                {
                    // options.UseYeKeModelBinder();
                    options.UseDefaultFilteredPagedQueryModelBinder();
                    options.UseFilteredPagedQueryModelBinder<TaskFilteredPagedQueryModel>();
                    options.Filters.Add<GlobalExceptionFilter>();
                })
                .AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
                })
                .AddAuthorization()
                .AddFormatterMappings()
                .AddJsonFormatters()
                .AddDataAnnotations()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    //options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //var resolver = options.SerializerSettings.ContractResolver;
                    //if (resolver == null) return;
                    //var res = resolver as DefaultContractResolver;
                    //res.NamingStrategy = null; // Enable PascalCasing
                })
                .AddDataAnnotationsLocalization(o =>
                {
                    o.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var localizationResource = type.GetTypeAttribute<LocalizationResourceAttribute>();
                        return localizationResource == null
                            ? factory.Create(type)
                            : factory.Create(localizationResource.Name, localizationResource.Location);
                    };
                })
                .AddFluentValidation(configuration =>
                    configuration.RegisterValidatorsFromAssemblyContaining<Program>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressUseValidationProblemDetailsForInvalidModelStateResponses = true;
                    options.SuppressMapClientErrors = true;
                    //options.SuppressModelStateInvalidFilter = true;
                });

            services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");
            services.AddSignalR();
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authentication = configuration.GetSection("Authentication");
            services.Configure<TokenOptions>(options => authentication.Bind(options));

            var signingKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authentication[nameof(TokenOptions.SigningKey)]));
            var encryptingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authentication[nameof(TokenOptions.EncryptingKey)])); //must be 16 character

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
                TokenDecryptionKey = encryptingKey,
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
                        OnTokenValidated = context =>
                        {
                            var tokenValidatorService = context.HttpContext.RequestServices
                                .GetRequiredService<ITokenValidator>();
                            return tokenValidatorService.ValidateAsync(context);
                        },
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/notificationhub"))
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