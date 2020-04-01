using DNTFrameworkCore.Extensibility;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Extensibility
{
    [TestFixture]
    public class ExtensionPropertyTests
    {
        [Test]
        public void Should_ExtensionField_ExtensionMethod_Add_New_Field()
        {
            var instance = new
            {
                Field = "Value"
            };

            instance.ExtensionProperty("NewField", true);

            instance.ExtensionProperty<bool>("NewField").ShouldBe(true);
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
            instance.ExtensionProperty("NewField", property);

            dynamic value = instance.ExtensionProperty("NewField");
            Assert.AreEqual(value.Field, true);
        }

        [Test]
        public void Should_ExtensionField_ExtensionMethod_Add_New_Field_With_Accessors()
        {
            var instance = new Person
            {
                Name = "Rabbal"
            };

            instance.ExtensionProperty("NewField", p => $"__{p.Name}__", typeof(string));

            instance.ExtensionProperty<string>("NewField").ShouldBe($"__{instance.Name}__");
        }

        private class Person
        {
            public string Name { get; set; }
        }
    }
}