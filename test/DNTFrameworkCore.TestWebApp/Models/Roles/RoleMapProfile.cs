using AutoMapper;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;

namespace DNTFrameworkCore.TestWebApp.Models.Roles
{
    public class RoleMapProfile : Profile
    {
        public RoleMapProfile()
        {
            CreateMap<RoleModel, RoleModalViewModel>(MemberList.None);
        }
    }
}