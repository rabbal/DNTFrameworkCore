using AutoMapper;
using DNTFrameworkCore.TestWebApp.Domain.Catalog;

namespace DNTFrameworkCore.TestWebApp.Application.Catalog.Models
{
    public class CatalogMapProfile: Profile
    {
        public CatalogMapProfile()
        {
           CreateMap<Product, ProductModel>(MemberList.None)
                .ReverseMap();
        }
    }
}
