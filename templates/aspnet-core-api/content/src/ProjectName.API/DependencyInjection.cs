using System;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Castle.Core.Internal;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Localization;
using DNTFrameworkCore.Web.EFCore.Cryptography;
using DNTFrameworkCore.Web.ExceptionHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectName.API.Authentication;
using ProjectName.API.Localization;
using ProjectName.Infrastructure.Context;
using ProjectName.Resources.Resources;
using ITranslationService = ProjectName.Application.Localization.ITranslationService;

namespace ProjectName.API
{
    public static class DependencyInjection
    {
        public static void AddWebApp(this IServiceCollection services)
        {
            AddInternalServices(services);

            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
            services.AddScoped<IStringLocalizer>(provider => provider.GetRequiredService<IStringLocalizer<SharedResource>>());
            services.AddSingleton<ITranslationService, TranslationService>();
            
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
                    //options.UseFilteredPagedRequestModelBinder();
                    //options.UseExceptionHandling();

                    // remove formatter that turns nulls into 204 - No Content responses
                    // this formatter breaks SPA's Http response JSON parsing
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                    options.OutputFormatters.Insert(0, new HttpNoContentOutputFormatter
                    {
                        TreatNullValueAsNoContent = false
                    });
                })
                .AddDataAnnotationsLocalization(o =>
                {
                    var location = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName).Name;

                    o.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var localizationResource = type.GetTypeAttribute<LocalizationResourceAttribute>();
                        return localizationResource == null
                            ? factory.Create(nameof(SharedResource), location)
                            : factory.Create(localizationResource.Name, localizationResource.Location);
                    };
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = false;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options => { options.UseFailureProblemDetailResponseFactory(); });
            // .AddNewtonsoftJson(options =>
            // {
            //     options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            //     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //     options.SerializerSettings.Converters.Add(new StringEnumConverter());
            // });

            services.AddDataProtection().PersistKeysToDbContext<ProjectNameDbContext>();

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
                    Description = "ProjectName API Documentation",
                    Contact = new OpenApiContact
                    {
                        Email = "gholamrezarabbal@gmail.com",
                        Name = "Salar Rabbal",
                        Url = new Uri("http://github.com/rabbal")
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
                                path.StartsWithSegments("/hubs"))
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

        private static void AddInternalServices(IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromCallingAssembly()
                .AddClasses(classes => classes.AssignableTo<ISingletonDependency>())
                .AsMatchingInterface()
                .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<IScopedDependency>())
                .AsMatchingInterface()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo<ITransientDependency>())
                .AsMatchingInterface()
                .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IBusinessEventHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }
    }
}