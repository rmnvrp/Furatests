using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Fura.Model;
using FuraAuth.ServiceModel;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using NUnit.Framework;

namespace Fura.Tests.Microservices.fura_expedition
{
    [TestFixture]
    public class FuraExpeditionTests
    {
        public readonly ServicesWrapper SW;

        public FuraExpeditionTests()
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
            SW.TestFixtureTearDown();
        }


        [Test]
        public void Test01_CreateRequestOfTransportation() // тест создания заявки и получения заявки по ID
        {
            var email  = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            var dto = new FuraExpedition.ServiceModel.CreateRequestOfTransportation();
            

            dto.Data = new RequestTransportationModel()
            {
                AdditionalConditions = new List<AdditionalConditionModel>() { new AdditionalConditionModel() { Content = "onjkjn", Name = "fdesd"} }   
              , Cargo = new CargoModel() { DangerousDocuments = "blablabla"}
            };


            SW.ExpeditionServiceClient.BearerToken = webToken.token.AccessToken;

            var  request = SW.ExpeditionServiceClient.Post(
                dto
                ); //создали заявку

            var dto2 = new FuraExpedition.ServiceModel.GetRequestOfTransportation { Id = request.Id };
            var Getrequest = SW.ExpeditionServiceClient.Get(dto2);
           // сравнить свойства модели
            Assert.AreEqual(Getrequest.Id, request.Id);
            

        }


        [Test]
        public void Test02_GetRequestOfTransportations() // тест получения списка заявок TOdo добавить тесты фильтрации по параметрам
        {

            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            var dto = new FuraExpedition.ServiceModel.CreateRequestOfTransportation();

            dto.Data = new RequestTransportationModel()
            {
                AdditionalConditions = new List<AdditionalConditionModel>() { new AdditionalConditionModel() { Content = "onjkjn", Name = "fdesd" } }
            };

            SW.ExpeditionServiceClient.BearerToken = webToken.token.AccessToken; // вот здесь вопрос, зачем так сделано?
            var request = SW.ExpeditionServiceClient.Post(
                dto
            ); //создали заявку

            var request2 = SW.ExpeditionServiceClient.Post(
                dto
            ); //создали заявку2
            
            var dto2 = new FuraExpedition.ServiceModel.GetRequestsOfTransportation() ; // обозвать по дургому переменную
            var Getrequest = SW.ExpeditionServiceClient.Get(dto2); // тут тоже
            // сравнить свойства модели
            //Assert.AreEqual(Getrequest.Count, 2);
            var first = Getrequest.Items.Find(m => m.Id == request.Id); // проверяем что в полученной модели есть нужные заявки
            var first2 = Getrequest.Items.Find(m => m.Id == request2.Id);
            Assert.IsNotNull(first);
            Assert.IsNotNull(first2);
        }




        /* [Test]
         public void Test02_GetRequestOfTransportation()
         {
             var email = SW.GenerateEmail();
             var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
             var dto = new FuraExpedition.ServiceModel.CreateRequestOfTransportation();
             dto.Data = new RequestTransportationModel()
             {
                 AdditionalConditions = new List<AdditionalConditionModel>() { new AdditionalConditionModel() { Content = "onjkjn", Name = "fdesd" } }
             };

             SW.ExpeditionServiceClient.BearerToken = webToken.token.AccessToken;

             var request = SW.ExpeditionServiceClient.Get(request);

         }*/

    }

}

