using System.ComponentModel;
using DNTFrameworkCore.Extensibility;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Extensibility
{
    [TestFixture]
    public class ExtensionPropertyTypeDescriptionProviderTests
    {
        [Test]
        public void Should_TypeDescriptor_GetProperties_Returns_ExtensionProperties_And_PredefinedProperties()
        {
            //Arrange
            var rabbal = new Person {Name = "Salar", Family = "Rabbal"};
            const string propertyName = "Title";
            const string propertyValue = "Software Engineer";

            //Act
            rabbal.ExtensionProperty(propertyName, propertyValue);
            var title = TypeDescriptor.GetProperties(rabbal).Find(propertyName, true);

            //Assert
            rabbal.ExtensionProperty<string>(propertyName).ShouldBe(propertyValue);
            title.ShouldNotBeNull();
            title.GetValue(rabbal).ShouldBe(propertyValue);
        }
        
        [TypeDescriptionProvider(typeof(ExtensionPropertyTypeDescriptionProvider<Person>))]
        private class Person
        {
            public string Name { get; set; }
            public string Family { get; set; }
        }
    }
}