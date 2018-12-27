using FuraErp.ServiceModel;
using FuraErp.ServiceModel.Molds;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fura.Tests.Microservices.fura_erp
{
    class RequestTests
    {
        public readonly ServicesWrapper SW;

        public RequestTests()
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


        [Test]
        public void RequestsFilterTest()
        {
            //Авторизация
            var email = SW.GenerateEmail();
            var webToken = SW.GetNewSupervisorsCheatWebToken(email, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = webToken.token.AccessToken;

            var createChartererMold = SW.CreateNewCharterer();

            var createJuridicalPersonMold = SW.CreateNewJuridicalPerson(createChartererMold.Id);

            //Создание
            var createInitialRequest = new FuraErp.ServiceModel.CreateInitialRequest
            {
                ChartererId = createChartererMold.Id,
                JuridicalPersonId = createJuridicalPersonMold.Id,
                Cargo = new CargoMold
                {
                    BruttoInTonns = "2",
                    CargoType = "Товары народного потребления",
                    PackagingType = "Паллета  EUR (0,8 x 1,2)",
                    VolumeInCubometres = "3",
                    VolumeInPallets = "1",
                },
                ChartererDescription = "хочу",
                ChartererRate = new ChartererRateMold
                {
                    Amount = 100500,
                    Currency = "RUB",
                    CarrierRequiredWithNDS = false
                },
                TransportRequirements = new TransportRequirementsMold
                {
                    AdditionalParameters = new List<string> { "мед. книжка", "гидроробот", "ремни", "коники" },
                    TransportPreset = "20т 82м3 33плт",
                    TypeOfBody = new List<string> { "рефрижераторный", "тентованный" },
                    TypeOfLoading = new List<string> { "задняя" }
                },
                StartRoutePoint = new RoutePointMold
                {
                    Locality = "г. Хургада",
                    Address = "МО №28 \"Автово\", Кировский район, Стачек проспект, 92",
                    DateTimeFrom = DateTimeOffset.UtcNow.AddDays(2).ToUnixTimeMilliseconds(),
                    Latitude = 59.868157,
                    Longitude = 30.262091,
                },
                FinishRoutePoint = new RoutePointMold
                {
                    Locality = "г. Израиль",
                    Address = "Ленинский район, ул. Бестужева, 58 ст1",
                    DateTimeFrom = DateTimeOffset.UtcNow.AddDays(2).ToUnixTimeMilliseconds(),
                    Latitude = 53.425056,
                    Longitude = 58.945383,
                },
                Priority = 1,
            };
            var createInitialRequestMold = SW.ErpServiceClient.Post(createInitialRequest);
            Assert.IsNotEmpty(createInitialRequestMold.Id);
            Assert.IsTrue(createInitialRequestMold.ChartererVersionId.StartsWith(createInitialRequest.ChartererId));

            //Новая авторизация
            var email2 = SW.GenerateEmail();
            var webToken2 = SW.GetNewSupervisorsCheatWebToken(email2, ServicesWrapper.webroles.experditor);
            SW.ErpServiceClient.BearerToken = webToken2.token.AccessToken;

            var createChartererMold2 = SW.CreateNewCharterer();

            var createJuridicalPersonMold2 = SW.CreateNewJuridicalPerson(createChartererMold2.Id);

            var requestId = createInitialRequestMold.ProtoRequests.First().RequestIds.First();

            //Создание другого запроса
            var createInitialRequest2 = new FuraErp.ServiceModel.CreateInitialRequest
            {
                ChartererId = createChartererMold2.Id,
                JuridicalPersonId = createJuridicalPersonMold2.Id,
                Cargo = new CargoMold
                {
                    BruttoInTonns = "5",
                    CargoType = "Кирпичи",
                    PackagingType = "Паллета FAN (0,9 x 1,5)",
                    VolumeInCubometres = "4",
                    VolumeInPallets = "2",
                },
                ChartererDescription = "хороший стройматериал",
                ChartererRate = new ChartererRateMold
                {
                    Amount = 12312,
                    Currency = "RUB",
                    CarrierRequiredWithNDS = true
                },
                TransportRequirements = new TransportRequirementsMold
                {
                    AdditionalParameters = new List<string> { "гидроробот", "коники" },
                    TransportPreset = "10т 52м3 15плт",
                    TypeOfBody = new List<string> { "тентованный" },
                    TypeOfLoading = new List<string> { "боковая" }
                },
                StartRoutePoint = new RoutePointMold
                {
                    Locality = "г. Норильск",
                    Address = "улица Завенягина, 2С1",
                    DateTimeFrom = DateTimeOffset.UtcNow.AddDays(2).ToUnixTimeMilliseconds(),
                    Latitude = 69.348448,
                    Longitude = 88.197951,
                },
                FinishRoutePoint = new RoutePointMold
                {
                    Locality = "Дудинка",
                    Address = "Красноярский край, Таймырский Долгано - Ненецкий район,Дудинка, улица Морозова, 4",
                    DateTimeFrom = DateTimeOffset.UtcNow.AddDays(2).ToUnixTimeMilliseconds(),
                    Latitude = 69.401727,
                    Longitude = 86.194254,
                },
                Priority = 2,
            };
            var createInitialRequestMold2 = SW.ErpServiceClient.Post(createInitialRequest2);
            Assert.IsNotEmpty(createInitialRequestMold2.Id);
            Assert.IsTrue(createInitialRequestMold2.ChartererVersionId.StartsWith(createInitialRequest2.ChartererId));

            var requestId2 = createInitialRequestMold2.ProtoRequests.First().RequestIds.First();

            //Проверка фильтров
            goto current;

            //Текстовый фильтр по всем полям - цель Cargo.CargoType
            AssertOneRequestFiltered(requestId2, new GetRequests
            {
                Filters = new GetRequests.FilterDefinition
                {
                    TextFilter = "кирпич"
                }
            });

            //Текстовый фильтр по всем полям - цель FinishRoutePoint.Address
            AssertOneRequestFiltered(requestId2, new GetRequests
            {
                Filters = new GetRequests.FilterDefinition
                {
                    TextFilter = "Мороз"
                }
            });

            //Фильтр только мои/не мои
            {
                //Фильтр только мои
                AssertOneRequestFiltered(requestId2, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        WhereIAmCreator = true
                    }
                });

                //Фильтр только не мои
                var getNotMyRequests = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        WhereIAmCreator = false
                    }
                };
                var getRequestsMold = SW.ErpServiceClient.Get(getNotMyRequests);
                Assert.IsTrue(getRequestsMold.Count >= 1);
                Assert.IsTrue(getRequestsMold.Items.Count >= 1);
                Assert.IsTrue(getRequestsMold.TotalCount >= 1);
                Assert.IsFalse(getRequestsMold.Items.Select(m => m.Id).Contains(requestId2));
            }

            //Фильтр по uid создателя
            {
                //Первый
                AssertOneRequestFiltered(requestId, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        CreatorUids = new List<string> { webToken.sid }
                    }
                });

                //Второй
                AssertOneRequestFiltered(requestId2, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        CreatorUids = new List<string> { webToken2.sid }
                    }
                });

                //Никакой
                var getRequestsNotExistCreatorUid = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        CreatorUids = new List<string> { requestId+"_" }
                    }
                };
                var getRequestsNotExistCreatorUidMold = SW.ErpServiceClient.Get(getRequestsNotExistCreatorUid);
                Assert.IsTrue(getRequestsNotExistCreatorUidMold.Count == 0);
                Assert.IsTrue(getRequestsNotExistCreatorUidMold.Items.Count == 0);
                Assert.IsTrue(getRequestsNotExistCreatorUidMold.TotalCount == 0);

                //Оба
                var getRequestsFewCreatorUid = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        CreatorUids = new List<string> { webToken.sid, webToken2.sid }
                    }
                };
                var getRequestsFewCreatorUidMold = SW.ErpServiceClient.Get(getRequestsFewCreatorUid);
                Assert.IsTrue(getRequestsFewCreatorUidMold.Count == 2);
                Assert.IsTrue(getRequestsFewCreatorUidMold.Items.Count == 2);
                Assert.IsTrue(getRequestsFewCreatorUidMold.TotalCount == 2);
            }

            //Фильтр по uid изменителя
            {
                //Первый
                AssertOneRequestFiltered(requestId, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        LastChangerUids = new List<string> { webToken.sid }
                    }
                });

                //Второй
                AssertOneRequestFiltered(requestId2, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        LastChangerUids = new List<string> { webToken2.sid }
                    }
                });

                //Никакой
                var getRequestsNotExistLastChangerUid = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        LastChangerUids = new List<string> { requestId + "_" }
                    }
                };
                var getRequestsNotExistLastChangerUidMold = SW.ErpServiceClient.Get(getRequestsNotExistLastChangerUid);
                Assert.IsTrue(getRequestsNotExistLastChangerUidMold.Count == 0);
                Assert.IsTrue(getRequestsNotExistLastChangerUidMold.Items.Count == 0);
                Assert.IsTrue(getRequestsNotExistLastChangerUidMold.TotalCount == 0);

                //Оба
                var getRequestsFewLastChangerUid = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        LastChangerUids = new List<string> { webToken.sid, webToken2.sid }
                    }
                };
                var getRequestsFewLastChangerUidMold = SW.ErpServiceClient.Get(getRequestsFewLastChangerUid);
                Assert.IsTrue(getRequestsFewLastChangerUidMold.Count == 2);
                Assert.IsTrue(getRequestsFewLastChangerUidMold.Items.Count == 2);
                Assert.IsTrue(getRequestsFewLastChangerUidMold.TotalCount == 2);
            }

            //Фильтр по id заказчика
            {
                //Первый
                AssertOneRequestFiltered(requestId, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        CharterersIds = new List<string> { createChartererMold.Id }
                    }
                });

                //Второй
                AssertOneRequestFiltered(requestId2, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        CharterersIds = new List<string> { createChartererMold2.Id }
                    }
                });

                //Никакой
                var getRequestsNotExistCharterersId = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        CharterersIds = new List<string> { requestId + "_" }
                    }
                };
                var getRequestsNotExistCharterersIdMold = SW.ErpServiceClient.Get(getRequestsNotExistCharterersId);
                Assert.IsTrue(getRequestsNotExistCharterersIdMold.Count == 0);
                Assert.IsTrue(getRequestsNotExistCharterersIdMold.Items.Count == 0);
                Assert.IsTrue(getRequestsNotExistCharterersIdMold.TotalCount == 0);

                //Оба
                var getRequestsFewCharterersId = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        CharterersIds = new List<string> { createChartererMold.Id, createChartererMold2.Id }
                    }
                };
                var getRequestsFewCharterersIdMold = SW.ErpServiceClient.Get(getRequestsFewCharterersId);
                Assert.IsTrue(getRequestsFewCharterersIdMold.Count == 2);
                Assert.IsTrue(getRequestsFewCharterersIdMold.Items.Count == 2);
                Assert.IsTrue(getRequestsFewCharterersIdMold.TotalCount == 2);
            }

            //Фильтр по стартовой точке
            {
                //Первый
                AssertOneRequestFiltered(requestId, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        StartLocations = new List<GetRequests.LocationFilter>()
                        {
                            new GetRequests.LocationFilter{Locality = "Хургада"}
                        }
                    }
                });

                //Второй
                AssertOneRequestFiltered(requestId2, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        StartLocations = new List<GetRequests.LocationFilter>()
                        {
                            new GetRequests.LocationFilter{Locality = "Норильск"}
                        }
                    }
                });

                //Никакой
                var getRequestsNotExistLocation = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        StartLocations = new List<GetRequests.LocationFilter>()
                        {
                            new GetRequests.LocationFilter{Locality = "Курск"}
                        }
                    }
                };
                var getRequestsNotExistLocationMold = SW.ErpServiceClient.Get(getRequestsNotExistLocation);
                Assert.IsTrue(getRequestsNotExistLocationMold.Count == 0);
                Assert.IsTrue(getRequestsNotExistLocationMold.Items.Count == 0);
                Assert.IsTrue(getRequestsNotExistLocationMold.TotalCount == 0);

                //Оба
                var getRequestsFewLocations = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        StartLocations = new List<GetRequests.LocationFilter>()
                        {
                            new GetRequests.LocationFilter{Locality = "Хургада"},
                            new GetRequests.LocationFilter{Locality = "Норильск"},
                        }
                    }
                };
                var getRequestsFewLocationsMold = SW.ErpServiceClient.Get(getRequestsFewLocations);
                Assert.IsTrue(getRequestsFewLocationsMold.Count == 2);
                Assert.IsTrue(getRequestsFewLocationsMold.Items.Count == 2);
                Assert.IsTrue(getRequestsFewLocationsMold.TotalCount == 2);
            }

            current:
            //Фильтр по финишной точке
            {
                //Первый
                AssertOneRequestFiltered(requestId, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        FinishLocations = new List<GetRequests.LocationFilter>()
                        {
                            new GetRequests.LocationFilter{Locality = "Израиль"}
                        }
                    }
                });

                //Второй
                AssertOneRequestFiltered(requestId2, new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        FinishLocations = new List<GetRequests.LocationFilter>()
                        {
                            new GetRequests.LocationFilter{Locality = "Дудинка"}
                        }
                    }
                });

                //Никакой
                var getRequestsNotExistLocation = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        FinishLocations = new List<GetRequests.LocationFilter>()
                        {
                            new GetRequests.LocationFilter{Locality = "Курск"}
                        }
                    }
                };
                var getRequestsNotExistLocationMold = SW.ErpServiceClient.Get(getRequestsNotExistLocation);
                Assert.IsTrue(getRequestsNotExistLocationMold.Count == 0);
                Assert.IsTrue(getRequestsNotExistLocationMold.Items.Count == 0);
                Assert.IsTrue(getRequestsNotExistLocationMold.TotalCount == 0);

                //Оба
                var getRequestsFewLocations = new GetRequests
                {
                    Filters = new GetRequests.FilterDefinition
                    {
                        FinishLocations = new List<GetRequests.LocationFilter>()
                        {
                            new GetRequests.LocationFilter{Locality = "Израиль"},
                            new GetRequests.LocationFilter{Locality = "Дудинка"},
                        }
                    }
                };
                var getRequestsFewLocationsMold = SW.ErpServiceClient.Get(getRequestsFewLocations);
                Assert.IsTrue(getRequestsFewLocationsMold.Count == 2);
                Assert.IsTrue(getRequestsFewLocationsMold.Items.Count == 2);
                Assert.IsTrue(getRequestsFewLocationsMold.TotalCount == 2);
            }
        }

        private void AssertOneRequestFiltered(string requestId, GetRequests getRequests)
        {
            var getRequestsMold = SW.ErpServiceClient.Get(getRequests);
            Assert.IsTrue(getRequestsMold.Count == 1);
            Assert.IsTrue(getRequestsMold.Items.Count == 1);
            Assert.IsTrue(getRequestsMold.TotalCount == 1);
            Assert.AreEqual(requestId, getRequestsMold.Items.First().Id);
        }
    }
}
