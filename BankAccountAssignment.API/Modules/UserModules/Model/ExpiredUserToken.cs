using System.ComponentModel.DataAnnotations;

namespace BankAccountAssignment.API.Modules.UserModules.Model {
    public class ExpiredUserToken {
        [Key]
        public int Id { get; set; } 
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}
