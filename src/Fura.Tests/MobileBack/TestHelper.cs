using System;
using System.Collections.Generic;
using FuraBack.ServiceModel;
using ServiceStack;

namespace FuraBack.Tests
{
    public class TestHelper
    {
        private CrowdTests _unitTests;

        public TestHelper(CrowdTests unitTests)
        {
            _unitTests = unitTests;
        }

        public void CreateKeys()
        {
            //Генерация пары ключей для провайдера авторизации
            var privateKey = RsaUtils.CreatePrivateKeyParams(RsaKeyLengths.Bit4096);
            var publicKey = privateKey.ToPublicRsaParameters();
            var privateKeyXml = privateKey.ToPrivateKeyXml();
            var publicKeyXml = privateKey.ToPublicKeyXml();
        }
        

        public long CreateTestEvent(String categoryId = "platon") //Метод для создания события(по дефолту создается платон_)
        {
            var createEvent = new CreateEvent();
            createEvent.Azimuth = 11.11;
            createEvent.Latitude = 11.11;
            createEvent.Longitude = 11.11;
            createEvent.CategoryId = categoryId;
            var createEventId = _unitTests.service.Post(createEvent);
            return createEventId;
        }

        public void DeleteTestEvent(long createdEventId) //Метод для удаления события
        {
            var delEventRequest = new DeleteEvent { EventId = createdEventId };
            _unitTests.service.Delete(delEventRequest);
        }


        public List<long> CreatEventAllCategories() // метод для создания событий всех категорий
        {
            //long createEventId = CreateTestEvent();
            var creatableeventsrequest = new GetCreatableEventCategories();
            var creatableeventsresult = _unitTests.service.Get(creatableeventsrequest);
            var listevents = new List<long>();
            foreach (var categoryModel in creatableeventsresult) //  
            {
                long createEventId = CreateTestEvent(categoryModel.InternalName);
                listevents.Add(createEventId);

            }

            return listevents;
        }


        public void DeleteEventAllCategories(List<long> listevents)   // метод для удаления всех событий списка
        {
            foreach (var ev in listevents)
                DeleteTestEvent(ev);
        }
        // Assert.Greater(createEventId, 0);
    }

}
