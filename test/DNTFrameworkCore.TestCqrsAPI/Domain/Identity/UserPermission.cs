namespace DNTFrameworkCore.TestCqrsAPI.Domain.Identity
{
    public class UserPermission : Permission
    {
        public long UserId { get; set; }
        public User User { get; set; }
    }
}