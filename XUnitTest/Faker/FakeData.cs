using Bogus;
using Models.Base;
using Models.Models;
using System.Collections.Generic;

namespace XUnitTest.Faker
{
    public class FakeData
    {
        public ApiResult<List<CategoryDto>> FakeCategoryData(int count)
        {
            var fakeCat = new Faker<CategoryDto>()
                .RuleFor(o => o.Id, f => 1)
                .RuleFor(o => o.Name, f => f.Lorem.Slug(4));

            return fakeCat.Generate(count);
        }
    }
}
