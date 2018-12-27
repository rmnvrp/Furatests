using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Internal;
using NUnit.Framework;

namespace Fura.Tests.Microservices.fura_auth
{
    public class AclTests
    {
        public readonly ServicesWrapper SW;

        public AclTests()
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
        public void postaclroles()
        {// методы доступен только для внутреннего пользователя, поэтому используется _privateauth
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.superadmin);// получили токен супеадмина
            SW.AuthServiceClient.BearerToken = webToken.token.AccessToken;

            var me_request = new FuraAuth.ServiceModel.GetMyProfile();
            var me_result = SW.AuthServiceClient.Get(me_request);

            //var post_new_role_request = new FuraAuth.ServiceModel.CreateRole(); параметры можно задать потом
            var post_new_role_request = new FuraAuth.ServiceModel.CreateRole{RoleName = "Testsuperadmin" , Description = "Тестовый пользователь", GrantedPermissions = new string[]{""}};
            var post_new_role_result = SW.AuthServiceClient.Post(post_new_role_request);       // to do  падает потому что не удаляются роли, а архивируются\
            var get_List_of_permisiion_request = new FuraAuth.ServiceModel.GetPermissions();
            var get_List_of_permisiion_result = SW.AuthServiceClient.Get(get_List_of_permisiion_request);
            //var get_list_of_users_reguest = new FuraAuth.ServiceModel.GetUsers();
            //var get_list_of_users_result = SW._privateAuthServiceClient.Post(get_list_of_users_reguest);
            Assert.IsTrue(get_List_of_permisiion_result.Roles.Any(model => model.Name== post_new_role_request.RoleName));

            var archive_role_request = new FuraAuth.ServiceModel.ArchiveRole { RoleName = post_new_role_request.RoleName };
            var delete_archive_role_result = SW.AuthServiceClient.Delete(archive_role_request);


        }
    
        
    }
}
