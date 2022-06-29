using AutoMapper;
using DatingBackEnd.DTOs;
using DatingBackEnd.Entities;

namespace DatingBackEnd.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, AppUserDTO>()
                .ForMember(dest =>dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));

            CreateMap<Photo, PhotoDTO>();

            CreateMap<MemberUpdateDto, AppUser>();

            CreateMap<RegisterDTO, AppUser>();

            CreateMap<AppUser, LikeDTO>();

            CreateMap<Message, MessageDto>()
              .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src =>
                  src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
              .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src =>
                  src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));

            CreateMap<MessageDto, Message>();
        }
    }
}
