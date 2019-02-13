namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public class UserPermission : Permission
    {
        public long UserId { get; set; }
        public User User { get; set; }
    }
}