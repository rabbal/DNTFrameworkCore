using AutoMapper;
using ProjectName.Domain.Identity;

namespace ProjectName.Application.Identity.Models
{
    public class IdentityMapProfile : Profile
    {
        public IdentityMapProfile()
        {
            //Required for ModifiedProperties collection of TrackableEntity and Model
            AllowNullCollections = true;

            CreateMap<Role, RoleModel>(MemberList.None).ReverseMap()
                .ForMember(d => d.NormalizedName, m => m.MapFrom(s => Role.NormalizeName(s.Name)));

            CreateMap<User, UserModel>(MemberList.None)
                .ReverseMap()
                .ForMember(d => d.NormalizedDisplayName, m => m.MapFrom(s => User.NormalizeDisplayName(s.DisplayName)))
                .ForMember(d => d.NormalizedUserName, m => m.MapFrom(s => User.NormalizeUserName(s.UserName)));

            CreateMap<UserRole, UserRoleModel>(MemberList.None).ReverseMap();
            CreateMap<UserPermission, PermissionModel>(MemberList.None).ReverseMap();
            CreateMap<RolePermission, PermissionModel>(MemberList.None).ReverseMap();
        }
    }
}