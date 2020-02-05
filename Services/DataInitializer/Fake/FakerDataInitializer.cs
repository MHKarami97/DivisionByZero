using Bogus;
using Data.Contracts;
using Entities.Post;
using Entities.User;
using System;
using System.Collections.Generic;

namespace Services.DataInitializer.Fake
{
    public class FakerDataInitializer : IDataInitializer
    {
        private readonly IRepository<Post> _repository;
        private readonly bool IsGenerate = true;

        public FakerDataInitializer(IRepository<Post> repository)
        {
            _repository = repository;
        }

        public void InitializeData()
        {
            if (!IsGenerate)
                return;

            var list = GetSampleTableData();

            _repository.AddRange(list);
        }

        private static IEnumerable<Post> GetSampleTableData()
        {
            var fakeCat = new Faker<Category>()
                .RuleFor(o => o.Name, f => f.Lorem.Slug(4));

            var fakeUser = new Faker<User>()
                .RuleFor(u => u.FullName, (f, u) => f.Name.FirstName() + " " + f.Name.FirstName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FullName))
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.Email))
                .RuleFor(u => u.Gender, f => f.PickRandom<GenderType>())
                .RuleFor(u => u.PasswordHash, f => f.Random.AlphaNumeric(10));

            var fakePost = new Faker<Post>()
                .RuleFor(o => o.Title, f => f.Lorem.Text())
                .RuleFor(o => o.ShortDescription, f => f.Lorem.Lines(2))
                .RuleFor(o => o.Image, f => f.Image.LoremFlickrUrl())
                .RuleFor(o => o.Text, f => f.Lorem.Paragraphs())
                .RuleFor(o => o.Rank, f => f.Random.Int(0, 5))
                .RuleFor(o => o.Time, f => f.Date.Past(2))
                .RuleFor(o => o.TimeToRead, f => f.Date.Between(DateTime.Now.Date, DateTime.Now.AddMinutes(45)))
                .RuleFor(o => o.View, f => f.Random.Int(0, 200))
                .RuleFor(o => o.Category, f => fakeCat.Generate())
                .RuleFor(o => o.Author, f => fakeUser.Generate());

            var posts = fakePost.Generate(100);

            return posts;
        }
    }
}