using FuraErp.ServiceModel.Molds;
using NUnit.Framework;

namespace Fura.Tests.Microservices.fura_erp
{
    class ServiceErpWrapper
    {
        private ChartererTests _chartererTests;

        public ServiceErpWrapper(ChartererTests chartererTests)
        {
            _chartererTests = chartererTests;
        }

        private ChartererMold CreateCharterer()
        {
            //Создание
            var createCharterer = new FuraErp.ServiceModel.CreateCharterer
            {
                Title = "Тестовый заказчик",
                WebSite = "www.test.com",
                FactAddress = "Москва, Красная площадь",
                PostAddress = "Моска, Китай-город"
            };
            var createChartererMold = _chartererTests.SW.ErpServiceClient.Post(createCharterer);
            Assert.IsNotEmpty(createChartererMold.Id);
            Assert.AreEqual(createCharterer.Title, createChartererMold.Title);
            Assert.AreEqual(createCharterer.WebSite, createChartererMold.WebSite);
            Assert.AreEqual(createCharterer.FactAddress, createChartererMold.FactAddress);
            Assert.AreEqual(createCharterer.PostAddress, createChartererMold.PostAddress);
            return createChartererMold;
        }
    }
}