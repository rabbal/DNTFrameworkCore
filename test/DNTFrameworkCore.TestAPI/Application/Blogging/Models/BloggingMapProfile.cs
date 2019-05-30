using AutoMapper;
using DNTFrameworkCore.TestAPI.Domain.Blogging;

namespace DNTFrameworkCore.TestAPI.Application.Blogging.Models
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
