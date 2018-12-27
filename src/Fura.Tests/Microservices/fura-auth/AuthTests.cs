using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using FuraAuth.ServiceModel;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using NUnit.Framework;

namespace Fura.Tests.Auth
{
    [TestFixture]
    public class AuthTests
    {
        public readonly ServicesWrapper SW;

        public AuthTests()
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
        public void Test01_Real_GetTokenViaMagicLink()
        {
            var before = DateTime.Now;
            var getMagicLink = new CreateMagicLink
            {
                Email = "CONTENT@GOFURA.COM"
            };
            var getMagicLinkResult = SW.AuthServiceClient.Post(getMagicLink);

            Assert.IsNotEmpty(getMagicLinkResult.Email);
            Assert.IsNotEmpty(getMagicLinkResult.SiteCode);
            Console.WriteLine("Sent Magic Link to: {0}", getMagicLinkResult.Email);
            Console.WriteLine("Recieved Site Code: {0}", getMagicLinkResult.SiteCode);
            var approveMagicLink = new ApproveMagicLink();
            using (var client = new ImapClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("imap.yandex.ru", 993, true);

                client.Authenticate("CONTENT@GOFURA.COM", "kvxynytwwfhbdgrt");

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                var html = "";
                var query = SearchQuery.DeliveredAfter(before)
                    .And(SearchQuery.SubjectContains("Fura Magic Link"))
                    .And(SearchQuery.FromContains("GoFura"))
                    .And(SearchQuery.NotDeleted)
                    .And(SearchQuery.NotSeen);
                while (true)
                {
                    inbox.Check();
                    var uniqueIds = inbox.Search(query).ToList();
                    if (uniqueIds.Count > 0)
                    {
                        var last = uniqueIds.Last();
                        var message = inbox.GetMessage(last);
                        html = message.HtmlBody;
                        inbox.MoveTo(last, client.GetFolder(SpecialFolder.Trash));

                        Console.WriteLine("Recieved Magic Link: {0}", html);
                        break;
                    }
                }

                var match = Regex.Match(html, "\\?email=(?<email>[^&]+)&code=(?<code>[^\"]+)\"");
                Assert.IsTrue(match.Success);
                approveMagicLink.Email = HttpUtility.UrlDecode(match.Groups["email"].Value.ToUpper());
                approveMagicLink.EmailCode = HttpUtility.UrlDecode(match.Groups["code"].Value);

                Console.WriteLine("Magic Link Email: {0}", approveMagicLink.Email);
                Console.WriteLine("Magic Link Email Code: {0}", approveMagicLink.EmailCode);
                client.Disconnect(true);
            }

            var approveResponse = SW.AuthServiceClient.Put(approveMagicLink);
            Assert.AreEqual("Ok", approveResponse.Status);
            Console.WriteLine("Email Code is approved");

            var authViaMagicLink = new AuthViaMagicLink
            {
                Email = approveMagicLink.Email,
                SiteCode = getMagicLinkResult.SiteCode
            };

            var tokenResult = SW.AuthServiceClient.Post(authViaMagicLink);
            Assert.IsNotEmpty(tokenResult.AccessToken);
            Assert.IsNotEmpty(tokenResult.RefreshToken);

            Console.WriteLine("Authentification via Magic Link is successed");
            Console.WriteLine("AccessToken: {0}", tokenResult.AccessToken);
            Console.WriteLine("RefreshToken: {0}", tokenResult.RefreshToken);
        }

        [Test]
        public void Test02_AuthViaPassword()
        {
            var superuser = new CreateTestUser
            {
                Email = Guid.NewGuid().ToString().ToUpper(),
                Password = Guid.NewGuid().ToString(),
                RoleName = Fura.Credentials.Role.Expeditor //todo сделать для всех
            };
            SW.CreateWebUser(superuser);
            var authViaPassword = new AuthViaPassword
            {
                Email = superuser.Email,
                Password = superuser.Password
            };
            var newtoken = SW.AuthServiceClient.Post(authViaPassword);
            Console.WriteLine("Auth via Password  is successed");
            Console.WriteLine("AccessToken: {0}", newtoken.AccessToken);
            Console.WriteLine("RefreshToken: {0}", newtoken.RefreshToken);
        }

        [Test]
        public void Test03_GetCheatTokenMultiSessions()
        {
            var testemail = "TEST2@TEST.TEST";
            var result = SW.GetNewSupervisorCheatWebToken(testemail);
            SW.GetWebToken(result.sid);
            SW.GetWebToken(result.sid);
        }

        [Test]
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
    }
}