using FuraErp.ServiceModel.Molds;
using NUnit.Framework;

namespace Fura.Tests.Microservices.fura_erp
{
    class TransportationTests
    {
        public readonly ServicesWrapper SW;

        public TransportationTests()
        {
            SW = new ServicesWrapper();
        }

        [SetUp]
        public void SetUp()
        {
            SW.SetUp();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            SW.TestFixtureTearDown(); // очистка после выполнения тестов
        }


        [Test]
        public void postinitialrequest() // тест Post и get character
        {
            var email = SW.GenerateEmail();
            var webToken =
                SW.GetNewSupervisorsCheatWebToken(email,
                    ServicesWrapper.webroles.superadmin); // получили токен супеадмина
            SW.AuthServiceClient.BearerToken = webToken.token.AccessToken;
            var post_initialrequest_request = new FuraErp.ServiceModel.CreateCarrier
            {
                Title = "Тестовый перевозчик",
                PreferredRoutes = new ProtoRouteMold[]
                {
                    new ProtoRouteMold()
                    {
                        ArriveLocality = "moscow",
                        DepartLocality = "saratov"
                    }
                }
            };
            var post_initialrequest_result = SW.ErpServiceClient.Post(post_initialrequest_request);
            Assert.IsNotEmpty(post_initialrequest_result.Id);
        }
    }
}
