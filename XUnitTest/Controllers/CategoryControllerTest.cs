using Moq;
using Xunit;
using System;
using AutoMapper;
using System.Linq;
using Models.Models;
using Data.Contracts;
using Entities.Post;
using XUnitTest.Faker;
using System.Threading;
using Models.CustomMapping;
using MyApi.Controllers.v1;
using System.Threading.Tasks;
using Repositories.Contracts;
using WebFramework.CustomMapping;

namespace XUnitTest.Controllers
{
    public class CategoryControllerTest
    {
        private readonly CategoriesController _controller;

        public CategoryControllerTest()
        {
            //fake data
            var fakeData = new FakeData();

            //setup mapper
            var repositoriesAssembly = typeof(BannerDto).Assembly;
            var assemblies = new[] { repositoriesAssembly };
            var allTypes = assemblies.SelectMany(a => a.ExportedTypes);
            var list = allTypes.Where(type => type.IsClass && !type.IsAbstract &&
                                              type.GetInterfaces().Contains(typeof(IHaveCustomMapping)))
                .Select(type => (IHaveCustomMapping)Activator.CreateInstance(type));

            var profile = new CustomMappingProfile(list);
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            var mapper = new Mapper(configuration);

            //setup ICategoryRepository
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(x => x.GetAllMainCat(new CancellationToken())).Returns(Task.FromResult(fakeData.FakeCategoryData(30)));

            //setup IRepository
            var mockRepository = new Mock<IRepository<Category>>();

            _controller = new CategoriesController(mockRepository.Object, mapper, mockCategoryRepository.Object);
        }

        [Fact]
        public async Task GetPersonsTest()
        {
            var results = await _controller.GetAllMainCat(new CancellationToken());

            var count = results.Data.Count;

            Assert.Equal(30, count);
        }
    }
}
