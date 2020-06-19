namespace DNTFrameworkCore.TestCqrsAPI.Domain.Identity
{
    public class RolePermission : Permission
    {
        public long RoleId { get; set; }
        public Role Role { get; set; }
    }
}