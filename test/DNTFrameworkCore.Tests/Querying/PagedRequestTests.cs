using DNTFrameworkCore.Querying;
using NUnit.Framework;

namespace DNTFrameworkCore.Tests.Querying
{
    [TestFixture]
    public class PagedRequestTests
    {
        [Test]
        public void Should_Use_Default_Sorting_When_SortExpressions_Is_Empty()
        {
            var request = new PagedRequest();
        }
    }
}