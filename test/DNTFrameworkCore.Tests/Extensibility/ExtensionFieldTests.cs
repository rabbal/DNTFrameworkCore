using DNTFrameworkCore.Extensibility;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Extensibility
{
    [TestFixture]
    public class ExtensionFieldTests
    {
        [Test]
        public void Should_ExtensionField_ExtensionMethod_Add_New_Field()
        {
            var instance = new
            {
                Field = "Value"
            };

            instance.ExtensionField("NewField", true);

            instance.ExtensionField<bool>("NewField").ShouldBe(true);
        }

        [Test]
        public void Should_ExtensionField_ExtensionMethod_Add_New_Complex_Field()
        {
            var instance = new
            {
                Field = "Value"
            };

            var property = new
            {
                Field = true
            };
            instance.ExtensionField("NewField", property);

            dynamic value = instance.ExtensionField("NewField");
            Assert.AreEqual(value.Field, true);
        }
    }
}