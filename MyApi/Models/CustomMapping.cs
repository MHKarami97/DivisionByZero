using AutoMapper;
using Entities.User;
using WebFramework.CustomMapping;

namespace MyApi.Models
{
    public class UserCustomMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<User, UserDto>().ReverseMap();

            profile.CreateMap<UserReturnDto, User>().ReverseMap().ForMember(
                dest => dest.Birthday,
                config => config.MapFrom(src => src.Birthday.ToString("d")));
        }
    }
}