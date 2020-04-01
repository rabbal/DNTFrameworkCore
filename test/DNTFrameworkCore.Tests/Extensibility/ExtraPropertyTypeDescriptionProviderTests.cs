using System.ComponentModel;
using DNTFrameworkCore.Extensibility;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Extensibility
{
    [TestFixture]
    public class ExtraPropertyTypeDescriptionProviderTests
    {
        [Test]
        public void Should_TypeDescriptor_GetProperties_Returns_ExtraProperties_And_PredefinedProperties()
        {
            //Arrange
            var rabbal = new Person {Name = "GholamReza", Family = "Rabbal"};
            const string propertyName = "Title";
            const string propertyValue = "Software Engineer";

            //Act
            rabbal.ExtraProperty(propertyName, propertyValue);
            var title = TypeDescriptor.GetProperties(rabbal).Find(propertyName, true);

            //Assert
            rabbal.ExtraProperty<string>(propertyName).ShouldBe(propertyValue);
            title.ShouldNotBeNull();
            title.GetValue(rabbal).ShouldBe(propertyValue);
        }
        
        [TypeDescriptionProvider(typeof(ExtraPropertyTypeDescriptionProvider<Person>))]
        private class Person
        {
            public string Name { get; set; }
            public string Family { get; set; }
        }
    }
}