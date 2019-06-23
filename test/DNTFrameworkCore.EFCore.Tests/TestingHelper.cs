using System;
using System.Threading.Tasks;

namespace DNTFrameworkCore.EntityFramework.Tests
{
    public static class TestingHelper
    {
        public static void ExecuteInParallel(Action test, int count = 10)
        {
            var tests = new Action[count];
            for (var i = 0; i < count; i++)
            {
                tests[i] = test;
            }

            Parallel.Invoke(tests);
        }
    }
}