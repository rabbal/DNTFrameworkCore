using System.Linq;
using AutoMapper;
using DNTFrameworkCore.TestWebApp.Domain.Identity;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Required for ModifiedProperties collection of TrackableEntity and Model
            AllowNullCollections = true;

            CreateMap<Role, RoleModel>(MemberList.None)
                .ReverseMap()
                .ForMember(d => d.NormalizedName, m => m.MapFrom(s => s.Name.ToUpperInvariant()));

            CreateMap<User, UserModel>(MemberList.None)
                .ForMember(d => d.Permissions, m => m.MapFrom(s => s.Permissions.Where(p => p.IsGranted)))
                .ForMember(d => d.IgnoredPermissions, m => m.MapFrom(s => s.Permissions.Where(p => !p.IsGranted)))
                .ReverseMap()
                .ForMember(d => d.Permissions, m => m.Ignore())
                .ForMember(d => d.NormalizedDisplayName, m => m.MapFrom(s => s.DisplayName.ToUpperInvariant()))
                .ForMember(d => d.NormalizedUserName, m => m.MapFrom(s => s.UserName.ToUpperInvariant()));
        }
    }
}