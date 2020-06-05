using AutoMapper;
using DNTFrameworkCore.TestWebApp.Domain.Catalog;

namespace DNTFrameworkCore.TestWebApp.Application.Catalog.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductModel>(MemberList.None)
                .ReverseMap();
        }
    }
}