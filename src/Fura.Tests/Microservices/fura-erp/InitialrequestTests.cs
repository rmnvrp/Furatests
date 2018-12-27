using System;
using System.Collections.Generic;
using System.Linq;
using FuraErp.ServiceModel.Molds;
using NUnit.Framework;
using ServiceStack;


namespace Fura.Tests.Microservices.fura_erp
{
    class InitialrequestTests
    {

        public readonly ServicesWrapper SW;

        public InitialrequestTests()
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
        public void InitialRequestMain()
        {
            //Авторизация
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = webToken.token.AccessToken;

            var createChartererMold = SW.CreateNewCharterer();

            var createJuridicalPersonMold = SW.CreateNewJuridicalPerson(createChartererMold.Id);

            //Создание
            var createInitialRequest = new FuraErp.ServiceModel.CreateInitialRequest
            {
                ChartererId = createChartererMold.Id,
                JuridicalPersonId = createJuridicalPersonMold.Id,
                Cargo = new CargoMold
                {
                    BruttoInTonns = "2",
                    CargoType = "Товары народного потребления",
                    PackagingType = "Паллета  EUR (0,8 x 1,2)",
                    VolumeInCubometres = "3",
                    VolumeInPallets = "1",
                },
                ChartererDescription = "хочу",
                ChartererRate = new ChartererRateMold
                {
                    Amount = 100500,
                    Currency = "RUB",
                    CarrierRequiredWithNDS = false
                },
                TransportRequirements = new TransportRequirementsMold
                {
                    AdditionalParameters = new List<string> { "мед. книжка", "гидроробот", "ремни", "коники" },
                    TransportPreset = "20т 82м3 33плт",
                    TypeOfBody = new List<string> { "рефрижераторный", "тентованный" },
                    TypeOfLoading = new List<string> { "задняя" }
                },
                StartRoutePoint = new RoutePointMold
                {
                    Locality = "г. Санкт-Петербург",
                    Address = "МО №28 \"Автово\", Кировский район, Стачек проспект, 92",
                    DateTimeFrom = DateTimeOffset.UtcNow.AddDays(2).ToUnixTimeMilliseconds(),
                    Latitude = 59.868157,
                    Longitude = 30.262091,
                },
                FinishRoutePoint = new RoutePointMold
                {
                    Locality = "г. Магнитогорск",
                    Address = "Ленинский район, ул. Бестужева, 58 ст1",
                    DateTimeFrom = DateTimeOffset.UtcNow.AddDays(2).ToUnixTimeMilliseconds(),
                    Latitude = 53.425056,
                    Longitude = 58.945383,
                },
                Priority = 1,
            };
            var createInitialRequestMold = SW.ErpServiceClient.Post(createInitialRequest);
            Assert.IsNotEmpty(createInitialRequestMold.Id);
            Assert.IsTrue(createInitialRequestMold.ChartererVersionId.StartsWith(createInitialRequest.ChartererId));

            var requestId = createInitialRequestMold.ProtoRequests.First().RequestIds.First();

            //Получаем заявку
            var getRequest = new FuraErp.ServiceModel.GetRequest { Id = requestId };
            var getRequestMold = SW.ErpServiceClient.Get(getRequest);
            Assert.IsNotEmpty(createInitialRequestMold.ChartererVersionId);
            Assert.AreEqual(createInitialRequest.ChartererDescription, getRequestMold.Request.Data.ChartererDescription);
            //TODO все поля

            //Получаем все заявки
            var getRequests = new FuraErp.ServiceModel.GetRequests { };
            var getRequestsMold = SW.ErpServiceClient.Get(getRequests);
            Assert.IsTrue(getRequestsMold.Count>0);
            Assert.IsTrue(getRequestsMold.Items.Count>0);
            Assert.IsTrue(getRequestsMold.TotalCount>0);

            //TODO протестировать обновление версии заказчика и юрлица при обновлении заявки
            //Изменение заявки
            var putRequest = new FuraErp.ServiceModel.PutRequest
            {
                Id = requestId,
                JuridicalPersonId = createJuridicalPersonMold.Id,
                Cargo = new CargoMold
                {
                    BruttoInTonns = "22",
                    CargoType = "Товары народного потребления2",
                    PackagingType = "Паллета  EUR (0,8 x 1,2)2",
                    VolumeInCubometres = "32",
                    VolumeInPallets = "12",
                },
                ChartererDescription = "хочу2",
                ChartererRate = new ChartererRateMold
                {
                    Amount = 1005002,
                    Currency = "RUB2",
                    CarrierRequiredWithNDS = true
                },
                TransportRequirements = new TransportRequirementsMold
                {
                    AdditionalParameters = new List<string> { "мед. книжка2", "гидроробот2", "ремни2", "коники2" },
                    TransportPreset = "20т 82м3 33плт2",
                    TypeOfBody = new List<string> { "рефрижераторный2", "тентованный2" },
                    TypeOfLoading = new List<string> { "задняя2" }
                },
                StartRoutePoint = new RoutePointMold
                {
                    Locality = "г. Санкт-Петербург2",
                    Address = "МО №28 \"Автово\", Кировский район, Стачек проспект, 922",
                    DateTimeFrom = DateTimeOffset.UtcNow.AddDays(2).ToUnixTimeMilliseconds(),
                    Latitude = 59.8681572,
                    Longitude = 30.2620912,
                },
                FinishRoutePoint = new RoutePointMold
                {
                    Locality = "г. Магнитогорск2",
                    Address = "Ленинский район, ул. Бестужева, 58 ст12",
                    DateTimeFrom = DateTimeOffset.UtcNow.AddDays(2).ToUnixTimeMilliseconds(),
                    Latitude = 53.4250562,
                    Longitude = 58.9453832,
                },
                Priority = 12,
            };
            var putRequestMold = SW.ErpServiceClient.Put(putRequest);

            //Добавление заметки
            var createNoteForRequest = new FuraErp.ServiceModel.CreateNoteForRequest
            {
                Id = requestId,
                Text = "Note1"
            };
            var createNoteForRequestMold = SW.ErpServiceClient.Post(createNoteForRequest);
            Assert.AreEqual(createNoteForRequest.Text, createNoteForRequestMold.Request.Data.Notes.First().Text);

            var createNoteForRequest2 = new FuraErp.ServiceModel.CreateNoteForRequest
            {
                Id = requestId,
                Text = "Note2"
            };
            var createNoteForRequestMold2 = SW.ErpServiceClient.Post(createNoteForRequest2);
            Assert.AreEqual(createNoteForRequest2.Text, createNoteForRequestMold2.Request.Data.Notes.Skip(1).First().Text);

            //Изменяем статус
            var changeStatusRequest = new FuraErp.ServiceModel.ChangeStatusRequest
            {
                Id = requestId,
                Status = RequestTransportationStatusMold.readyForApprove
            };
            var changeStatusRequestMold = SW.ErpServiceClient.Put(changeStatusRequest);
            Assert.AreEqual(changeStatusRequest.Status, changeStatusRequestMold.Request.Data.Status);
            Assert.AreEqual(2, changeStatusRequestMold.Request.Data.Notes.Count);

            var changeStatusRequest2 = new FuraErp.ServiceModel.ChangeStatusRequest
            {
                Id = requestId,
                Text = "ChangeStatus",
                Status = RequestTransportationStatusMold.approved
            };
            var changeStatusRequestMold2 = SW.ErpServiceClient.Put(changeStatusRequest2);
            Assert.AreEqual(changeStatusRequest2.Status, changeStatusRequestMold2.Request.Data.Status);
            Assert.AreEqual(changeStatusRequest2.Text, changeStatusRequestMold2.Request.Data.Notes.Last().Text);

            //Архивирование
            var archiveRequest = new FuraErp.ServiceModel.ArchiveRequest
            {
                Id = requestId
            };
            var archiveRequestMold = SW.ErpServiceClient.Delete(archiveRequest);
            Assert.AreEqual(archiveRequest.Id, archiveRequestMold.Id);
            Assert.AreEqual(ArchiveResult.ArchiveStatus.Archived, archiveRequestMold.Status);

            //Проверка результата архивирования
            var getRequest2 = new FuraErp.ServiceModel.GetRequest { Id = requestId };
            try
            {
                SW.ErpServiceClient.Get(getRequest2);
                Assert.IsTrue(false, "Должно возникунть исключение");
            }
            catch (WebServiceException e)
            {
                Assert.AreEqual(404, e.StatusCode);
            }

            //Повторное архивирование
            var archiveRequest2 = new FuraErp.ServiceModel.ArchiveRequest { Id = requestId };
            try
            {
                SW.ErpServiceClient.Delete(archiveRequest2);
                Assert.IsTrue(false, "Должно возникунть исключение");
            }
            catch (WebServiceException e)
            {
                Assert.AreEqual(404, e.StatusCode);
            }

        }
    }
}
