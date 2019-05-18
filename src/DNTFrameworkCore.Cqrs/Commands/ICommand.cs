using DNTFrameworkCore.Functional;
using MediatR;

namespace DNTFrameworkCore.Cqrs.Commands
{
    public interface ICommand : IRequest<Result>
    {
    }
}
