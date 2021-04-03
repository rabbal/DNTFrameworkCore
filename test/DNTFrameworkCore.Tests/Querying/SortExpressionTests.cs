using DNTFrameworkCore.Querying;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Querying
{
    [TestFixture]
    public class SortExpressionTests
    {
        [Test,
         TestCase("field.desc"),
         TestCase("field_desc"),
         TestCase("field:desc"),
         TestCase("field desc"),
         TestCase("-field"),
         TestCase("desc(field)")]
        public void Should_FromString_Return_SortExpression_Instance_With_Supported_Descending_Formats(string sorting)
        {
            var result = SortExpression.FromString(sorting);

            result.ShouldNotBeNull();
            result.Descending.ShouldBeTrue();
        }
    }
}