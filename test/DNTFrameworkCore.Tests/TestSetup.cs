using NUnit.Framework;

namespace DNTFrameworkCore.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void Startup()
        {
        }

        [OneTimeTearDown]
        public void TearDown()
        {

        }
    }
}
