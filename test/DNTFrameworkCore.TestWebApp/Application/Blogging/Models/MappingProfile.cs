using AutoMapper;
using DNTFrameworkCore.TestWebApp.Domain.Blogging;

namespace DNTFrameworkCore.TestWebApp.Application.Blogging.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BlogModel, Blog>(MemberList.None)
                .ForMember(d => d.NormalizedTitle, m => m.MapFrom(s => s.Title.ToUpperInvariant()))
                .ReverseMap();
        }
    }
}