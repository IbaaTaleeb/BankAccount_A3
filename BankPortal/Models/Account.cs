using System.ComponentModel.DataAnnotations;

namespace BankPortal.Models
{
    public class Account
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Account number must be a 10-digit unique number")]
        public string Number { get; set; }

        [Required]
        [StringLength(35, ErrorMessage = "Holder name cannot exceed 35 characters")]
        public string HolderName { get; set; }

        [Required]
        [RegularExpression(@"^\d{1,13}$", ErrorMessage = "Phone number must be up to 13 digits")]
        public string AssociatedPhoneNumber { get; set; }

        public bool IsActive { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        public decimal Balance { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public int CategoryID { get; set; }  // Assuming this will be mapped from Category
        public string CategoryName { get; set; } // Optional if category name is needed

        [Required]
        public int RoleId { get; set; }  // Role information if needed
    }

}
