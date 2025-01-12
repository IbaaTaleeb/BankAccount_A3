using BankAccountAssignment.API.Modules.BankAccountModule.Model.BankAccountValidations;
using System.ComponentModel.DataAnnotations;

namespace WebBank.Models
{

    public class UserLoginDTOs
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
    public class UserRegistrationDTOs
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters.")]
        public string Password { get; set; }

        [Required]
        [ValidateAge]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public string MobileNumber { get; set; }
    }

}