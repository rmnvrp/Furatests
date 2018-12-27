using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Fura.Tests;
using FuraBack.ServiceModel;
using FuraWebBack.ServiceModel;
using NUnit.Framework;
using DriverStatusEnumModel = FuraBack.ServiceModel.DriverStatusEnumModel;
using GetSupervisingRequests = FuraWebBack.ServiceModel.GetSupervisingRequests;

namespace FuraWebBack.Tests
{
    [TestFixture]
    public class SupervisingTests
    {
        private readonly ServicesWrapper SW;

        public SupervisingTests()
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
        public void Test04_PostSupervisingRequests()
        {
            var testphone = SW.GeneratePhone();
            var testemail = SW.GenerateEmail();
            SW.GetNewSupervisorCheatWebToken(testemail);
            var getSupervisingRequests = new GetSupervisingRequests();
            var existRequests = SW.WebServiceClient.Get(getSupervisingRequests);
            Assert.NotNull(existRequests);
            Assert.AreEqual(0, existRequests.Count);
            Console.WriteLine("Total Requests: {0}", existRequests.Count);

            var drivername = "Test1";
            var supervisingRequests = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }
            };
            var result = SW.WebServiceClient.Post(supervisingRequests);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            var newreq = result.First();
            Assert.AreEqual(testphone, newreq.Phone);
            Assert.AreEqual(drivername, newreq.DriverName);
            Assert.IsNull(newreq.Accepted);
            Assert.IsNull(newreq.AcceptedTime);
            Assert.IsNotEmpty(newreq.Rid);
            Console.WriteLine("Created request: {0}", newreq.Rid);

            var actualRequests = SW.WebServiceClient.Get(getSupervisingRequests);
            Assert.NotNull(getSupervisingRequests);
            Assert.AreEqual(existRequests.Count + 1, actualRequests.Count);
            Console.WriteLine("Total Requests: {0}", actualRequests.Count);
        }

        /// <summary>
        /// проверка публикации двух запросов на один номер от одной компании
        /// </summary>
        /// <returns></returns>
        [Test]
        public void Test04_1_PostSupervisingRequests()
        {

            var testphone = SW.GeneratePhone();
            var testemail = SW.GenerateEmail();

            var testcompany = SW.CreateCompany();

            SW.GetNewSupervisorCheatWebToken(testemail, testcompany);

            var drivername = "Test1";
            var supervisingRequests = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }

            };
            var result = SW.WebServiceClient.Post(supervisingRequests);
            
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            var newreq = result.First();
            Assert.AreEqual(testphone, newreq.Phone);
            Assert.AreEqual(drivername, newreq.DriverName);
            Assert.IsNull(newreq.Accepted);
            Assert.IsNull(newreq.AcceptedTime);
            Assert.IsNotEmpty(newreq.Rid);
            Console.WriteLine("Created request: {0}", newreq.Rid);


            var getSupervisingRequests = new GetSupervisingRequests();

            var actualRequests = SW.WebServiceClient.Get(getSupervisingRequests);
            Assert.NotNull(getSupervisingRequests);
            Assert.AreEqual(1, actualRequests.Count);
            Console.WriteLine("Total Requests: {0}", actualRequests.Count);

            var testemail2 = SW.GenerateEmail();
            SW.GetNewSupervisorCheatWebToken(testemail2, testcompany);
                           
            // создаю второй запрос на тот же номер
            var supervisingRequests2 = new PostSupervisingRequests            
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }
            };
            var result2 = SW.WebServiceClient.Post(supervisingRequests2);
            
            Assert.NotNull(result2);
            Assert.AreEqual(1, result2.Count);
            var newreq2 = result2.First();
            Assert.IsTrue(newreq2.Existed);
            Assert.AreEqual(testphone, newreq2.Phone);
            Assert.AreEqual(drivername, newreq2.DriverName);
            Assert.IsNull(newreq2.Accepted);
            Assert.IsNull(newreq2.AcceptedTime);
            Assert.IsNotEmpty(newreq2.Rid);
            Console.WriteLine("Created request2: {0}", newreq2.Rid);
            Assert.AreEqual(newreq.Rid, newreq2.Rid);

            var getSupervisingRequests2 = new GetSupervisingRequests();

            var actualRequests2 = SW.WebServiceClient.Get(getSupervisingRequests2);
            Assert.NotNull(getSupervisingRequests2);
            Assert.AreEqual(1, actualRequests2.Count);
            Console.WriteLine("Total Requests: {0}", actualRequests2.Count);
        }
        /// <summary>
        /// Проверка публикации двух запросов на отслеживание от двух разных компаний
        /// </summary>
        /// <returns></returns>
        [Test]
        public void Test04_2_PostSupervisingRequests()
        {
            //Очистка данных от тестов
            var testphone = SW.GeneratePhone();

            // создаю supervisor
            var testemail = SW.GenerateEmail();
            var testcompany = SW.CreateCompany();
            SW.GetNewSupervisorCheatWebToken(testemail, testcompany);

            // создаем запрос от первой компании
            var drivername = "Test1";
            var supervisingRequests = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }

            };
            var result = SW.WebServiceClient.Post(supervisingRequests);

            // проверяем результат
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            var newreq = result.First();
            Assert.AreEqual(testphone, newreq.Phone);
            Assert.AreEqual(drivername, newreq.DriverName);
            Assert.IsNull(newreq.Accepted);
            Assert.IsNull(newreq.AcceptedTime);
            Assert.IsNotEmpty(newreq.Rid);
            Console.WriteLine("Created request: {0}", newreq.Rid);

            // получаю все непринятые запросы от supervisor
            var getSupervisingRequests = new GetSupervisingRequests();
            var actualRequests = SW.WebServiceClient.Get(getSupervisingRequests);
            Assert.NotNull(getSupervisingRequests);
            Assert.AreEqual(1, actualRequests.Count);
            Console.WriteLine("Total Requests: {0}", actualRequests.Count);

            // создаю supervisor2
            var testemail2 = SW.GenerateEmail();
            var testcompany2 = SW.CreateCompany();
            SW.GetNewSupervisorCheatWebToken(testemail2, testcompany2);

            // создаем запрос от II компании
            var supervisingRequests2 = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }
            };
            var result2 = SW.WebServiceClient.Post(supervisingRequests2);

            // проверяем результат
            Assert.NotNull(result2);
            Assert.AreEqual(1, result2.Count);
            var newreq2 = result2.First();
            Assert.AreEqual(testphone, newreq2.Phone);
            Assert.AreEqual(drivername, newreq2.DriverName);
            Assert.IsNull(newreq2.Accepted);
            Assert.IsNull(newreq2.AcceptedTime);
            Assert.IsNotEmpty(newreq2.Rid);
            Console.WriteLine("Created request2: {0}", newreq2.Rid);

            // получаю все непринятые запросы от supervisor2
            var getSupervisingRequests2 = new GetSupervisingRequests();
            var actualRequests2 = SW.WebServiceClient.Get(getSupervisingRequests2);
            Assert.NotNull(getSupervisingRequests2);
            Assert.AreEqual(1, actualRequests2.Count);
            Console.WriteLine("Total Requests: {0}", actualRequests2.Count);


        }
        /// <summary>
        /// Проверка отображения получения непринятых запросов для superadmina
        /// </summary>
        /// <returns></returns>
        [Test]
        public void Test04_3_PostSuperRequests()
        {
            //Очистка данных от тестов
            var testphone = SW.GeneratePhone();

            // создаю supervisor
            var testemail = SW.GenerateEmail();
            var testcompany = SW.CreateCompany();
            SW.GetNewSupervisorCheatWebToken(testemail, testcompany);

            // создаем запрос от первой компании
            var drivername = "Test1";
            var supervisingRequests = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }

            };
            var result = SW.WebServiceClient.Post(supervisingRequests);

            // проверяем результат
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            var newreq = result.First();
            Assert.AreEqual(testphone, newreq.Phone);
            Assert.AreEqual(drivername, newreq.DriverName);
            Assert.IsNull(newreq.Accepted);
            Assert.IsNull(newreq.AcceptedTime);
            Assert.IsNotEmpty(newreq.Rid);
            Console.WriteLine("Created request: {0}", newreq.Rid);

            // получаю все непринятые запросы от supervisor
            var getSupervisingRequests = new GetSupervisingRequests();
            var actualRequests = SW.WebServiceClient.Get(getSupervisingRequests);
            Assert.NotNull(getSupervisingRequests);
            Assert.AreEqual(1, actualRequests.Count);
            Console.WriteLine("Total Requests: {0}", actualRequests.Count);

            // создаю supervisor2
            var testemail2 = SW.GenerateEmail();
            var testcompany2 = SW.CreateCompany();
            SW.GetNewSupervisorCheatWebToken(testemail2, testcompany2);

            // создаем запрос от II компании
            var supervisingRequests2 = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }
            };
            var result2 = SW.WebServiceClient.Post(supervisingRequests2);

            // проверяем результат
            Assert.NotNull(result2);
            Assert.AreEqual(1, result2.Count);
            var newreq2 = result2.First();
            Assert.AreEqual(testphone, newreq2.Phone);
            Assert.AreEqual(drivername, newreq2.DriverName);
            Assert.IsNull(newreq2.Accepted);
            Assert.IsNull(newreq2.AcceptedTime);
            Assert.IsNotEmpty(newreq2.Rid);
            Console.WriteLine("Created request2: {0}", newreq2.Rid);

            // получаю все непринятые запросы от supervisor2
            var getSupervisingRequests2 = new GetSupervisingRequests();
            var actualRequests2 = SW.WebServiceClient.Get(getSupervisingRequests2);
            Assert.NotNull(getSupervisingRequests2);
            Assert.AreEqual(1, actualRequests2.Count);
            Console.WriteLine("Total Requests: {0}", actualRequests2.Count);

            // создаю superadmina
            var testemail3 = SW.GenerateEmail();
            SW.GetNewSupervisorsCheatWebToken(testemail3);

            // получаю все непринятые запросы для superadmin
            var getSupervisingRequests3 = new GetSupervisingRequests();
            var actualRequests3 = SW.WebServiceClient.Get(getSupervisingRequests3);
            Assert.NotNull(getSupervisingRequests3);
            Assert.GreaterOrEqual(actualRequests3.Count, 2);
            // Assert.Contains(newreq2.Rid, actualRequests3.Select(request => request.Rid).ToList()); 
            Assert.IsTrue(actualRequests3.Any(request => request.Rid == newreq.Rid));
            Assert.IsTrue(actualRequests3.Any(request => request.Rid == newreq2.Rid));
            Console.WriteLine("Total Requests: {0}", actualRequests3.Count);
        }


        [Test]
        public void Test04_4_PostSuperRequests()
        {
            //Очистка данных от тестов
            var testphone = SW.GeneratePhone();

            // создаю supervisor
            var testemail = SW.GenerateEmail();
            var testcompany = SW.CreateCompany();
            SW.GetNewSupervisorCheatWebToken(testemail, testcompany);

            // создаем запрос от первой компании
            var drivername = "Test1";
            var supervisingRequests = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }

            };
            var result = SW.WebServiceClient.Post(supervisingRequests);

            // проверяем результат
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            var newreq = result.First();
            Assert.AreEqual(testphone, newreq.Phone);
            Assert.AreEqual(drivername, newreq.DriverName);
            Assert.IsNull(newreq.Accepted);
            Assert.IsNull(newreq.AcceptedTime);
            Assert.IsNotEmpty(newreq.Rid);
            Console.WriteLine("Created request: {0}", newreq.Rid);

            // получаю все непринятые запросы от supervisor
            var getSupervisingRequests = new GetSupervisingRequests();
            var actualRequests = SW.WebServiceClient.Get(getSupervisingRequests);
            Assert.NotNull(getSupervisingRequests);
            Assert.AreEqual(1, actualRequests.Count);
            Console.WriteLine("Total Requests: {0}", actualRequests.Count);

            // создаю supervisor2
            var testemail2 = SW.GenerateEmail();
            var testcompany2 = SW.CreateCompany();
            SW.GetNewSupervisorCheatWebToken(testemail2, testcompany2);

            // создаем запрос от II компании
            var supervisingRequests2 = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }
            };
            var result2 = SW.WebServiceClient.Post(supervisingRequests2);

            // проверяем результат
            Assert.NotNull(result2);
            Assert.AreEqual(1, result2.Count);
            var newreq2 = result2.First();
            Assert.AreEqual(testphone, newreq2.Phone);
            Assert.AreEqual(drivername, newreq2.DriverName);
            Assert.IsNull(newreq2.Accepted);
            Assert.IsNull(newreq2.AcceptedTime);
            Assert.IsNotEmpty(newreq2.Rid);
            Console.WriteLine("Created request2: {0}", newreq2.Rid);

            // получаю все непринятые запросы от supervisor2
            var getSupervisingRequests2 = new GetSupervisingRequests();
            var actualRequests2 = SW.WebServiceClient.Get(getSupervisingRequests2);
            Assert.NotNull(getSupervisingRequests2);
            Assert.AreEqual(1, actualRequests2.Count);
            Console.WriteLine("Total Requests: {0}", actualRequests2.Count);

            // создаю superadmina
            var testemail3 = SW.GenerateEmail();
            SW.GetNewSupervisorsCheatWebToken(testemail3);

            // получаю все непринятые запросы для superadmin
            var getSupervisingRequests3 = new GetSupervisingRequests();
            var actualRequests3 = SW.WebServiceClient.Get(getSupervisingRequests3);
            Assert.NotNull(getSupervisingRequests3);
            Assert.GreaterOrEqual(actualRequests3.Count, 2);
            // Assert.Contains(newreq2.Rid, actualRequests3.Select(request => request.Rid).ToList()); 
            Assert.IsTrue(actualRequests3.Any(request => request.Rid == newreq.Rid));
            Assert.IsTrue(actualRequests3.Any(request => request.Rid == newreq2.Rid));
            Console.WriteLine("Total Requests: {0}", actualRequests3.Count);
        }


        [Test]
        public void Test05_DeclineSupervisingRequest()
        {
            var testphone = SW.GeneratePhone();
            var testemail = SW.GenerateEmail();
            SW.GetNewSupervisorCheatWebToken(testemail);

            var drivername = "Test1";
            var supervisingRequests = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, drivername } }
            };
            var result = SW.WebServiceClient.Post(supervisingRequests);
            var newreq = result.First();
            Console.WriteLine("Created request: {0}", newreq.Rid);

            var declineNewRequest = new DeclineSupervisingRequest { Rid = newreq.Rid };
            var declined = SW.WebServiceClient.Delete(declineNewRequest);
            Assert.NotNull(declined);
            Assert.IsFalse(declined.Accepted);
            Assert.NotNull(declined.DeclinedTime);
            Assert.Greater(declined.DeclinedTime, declined.RequestTime);
            Console.WriteLine("Declined request: {0}", declined.Rid);

            var getSupervisingRequests = new GetSupervisingRequests();
            var existRequests = SW.WebServiceClient.Get(getSupervisingRequests);
            var existRequest = existRequests.First();
            Assert.AreEqual(declined.Rid, existRequest.Rid);
            Assert.AreEqual(declined.Phone, existRequest.Phone);
            Assert.AreEqual(declined.Accepted, existRequest.Accepted);
            Assert.AreEqual(declined.DeclinedTime, existRequest.DeclinedTime);
        }

        [Test]
        public void Test06_GetSupervisingDrivers()
        {
            var testuid = $"user:{Guid.NewGuid()}";
            var testphone = SW.GeneratePhone();
            var testname = "Test" + testuid;

            var testemail = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail);

            var mobileToken = SW.GetCheatMobileToken(testphone);

            Console.WriteLine("Created user: {0}", mobileToken.UID);
            // проверяем что ноль
            var getSupervisingDrivers = new GetSupervisingDrivers();
            var drivers = SW.WebServiceClient.Get(getSupervisingDrivers);
            Assert.NotNull(drivers);
            Assert.AreEqual(0, drivers.Count);

            var supervisingRequests = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, testname } }
            };
            var result = SW.WebServiceClient.Post(supervisingRequests);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            var newreq = result.First();
            Console.WriteLine("Posted Supervising Request: {0}", newreq.Rid);

            SW.NewCheatApprovedSupervisingRequest(testphone, webToken.token.AccessToken);
            Console.WriteLine("Supervising Request is Approved: {0}", newreq.Rid);

            var getSupervisingDrivers2 = new GetSupervisingDrivers();
            var actualdrivers = SW.WebServiceClient.Get(getSupervisingDrivers2);
            Assert.NotNull(actualdrivers);
            Assert.AreEqual(1, actualdrivers.Count);
            var driver = actualdrivers.First();
            Console.WriteLine("Supervising driver: {0}", driver.Uid);
        }

        /// <summary>
        /// Запрос получения водителей для суперадмина, когда есть один и тот же водитель есть у двух supervisorov
        /// </summary>
        /// <returns></returns>
        [Test]
        public void Test06_1GetSupervisingDrivers()
        {
            var testuid = $"user:{Guid.NewGuid()}";
            var testphone = SW.GeneratePhone();
            var testname = "Test1" + testuid;

            var testemail = SW.GenerateEmail();
            var testcompany = SW.CreateCompany();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail, testcompany);

            var mobileToken = SW.GetCheatMobileToken(testphone);

            Console.WriteLine("Created user: {0}", mobileToken.UID);

            var getSupervisingDrivers = new GetSupervisingDrivers();
            var drivers = SW.WebServiceClient.Get(getSupervisingDrivers);
            Assert.NotNull(drivers);
            Assert.AreEqual(0, drivers.Count);

            var supervisingRequests = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, testname } }
            };
            var result = SW.WebServiceClient.Post(supervisingRequests);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            var newreq = result.First();
            Console.WriteLine("Posted Supervising Request: {0}", newreq.Rid);
            SW.NewCheatApprovedSupervisingRequest(testphone, webToken.token.AccessToken);
            Console.WriteLine("Supervising Request is Approved: {0}", newreq.Rid);

            var getSupervisingDrivers2 = new GetSupervisingDrivers();
            var actualdrivers = SW.WebServiceClient.Get(getSupervisingDrivers2);
            Assert.NotNull(actualdrivers);
            Assert.AreEqual(1, actualdrivers.Count);
            var driver = actualdrivers.First();
            Console.WriteLine("Supervising driver: {0}", driver.Uid);

            //создаем запрос на того же водителя с тем же номером, но от другого supervisora
            var testname2 = "Test2" + testuid;

            var testemail2 = SW.GenerateEmail();
            var testcompany2 = SW.CreateCompany();
            var webToken2 = SW.GetNewSupervisorCheatWebToken(testemail2, testcompany2);

            var getSupervisingDrivers22 = new GetSupervisingDrivers();
            var drivers2 = SW.WebServiceClient.Get(getSupervisingDrivers22);
            Assert.NotNull(drivers2);
            Assert.AreEqual(0, drivers2.Count);

            var supervisingRequests2 = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, testname2 } }
            };
            var result2 = SW.WebServiceClient.Post(supervisingRequests2);
            Assert.NotNull(result2);
            Assert.AreEqual(1, result2.Count);
            var newreq2 = result2.First();
            Console.WriteLine("Posted Supervising Request2: {0}", newreq2.Rid);
            SW.NewCheatApprovedSupervisingRequest(testphone, webToken2.token.AccessToken);
            Console.WriteLine("Supervising Request is Approved2: {0}", newreq2.Rid);
            // создаю superadmina
            var testemail3 = SW.GenerateEmail();
            SW.GetNewSupervisorsCheatWebToken(testemail3);

            // получаю водителей для superadmina
            var getSuperadminDrivers = new GetSupervisingDrivers();
            var actualdrivers3 = SW.WebServiceClient.Get(getSuperadminDrivers);
            Assert.NotNull(actualdrivers3);
            Assert.GreaterOrEqual(1, actualdrivers3.Count(d => d.Uid == testuid));
            Console.WriteLine("Supeadmin driver: {0}", actualdrivers3.Count);
        }

        [Test]
        public void Test07_Real_GetSupervisingDrivers()
        {
            var testphone = SW.GeneratePhone();

            var mobileTokenResult = SW.GetAnonimToken();
            Assert.NotNull(mobileTokenResult);
            Assert.IsNotEmpty(mobileTokenResult.Token);
            Assert.IsNotEmpty(mobileTokenResult.UID);
            Console.WriteLine("Recieved anonim token: {0}", mobileTokenResult.Token);
            Console.WriteLine("Recieved anonim UID: {0}", mobileTokenResult.UID);
            SW.MobileServiceClient.BearerToken = mobileTokenResult.Token;
            var tokenTest = new TokenTest { UID = mobileTokenResult.UID };
            var resultTokenTest = SW.MobileServiceClient.Get(tokenTest);
            Assert.IsTrue(resultTokenTest.Ok);
            Console.WriteLine("Token Test is Ok: {0}", resultTokenTest.Ok);

            var token = SW.GetCheatMobileToken(testphone);
            Assert.NotNull(token);
            SW.MobileServiceClient.BearerToken = token.Token;
            Console.WriteLine("Recieved registered token: {0}", token.Token);
            Console.WriteLine("Recieved registered UID: {0}", token.UID);

            var testemail = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail);

            var getSupervisingDrivers = new GetSupervisingDrivers();
            var drivers = SW.WebServiceClient.Get(getSupervisingDrivers);
            Assert.NotNull(drivers);
            Assert.AreEqual(0, drivers.Count);
            var testname = "TestPhone:" + testphone;
            var supervisingRequests = new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> { { testphone, testname } }
            };
            var result = SW.WebServiceClient.Post(supervisingRequests);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            var newreq = result.First();
            Console.WriteLine("Posted Supervising Request: {0}", newreq.Rid);

            var getSupervisingRequests = new FuraBack.ServiceModel.GetSupervisingRequests();
            var mobileRequests = SW.MobileServiceClient.Get(getSupervisingRequests);
            Assert.NotNull(mobileRequests);
            Assert.AreEqual(1, mobileRequests.Count);
            var mobilereq = mobileRequests.First();

            Assert.AreEqual(newreq.Rid, mobilereq.Rid);
            Assert.AreEqual(newreq.RequestTime, mobilereq.RequestTime);
            Assert.AreEqual(newreq.SupervisorName, mobilereq.SupervisorName);
            Console.WriteLine("Supervising Request on Mobile is Identical: {0}", newreq.Rid);

            var accept = new ResponseSupervisingRequest
            {
                Rid = mobilereq.Rid,
                Accept = true
            };
            var accepted = SW.MobileServiceClient.Post(accept);
            Assert.NotNull(accepted);
            Assert.AreEqual(testemail, accepted.SupervisorEmail);
            Assert.AreEqual(webToken.sid, accepted.SupervisorId);
            Console.WriteLine("Supervising Request is Approved: {0}", newreq.Rid);

            var getSupervisingDrivers2 = new GetSupervisingDrivers();
            var actualdrivers = SW.WebServiceClient.Get(getSupervisingDrivers2);
            Assert.NotNull(actualdrivers);
            Assert.AreEqual(1, actualdrivers.Count);
            var driver = actualdrivers.First();

            Assert.AreEqual(token.UID, driver.Uid);
            Assert.AreEqual(testphone, driver.Phone);
            Assert.AreEqual(testname, driver.DriverName);
            Assert.IsNull(driver.LastPosition);
            Console.WriteLine("Supervisor get driver: {0}", driver.Uid);

            var getSupervisors = new GetSupervisors();
            var supervisors = SW.MobileServiceClient.Get(getSupervisors);
            Assert.NotNull(supervisors);
            Assert.AreEqual(1, supervisors.Count);
            var supervisor = supervisors.First();

            //Assert.AreEqual(accepted.Uid, supervisor.Uid);
            //Assert.AreEqual(accepted.Phone, supervisor.Phone);
            //Assert.AreEqual(accepted.Email, supervisor.Email);
            //Assert.AreEqual(accepted.Name, supervisor.Name);
            //Assert.AreEqual(accepted.RegisteredTime, supervisor.RegisteredTime);
            //Console.WriteLine("Supervisor info on Mobile is Identical: {0}", supervisor.Uid);
            Assert.AreEqual(accepted.SupervisorId, supervisor.SupervisorId);
            Assert.AreEqual(accepted.SupervisorName, supervisor.SupervisorName);
            Assert.AreEqual(accepted.SupervisorPhone, supervisor.SupervisorPhone);
            Assert.AreEqual(accepted.SupervisorEmail, supervisor.SupervisorEmail);
            Assert.AreEqual(accepted.CompanyId, supervisor.CompanyId);
            Assert.AreEqual(accepted.CompanyName, supervisor.CompanyName);
            Assert.AreEqual(accepted.AcceptedTime, supervisor.AcceptedTime);
            Console.WriteLine("Supervisor info on Mobile is Identical: {0}", supervisor.SupervisorId);
        }

        [Test]
        public void Test08_GetCheatMobileToken()
        {
            var testphone = SW.GeneratePhone();
            var mobileToken = SW.GetCheatMobileToken(testphone);
            Assert.IsNotNull(mobileToken);
            Assert.IsNotEmpty(mobileToken.UID);
            Assert.IsNotEmpty(mobileToken.RefreshToken);
            Assert.IsNotEmpty(mobileToken.Token);
        }

        [Test]
        public void Test09_Real_GetSupervisingTransportations()
        {
            var testphone = SW.GeneratePhone();
            var mobileToken = SW.GetCheatMobileToken(testphone);

            var testemail = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail);

            SW.NewCheatApprovedSupervisingRequest(testphone, webToken.token.AccessToken);
            

            var getstatus = new GetDriverStatus();
            var driverStatusModel = SW.MobileServiceClient.Get(getstatus);
            Assert.IsNull(driverStatusModel);
            var random = new Random();
            var orderWaitingStatus = new PostDriverStatus
            {
                Latitude = random.NextDouble() * 90,
                Longitude = random.NextDouble() * 90,
                Status = DriverStatusEnumModel.OrderWaiting
            };
            var postDriverStatusResponse = SW.MobileServiceClient.Post(orderWaitingStatus);
            Assert.AreEqual(HttpStatusCode.NoContent, postDriverStatusResponse.StatusCode);
            var driverStatusModel2 = SW.MobileServiceClient.Get(getstatus);
            Assert.IsNotNull(driverStatusModel2);
            Assert.AreEqual(orderWaitingStatus.Status, driverStatusModel2.Status);

            var postTelematic = new PostTelematic
            {
                Latitude = random.NextDouble() * 90,
                Longitude = random.NextDouble() * 90
            };
            var telematicResponse = SW.MobileServiceClient.Post(postTelematic);
            Assert.AreEqual(HttpStatusCode.NoContent, telematicResponse.StatusCode);

            var goingToTheLoadingZone = new PostDriverStatus
            {
                SupervisorId = webToken.sid,
                Latitude = random.NextDouble() * 90,
                Longitude = random.NextDouble() * 90,
                Status = DriverStatusEnumModel.GoingToTheLoadingZone
            };
            var postDriverStatusResponse2 = SW.MobileServiceClient.Post(goingToTheLoadingZone);
            Assert.AreEqual(HttpStatusCode.NoContent, postDriverStatusResponse2.StatusCode);

            var getSupervisingTransportations = new GetSupervisingTransportations();
            var transportations = SW.WebServiceClient.Get(getSupervisingTransportations);
            Assert.IsNotNull(transportations);
            Assert.AreEqual(1, transportations.Count);
            var trans = transportations.First();
            Assert.AreEqual(mobileToken.UID, trans.Uid);
        }

        [Test]
        public void Test10_Real_GenerateSupervisingTransportationLink()
        {
            var testphone = SW.GeneratePhone();
            var mobileToken = SW.GetCheatMobileToken(testphone);

            var testemail = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail);
            var trans = SW.GetTransportation(testphone, mobileToken, webToken, testemail);

            var generatelink = new GenerateSupervisingTransportationLink
            {
                Tid = trans.Tid,
                Pass = trans.Tid
            };
            var link = SW.WebServiceClient.Post(generatelink);
            Assert.AreEqual(generatelink.Tid, link.Tid);
            Assert.AreEqual(generatelink.Tid, link.Pass);
        }

        [Test]
        public void Test11_Real_GetTransportationFull()
        {
            var testphone = SW.GeneratePhone();
            var mobileToken = SW.GetCheatMobileToken(testphone);

            var testemail = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail);
            var trans = SW.GetTransportation(testphone, mobileToken, webToken, testemail);

            var transportationFull = new GetSupervisingTransportationFull
            {
                Tid = trans.Tid
            };
            var transportationFullModel = SW.WebServiceClient.Get(transportationFull);
            Assert.AreEqual(trans.Tid, transportationFullModel.Tid);
            Assert.AreEqual(trans.Uid, transportationFullModel.Uid);
            Assert.AreEqual(trans.Status, transportationFullModel.Status);
            Assert.AreEqual(1, transportationFullModel.Positions.Length);
            Assert.AreEqual(1, transportationFullModel.Statuses.Length);
        }

        [Test]
        public void Test11_1_Real_GetLongTransportationFull()
        {
            var testphone = SW.GeneratePhone();
            var mobileToken = SW.GetCheatMobileToken(testphone);

            var testemail = SW.GenerateEmail();
            var testcompany = SW.CreateCompany();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail, testcompany);
            var trans = SW.GetLongTransportation(testphone, mobileToken, webToken, testcompany);

            var transportationFull = new GetSupervisingTransportationFull
            {
                Tid = trans.Tid
            };
            var transportationFullModel = SW.WebServiceClient.Get(transportationFull);
            Assert.AreEqual(trans.Tid, transportationFullModel.Tid);
            Assert.AreEqual(trans.Uid, transportationFullModel.Uid);
            Assert.AreEqual(trans.Status, transportationFullModel.Status);
            Assert.AreEqual(10, transportationFullModel.Positions.Length);
            Assert.AreEqual(11, transportationFullModel.Statuses.Length);
        }

        [Test]
        public void Test12_Real_GetTransportationFullByLink()
        {
            var testphone = SW.GeneratePhone();
            var mobileToken = SW.GetCheatMobileToken(testphone);

            var testemail = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail);
            var trans = SW.GetTransportation(testphone, mobileToken, webToken, testemail);

            var random = new Random();
            var generatelink = new GenerateSupervisingTransportationLink
            {
                Tid = trans.Tid,
                Pass = random.Next().ToString()
            };
            var link = SW.WebServiceClient.Post(generatelink);

            var transportationFull = new GetSupervisingTransportationFull
            {
                Tid = link.Tid,
                Pass = link.Pass
            };
            SW.WebServiceClient.BearerToken = null;
            var transportationFullModel = SW.WebServiceClient.Get(transportationFull);
            Assert.AreEqual(trans.Tid, transportationFullModel.Tid);
            Assert.AreEqual(trans.Uid, transportationFullModel.Uid);
            Assert.AreEqual(trans.Status, transportationFullModel.Status);
            Assert.AreEqual(1, transportationFullModel.Positions.Length);
            Assert.AreEqual(1, transportationFullModel.Statuses.Length);
        }

        [Test]
        public void Test13_Real_PostDriversSearchRequest()
        {
            var testphone = SW.GeneratePhone();
            var mobileToken = SW.GetCheatMobileToken(testphone);

            var testcompany = SW.CreateCompany();
            var testemail = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail, testcompany);

            SW.NewCheatApprovedSupervisingRequest(testphone, webToken.token.AccessToken);

            var random = new Random();

            var postDriverStatus = new FuraBack.ServiceModel.PostDriverStatus
            {
                Status = DriverStatusEnumModel.OrderWaiting,
                CompanyId = testcompany,
                Latitude = random.NextDouble() * 180,
                Longitude = random.NextDouble() * 180,
                OrderWaitingForm = new FuraBack.ServiceModel.DriverFormModel
                {
                    StartLoadingTime = DateTimeOffset.UtcNow.AddDays(random.Next(0, 2)).ToUnixTimeMilliseconds(),
                    FinishLoadingTime = DateTimeOffset.UtcNow.AddDays(random.Next(2, 7)).ToUnixTimeMilliseconds(),
                    Tonnage = random.NextDouble() * 10,
                    BodySpaceInCubicMeters = random.NextDouble() * 10,
                    BodyHeight = random.NextDouble() * 10,
                    BodyLength = random.NextDouble() * 10,
                    BodyWidth = random.NextDouble() * 10,
                    FinishLocationLatitude = random.NextDouble() * 90,
                    FinishLocationLongitude = random.NextDouble() * 90,
                    FinishLocationRadius = random.NextDouble() * 1000,
                    FinishLocationName = "FinishLocation" + random.Next(),
                    StartLocationLatitude = random.NextDouble() * 90,
                    StartLocationLongitude = random.NextDouble() * 90,
                    StartLocationRadius = random.NextDouble() * 1000,
                    StartLocationName = "StartLocation" + random.Next(),
                    LoadType = new List<int> { 1, 2, 3 },
                    TruckBodyType = new List<int> { 4, 5, 6 },
                }
            };
            SW.MobileServiceClient.BearerToken = mobileToken.Token;
            var result = SW.MobileServiceClient.Post(postDriverStatus);

            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
            var orderWaitingForm = postDriverStatus.OrderWaitingForm;
            var postDriversSearch = new PostDriversSearchRequest
            {
                Filter = new DriversFilterModel
                {
                    BodyHeight = orderWaitingForm.BodyHeight,
                    BodyLength = orderWaitingForm.BodyLength,
                    BodyWidth = orderWaitingForm.BodyWidth,
                    CargoVolumeInCubicMeters = orderWaitingForm.BodySpaceInCubicMeters,
                    CargoWeightInTons = orderWaitingForm.Tonnage,
                    FinishLoadingTime = orderWaitingForm.FinishLoadingTime,
                    FinishLocationLatitude = orderWaitingForm.FinishLocationLatitude,
                    FinishLocationLongitude = orderWaitingForm.FinishLocationLongitude,
                    LoadType = orderWaitingForm.LoadType,
                    TruckBodyType = orderWaitingForm.TruckBodyType,
                    StartLoadingTime = orderWaitingForm.StartLoadingTime,
                    StartLocationLatitude = orderWaitingForm.StartLocationLatitude,
                    StartLocationLongitude = orderWaitingForm.StartLocationLongitude,
                }
            };
            var search = SW.WebServiceClient.Post(postDriversSearch);
            Assert.AreEqual(1, search.Count);
            var form = search.First();
            Assert.IsTrue(Math.Abs(orderWaitingForm.StartLocationLatitude.Value - form.StartLocationLatitude.Value) <= 0.00000001);
            Assert.AreEqual(orderWaitingForm.StartLocationName, form.StartLocationName);
            Assert.AreEqual(orderWaitingForm.FinishLocationName, form.FinishLocationName);
        }

        [Test]
        public void Test14_PostDriversSearchRequest()
        {
            var testphone = SW.GeneratePhone();
            var mobileToken = SW.GetCheatMobileToken(testphone);

            var companyId = SW.CreateCompany();
            var testemail = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail, companyId);

            var request = SW.NewCheatApprovedSupervisingRequest(testphone, webToken.token.AccessToken);

            var orderWaitingForm = GenerateOrderWaitingForm();

            SW.MobileServiceClient.Post(new FuraBack.ServiceModel.PostDriverStatus()
            {
                OrderWaitingForm = orderWaitingForm,
                CompanyId = companyId,
                Status = DriverStatusEnumModel.OrderWaiting,
                Latitude = 50,
                Longitude = 34
            });
            
            var postDriversSearch = new PostDriversSearchRequest
            {
                Filter = new DriversFilterModel
                {
                    BodyHeight = orderWaitingForm.BodyHeight,
                    BodyLength = orderWaitingForm.BodyLength,
                    BodyWidth = orderWaitingForm.BodyWidth,
                    CargoVolumeInCubicMeters = orderWaitingForm.BodySpaceInCubicMeters,
                    CargoWeightInTons = orderWaitingForm.Tonnage,
                    FinishLoadingTime = orderWaitingForm.FinishLoadingTime,
                    FinishLocationLatitude = orderWaitingForm.FinishLocationLatitude,
                    FinishLocationLongitude = orderWaitingForm.FinishLocationLongitude,
                    LoadType = orderWaitingForm.LoadType,
                    TruckBodyType = orderWaitingForm.TruckBodyType,
                    StartLoadingTime = orderWaitingForm.StartLoadingTime,
                    StartLocationLatitude = orderWaitingForm.StartLocationLatitude,
                    StartLocationLongitude = orderWaitingForm.StartLocationLongitude,
                }
            };
            var search = SW.WebServiceClient.Post(postDriversSearch);
            Assert.AreEqual(1, search.Count);
            var form = search.First();
            Assert.IsTrue(Math.Abs(orderWaitingForm.StartLocationLatitude.Value - form.StartLocationLatitude.Value) <= 0.00000001);
        }

        [Test]
        [TestCase(1, 20)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(0, 0)]
        [TestCase(2, 1)]
        [TestCase(20, 20)]
        public void Test15_GetDriversFilterRequest(int count = 1, int limit = 20)
        {
            var testcompany = SW.CreateCompany();
            var testemail = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorCheatWebToken(testemail, testcompany);
            DriversFilterModel driversFilterModel = null;
            for (int i = 0; i < count; i++)
            {
                driversFilterModel = GenerateFilterSearch(webToken.sid);
                SW.WebServiceClient.Post(new PostDriversSearchRequest
                {
                    Filter = driversFilterModel
                });
            }
            Thread.Sleep(2000);
            var getDriversFilterRequest = new GetDriversFilterRequest { Limit = limit };
            var search = SW.WebServiceClient.Get(getDriversFilterRequest);
            var must = Math.Min(count, limit);
            Assert.AreEqual(must, search.Count);
            var form = search.FirstOrDefault();
            if (driversFilterModel != null && form != null)
            {
                Assert.IsTrue(Math.Abs(driversFilterModel.StartLocationLatitude.Value - form.StartLocationLatitude.Value) <= 0.00000001);
            }
        }

        private static FuraBack.ServiceModel.DriverFormModel GenerateOrderWaitingForm()
        {
            var random = new Random();

            var orderWaitingForm = new FuraBack.ServiceModel.DriverFormModel
            {
                StartLoadingTime = DateTimeOffset.UtcNow.AddDays(random.Next(0, 2)).ToUnixTimeMilliseconds(),
                FinishLoadingTime = DateTimeOffset.UtcNow.AddDays(random.Next(2, 7)).ToUnixTimeMilliseconds(),
                Tonnage = random.NextDouble() * 10,
                BodySpaceInCubicMeters = random.NextDouble() * 10,
                BodyHeight = random.NextDouble() * 10,
                BodyLength = random.NextDouble() * 10,
                BodyWidth = random.NextDouble() * 10,
                FinishLocationLatitude = random.NextDouble() * 90,
                FinishLocationLongitude = random.NextDouble() * 90,
                FinishLocationRadius = random.NextDouble() * 1000,
                StartLocationLatitude = random.NextDouble() * 90,
                StartLocationLongitude = random.NextDouble() * 90,
                StartLocationRadius = random.NextDouble() * 1000,
                LoadType = new List<int> { 1, 2, 3 },
                TruckBodyType = new List<int> { 4, 5, 6 },
            };
            return orderWaitingForm;
        }

        private static DriversFilterModel GenerateFilterSearch(string uid)
        {
            var random = new Random();

            var orderWaitingForm = new DriversFilterModel()
            {
                SupervisorId = uid,
                IssuedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                StartLoadingTime = DateTimeOffset.UtcNow.AddDays(random.Next(0, 2)).ToUnixTimeMilliseconds(),
                FinishLoadingTime = DateTimeOffset.UtcNow.AddDays(random.Next(2, 7)).ToUnixTimeMilliseconds(),
                CargoWeightInTons = random.NextDouble() * 10,
                CargoVolumeInCubicMeters = random.NextDouble() * 10,
                BodyHeight = random.NextDouble() * 10,
                BodyLength = random.NextDouble() * 10,
                BodyWidth = random.NextDouble() * 10,
                FinishLocationLatitude = random.NextDouble() * 90,
                FinishLocationLongitude = random.NextDouble() * 90,
                StartLocationLatitude = random.NextDouble() * 90,
                StartLocationLongitude = random.NextDouble() * 90,
                LoadType = new List<int> { 1, 2, 3 },
                TruckBodyType = new List<int> { 4, 5, 6 },
            };
            return orderWaitingForm;
        }
    }
}