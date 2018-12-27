using System;
using System.Collections.Generic;
using System.Linq;
using FuraErp.ServiceModel;
using FuraErp.ServiceModel.Molds;
using NUnit.Framework;
using ServiceStack;

namespace Fura.Tests.Microservices.fura_erp
{
    class ChartererTests
    {
        public readonly ServicesWrapper SW;

        public ChartererTests()
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

        /// <summary>
        /// Основные операции с заказчиком
        /// </summary>
        [Test]
        public void PostChartererMain()
        {
            //Авторизация
            var email = SW.GenerateEmail();
            var (token, _) = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = token.AccessToken;

            var createChartererMold = SW.CreateNewCharterer();
        }


        [Test]
        public void GetChartererMain()
        {
            //Авторизация
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = webToken.token.AccessToken;
            var createChartererMold = SW.CreateNewCharterer();

            //Получение
            var getCharterer = new FuraErp.ServiceModel.GetCharterer
            {
                Id = createChartererMold.Id
            };
            var getChartererMold = SW.ErpServiceClient.Get(getCharterer);
            Assert.AreEqual(createChartererMold.Id, getChartererMold.Id);
            Assert.AreEqual(createChartererMold.Title, getChartererMold.Title);
            Assert.AreEqual(createChartererMold.WebSite, getChartererMold.WebSite);
            Assert.AreEqual(createChartererMold.FactAddress, getChartererMold.FactAddress);
            Assert.AreEqual(createChartererMold.PostAddress, getChartererMold.PostAddress);
        }


        [Test]
        public void PutChartererMain()
        {
            //Авторизация
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = webToken.token.AccessToken;
            var createChartererMold = SW.CreateNewCharterer();


            //Изменение
            var changeCharterer = new FuraErp.ServiceModel.ChangeCharterer
            {
                Id = createChartererMold.Id,
                Title = "_Тестовый заказчик",
                WebSite = "_www.test.com",
                FactAddress = "_Москва, Красная площадь",
                PostAddress = "_Моска, Китай-город"
            };
            var changeChartererMold = SW.ErpServiceClient.Put(changeCharterer);
            Assert.AreEqual(changeChartererMold.Id, changeCharterer.Id);
            Assert.AreEqual(changeChartererMold.Title, changeCharterer.Title);
            Assert.AreEqual(changeChartererMold.WebSite, changeCharterer.WebSite);
            Assert.AreEqual(changeChartererMold.FactAddress, changeCharterer.FactAddress);
            Assert.AreEqual(changeChartererMold.PostAddress, changeCharterer.PostAddress);

            //Получение всех
            var createCharterer2 = new FuraErp.ServiceModel.CreateCharterer
            {
                Title = "Тестовый заказчик2",
                WebSite = "www.test.com2",
                FactAddress = "Питер, Фонтанка",
                PostAddress = "Питер, Невский"
            };
            var createChartererMold2 = SW.ErpServiceClient.Post(createCharterer2);  //to do SW.ErpServiceClient.Post ( метод по фрапеере надо сделать так, чтобы ему можно было на вход что-то подать)

            var getCharterers = new GetCharterers();
            var getChartererMolds = SW.ErpServiceClient.Get(getCharterers);
            Assert.NotNull(getChartererMolds.Items.Find(m => m.Id == createChartererMold.Id));
            Assert.NotNull(getChartererMolds.Items.Find(m => m.Id == createChartererMold2.Id));
        }


        [Test]
            public void DeleteChartererMain()
            {
            //Авторизация
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = webToken.token.AccessToken;
            var createChartererMold = SW.CreateNewCharterer();

            //Архивирование
            var archiveCharterer = new FuraErp.ServiceModel.ArchiveCharterer
            {
                Id = createChartererMold.Id
            };
            var archiveChartererMold = SW.ErpServiceClient.Delete(archiveCharterer);
            Assert.AreEqual(archiveCharterer.Id, archiveChartererMold.Id);
            Assert.AreEqual(ArchiveResult.ArchiveStatus.Archived, archiveChartererMold.Status);
            try
            {
                var archiveChartererMold2 = SW.ErpServiceClient.Delete(archiveCharterer);
                Assert.IsTrue(false, "Должно возникунть исключение");
            }
            catch (WebServiceException e)
            {
                Assert.AreEqual(404, e.StatusCode);
            }
        }

        /// <summary>
        /// Основные операции с юридическим лицом
        /// </summary>
        [Test]
        public void JuridicalPersonMain()
        {
            //Авторизация
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = webToken.token.AccessToken;

            //Создание
            var createChartererMold = SW.CreateNewCharterer();

            var createJuridicalPersonMold = SW.CreateNewJuridicalPerson(createChartererMold.Id);

            //Получение
            var getJuridicalPerson = new FuraErp.ServiceModel.GetJuridicalPerson
            {
                Id = createJuridicalPersonMold.Id
            };
            var getJuridicalPersonMold = SW.ErpServiceClient.Get(getJuridicalPerson);
            Assert.AreEqual(createJuridicalPersonMold.Id, getJuridicalPersonMold.Id);
            Assert.AreEqual(createJuridicalPersonMold.Title, getJuridicalPersonMold.Title);
            Assert.AreEqual(createJuridicalPersonMold.FactAddress, getJuridicalPersonMold.FactAddress);
            Assert.AreEqual(createJuridicalPersonMold.PostAddress, getJuridicalPersonMold.PostAddress);
            Assert.AreEqual(createJuridicalPersonMold.JuridicalAddress, getJuridicalPersonMold.JuridicalAddress);
            Assert.AreEqual(createJuridicalPersonMold.JuridicalType, getJuridicalPersonMold.JuridicalType);
            Assert.AreEqual(createJuridicalPersonMold.Inn, getJuridicalPersonMold.Inn);
            Assert.AreEqual(createJuridicalPersonMold.Kpp, getJuridicalPersonMold.Kpp);
            Assert.AreEqual(createJuridicalPersonMold.Ogrn, getJuridicalPersonMold.Ogrn);
            Assert.AreEqual(createJuridicalPersonMold.RegistrationCountry, getJuridicalPersonMold.RegistrationCountry);
            Assert.AreEqual(createJuridicalPersonMold.ShortTitle, getJuridicalPersonMold.ShortTitle);

            //Изменение
            var changeJuridicalPerson = new FuraErp.ServiceModel.ChangeJuridicalPerson
            {
                Id = createJuridicalPersonMold.Id,
                Title = "_Тестовый юрик",
                FactAddress = "_Москва, Красная площадь",
                PostAddress = "_Моска, Китай-город",
                JuridicalAddress = "_Москва, Красная площадь, 2",
                JuridicalType = "_ООО",
                Inn = "_123456789",
                Kpp = "_0998772",
                Ogrn = "_4574359834758934",
                RegistrationCountry = "_Россия",
                ShortTitle = "_Тестировщик",
            };
            var changeJuridicalPersonMold = SW.ErpServiceClient.Put(changeJuridicalPerson);
            Assert.AreEqual(changeJuridicalPersonMold.Id, changeJuridicalPerson.Id);
            Assert.AreEqual(changeJuridicalPersonMold.Title, changeJuridicalPerson.Title);
            Assert.AreEqual(changeJuridicalPersonMold.FactAddress, changeJuridicalPerson.FactAddress);
            Assert.AreEqual(changeJuridicalPersonMold.PostAddress, changeJuridicalPerson.PostAddress);
            Assert.AreEqual(changeJuridicalPersonMold.JuridicalAddress, changeJuridicalPerson.JuridicalAddress);
            Assert.AreEqual(changeJuridicalPersonMold.JuridicalType, changeJuridicalPerson.JuridicalType);
            Assert.AreEqual(changeJuridicalPersonMold.Inn, changeJuridicalPerson.Inn);
            Assert.AreEqual(changeJuridicalPersonMold.Kpp, changeJuridicalPerson.Kpp);
            Assert.AreEqual(changeJuridicalPersonMold.Ogrn, changeJuridicalPerson.Ogrn);
            Assert.AreEqual(changeJuridicalPersonMold.RegistrationCountry, changeJuridicalPerson.RegistrationCountry);
            Assert.AreEqual(changeJuridicalPersonMold.ShortTitle, changeJuridicalPerson.ShortTitle);

            //Получение всех
            var createJuridicalPerson2 = new FuraErp.ServiceModel.CreateJuridicalPerson
            {
                Title = "Тестовый юрик2",
                FactAddress = "Москва, Красная площадь2",
                PostAddress = "Моска, Китай-город2",
                JuridicalAddress = "Москва, Красная площадь, 22",
                JuridicalType = "ООО2",
                Inn = "1234567892",
                Kpp = "09987722",
                Ogrn = "45743598347589342",
                RegistrationCountry = "Россия2",
                ShortTitle = "Тестировщик2",
            };
            var createJuridicalPersonMold2 = SW.ErpServiceClient.Post(createJuridicalPerson2);

            var getJuridicalPersons = new FuraErp.ServiceModel.GetJuridicalPersons();
            var getJuridicalPersonMolds = SW.ErpServiceClient.Get(getJuridicalPersons);
            Assert.NotNull(getJuridicalPersonMolds.Items.Find(m => m.Id == createJuridicalPersonMold.Id));
            Assert.NotNull(getJuridicalPersonMolds.Items.Find(m => m.Id == createJuridicalPersonMold2.Id));

            //Архивирование
            var archiveJuridicalPerson = new FuraErp.ServiceModel.ArchiveJuridicalPerson
            {
                Id = createJuridicalPersonMold.Id
            };
            var archiveJuridicalPersonMold = SW.ErpServiceClient.Delete(archiveJuridicalPerson);
            Assert.AreEqual(archiveJuridicalPerson.Id, archiveJuridicalPersonMold.Id);
            Assert.AreEqual(ArchiveResult.ArchiveStatus.Archived, archiveJuridicalPersonMold.Status);

            //Попытка повторного удаления
            try
            {
                var archiveJuridicalPersonMold2 = SW.ErpServiceClient.Delete(archiveJuridicalPerson);
                Assert.IsTrue(false, "Должно возникунть исключение");
            }
            catch (WebServiceException e)
            {
                Assert.AreEqual(404, e.StatusCode);
            }
        }

        [Test]
        public void JuridicalPersonAndChartererRelation()
        {
            //Авторизация
            var email = SW.GenerateEmail();
            var (token, _) = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = token.AccessToken;

            //Создание заказчика
            var createChartererMold = SW.CreateNewCharterer();

            //Создание юрика
            var createJuridicalPersonMold = SW.CreateNewJuridicalPerson(createChartererMold.Id);

            //Получение заказчика
            var getCharterer = new FuraErp.ServiceModel.GetCharterer
            {
                Id = createChartererMold.Id
            };
            var getChartererMold = SW.ErpServiceClient.Get(getCharterer);
            Assert.AreEqual(createChartererMold.Id, getChartererMold.Id);
            Assert.AreEqual(createChartererMold.Title, getChartererMold.Title);
            Assert.AreEqual(createChartererMold.WebSite, getChartererMold.WebSite);
            Assert.AreEqual(createChartererMold.FactAddress, getChartererMold.FactAddress);
            Assert.AreEqual(createChartererMold.PostAddress, getChartererMold.PostAddress);

            Assert.AreEqual(1, getChartererMold.JuridicalIds.Count);
            Assert.AreEqual(createJuridicalPersonMold.Id, getChartererMold.JuridicalIds.First());

            //Устанавка юрика по умолчанию
            var setChartererDefaultJuridicalPerson = new FuraErp.ServiceModel.SetChartererDefaultJuridicalPerson
            {
                ChartererId = createChartererMold.Id,
                JuridicalPersonId = createJuridicalPersonMold.Id
            };
            var setChartererDefaultJuridicalPersonMold = SW.ErpServiceClient.Put(setChartererDefaultJuridicalPerson);

            Assert.AreEqual(createJuridicalPersonMold.Id, setChartererDefaultJuridicalPersonMold.Id);
            Assert.AreEqual(SetDefaultResult.SetDefaultStatus.AlreadySetted, setChartererDefaultJuridicalPersonMold.Status);

            //Создание юрика 2
            var createJuridicalPerson2 = new FuraErp.ServiceModel.CreateJuridicalPerson
            {
                OwnerType = OwnerTypeEnum.Charterer,
                OwnerId = createChartererMold.Id,
                Title = "Тестовый юрик2",
                FactAddress = "Москва, Красная площадь2",
                PostAddress = "Моска, Китай-город2",
                JuridicalAddress = "Москва, Красная площадь, 22",
                JuridicalType = "ООО2",
                Inn = "1234567892",
                Kpp = "09987722",
                Ogrn = "45743598347589342",
                RegistrationCountry = "Россия2",
                ShortTitle = "Тестировщик2",
            };
            var createJuridicalPersonMold2 = SW.ErpServiceClient.Post(createJuridicalPerson2);

            //Получение заказчика с двумя юриками
            var getChartererMold2 = SW.ErpServiceClient.Get(getCharterer);

            Assert.AreEqual(2, getChartererMold2.JuridicalIds.Count);
            Assert.AreEqual(createJuridicalPersonMold.Id, getChartererMold2.JuridicalIds.First());
            Assert.AreEqual(createJuridicalPersonMold2.Id, getChartererMold2.JuridicalIds.Skip(1).First());

            //Установка второго юрика по умолчанию
            var setChartererDefaultJuridicalPerson2 = new FuraErp.ServiceModel.SetChartererDefaultJuridicalPerson
            {
                ChartererId = createChartererMold.Id,
                JuridicalPersonId = createJuridicalPersonMold2.Id
            };
            var setChartererDefaultJuridicalPersonMold2 = SW.ErpServiceClient.Put(setChartererDefaultJuridicalPerson2);

            Assert.AreEqual(createJuridicalPersonMold2.Id, setChartererDefaultJuridicalPersonMold2.Id);
            Assert.AreEqual(SetDefaultResult.SetDefaultStatus.Setted, setChartererDefaultJuridicalPersonMold2.Status);

            //Проверка порядка юриков
            var getChartererMold3 = SW.ErpServiceClient.Get(getCharterer);

            Assert.AreEqual(2, getChartererMold3.JuridicalIds.Count);
            Assert.AreEqual(createJuridicalPersonMold2.Id, getChartererMold3.JuridicalIds.First());
            Assert.AreEqual(createJuridicalPersonMold.Id, getChartererMold3.JuridicalIds.Skip(1).First());

            //Удаление второго юрика
            var deleteJuridicalPersonFromCharterer = new FuraErp.ServiceModel.DeleteJuridicalPersonFromCharterer
            {
                ChartererId = createChartererMold.Id,
                JuridicalPersonId = createJuridicalPersonMold2.Id
            };
            var deleteJuridicalPersonFromChartererMold = SW.ErpServiceClient.Delete(deleteJuridicalPersonFromCharterer);

            Assert.AreEqual(createJuridicalPersonMold2.Id, deleteJuridicalPersonFromChartererMold.Id);
            Assert.AreEqual(ArchiveResult.ArchiveStatus.Archived, deleteJuridicalPersonFromChartererMold.Status);

            //Попытка повторного удаления
            try
            {
                var deleteJuridicalPersonFromChartererMold2 = SW.ErpServiceClient.Delete(deleteJuridicalPersonFromCharterer);
                Assert.IsTrue(false, "Должно возникунть исключение");
            }
            catch (WebServiceException e)
            {
                Assert.AreEqual(404, e.StatusCode);
            }

            //Проверка заказчика, что 1 юрик остался
            var getChartererMold4 = SW.ErpServiceClient.Get(getCharterer);

            Assert.AreEqual(1, getChartererMold4.JuridicalIds.Count);
            Assert.AreEqual(createJuridicalPersonMold.Id, getChartererMold4.JuridicalIds.First());
        }

        [Test]
        public void OperationAccountAndJuridicalPersonRelation()
        {
            //Авторизация
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = webToken.token.AccessToken;

            //Создание заказчика
            var createChartererMold = SW.CreateNewCharterer();

            //Создание юрика
            var createJuridicalPersonMold = SW.CreateNewJuridicalPerson(createChartererMold.Id);

            //Создание расчетного счёта юрика
            var createOperationAccountForJuridicalPerson = new FuraErp.ServiceModel.CreateOperationAccountForJuridicalPerson
            {
                JuridicalPersonId = createJuridicalPersonMold.Id,
                BankTitle = "Банк",
                Bic = "324523",
                CorrespondentAccount = "464352",
                PaymentAccount = "262541562526"
            };
            var createOperationAccountForJuridicalPersonMold = SW.ErpServiceClient.Post(createOperationAccountForJuridicalPerson);

            Assert.IsNotEmpty(createOperationAccountForJuridicalPersonMold.Id);
            Assert.AreEqual(createOperationAccountForJuridicalPerson.BankTitle, createOperationAccountForJuridicalPersonMold.BankTitle);
            Assert.AreEqual(createOperationAccountForJuridicalPerson.Bic, createOperationAccountForJuridicalPersonMold.Bic);
            Assert.AreEqual(createOperationAccountForJuridicalPerson.CorrespondentAccount, createOperationAccountForJuridicalPersonMold.CorrespondentAccount);
            Assert.AreEqual(createOperationAccountForJuridicalPerson.PaymentAccount, createOperationAccountForJuridicalPersonMold.PaymentAccount);

            //Получение юрика
            var getJuridicalPerson = new FuraErp.ServiceModel.GetJuridicalPerson
            {
                Id = createJuridicalPersonMold.Id
            };
            var getJuridicalPersonMold = SW.ErpServiceClient.Get(getJuridicalPerson);
            Assert.AreEqual(getJuridicalPerson.Id, getJuridicalPersonMold.Id);

            Assert.AreEqual(1, getJuridicalPersonMold.OperationAccounts.Count);
            Assert.AreEqual(createOperationAccountForJuridicalPersonMold.Id, getJuridicalPersonMold.OperationAccounts.First().Id);

            //Устанавка счёта по умолчанию
            var juridicalPersonDefaultOperationAccount = new FuraErp.ServiceModel.SetJuridicalPersonDefaultOperationAccount
            {
                OperationAccountId = createOperationAccountForJuridicalPersonMold.Id,
                JuridicalPersonId = createJuridicalPersonMold.Id
            };
            var juridicalPersonDefaultOperationAccountMold = SW.ErpServiceClient.Put(juridicalPersonDefaultOperationAccount);

            Assert.AreEqual(createOperationAccountForJuridicalPersonMold.Id, juridicalPersonDefaultOperationAccountMold.Id);
            Assert.AreEqual(SetDefaultResult.SetDefaultStatus.AlreadySetted, juridicalPersonDefaultOperationAccountMold.Status);

            //Создание расчетного счёта 2 юрика
            var createOperationAccountForJuridicalPerson2 = new FuraErp.ServiceModel.CreateOperationAccountForJuridicalPerson
            {
                JuridicalPersonId = createJuridicalPersonMold.Id,
                BankTitle = "Банк2",
                Bic = "3245232",
                CorrespondentAccount = "4643522",
                PaymentAccount = "2625415625262"
            };
            var createOperationAccountForJuridicalPersonMold2 = SW.ErpServiceClient.Post(createOperationAccountForJuridicalPerson2);

            //Получение юрика с двумя счетами
            var getJuridicalPersonMold2 = SW.ErpServiceClient.Get(getJuridicalPerson);
            Assert.AreEqual(getJuridicalPerson.Id, getJuridicalPersonMold2.Id);

            Assert.AreEqual(2, getJuridicalPersonMold2.OperationAccounts.Count);
            Assert.AreEqual(createOperationAccountForJuridicalPersonMold.Id, getJuridicalPersonMold2.OperationAccounts.First().Id);
            Assert.AreEqual(createOperationAccountForJuridicalPersonMold2.Id, getJuridicalPersonMold2.OperationAccounts.Skip(1).First().Id);

            //Установка второго счёта по умолчанию
            var setJuridicalPersonDefaultOperationAccount2 = new FuraErp.ServiceModel.SetJuridicalPersonDefaultOperationAccount
            {
                OperationAccountId = createOperationAccountForJuridicalPersonMold2.Id,
                JuridicalPersonId = createJuridicalPersonMold.Id
            };
            var setJuridicalPersonDefaultOperationAccountMold2 = SW.ErpServiceClient.Put(setJuridicalPersonDefaultOperationAccount2);

            Assert.AreEqual(createOperationAccountForJuridicalPersonMold2.Id, setJuridicalPersonDefaultOperationAccountMold2.Id);
            Assert.AreEqual(SetDefaultResult.SetDefaultStatus.Setted, setJuridicalPersonDefaultOperationAccountMold2.Status);

            //Проверка порядка юриков
            var getJuridicalPersonMold3 = SW.ErpServiceClient.Get(getJuridicalPerson);

            Assert.AreEqual(2, getJuridicalPersonMold3.OperationAccounts.Count);
            Assert.AreEqual(createOperationAccountForJuridicalPersonMold2.Id, getJuridicalPersonMold3.OperationAccounts.First().Id);
            Assert.AreEqual(createOperationAccountForJuridicalPersonMold.Id, getJuridicalPersonMold3.OperationAccounts.Skip(1).First().Id);

            //Удаление второго юрика
            var deleteOperationAccountFromJuridicalPerson = new FuraErp.ServiceModel.DeleteOperationAccountFromJuridicalPerson
            {
                OperationAccountId = createOperationAccountForJuridicalPersonMold.Id,
                JuridicalPersonId = createJuridicalPersonMold.Id
            };
            var deleteOperationAccountFromJuridicalPersonMold = SW.ErpServiceClient.Delete(deleteOperationAccountFromJuridicalPerson);

            Assert.AreEqual(createOperationAccountForJuridicalPersonMold.Id, deleteOperationAccountFromJuridicalPersonMold.Id);
            Assert.AreEqual(ArchiveResult.ArchiveStatus.Archived, deleteOperationAccountFromJuridicalPersonMold.Status);

            //Попытка повторного удаления
            try
            {
                var deleteOperationAccountFromJuridicalPerson2 = SW.ErpServiceClient.Delete(deleteOperationAccountFromJuridicalPerson);
                Assert.IsTrue(false, "Должно возникунть исключение");
            }
            catch (WebServiceException e)
            {
                Assert.AreEqual(404, e.StatusCode);
            }

            //Получение юрика с 1 счётом
            var getJuridicalPersonMold4 = SW.ErpServiceClient.Get(getJuridicalPerson);
            Assert.AreEqual(getJuridicalPerson.Id, getJuridicalPersonMold4.Id);

            Assert.AreEqual(1, getJuridicalPersonMold4.OperationAccounts.Count);
            Assert.AreEqual(createOperationAccountForJuridicalPersonMold2.Id, getJuridicalPersonMold4.OperationAccounts.First().Id);
        }

        [Test]
        public void ContactPersonAndChartererRelation()
        {
            //Авторизация
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = webToken.token.AccessToken;

            //Создание заказчика
            var createChartererMold = SW.CreateNewCharterer();

            //Создание контакта
            var createContactPerson = new FuraErp.ServiceModel.CreateContactPerson
            {
                OwnerType = OwnerTypeEnum.Charterer,
                OwnerId = createChartererMold.Id,
                FullName = "Тестовый контакт",
                Email = "test@tt.t",
                JobTitle = "Магистр",
                Phone = "-124124154541",
                RightToSign = "да",
                RoleInSystem = "Владыка",
                WarrantDocument = "Документ",
                Contacts = new List<ContactMold>
                {
                    new ContactMold{Name = "VK", Value = "vkvkv"}
                }
            };
            var createContactPersonMold = SW.ErpServiceClient.Post(createContactPerson);
            Assert.IsNotEmpty(createContactPersonMold.Id);
            Assert.AreEqual(createContactPerson.FullName, createContactPersonMold.FullName);
            Assert.AreEqual(createContactPerson.Email, createContactPersonMold.Email);
            Assert.AreEqual(createContactPerson.JobTitle, createContactPersonMold.JobTitle);
            Assert.AreEqual(createContactPerson.Phone, createContactPersonMold.Phone);
            Assert.AreEqual(createContactPerson.RightToSign, createContactPersonMold.RightToSign);
            Assert.AreEqual(createContactPerson.RoleInSystem, createContactPersonMold.RoleInSystem);
            Assert.AreEqual(createContactPerson.WarrantDocument, createContactPersonMold.WarrantDocument);
            Assert.AreEqual(createContactPerson.Contacts.First().Name, createContactPersonMold.Contacts.First().Name);
            Assert.AreEqual(createContactPerson.Contacts.First().Value, createContactPersonMold.Contacts.First().Value);

            //Получение заказчика
            var getCharterer = new FuraErp.ServiceModel.GetCharterer
            {
                Id = createChartererMold.Id
            };
            var getChartererMold = SW.ErpServiceClient.Get(getCharterer);
            Assert.AreEqual(createChartererMold.Id, getChartererMold.Id);
            Assert.AreEqual(createChartererMold.Title, getChartererMold.Title);
            Assert.AreEqual(createChartererMold.WebSite, getChartererMold.WebSite);
            Assert.AreEqual(createChartererMold.FactAddress, getChartererMold.FactAddress);
            Assert.AreEqual(createChartererMold.PostAddress, getChartererMold.PostAddress);

            Assert.AreEqual(1, getChartererMold.ContactPersons.Count);
            Assert.AreEqual(createContactPersonMold.Id, getChartererMold.ContactPersons.First().Id);

            //Устанавка контакта по умолчанию
            var setChartererDefaultContactPerson = new FuraErp.ServiceModel.SetChartererDefaultContactPerson
            {
                ChartererId = createChartererMold.Id,
                ContactPersonId = createContactPersonMold.Id
            };
            var setChartererDefaultContactPersonMold = SW.ErpServiceClient.Put(setChartererDefaultContactPerson);

            Assert.AreEqual(createContactPersonMold.Id, setChartererDefaultContactPersonMold.Id);
            Assert.AreEqual(SetDefaultResult.SetDefaultStatus.AlreadySetted, setChartererDefaultContactPersonMold.Status);

            //Создание контакта 2
            var createContactPerson2 = new FuraErp.ServiceModel.CreateContactPerson
            {
                OwnerType = OwnerTypeEnum.Charterer,
                OwnerId = createChartererMold.Id,
                FullName = "Тестовый контакт2",
                Email = "test@tt.t2",
                JobTitle = "Магистр2",
                Phone = "-1241241545412",
                RightToSign = "да2",
                RoleInSystem = "Владыка2",
                WarrantDocument = "Документ2",
                Contacts = new List<ContactMold>
                {
                    new ContactMold{Name = "VK2", Value = "vkvkv2"}
                }
            };
            var createContactPersonMold2 = SW.ErpServiceClient.Post(createContactPerson2);

            //Получение заказчика с двумя контактами
            var getChartererMold2 = SW.ErpServiceClient.Get(getCharterer);

            Assert.AreEqual(2, getChartererMold2.ContactPersons.Count);
            Assert.AreEqual(createContactPersonMold.Id, getChartererMold2.ContactPersons.First().Id);
            Assert.AreEqual(createContactPersonMold2.Id, getChartererMold2.ContactPersons.Skip(1).First().Id);

            //Установка второго контакта по умолчанию
            var setChartererDefaultContactPerson2 = new FuraErp.ServiceModel.SetChartererDefaultContactPerson
            {
                ChartererId = createChartererMold.Id,
                ContactPersonId = createContactPersonMold2.Id
            };
            var setChartererDefaultContactPersonMold2 = SW.ErpServiceClient.Put(setChartererDefaultContactPerson2);

            Assert.AreEqual(createContactPersonMold2.Id, setChartererDefaultContactPersonMold2.Id);
            Assert.AreEqual(SetDefaultResult.SetDefaultStatus.Setted, setChartererDefaultContactPersonMold2.Status);

            //Проверка порядка контактов
            var getChartererMold3 = SW.ErpServiceClient.Get(getCharterer);

            Assert.AreEqual(2, getChartererMold3.ContactPersons.Count);
            Assert.AreEqual(createContactPersonMold2.Id, getChartererMold3.ContactPersons.First().Id);
            Assert.AreEqual(createContactPersonMold.Id, getChartererMold3.ContactPersons.Skip(1).First().Id);

            //Удаление второго контакта
            var deleteContactPersonFromCharterer = new FuraErp.ServiceModel.DeleteContactPersonFromCharterer
            {
                ChartererId = createChartererMold.Id,
                ContactPersonId = createContactPersonMold2.Id
            };
            var deleteContactPersonFromChartererMold = SW.ErpServiceClient.Delete(deleteContactPersonFromCharterer);

            Assert.AreEqual(createContactPersonMold2.Id, deleteContactPersonFromChartererMold.Id);
            Assert.AreEqual(ArchiveResult.ArchiveStatus.Archived, deleteContactPersonFromChartererMold.Status);

            //Попытка повторного удаления
            try
            {
                var deleteContactPersonFromChartererMold2 = SW.ErpServiceClient.Delete(deleteContactPersonFromCharterer);
                Assert.IsTrue(false, "Должно возникунть исключение");
            }
            catch (WebServiceException e)
            {
                Assert.AreEqual(404, e.StatusCode);
            }

            //Проверка заказчика, что 1 контакт остался
            var getChartererMold4 = SW.ErpServiceClient.Get(getCharterer);

            Assert.AreEqual(1, getChartererMold4.ContactPersons.Count);
            Assert.AreEqual(createContactPersonMold.Id, getChartererMold4.ContactPersons.First().Id);
        }
    }
}
