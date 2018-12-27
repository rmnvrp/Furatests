using System;
using System.Collections.Generic;
using System.Net;
using Fura.Credentials;
using FuraAuth.ServiceModel;
using TokenResult = FuraAuth.ServiceModel.TokenResult;

namespace Fura.Tests
{
    public partial class ServicesWrapper
    {
        /// <summary>
        /// Создание сотрудника
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public (TokenResult token, string sid) GetNewSupervisorCheatWebToken(string email, string cid = null)
        {
            var userResult = CreateNewSupervisor(email, Role.Cooperator, cid);

            var role = new SetTestUserRole
            {
                UserId = userResult.UserId,
                RoleName = Role.Cooperator
            };
            _privateAuthServiceClient.Put<HttpWebResponse>(role);
            Console.WriteLine($"New Superadmin created uid:{userResult.UserId}");

            var tokenresult = GetWebToken(userResult.UserId);

            SetTokenForWebServices(tokenresult.AccessToken);

            return (tokenresult, userResult.UserId);
        }

        /// <summary>
        /// Создание тестового Web пользователя
        /// </summary>
        /// <param name="supervisor"></param>
        /// <returns></returns>
        public CreateUserResult CreateWebUser(CreateTestUser supervisor)
        {
            return _privateAuthServiceClient.Post(supervisor);
        }

        private CreateUserResult CreateNewSupervisor(string email, string roleName, string cid)
        {
            var super = new CreateTestUser
            {
                Email = email,
                RoleName = roleName
            };
            var userResult = CreateWebUser(super);

            if (cid != null)
            {
                var put = new SetTestUserCompany
                {
                    UserId = userResult.UserId,
                    CompanyId = cid
                };
                _privateAuthServiceClient.Put<HttpWebResponse>(put);
            }

            return userResult;
        }

        /// <summary>
        /// Получение пары токенов для Web-пользователя
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public TokenResult GetWebToken(string uid)
        {
            return _privateAuthServiceClient.Get(new GetTestToken() { UserId = uid });
        }

        /// <summary>
        /// Установка токена доступа для Web-сервисов
        /// </summary>
        /// <param name="token"></param>
        public void SetTokenForWebServices(string token)
        {
            WebServiceClient.BearerToken = token;
            AuthServiceClient.BearerToken = token;
        }

        /// <summary>
        /// Cоздание суперадмина
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role1"></param>
        /// <returns></returns>
        public (TokenResult token, string sid) GetNewSupervisorsCheatWebToken(string email, webroles role1 = webroles.superadmin)
        {
            var rolename = "";
            switch (role1)
            {
                case webroles.coopertor:
                    rolename = Role.Cooperator;
                    break;
                case webroles.experditor:
                    rolename = Role.Expeditor;
                    break;
                case webroles.superadmin:
                    rolename = Role.Superadmin;
                    break;
            }
            var userResult = CreateNewSupervisor(email, rolename, null);
            var role = new SetTestUserRole
            {
                UserId = userResult.UserId,
                RoleName = rolename
            };
            _privateAuthServiceClient.Put<HttpWebResponse>(role);
            Console.WriteLine($"New Superadmin created uid:{userResult.UserId}");

            var tokenresult = GetWebToken(userResult.UserId);

            SetTokenForWebServices(tokenresult.AccessToken);

            return (tokenresult, userResult.UserId);
        }

        public enum webroles
        {
            coopertor,
            experditor,
            superadmin
            
        }

        /// <summary>
        /// Создание новой компании
        /// </summary>
        public string CreateCompany()
        {
            var createTestCompany = new CreateTestCompany
            {
                CompanyName = Guid.NewGuid().ToString()
            };
            var company = _privateAuthServiceClient.Post(createTestCompany);
            return company.CompanyId;
        }
    }
}