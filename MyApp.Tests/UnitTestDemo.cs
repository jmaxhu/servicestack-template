using NUnit.Framework;
using MyApp.ServiceInterface;
using MyApp.ServiceModel;

namespace MyApp.Tests
{
    [TestFixture]
    public class UnitTestDemo : UnitTestBase
    {
//        [Test]
        public void Can_call_MyServices()
        {
            var service = AppHost.Resolve<BizService>();

            var response = service.Post(new SaveBiz {TableName = "World"});

            Assert.That(response.Result, Is.EqualTo("Hello, World!"));
        }
    }
}