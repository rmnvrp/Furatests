using System;
using System.Collections.Generic;
using System.Net;
using FuraBack.ServiceModel;
using FuraErp.ServiceModel;
using FuraErp.ServiceModel.Molds;
using NUnit.Framework;
using ServiceStack;

namespace Fura.Tests
{
    public partial class ServicesWrapper
    {
        public ServicesWrapper(EnvEnum env = EnvEnum.Work, DebugService debugService = DebugService.No, bool cleanDataAfterTest = true)
        {
            Env = env;
            IsDebug = debugService;
            CleanDataAfterTest = cleanDataAfterTest;
            var apiAddress = PublicApiEntryPoint[Env];
            var privateApiAddress = PrivateApiEntryPoint[Env];

            WebServiceClient = new JsonServiceClient($"{apiAddress}web/");
            _privateWebServiceClient = new JsonServiceClient($"{privateApiAddress}:8039/");
            AuthServiceClient = new JsonServiceClient($"{apiAddress}v2/auth/");
            _privateAuthServiceClient = new JsonServiceClient($"{privateApiAddress}:8059/");
            MobileServiceClient = new JsonServiceClient(apiAddress);
            _privateMobileServiceClient = new JsonServiceClient($"{privateApiAddress}:8009/");
            ExpeditionServiceClient = new JsonServiceClient($"{apiAddress}v2/expedition");
            _privateExpeditionServiceClient = new JsonServiceClient($"{privateApiAddress}:8069/");
            PersonalPromptsServiceClient = new JsonServiceClient($"{apiAddress}v2/personal-prompts");
            _privatePersonalPromptsServiceClient = new JsonServiceClient($"{privateApiAddress}:8088/");
            ErpServiceClient = new JsonServiceClient($"{apiAddress}v2/erp");
            _privateErpServiceClient = new JsonServiceClient($"{privateApiAddress}:8087/"); ;

            switch (IsDebug)
            {
                case DebugService.No:
                    break;
                case DebugService.Web:
                    WebServiceClient = new JsonServiceClient("http://localhost:57000/");
                    _privateWebServiceClient = new JsonServiceClient("http://localhost:57001/");
                    break;
                case DebugService.Auth:
                    AuthServiceClient = new JsonServiceClient("http://localhost:58000/");
                    _privateAuthServiceClient = new JsonServiceClient("http://localhost:58001/");
                    break;
                case DebugService.Mobile:
                    MobileServiceClient = new JsonServiceClient("http://localhost:55000/");
                    _privateMobileServiceClient = new JsonServiceClient("http://localhost:55001/");
                    break;
                case DebugService.Expedition:
                    ExpeditionServiceClient = new JsonServiceClient("http://localhost:8000/");
                    _privateExpeditionServiceClient = new JsonServiceClient("http://localhost:8001/");
                    break;
                case DebugService.Personal:
                    PersonalPromptsServiceClient = new JsonServiceClient("http://localhost:8000/");
                    _privatePersonalPromptsServiceClient = new JsonServiceClient("http://localhost:8001/");
                    break;
                case DebugService.Erp:
                    ErpServiceClient = new JsonServiceClient("http://localhost:8000/");
                    _privateErpServiceClient = new JsonServiceClient("http://localhost:8001/");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum DebugService
        {
            No,
            Web,
            Auth,
            Mobile,
            Expedition,
            Personal,
            Erp,
        }

        /// <summary>
        /// Эта переменная указывает с каким окружением мы работаем
        /// </summary>
        public readonly EnvEnum Env;

        /// <summary>
        /// Включен ли режим отладки
        /// </summary>
        public readonly DebugService IsDebug;

        /// <summary>
        /// Очищать ли тестовые данные по завершению
        /// </summary>
        public readonly bool CleanDataAfterTest;

        /// <summary>
        /// Перечисление стэндов
        /// </summary>
        public enum EnvEnum
        {
            Public,
            Work,
            Staging
        }

        //Клиенты к публичным API
        public readonly JsonServiceClient WebServiceClient;
        public readonly JsonServiceClient AuthServiceClient;
        public readonly JsonServiceClient MobileServiceClient;
        public readonly JsonServiceClient ExpeditionServiceClient;
        public readonly JsonServiceClient PersonalPromptsServiceClient;
        public readonly JsonServiceClient ErpServiceClient;
        //Клиенты к внутреннним API
        private readonly JsonServiceClient _privateWebServiceClient;
        public readonly JsonServiceClient _privateAuthServiceClient;
        private readonly JsonServiceClient _privateMobileServiceClient;
        private readonly JsonServiceClient _privateExpeditionServiceClient;
        private readonly JsonServiceClient _privatePersonalPromptsServiceClient;
        private readonly JsonServiceClient _privateErpServiceClient;

        /// <summary>
        /// Точки входа публичных API по стэндам
        /// </summary>
        private static readonly Dictionary<EnvEnum, string> PublicApiEntryPoint = new Dictionary<EnvEnum, string>
        {
            {EnvEnum.Public, "https://gofura.com/api/"},
            {EnvEnum.Work, "https://work.gofura.com/api/"},
            {EnvEnum.Staging, "https://stage.gofura.com/api/"}
        };

        /// <summary>
        /// Точки входа внутренних API по стэндам
        /// </summary>
        private static readonly Dictionary<EnvEnum, string> PrivateApiEntryPoint = new Dictionary<EnvEnum, string>
        {
            {EnvEnum.Public, "https://d-prod-1.int.gofura.com"},
            {EnvEnum.Work, "http://d-work1.int.gofura.com"},
            {EnvEnum.Staging, "http://d-stage1.int.gofura.com"}
        };

        /// <summary>
        /// Вывод тестовых данных вначале каждого теста
        /// </summary>
        public void SetUp()
        {
            Console.WriteLine($"Environment: {Env}");
            Console.WriteLine($"Is Debug: {IsDebug}");
            Console.WriteLine($"DateTime: {DateTime.Now}");
        }

        /// <summary>
        /// Освобождение ресурсов и очистка данных в конце тестовой сессии
        /// </summary>
        public void TestFixtureTearDown()
        {
            WebServiceClient.Dispose();
            AuthServiceClient.Dispose();
            MobileServiceClient.Dispose();
            ExpeditionServiceClient.Dispose();
            PersonalPromptsServiceClient.Dispose();
            ErpServiceClient.Dispose();


            _privateWebServiceClient.Dispose();
            _privateAuthServiceClient.Dispose();
            _privateMobileServiceClient.Dispose();
            _privateExpeditionServiceClient.Dispose();
            _privatePersonalPromptsServiceClient.Dispose();
            _privateErpServiceClient.Dispose();

            if (CleanDataAfterTest)
            {
                CleanAll();
            }
        }

        /// <summary>
        /// Очистка всех тестовых данных
        /// </summary>
        private void CleanAll()
        {
            var list = new List<Action>
            {
                () => _privateAuthServiceClient.Delete<HttpWebResponse>(new FuraAuth.ServiceModel.ClearAllTestData()),
                () => _privateWebServiceClient.Delete<HttpWebResponse>(new FuraWebBack.ServiceModel.ClearAllTestData()),
                () => _privateMobileServiceClient.Delete<HttpWebResponse>(new FuraBack.ServiceModel.ClearAllTestData()),
                () => _privateExpeditionServiceClient.Delete<HttpWebResponse>(new FuraExpedition.ServiceModel.ClearAllTestData()),
                () => _privatePersonalPromptsServiceClient.Delete<HttpWebResponse>(new FuraPersonalPrompts.ServiceModel.ClearAllTestData()),
                () => _privateErpServiceClient.Delete<HttpWebResponse>(new FuraErp.ServiceModel.ClearAllTestData())
            };
            foreach (var action in list)
            {
                try { action.Invoke(); } catch { }
            }
        }

        /// <summary>
        /// Возвращает каждый раз следующее число
        /// </summary>
        private int Counter => _counter++;
        private int _counter = 1;

        /// <summary>
        /// Возвращает новый уникальный адрес почты
        /// </summary>
        public string GenerateEmail() => $"TEST{Counter}@EMAIL.COM";

        /// <summary>
        /// Возвращает новый уникальный номер телефона
        /// </summary>
        public string GeneratePhone() => $"777777{Counter:D4}";
    }
}
