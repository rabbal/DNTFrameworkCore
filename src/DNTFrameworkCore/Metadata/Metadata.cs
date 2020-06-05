using System;
using System.Threading;

namespace DNTFrameworkCore.Metadata
{
    public sealed class Metadata
    {
        private static readonly Lazy<Metadata> _instance =
            new Lazy<Metadata>(() => new Metadata(), LazyThreadSafetyMode.ExecutionAndPublication);

        private Metadata()
        {
        }
    }
}