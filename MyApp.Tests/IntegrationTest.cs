using ServiceStack;
using NUnit.Framework;
using MyApp.ServiceModel;

namespace MyApp.Tests
{
//    [TestFixture]
    public class IntegrationTest
    {
        const string BaseUri = "http://localhost:2000/";
        private readonly ServiceStackHost appHost;

        public IntegrationTest()
        {
            appHost = new AppHost()
                .Init()
                .Start(BaseUri);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();

        public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);

//        [Test]
        public void Can_call_Hello_Service()
        {
            var client = CreateClient();

            var response = client.Get(new GetBizs {TableName = "World"});

            Assert.That(response, Is.EqualTo("Hello, World!"));
        }
    }
}