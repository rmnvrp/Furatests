using System.Collections.Generic;
using NUnit.Framework;
using ServiceStack;
using FuraBack.ServiceModel;
using System.Linq;
using Fura.Tests;


namespace FuraBack.Tests
{
    [TestFixture]
    public class MetadataTests
    {
        public readonly JsonServiceClient service;

        public readonly ServicesWrapper SW;

        public MetadataTests()
        {
            SW = new ServicesWrapper();
            service = SW.MobileServiceClient;
            var resultGetToken = SW.GetAnonimToken();
            service.BearerToken = resultGetToken.Token;
        }

      

       [SetUp]
        public void SetUp()
        {
            SW.SetUp();
        }

        [Test]
        public void Test_GetCategories() // пока проверяю только что что-то отдает
        {
            var GetCategoriesRequest = new Categories();
            var GetCategoriesResult = service.Get(GetCategoriesRequest);
            var events = GetCategoriesResult.ToList();
            Assert.IsTrue(events.Any());
            Assert.IsTrue(events.Any(m => m.Name != null)); // у одного из элементов сущесвтует поле name
            Assert.IsTrue(events.Any(m => m.ShowInMenu != default(bool))); // проверка существования
            //проверяем что модель не изменилась
            Assert.IsTrue(events.Any(m => m.MarkerUrl != null));
        }

        /*
        var names = new HashSet<string>
        {
            "Id",
            "Name",
        };

        var props = typeof(CategoryModel).GetProperties();
        foreach (var propertyInfo in props)
        {
            Assert.IsTrue(names.Contains(propertyInfo.Name), $"New property is found: {propertyInfo.Name}");
        }
    }

*/

        [Test]
        public void Test_GetCreatableevents() // пока проверяю только что что-то отдает
        {
            var GetCreatableeventsRequest = new GetCreatableEventCategories();
            var GetCreatableeventsResult = service.Get(GetCreatableeventsRequest);
            var events = GetCreatableeventsResult.ToList();
            Assert.IsTrue(events.Any());
            Assert.IsTrue(events.Any(m => m.Id  > 0)); // ? больше нуля ли не ноль*
           // Assert.IsTrue(events.Any(m => m.Id == 1 && m.Name == "name")); // ? как создать вложенный набор условий например id=1 другое поле =n

            Assert.IsTrue(events.Any(m => m.Color != null));
            Assert.IsTrue(events.Any(m => m.IconUrl != null)); // добавить позже проверку валидности ссылки в отдельный тест
            Assert.IsTrue(events.Any(m => m.Name != null));
            Assert.IsTrue(events.Any(m => m.InternalName != null));
            Assert.IsTrue(events.Any(m => m.Attributes != null));
            //Добавить проверку вложенных данных в m.Attributes
        }

        [Test] // Необходимо залогиниться под авторизованным пользвателем
        public void Test_GetLoadtypes()
        {
            var GetLoadtypesRequest = new GetLoadTypes();
            var GetLoadtypesResult = service.Get(GetLoadtypesRequest);
            var events = GetLoadtypesResult.ToList();
            Assert.IsTrue(events.Any());
            Assert.IsTrue(events.Any(m => m.Id > 0));
            Assert.IsTrue(events.Any(m => m.Name != null));//возможно стоит зафиксировать все 12 типоа и проверять их наличие
        }

        [Test]
        public void Test_GetNotificationevents()
        {
            var GetNotificationeventsRequest = new GetNotificationEventCategories();
            var GetNotificationeventsResult = service.Get(GetNotificationeventsRequest);
            var events = GetNotificationeventsResult.ToList();
            Assert.IsTrue(events.Any());
            Assert.IsTrue(events.Any(m => m.SoundUrl != null));
            Assert.IsTrue(events.Any(m => m.MetresToShow!= null)); //? как реализовать проверку наличия не для интовых
            Assert.IsTrue(events.Any(m => m.HowMuchSecondsShow != null));//? как задать диапазон
            Assert.IsTrue(events.Any(m => m.UpToThePoint != null)); // проверка наличия, возможно позже или в отдельных тестах стоит привязаться к категории и провериять 
            Assert.IsTrue(events.Any(m => m.InternalName != null));
        }

        [Test]// Необходимо залогиниться под авторизованным пользвателем
        public void Test_GetTimeperiods()
        {
            var GetTimeperiodRequest = new GetTimePeriods();
            var GetTimeperiodResult = service.Get(GetTimeperiodRequest);
            var events = GetTimeperiodResult.ToList();
            Assert.IsTrue(events.Any());
            Assert.IsTrue(events.Any(m => m.Id != 0));
            Assert.IsTrue(events.Any(m => m.Name != null));
            Assert.IsTrue(events.Any(m => m.OffsetFromNowInDays != null));
            Assert.IsTrue(events.Any(m => m.DelayInDays != null)); // ? добавить привязку к текущей модели?


        }

        [Test]// Необходимо залогиниться под авторизованным пользвателем
        public void Test_Gettruckbodytypes()
        {
            var GettruckbodytypesRequest = new GetTimePeriods();
            var GettruckbodytypesResult = service.Get(GettruckbodytypesRequest);
            var events = GettruckbodytypesResult.ToList();
            Assert.IsTrue(events.Any());
            Assert.IsTrue(events.Any(m => m.Id != null));
            Assert.IsTrue(events.Any(m => m.Name != null));
        }

        [Test]
        public void Test_GetVariables()
        {
            var GetVariablesRequest = new GetVariables();
            var GetVariablesResult = service.Get(GetVariablesRequest);
            var events = GetVariablesResult.ToList();
            Assert.IsTrue(events.Any());
            //Assert.IsTrue(events.Any(m => m.re != null));
        }

    }
}