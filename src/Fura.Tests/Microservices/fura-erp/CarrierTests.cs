using FuraErp.ServiceModel.Molds;
using NUnit.Framework;

namespace Fura.Tests.Microservices.fura_erp
{
    class CarrierTests
    {
        public readonly ServicesWrapper SW;

        public CarrierTests()
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
        public void postcarrier() // тест Post и get carrier
        {
            var email = SW.GenerateEmail();
            var webToken =
                SW.GetNewSupervisorsCheatWebToken(email,
                    ServicesWrapper.webroles.superadmin); // получили токен супеадмина
            SW.AuthServiceClient.BearerToken = webToken.token.AccessToken;
            var post_new_carrier_result = SW.CreateNewCarrier();   // создали 
            var get_carrier_request = new FuraErp.ServiceModel.GetCarrier { Id = post_new_carrier_result.Id };
            var get_carrier_result = SW._privateAuthServiceClient.Get(get_carrier_request);
            Assert.AreEqual(get_carrier_result.Id, post_new_carrier_result.Id);

        }


        [Test]
        public void putcarrier() //todo проверить put и delete carrier
        {
            var email = SW.GenerateEmail();
            var webToken =
                SW.GetNewSupervisorsCheatWebToken(email,
                    ServicesWrapper.webroles.superadmin); // получили токен супеадмина
            SW.AuthServiceClient.BearerToken = webToken.token.AccessToken;
            var post_new_carrier_request = new FuraErp.ServiceModel.CreateCarrier
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
            var post_new_carrier_result = SW._privateAuthServiceClient.Post(post_new_carrier_request);
            var get_carrier_request = new FuraErp.ServiceModel.GetCarrier();
            var get_carrier_result = SW._privateAuthServiceClient.Get(get_carrier_request);
            Assert.AreEqual(get_carrier_result.Id, post_new_carrier_result.Id);

        }


        [Test]
        public void postdriver()
        {
            var email = SW.GenerateEmail();
            var webToken =
                SW.GetNewSupervisorsCheatWebToken(email,
                    ServicesWrapper.webroles.superadmin); // получили токен супеадмина
            SW.AuthServiceClient.BearerToken = webToken.token.AccessToken;
            var post_new_carrier_request = new FuraErp.ServiceModel.CreateCarrier
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
            var post_new_carrier_result = SW._privateAuthServiceClient.Post(post_new_carrier_request);

            var post_new_driver_reguest = new FuraErp.ServiceModel.CreateCarrierDriver
            {
                CarrierId = post_new_carrier_result.Id,
                FullName = "gec"
            };
            var post_new_driver_result = SW._privateAuthServiceClient.Post(post_new_driver_reguest);
            Assert.IsNotEmpty(post_new_driver_result.Id); // проверяем что вернулост Id
        }


        [Test]
        public void getdriver()
        {
            var email = SW.GenerateEmail();
            var webToken =
                SW.GetNewSupervisorsCheatWebToken(email,
                    ServicesWrapper.webroles.superadmin); // получили токен супеадмина
            SW.AuthServiceClient.BearerToken = webToken.token.AccessToken;
            var post_new_carrier_request = new FuraErp.ServiceModel.CreateCarrier
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
            var post_new_carrier_result = SW.ErpServiceClient.Post(post_new_carrier_request);

            var post_new_driver_reguest = new FuraErp.ServiceModel.CreateCarrierDriver
            {
                CarrierId = post_new_carrier_result.Id,
                FullName = "gec"
            };
            var post_new_driver_result = SW.ErpServiceClient.Post(post_new_driver_reguest);

            var post_new_driver_reguest2 = new FuraErp.ServiceModel.CreateCarrierDriver
            {
                CarrierId = post_new_carrier_result.Id,
                FullName = "gec2"
            };

            var post_new_driver_result2 = SW.ErpServiceClient.Post(post_new_driver_reguest2);
            Assert.IsNotEmpty(post_new_driver_result.Id); // проверяем что вернулост Id
            Assert.IsNotEmpty(post_new_driver_result2.Id);
            var get_driver_request = new FuraErp.ServiceModel.GetCarrierDriver // тест получения водителя/водителей
            {
                CarrierId = post_new_carrier_result.Id
            };

            var get_drivers_reguest = new FuraErp.ServiceModel.GetCarrierDrivers
            {
                CarrierId = post_new_carrier_result.Id
            };
            var get_driver_result = SW.ErpServiceClient.Get(get_driver_request);
            var get_drivers_result = SW.ErpServiceClient.Get(get_drivers_reguest);


            var first = get_drivers_result.Items.Find(m =>
                m.Id == post_new_driver_result.Id); // проверяем что в полученной модели есть нужные заявки
            var first2 = get_drivers_result.Items.Find(m => m.Id == post_new_driver_result2.Id);
            Assert.IsNotNull(first);
            Assert.IsNotNull(first2);
        }

        [Test]
        public void postvehicle() // тест Post и get carrier
        {
            var email = SW.GenerateEmail();
            var webToken =
                SW.GetNewSupervisorsCheatWebToken(email,
                    ServicesWrapper.webroles.superadmin); // получили токен супеадмина
            SW.AuthServiceClient.BearerToken = webToken.token.AccessToken;
            var post_new_carrier_request = new FuraErp.ServiceModel.CreateCarrier
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
            var post_new_carrier_result = SW.ErpServiceClient.Post(post_new_carrier_request);
            var get_carrier_request = new FuraErp.ServiceModel.GetCarrier();
            var get_carrier_result = SW.ErpServiceClient.Get(get_carrier_request);
            Assert.AreEqual(get_carrier_result.Id, post_new_carrier_result.Id);

            var post_vehicle_request = new FuraErp.ServiceModel.CreateCarrierVehicle
            {
                CarrierId = post_new_carrier_result.Id,
                Model = "test",
                RegistrationNumber = "test",
                TypeOfBody = "test",
                TypeOfLoading = "test",
                CarryingCapacity = "test",
                Capacity = "test",
                Length = "test",
                Width = "test",
                Height = "test",

            };
            var post_vehicle_result = SW.ErpServiceClient.Post(post_vehicle_request);
            Assert.IsNotEmpty(post_vehicle_result.Id);
            Assert.AreSame(post_vehicle_result.CarrierId, get_carrier_result.Id); // проверяем что в ответе прикрепилось к верному перевозчику

        }




        // todo проверить putdriver;





    }




}
