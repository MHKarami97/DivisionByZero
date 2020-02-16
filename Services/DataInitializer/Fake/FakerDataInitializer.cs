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
        private const bool IsGenerate = false;
        //SET IDENTITY_INSERT dbo.Clients ON

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
            var id = 1000;
            var id2 = 1000;

            var fakeCat = new Faker<Category>()
                .RuleFor(o => o.Id, f => id)
                .RuleFor(o => o.Name, f => f.Lorem.Slug(4));

            var fakeComment = new Faker<Comment>()
                .RuleFor(o => o.Id, f => id2++)
                .RuleFor(u => u.Text, f => f.Lorem.Lines(1))
                .RuleFor(u => u.Time, f => f.Date.Past())
                .RuleFor(u => u.PostId, f => id)
                .RuleFor(u => u.UserId, f => id);


            var fakeUser = new Faker<User>()
                .RuleFor(o => o.Id, f => id)
                .RuleFor(u => u.FullName, (f, u) => f.Name.FirstName() + " " + f.Name.FirstName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FullName))
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.Email))
                .RuleFor(u => u.Gender, f => f.PickRandom<GenderType>())
                .RuleFor(u => u.PasswordHash, f => f.Random.AlphaNumeric(10));

            var fakePost = new Faker<Post>()
                .RuleFor(o => o.Id, f => id)
                .RuleFor(o => o.Title, f => f.Lorem.Text())
                .RuleFor(o => o.ShortDescription, f => f.Lorem.Lines(2))
                .RuleFor(o => o.Image, f => f.Image.LoremFlickrUrl())
                .RuleFor(o => o.Text, f => f.Lorem.Paragraphs())
                .RuleFor(o => o.Time, f => f.Date.Past(2))
                .RuleFor(o => o.TimeToRead, f => f.Random.Int(1,45))
                .RuleFor(o => o.Comments, f => fakeComment.Generate(2))
                .RuleFor(o => o.Category, f => fakeCat.Generate())
                .RuleFor(o => o.CategoryId, f => id)
                .RuleFor(o => o.Author, f => fakeUser.Generate())
                .RuleFor(o => o.AuthorId, f => id++);

            var posts = fakePost.Generate(20);

            return posts;
        }
    }
}