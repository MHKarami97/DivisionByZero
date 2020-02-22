using AutoMapper;
using Entities.User;
using Models.CustomMapping;

namespace Models.Models
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