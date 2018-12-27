using System;
using System.Collections.Generic;
using System.Threading;
using Fura.Model;
using NUnit.Framework;

namespace Fura.Tests.Microservices.integration
{
    [TestFixture]
    public class FuraExpeditionTests1
    {
        public readonly ServicesWrapper SW;

        public FuraExpeditionTests1()
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

        /// <summary>
        /// тест парсинг сущностей и сохранение в персональный справочник
        /// </summary>
        [Test]
        public void Test01_CreateRequestOfTransportation() 
        {
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            var dto = new FuraExpedition.ServiceModel.CreateRequestOfTransportation();

            dto.Data = new RequestTransportationModel()
            {
                AdditionalConditions = new List<AdditionalConditionModel>()
                {
                    new AdditionalConditionModel() {Content = "onjkjn", Name = "fdesd"}
                },
                Cargo = new CargoModel() {DangerousDocuments = "blablabla"}
            };
            
            SW.ExpeditionServiceClient.BearerToken = webToken.token.AccessToken;
            var request = SW.ExpeditionServiceClient.Post(dto); //создали заявку
            Thread.Sleep(TimeSpan.FromSeconds(1));
            var dto2 = new FuraExpedition.ServiceModel.GetRequestOfTransportation {Id = request.Id}; // запрсо нужен потому что post не возвращает модель
            var Getrequest = SW.ExpeditionServiceClient.Get(dto2);
            Assert.AreEqual(Getrequest.Id, request.Id);
            //Thread.Sleep(TimeSpan.FromSeconds(1));// pflth;rf
            var cargoid= Getrequest.Data.Cargo.Id;// id справочника cargo
            var requestpersonal = new FuraPersonalPrompts.ServiceModel.GetCargoPrompt(){ Id = cargoid };
            SW.PersonalPromptsServiceClient.BearerToken = webToken.token.AccessToken;
            var getrequestcargo = SW.PersonalPromptsServiceClient.Get(requestpersonal);
            Assert.AreEqual(cargoid, getrequestcargo.Id);
            Assert.AreEqual(dto.Data.Cargo.DangerousDocuments, getrequestcargo.Data.DangerousDocuments);

        }
    }
}


