using System;
using System.Net;
using System.Threading.Tasks;
using FuraBack.ServiceModel;
using NUnit.Framework;

namespace Fura.Tests
{
    public partial class ServicesWrapper
    {
        private void SendTelematic(PostTelematic postTelematic)
        {
            var telematicResponse = MobileServiceClient.Post<HttpWebResponse>(postTelematic);
            Assert.AreEqual(HttpStatusCode.NoContent, telematicResponse.StatusCode);
            Console.WriteLine($"Sent Telematic: lat={postTelematic.Latitude}, lon={postTelematic.Longitude}");
        }

        private void SendDriverStatus(PostDriverStatus status)
        {
            var postDriverStatusResponse = MobileServiceClient.Post<HttpWebResponse>(status);
            Assert.AreEqual(HttpStatusCode.NoContent, postDriverStatusResponse.StatusCode);
            Console.WriteLine(
                $"Sent Driver Status: status={status.Status}, lat={status.Latitude}, lon={status.Longitude}");
        }

        public TokenResult GetCheatMobileToken(string testphone)
        {
            var user = _privateMobileServiceClient.Post(new CreateTestUser { Phone = testphone });
            Console.WriteLine("Created registered user with UID: {0}", user.Uid);
            Console.WriteLine("Phone: {0}", testphone);
            var getTokenResult = _privateMobileServiceClient.Get(new GetTestToken { Uid = user.Uid });

            //Прописывание AccessToken в MobileServiceClient.BearerToken
            MobileServiceClient.BearerToken = getTokenResult.Token;
            Console.WriteLine("Given new Token");
            Console.WriteLine("AccessToken: {0}", getTokenResult.Token);

            return getTokenResult;
        }

        /// <summary>
        /// Получение нового анонимного токена и прописывание его в MobileServiceClient.BearerToken
        /// </summary>
        /// <returns></returns>
        public TokenResult GetAnonimToken()
        {
            var user = _privateMobileServiceClient.Get(new CreateAnonimTestUser());
            Console.WriteLine("Created anonim user with UID: {0}", user.Uid);

            var getTokenResult = _privateMobileServiceClient.Get(new GetTestToken { Uid = user.Uid });

            //Прописывание AccessToken в MobileServiceClient.BearerToken
            MobileServiceClient.BearerToken = getTokenResult.Token;
            Console.WriteLine("Given new Token");
            Console.WriteLine("AccessToken: {0}", getTokenResult.Token);

            return getTokenResult;
        }

        public string GetLastSms(string uid)
        {
            var smsCode = _privateMobileServiceClient.Get(new GetLastSms { Uid = uid });
            return smsCode.Code;
        }

        public TokenResult ResultApprovePhone()
        {
            var testPhone = this.GeneratePhone();
            var getTokenResult = this.GetAnonimToken();
            var requestRegisterPhone = new RegisterPhone {Phone = testPhone};
            var resultRegisterPhone = this.MobileServiceClient.Post(requestRegisterPhone);
            Assert.AreEqual(HttpStatusCode.NoContent, resultRegisterPhone.StatusCode);
            Console.WriteLine("Sended sms to {0}", testPhone);

            //Генерация смс кода и сохрание в базу
            var smsCode = this.GetLastSms(getTokenResult.UID);
            Console.WriteLine("Generated SMS code is {0}", smsCode);
            //Подтверждение кода
            var requestApprovePhone = new ApprovePhone {Code = smsCode};
            var resultApprovePhone = this.MobileServiceClient.Post(requestApprovePhone);
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
            return resultApprovePhone;
        }
    }
}