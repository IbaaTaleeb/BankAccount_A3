using System.ComponentModel.DataAnnotations;
using BankAccountAssignment.API.Modules.BankAccountModule.Model;

namespace BankAccountAssignment.API.Modules.UserModules.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public string MobileNumber { get; set; }

        //user can have many bankAccounts
        public ICollection<BankAccount> BankAccounts { get; set; }

        // Add PasswordHash for authentication
        public string PasswordHash { get; set; }
    }

}
