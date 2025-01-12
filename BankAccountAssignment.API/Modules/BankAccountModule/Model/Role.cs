using BankAccountAssignment.API.Modules.UserModules.Model;

namespace BankAccountAssignment.API.Modules.BankAccountModule.Model
{
    public class Role
    {
        public int Id { get; set; } // Primary Key
        public string RoleName { get; set; }
    }
    public enum RoleTypeEnum {
        Customer = 1,
        Teller = 2,
        Admin = 3        
    }
}
