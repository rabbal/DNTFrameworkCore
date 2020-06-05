using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales.Rules
{
    public interface ISaleMethodRules : IRules
    {
        Task<bool> IsTitleUnique(Title title, int id);
    }
}