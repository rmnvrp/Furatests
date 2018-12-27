using FuraErp.ServiceModel;
using FuraErp.ServiceModel.Molds;
using NUnit.Framework;

namespace Fura.Tests
{
    public partial class ServicesWrapper
    {
        public ChartererMold CreateNewCharterer()
        {
            //Создание
            var createCharterer = new FuraErp.ServiceModel.CreateCharterer
            {
                Title = "Тестовый заказчик",
                WebSite = "www.test.com",
                FactAddress = "Москва, Красная площадь",
                PostAddress = "Моска, Китай-город"
            };
            var createChartererMold = this.ErpServiceClient.Post(createCharterer);
            Assert.IsNotEmpty(createChartererMold.Id);
            Assert.AreEqual(createCharterer.Title, createChartererMold.Title);
            Assert.AreEqual(createCharterer.WebSite, createChartererMold.WebSite);
            Assert.AreEqual(createCharterer.FactAddress, createChartererMold.FactAddress);
            Assert.AreEqual(createCharterer.PostAddress, createChartererMold.PostAddress);
            return createChartererMold;
        }

        public CarrierMold CreateNewCarrier()
        {
            var post_new_carrier_request = new FuraErp.ServiceModel.CreateCarrier
            {
                Title = "Тестовый перевозчик",
                PreferredRoutes = new ProtoRouteMold[]
                {
                    new ProtoRouteMold()
                    {
                        ArriveLocality = "moscow",
                        DepartLocality = ",fkf,kfk"
                    }
                }
            };
            var post_new_carrier_result = this._privateAuthServiceClient.Post(post_new_carrier_request);
            return post_new_carrier_result;
        }

        public JuridicalPersonMold CreateNewJuridicalPerson(string chartererId)
        {
            var createJuridicalPerson = new FuraErp.ServiceModel.CreateJuridicalPerson
            {
                OwnerType = OwnerTypeEnum.Charterer,
                OwnerId = chartererId,
                Title = "Тестовый юрик",
                FactAddress = "Москва, Красная площадь",
                PostAddress = "Моска, Китай-город",
                JuridicalAddress = "Москва, Красная площадь, 2",
                JuridicalType = "ООО",
                Inn = "123456789",
                Kpp = "0998772",
                Ogrn = "4574359834758934",
                RegistrationCountry = "Россия",
                ShortTitle = "Тестировщик",
            };
            var createJuridicalPersonMold = this.ErpServiceClient.Post(createJuridicalPerson);
            Assert.IsNotEmpty(createJuridicalPersonMold.Id);
            Assert.AreEqual(createJuridicalPerson.Title, createJuridicalPersonMold.Title);
            Assert.AreEqual(createJuridicalPerson.FactAddress, createJuridicalPersonMold.FactAddress);
            Assert.AreEqual(createJuridicalPerson.PostAddress, createJuridicalPersonMold.PostAddress);
            Assert.AreEqual(createJuridicalPerson.JuridicalAddress, createJuridicalPersonMold.JuridicalAddress);
            Assert.AreEqual(createJuridicalPerson.JuridicalType, createJuridicalPersonMold.JuridicalType);
            Assert.AreEqual(createJuridicalPerson.Inn, createJuridicalPersonMold.Inn);
            Assert.AreEqual(createJuridicalPerson.Kpp, createJuridicalPersonMold.Kpp);
            Assert.AreEqual(createJuridicalPerson.Ogrn, createJuridicalPersonMold.Ogrn);
            Assert.AreEqual(createJuridicalPerson.RegistrationCountry, createJuridicalPersonMold.RegistrationCountry);
            Assert.AreEqual(createJuridicalPerson.ShortTitle, createJuridicalPersonMold.ShortTitle);
            return createJuridicalPersonMold;
        }
    }
}