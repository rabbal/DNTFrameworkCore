using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Configuration
{
    public interface IKeyValueService : IApplicationService
    {
        Task SetValueAsync(string key, string value);
        Task<Maybe<string>> LoadValueAsync(string key);
        Task<bool> IsTamperedAsync(string key);
    }
}