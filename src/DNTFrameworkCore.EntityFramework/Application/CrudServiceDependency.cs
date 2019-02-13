using System;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.Eventing;

namespace DNTFrameworkCore.EntityFramework.Application
{
    public class CrudServiceDependency : IScopedDependency
    {
        public IEventBus EventBus { get; }
        public IUnitOfWork UnitOfWork { get; }

        public CrudServiceDependency(IUnitOfWork uow, IEventBus bus)
        {
            UnitOfWork = uow ?? throw new ArgumentNullException(nameof(uow));
            EventBus = bus ?? throw new ArgumentNullException(nameof(bus));
        }
    }
}