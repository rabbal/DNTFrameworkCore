using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Eventing;

namespace DNTFrameworkCore.Application
{  
    public class DeletedBusinessEvent<TModel, TKey> : IBusinessEvent
        where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
    {
        public DeletedBusinessEvent(IEnumerable<TModel> models)
        {
            Models = models.ToList();
        }

        public IReadOnlyList<TModel> Models { get; }
    }
}