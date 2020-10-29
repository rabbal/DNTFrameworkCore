using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.Dependency
{
    public static class DuplicateServiceFinder
    {
        private static List<(Type ServiceType, int RegistrationCount)> _duplicateServices;

        public static IHostBuilder CountDuplicateServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                _duplicateServices = services.Where(
                        serviceDescriptor => !serviceDescriptor.ServiceType.Assembly.FullName.Contains("Microsoft"))
                    .GroupBy(serviceDescriptor => serviceDescriptor.ServiceType)
                    .Where(g => g.Count() > 1)
                    .Select(g => (g.Key, g.Count()))
                    .ToList();
            });
            return hostBuilder;
        }

        public static IHost ReportDuplicateServices(this IHost host)
        {
            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DependencyInjection.Duplicate");
            _duplicateServices.ForEach(item =>
                logger.LogWarning(
                    $"Service Type: `{item.ServiceType}` -> Registration times: {item.RegistrationCount}"));
            return host;
        }
    }
}