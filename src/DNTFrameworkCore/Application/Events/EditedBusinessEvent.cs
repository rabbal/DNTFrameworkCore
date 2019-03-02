using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Eventing;

namespace DNTFrameworkCore.Application.Events
{
    public class EditedBusinessEvent<TModel,TKey> : IBusinessEvent
        where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
    {
        public EditedBusinessEvent(IEnumerable<ModifiedModel<TModel>> models)
        {
            Models = models.ToImmutableList();
        }

        public IReadOnlyList<ModifiedModel<TModel>> Models { get; }
    }
}