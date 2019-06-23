using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class SaleMethod : Entity, IHasRowVersion
    {
        public string Title { get; set; }
        public SaleMethodNature Nature { get; set; }
        public byte[] RowVersion { get; set; }
        //...
        
        public ICollection<SaleMethodProduct> Products { get; set; }
        public ICollection<SaleMethodPriceType> PriceTypes { get; set; }
    }
}