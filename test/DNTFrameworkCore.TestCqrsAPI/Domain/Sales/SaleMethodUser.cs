using DNTFrameworkCore.Domain;
using DNTFrameworkCore.TestCqrsAPI.Domain.Identity;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class SaleMethodUser : Entity
    {
        public User User { get; private set; }
        public long UserId { get; private set; }

        private SaleMethodUser() //Required for ORM
        {
        }

        public SaleMethodUser(long userId)
        {
            UserId = userId;
        }
    }
}