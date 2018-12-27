using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fura.Model;
using FuraPersonalPrompts.ServiceModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Internal;
using NUnit.Framework;

namespace Fura.Tests.Microservices.fura_personalpromt
{
    class CargopromtTests
    {
        public readonly ServicesWrapper SW;

        public CargopromtTests()
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
        /// Тест создания заявки
        /// 
        /// </summary>
        [Test]
        public void postcargopromts()  // метод не
        {
            // методы доступен только для внутреннего пользователя, поэтому используется _privateauth
           /* var email = SW.GenerateEmail();
            var webToken =
                SW.GetNewSupervisorsCheatWebToken(email,
                    ServicesWrapper.webroles.superadmin); // получили токен супеадмина
            SW.AuthServiceClient.BearerToken = webToken.token.AccessToken;*/

            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);// 
            SW.PersonalPromptsServiceClient.BearerToken = webToken.token.AccessToken;

            var cargo_promts_request = new FuraPersonalPrompts.ServiceModel.CreateCargoPrompt  //
            {

                UsedInRequests = new List<string> {"Тест"},                       // 


                Data = new CargoModel()
                {
                    Id = "561",
                    AmountOfPlaces = "fhds",// ,
                    BruttoBiggest = "erfiof"
                   
                }

               
            };
            var cargo_promts_result = SW.PersonalPromptsServiceClient.Post(cargo_promts_request);// как тут параметр нужный задать? у меня есть , lf 
        }
    }
}
