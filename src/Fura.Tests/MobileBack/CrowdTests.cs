using System.Collections.Generic;
using NUnit.Framework;
using ServiceStack;
using FuraBack.ServiceModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Fura.Tests;


namespace FuraBack.Tests
{
    [TestFixture]
    public class CrowdTests
    {
        public readonly JsonServiceClient service;

        private readonly TestHelper testHelper;
        public readonly ServicesWrapper SW;

        [SetUp]
        public void SetUp()
        {
            SW.SetUp();
        }

        public CrowdTests()
        {
            SW = new ServicesWrapper();
            service = SW.MobileServiceClient;
            var resultGetToken = SW.GetAnonimToken();

            service.BearerToken = resultGetToken.Token;
            testHelper = new TestHelper(this);
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            service.Dispose();
        }

        //.[Test]
        [Test]
        public void Test_CreatEvent()
        {
            long createEventId = testHelper.CreateTestEvent();
            Assert.Greater(createEventId, 0);
        }


        [Test]
        public void Test_UploadVoice()
        {
            long createEventId = testHelper.CreateTestEvent();
            var uploadVoice = new UploadVoice { EventId = createEventId };
            var file = new FileInfo(@"C:\Users\Роман\Downloads\Free-Converter.com-20180302091230-2863244991.aac");
            var result = service.PostFileWithRequest<string>(file, uploadVoice);
            var isMatchResult = Regex.IsMatch(result, "https://(((work)|(stage))\\.)?gofura\\.com/content/voices/[0-9]+-[a-z0-9]+[a-z0-9]+-[a-z0-9]+-[a-z0-9]+-[a-z0-9]+-[a-z0-9]+");
            Assert.IsTrue(isMatchResult);
        }


        [Test]
        public void Test_GetEvent()
        {
            var createdEventId = testHelper.CreateTestEvent();
            var getEventRequest = new GetEvent { EventId = createdEventId };
            var getEventResult = service.Get(getEventRequest);

            Assert.AreEqual(createdEventId, getEventResult.Id);
        }


        [Test]
        public void Test_GetVoice()
        {
            long createdEventId = testHelper.CreateTestEvent(); // создал краудсорсинговую точку
            var uploadVoice = new UploadVoice { EventId = createdEventId };
            var file = new FileInfo(@"C:\Users\Роман\Downloads\Free-Converter.com-20180302091230-2863244991.aac");
            service.PostFileWithRequest<string>(file, uploadVoice); //загружаю в созданную точку файл
            var getVoice = new GetVoice { EventId = createdEventId };
            var result = service.Get(getVoice); // проверяю что гет запрос отдает ссылку в нужном формате
            var isMatchResult = Regex.IsMatch(result, "https://(((work)|(stage))\\.)?gofura\\.com/content/voices/[0-9]+-[a-z0-9]+[a-z0-9]+-[a-z0-9]+-[a-z0-9]+-[a-z0-9]+-[a-z0-9]+");
            Assert.IsTrue(isMatchResult);
        }


        [Test]
        public void Test_DeleteEvent()
        {
            var createdEventId = testHelper.CreateTestEvent();
            testHelper.DeleteTestEvent(createdEventId);
            var getEventRequest = new GetEvent { EventId = createdEventId };
            try
            {
                var getEventResult = service.Get(getEventRequest);
                Assert.IsTrue(false, "Должно возникунть исключение");
            }
            catch (WebServiceException e)
            {
                Assert.AreEqual(500, e.StatusCode);                        // исправить потом, обрабатывать толкьо 404
            }

        }


        [Test]
        public void Test_PostGetEventsOnRoute()
        {
            long createdEventId = testHelper.CreateTestEvent(); // создал краудсорсинговую точку

            var PostGetEventsOnRouteRequest =
                new GetEventsOnRoute { Route = "o|xbAo|xbAo}@o}@", Categories = new List<string> { "platon" } };

            var PostGetEventsOnRouteResult = service.Post(PostGetEventsOnRouteRequest);
            //Thread.Sleep(10000);
            var events = PostGetEventsOnRouteResult.Data.ToList();
            Assert.IsTrue(events.Any());
            //Assert.AreEqual();

        }


        [Test]
        public void Test_GetEventsAroundLocation()
        {
            long createdEventId = testHelper.CreateTestEvent(); // создал краудсорсинговую точку, необходимо, чтобы точка вокруг посика точно была
            try
            {
                var GetEventsAroundLocationRequest =
                    new GetEventsAroundLocation
                    {
                        Azimuth = 11.11,
                        Longitude = 11.11,
                        Latitude = 11.11,
                        Categories = new List<string> { "platon" }, // добавить потом проверку всех категорий
                    };
                var GetEventsAroundLocationResult = service.Get(GetEventsAroundLocationRequest);
                var events = GetEventsAroundLocationResult.Data.ToList();
                Assert.IsTrue(events.Any());

            }
            finally
            {
                testHelper.DeleteTestEvent(createdEventId);
            }


        }


        [Test]
        public void Test_GetEventsInBounds()
        {
            long createdEventId = testHelper.CreateTestEvent();  // создаю событие в заданном квадрате
            try
            {
                var GetEventsInBoundsRequest =
                    new GetEventsInBounds
                    {
                        East = 11.12,
                        West = 11.10,
                        North = 11.12,
                        South = 11.10,
                        Categories = new List<string> { "platon" },
                    };
                var GetEventsInBoundsResult = service.Get(GetEventsInBoundsRequest);
                var events = GetEventsInBoundsResult.Data.ToList();
                Assert.IsTrue(events.Any());
            }

            finally
            {
                testHelper.DeleteTestEvent(createdEventId);    // удаляю точки
            }
        }

    }
}    