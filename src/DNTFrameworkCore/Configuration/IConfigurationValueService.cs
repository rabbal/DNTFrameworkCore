using System.Threading.Tasks;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Configuration
{
    public interface IConfigurationValueService : IApplicationService
    {
        Task SaveValueAsync(string key, string value);
        Task<Maybe<string>> FindValueAsync(string key);
    }
}