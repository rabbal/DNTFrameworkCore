using DNTFrameworkCore.Extensibility;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Extensibility
{
    [TestFixture]
    public class ExtraPropertyTests
    {
        [Test]
        public void Should_ExtraField_ExtensionMethod_Add_New_Field()
        {
            var instance = new
            {
                Field = "Value"
            };

            instance.ExtraProperty("NewField", true);

            instance.ExtraProperty<bool>("NewField").ShouldBe(true);
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
            instance.ExtraProperty("NewField", property);

            dynamic value = instance.ExtraProperty("NewField");
            Assert.AreEqual(value.Field, true);
        }

        [Test]
        public void Should_ExtraField_ExtensionMethod_Add_New_Field_With_Accessors()
        {
            var instance = new Person
            {
                Name = "Rabbal"
            };

            instance.ExtraProperty("NewField", p => $"__{p.Name}__", typeof(string));

            instance.ExtraProperty<string>("NewField").ShouldBe($"__{instance.Name}__");
        }

        private class Person
        {
            public string Name { get; set; }
        }
    }
}