using System;
using System.Net;
using System.Threading.Tasks;
using Fura.Interface.DB.Entities;
using Fura.Tests;
using FuraBack.ServiceModel;
using NUnit.Framework;
using Sms.Ru.ServiceModel;
using FuraBack;


namespace FuraBack.Tests
{
    [TestFixture]
    public class AuthTests
    {
        /// <summary>
        /// Обёртка над сервисами, помогает писать тесты
        /// </summary>
        public readonly ServicesWrapper SW;

        /// <summary>
        /// Конструктор тестов
        /// </summary>
        public AuthTests()
        {
            SW = new ServicesWrapper();
        }

        /// <summary>
        /// Метод вызываемый в конце тестовой сессии. Должен освобождать ресурсы, закрывать соединения и чистить базу от тестовых данных.
        /// </summary>
        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            SW.TestFixtureTearDown();
        }

        [SetUp]
        public void SetUp()
        {
            SW.SetUp();
        }

        /// <summary>
        /// Проверка получения нового анонимного токена
        /// </summary>
        [Test]
        public void Test01_GetToken()
        {
            var resultGetToken = SW.GetAnonimToken();
            Assert.IsNotNull(resultGetToken.UID);
            Assert.IsNotNull(resultGetToken.Token);
            Assert.IsNotEmpty(resultGetToken.UID);
            Assert.IsNotEmpty(resultGetToken.Token);
        }

        /// <summary>
        /// Тест метода проверки токена
        /// </summary>
        [Test]
        public void Test02_TokenTest()
        {
            var getTokenResult = SW.GetAnonimToken();

            DoTokenTest(getTokenResult.UID);
        }

        /// <summary>
        /// Должен быть реальный рабочий номер, на который будет уходить смс.
        /// </summary>
        string realPhone = "79998643216";

        /// <summary>
        /// Реальная проверка регистрации
        /// </summary>
        /// <returns></returns>
        [Test]
        public void Test03_Real_RegisterPhone()
        {
            //Получение нового анонимного токена
            var getTokenResult = SW.GetAnonimToken();

            //Отправка запроса на регистрацию номера
            var requestRegisterPhone = new RegisterPhone { Phone = realPhone };
            var resultRegisterPhone = SW.MobileServiceClient.Post(requestRegisterPhone);
            Assert.AreEqual(HttpStatusCode.NoContent, resultRegisterPhone.StatusCode);
            Console.WriteLine("Sended sms to {0}", realPhone);

            //Извлечение смс кода из базы
            var smsCode = SW.GetLastSms(getTokenResult.UID);
            Console.WriteLine("SMS code is {0}", smsCode);

            //Подтверждение кода
            var requestApprovePhone = new ApprovePhone { Code = smsCode };
            var resultApprovePhone = SW.MobileServiceClient.Post(requestApprovePhone);
            Console.WriteLine("Given new Token");
            Console.WriteLine("UID: {0}", resultApprovePhone.UID);
            Console.WriteLine("AccessToken: {0}", resultApprovePhone.Token);
            Console.WriteLine("RefreshToken: {0}", resultApprovePhone.RefreshToken);
            Assert.IsNotNull(resultApprovePhone.UID);
            Assert.IsNotNull(resultApprovePhone.Token);
            Assert.IsNotNull(resultApprovePhone.RefreshToken);
            Assert.IsNotEmpty(resultApprovePhone.UID);
            Assert.IsNotEmpty(resultApprovePhone.Token);
            Assert.IsNotEmpty(resultApprovePhone.RefreshToken);

            //Проверка токена
            SW.MobileServiceClient.BearerToken = resultApprovePhone.Token;
            DoTokenTest(resultApprovePhone.UID);
        }

        public const string RegisterPhoneFunction = "DriverRegisterPhone";

        /// <summary>
        /// Проверка метода подтверждения регистрации номера
        /// </summary>
        /// <returns></returns>
        [Test]
        public void Test04_ApprovePhone()
        {
            //Очистка базы от предыдущих запусков
            var testPhone = SW.GeneratePhone();

            //Получение нового анонимного токена
            var getTokenResult = SW.GetAnonimToken();

            //Отправка запроса на регистрацию номера
            var requestRegisterPhone = new RegisterPhone { Phone = testPhone };
            var resultRegisterPhone = SW.MobileServiceClient.Post(requestRegisterPhone);
            Assert.AreEqual(HttpStatusCode.NoContent, resultRegisterPhone.StatusCode);
            Console.WriteLine("Sended sms to {0}", testPhone);

            //Генерация смс кода и сохрание в базу
            var smsCode = SW.GetLastSms(getTokenResult.UID);
            Console.WriteLine("Generated SMS code is {0}", smsCode);

            //Подтверждение кода
            var requestApprovePhone = new ApprovePhone { Code = smsCode };
            var resultApprovePhone = SW.MobileServiceClient.Post(requestApprovePhone);
            Console.WriteLine("Given new Token");
            Console.WriteLine("UID: {0}", resultApprovePhone.UID);
            Console.WriteLine("AccessToken: {0}", resultApprovePhone.Token);
            Console.WriteLine("RefreshToken: {0}", resultApprovePhone.RefreshToken);
            Assert.IsNotNull(resultApprovePhone.UID);
            Assert.IsNotNull(resultApprovePhone.Token);
            Assert.IsNotNull(resultApprovePhone.RefreshToken);
            Assert.IsNotEmpty(resultApprovePhone.UID);
            Assert.IsNotEmpty(resultApprovePhone.Token);
            Assert.IsNotEmpty(resultApprovePhone.RefreshToken);

            //Проверка токена
            SW.MobileServiceClient.BearerToken = resultApprovePhone.Token;
            DoTokenTest(resultApprovePhone.UID);

            



        }

       /* [Test]
        public void Test04_RefreshToken()
        {
            var testemail = "TEST3@TEST.TEST";
            var tokenResult = (SW.GetNewSupervisorCheatWebToken(testemail)).token;
            SW.AuthServiceClient.BearerToken = tokenResult.RefreshToken;
            var refresh = new PostRefreshToken();
            var newtoken = SW.AuthServiceClient.Post(refresh);
            Console.WriteLine("Refreshing token is successed");
            Console.WriteLine("AccessToken: {0}", newtoken.AccessToken);
            Console.WriteLine("RefreshToken: {0}", newtoken.RefreshToken);
            Assert.AreNotEqual(tokenResult.AccessToken, newtoken.AccessToken);
            Assert.AreNotEqual(tokenResult.RefreshToken, newtoken.RefreshToken);
        }
        */

        [Test]
        public void Test05_PostRefreshToken() //todo сделать для анонимных пользователей и для авторизавнных по номеру телефона
        {
            var resultApprovePhone = SW.ResultApprovePhone();

            SW.MobileServiceClient.BearerToken = resultApprovePhone.RefreshToken;
            var requestRefreshToken = new PostRefreshToken();
            var resultRefreshToken= SW.MobileServiceClient.Post(requestRefreshToken);

            Console.WriteLine("Given new Token");
            Console.WriteLine("UID: {0}", resultRefreshToken.UID);
            Console.WriteLine("AccessToken: {0}", resultRefreshToken.Token);
            Console.WriteLine("RefreshToken: {0}", resultRefreshToken.RefreshToken);
            Assert.IsNotEmpty(resultRefreshToken.UID);
            Assert.IsNotEmpty(resultRefreshToken.Token);
            Assert.IsNotEmpty(resultRefreshToken.RefreshToken);
            Assert.AreNotEqual(resultApprovePhone.Token, resultRefreshToken.Token);
            Assert.AreNotEqual(resultApprovePhone.RefreshToken, resultRefreshToken.RefreshToken);
            Assert.AreEqual(resultRefreshToken.UID, resultApprovePhone.UID);





        }

        public const string DeletePhoneFunction = "DriverDeletePhone";

        /*[Test]
        public void Test06_DeletePhone()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Test07_DeletePhoneApprove()
        {
            throw new NotImplementedException();
        }

        public const string ChangePhoneFunction = "DriverChangePhone";

        [Test]
        public void Test08_ChangePhone()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Test09_ChangePhoneApprove()
        {
            throw new NotImplementedException();
        }*/

        //[Test]
        //public void Test10_MetadataLoadTypes()
        //{
        //    GetAnonimToken();
        //    //SW.MobileServiceClient.Get(new GetLoadTypes)
        //}

        /// <summary>
        /// Проверка валидности токена и соответсвия его с UID
        /// </summary>
        /// <param name="uid"></param>
        private void DoTokenTest(string uid)
        {
            var tokenTestResult = SW.MobileServiceClient.Get(new TokenTest { UID = uid });

            Assert.IsTrue(tokenTestResult.Ok);
            Console.WriteLine("TokenTest Result is Ok: {0}", tokenTestResult.Ok);
        }
    }
}