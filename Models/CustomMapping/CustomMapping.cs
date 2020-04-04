using AutoMapper;
using Common.Utilities;
using Entities.Contact;
using Entities.Employ;
using Entities.Post;
using Models.Models;
using System;

//Tip: Reason is because, UseValue is a static,
//so it’s set once when the MapProfile is instantiated and all subsequent .Map()
//invokes will use the same static value. Hence the sticky time value
namespace Models.CustomMapping
{
    public class PostCustomMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<Post, PostDto>().ReverseMap()
                .ForMember(dest => dest.Time,
                    opt =>
                        opt.MapFrom(src => DateTimeOffset.Now))

                .ForMember(dest => dest.Text,
                    opt =>
                        opt.MapFrom(src => src.Text.FixPersianChars()))

                .ForMember(dest => dest.Title,
                    opt =>
                        opt.MapFrom(src => src.Title.FixPersianChars()))

                .ForMember(dest => dest.ShortDescription,
                    opt =>
                        opt.MapFrom(src => src.ShortDescription.FixPersianChars()));
        }
    }

    public class CommentCustomMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<Comment, CommentDto>().ReverseMap()
                .ForMember(dest => dest.Time,
                    opt =>
                        opt.MapFrom(src => DateTimeOffset.Now))

                .ForMember(dest => dest.Text,
                    opt =>
                        opt.MapFrom(src => src.Text.FixPersianChars()));
        }
    }

    public class ContactCustomMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<Contact, ContactDto>().ReverseMap()
                .ForMember(dest => dest.Time,
                    opt =>
                        opt.MapFrom(src => DateTimeOffset.Now))

                .ForMember(dest => dest.Text,
                    opt =>
                        opt.MapFrom(src => src.Text.FixPersianChars()));
        }
    }

    public class EmployCustomMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<Employ, EmployDto>().ReverseMap()
                .ForMember(dest => dest.Time,
                    opt =>
                        opt.MapFrom(src => DateTimeOffset.Now))

                .ForMember(dest => dest.Text,
                    opt =>
                        opt.MapFrom(src => src.Text.FixPersianChars()))

                .ForMember(dest => dest.Title,
                    opt =>
                        opt.MapFrom(src => src.Title.FixPersianChars()));
        }
    }
}
