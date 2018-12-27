using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FuraBack.ServiceModel;
using FuraWebBack.ServiceModel;
using NUnit.Framework;
using DriverStatusEnumModel = FuraBack.ServiceModel.DriverStatusEnumModel;

namespace Fura.Tests
{
    public partial class ServicesWrapper
    {
        public TransportationModel GetTransportation(string testphone,
            FuraBack.ServiceModel.TokenResult mobileToken,
            (FuraAuth.ServiceModel.TokenResult token, string sid) webToken, string cid)
        {
            string uid = mobileToken.UID;
            string sid = webToken.sid;

            NewCheatApprovedSupervisingRequest(testphone, webToken.token.AccessToken);

            var random = new Random();
            var orderWaitingStatus = new PostDriverStatus
            {
                Latitude = random.NextDouble() * 90,
                Longitude = random.NextDouble() * 90,
                Status = DriverStatusEnumModel.OrderWaiting
            };
            MobileServiceClient.BearerToken = mobileToken.Token;
            var postDriverStatusResponse = MobileServiceClient.Post(orderWaitingStatus);
            Assert.AreEqual(HttpStatusCode.NoContent, postDriverStatusResponse.StatusCode);

            var goingToTheLoadingZone = new PostDriverStatus
            {
                SupervisorId = sid,
                Latitude = random.NextDouble() * 90,
                Longitude = random.NextDouble() * 90,
                Status = DriverStatusEnumModel.GoingToTheLoadingZone
            };
            var postDriverStatusResponse2 = MobileServiceClient.Post(goingToTheLoadingZone);
            Assert.AreEqual(HttpStatusCode.NoContent, postDriverStatusResponse2.StatusCode);

            var postTelematic = new PostTelematic
            {
                Latitude = random.NextDouble() * 90,
                Longitude = random.NextDouble() * 90
            };
            var telematicResponse = MobileServiceClient.Post(postTelematic);
            Assert.AreEqual(HttpStatusCode.NoContent, telematicResponse.StatusCode);

            var getSupervisingTransportations = new GetSupervisingTransportations();
            WebServiceClient.BearerToken = webToken.token.AccessToken;
            var transportations = WebServiceClient.Get(getSupervisingTransportations);
            Assert.IsNotNull(transportations);
            Assert.AreEqual(1, transportations.Count);
            var trans = transportations.First();
            Assert.AreEqual(uid, trans.Uid);
            return trans;
        }

        public TransportationModel GetLongTransportation(string testphone,
            FuraBack.ServiceModel.TokenResult mobileToken,
            (FuraAuth.ServiceModel.TokenResult token, string sid) webToken, string cid)
        {
            string uid = mobileToken.UID;
            string sid = webToken.sid;

            NewCheatApprovedSupervisingRequest(testphone, webToken.token.AccessToken);

            MobileServiceClient.BearerToken = mobileToken.Token;
            var random = new Random();
            var driverStatus = new PostDriverStatus
            {
                Latitude = random.NextDouble() * 90,
                Longitude = random.NextDouble() * 90,
                Status = DriverStatusEnumModel.OrderWaiting
            };
            ShiftCoordinates(driverStatus);
            SendDriverStatus(driverStatus);

            driverStatus.SupervisorId = sid;
            driverStatus.CompanyId = cid;
            driverStatus.Status = DriverStatusEnumModel.GoingToTheLoadingZone;
            ShiftCoordinates(driverStatus);
            SendDriverStatus(driverStatus);
            driverStatus.SupervisorId = null;
            driverStatus.CompanyId = null;

            var statuses = new[]
            {
                DriverStatusEnumModel.LoadStartWaiting,
                DriverStatusEnumModel.Loading,
                DriverStatusEnumModel.GoingToTheUnloadingZone,
                DriverStatusEnumModel.Rest,
                DriverStatusEnumModel.GoingToTheUnloadingZone,
                DriverStatusEnumModel.UnloadStartWaiting,
                DriverStatusEnumModel.Alarm,
                DriverStatusEnumModel.UnloadStartWaiting,
                DriverStatusEnumModel.Unloading,
                DriverStatusEnumModel.Vacationing,
            };

            var telematic = new PostTelematic();

            foreach (var driverStatusEnumModel in statuses)
            {
                telematic.Longitude = driverStatus.Longitude;
                telematic.Latitude = driverStatus.Latitude;
                ShiftCoordinates(telematic);
                SendTelematic(telematic);

                driverStatus.Longitude = telematic.Longitude;
                driverStatus.Latitude = telematic.Latitude;
                driverStatus.Status = driverStatusEnumModel;
                ShiftCoordinates(driverStatus);
                SendDriverStatus(driverStatus);
            }

            var getSupervisingTransportations = new GetSupervisingTransportations();
            WebServiceClient.BearerToken = webToken.token.AccessToken;
            var transportations = WebServiceClient.Get(getSupervisingTransportations);
            Assert.IsNotNull(transportations);
            Assert.AreEqual(1, transportations.Count);
            var trans = transportations.First();
            Assert.AreEqual(uid, trans.Uid);
            return trans;
        }

        private double GetCoordinateChange()
        {
            var random = new Random();
            return (random.NextDouble() - 0.5d) * 0.00001;
        }

        private void ShiftCoordinates(PostTelematic postTelematic)
        {
            postTelematic.Latitude += GetCoordinateChange();
            postTelematic.Longitude += GetCoordinateChange();
        }

        private void ShiftCoordinates(PostDriverStatus status)
        {
            status.Latitude += GetCoordinateChange();
            status.Longitude += GetCoordinateChange();
        }

        public FuraWebBack.ServiceModel.SupervisingRequestModel NewCheatApprovedSupervisingRequest(
            string testphone, string accessToken)
        {
            WebServiceClient.BearerToken = accessToken;
            var requests = WebServiceClient.Post(new PostSupervisingRequests
            {
                Phones = new Dictionary<string, string> {{testphone, $"test_driver_{testphone}"}}
            });
            var request = requests.First();
            _privateWebServiceClient.Put<HttpWebResponse>(new ChangeStatusSupervisingRequest
            {
                Rid = request.Rid,
                Accepted = true
            });

            Console.WriteLine("Supervising Request is Approved: {0}", request.Rid);
            return request;
        }
    }
}