using DNTFrameworkCore.TestTenancy.Domain.Blogging;

namespace DNTFrameworkCore.TestTenancy.Application.Blogging.Models
{
    public class BloggingMapProfile : Profile
    {
        public BloggingMapProfile()
        {
            //Required for ModifiedProperties collection of TrackableEntity and Model
            AllowNullCollections = true;
            
            CreateMap<Blog, BlogModel>(MemberList.None)
                .ReverseMap()
                .ForMember(d => d.NormalizedTitle, m => m.MapFrom(s => s.Title.ToUpperInvariant()));
        }
    }
}