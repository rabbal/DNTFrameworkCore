using DNTFrameworkCore.Extensibility;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Extensibility
{
    [TestFixture]
    public class ExtraFieldTests
    {
        [Test]
        public void Should_ExtraField_ExtensionMethod_Add_New_Field()
        {
            var instance = new
            {
                Field = "Value"
            };

            instance.ExtraField("NewField", true);

            instance.ExtraField<bool>("NewField").ShouldBe(true);
        }

        [Test]
        public void Should_ExtraField_ExtensionMethod_Add_New_Complex_Field()
        {
            var instance = new
            {
                Field = "Value"
            };

            var property = new
            {
                Field = true
            };
            instance.ExtraField("NewField", property);

            dynamic value = instance.ExtraField("NewField");
            Assert.AreEqual(value.Field, true);
        }
    }
}