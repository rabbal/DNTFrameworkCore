using AutoMapper;
using DNTFrameworkCore.TestWebApp.Domain.Blogging;

namespace DNTFrameworkCore.TestWebApp.Application.Blogging.Models
{
    public class BloggingMapProfile : Profile
    {
        public BloggingMapProfile()
        {
           CreateMap<Blog, BlogModel>(MemberList.None)
                .ReverseMap()
                .ForMember(d => d.NormalizedTitle, m => m.MapFrom(s => s.Title.ToUpperInvariant()));
        }
    }
}
