using AutoMapper;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using DNTFrameworkCore.TestAPI.Helpers;
using DNTFrameworkCore.Cryptography;
using System;

namespace DNTFrameworkCore.TestAPI.Application.Identity.Models
{
    public class IdentityMapProfile : Profile
    {
        public IdentityMapProfile()
        {
            //Required for ModifiedProperties collection of TrackableEntity and Model
            AllowNullCollections = true;
            
            CreateMap<Role, RoleModel>(MemberList.None).ReverseMap()
                .ForMember(d => d.NormalizedName, m => m.MapFrom(s => s.Name.ToUpperInvariant()));

            CreateMap<User, UserModel>(MemberList.None)
                .ReverseMap()
                .ForMember(d => d.NormalizedDisplayName, m => m.MapFrom(s => s.DisplayName.NormalizePersianTitle()))
                .ForMember(d => d.NormalizedUserName, m => m.MapFrom(s => s.UserName.ToUpperInvariant()));

            CreateMap<UserRole, UserRoleModel>(MemberList.None).ReverseMap();
            CreateMap<UserPermission, PermissionModel>(MemberList.None).ReverseMap();
            CreateMap<RolePermission, PermissionModel>(MemberList.None).ReverseMap();
        }
    }
}