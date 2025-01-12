using BankAccountAssignment.API.Modules.BankAccountModule.Model.TransactionModel;
using BankAccountAssignment.API.Modules.UserModules.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankAccountAssignment.API.Modules.BankAccountModule.Model
{
    public class BankAccount
    {
        [Key]
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Account number must be a 10-digit unique number")]
        public string Number { get; set; }

        [Required]
        [StringLength(35, ErrorMessage = "Holder name cannot exceed 35 characters")]
        public string HolderName { get; set; }

        [Required]
        [RegularExpression(@"^\d{1,13}$", ErrorMessage = "Phone number must be up to 13 digits")]
        public string AssociatedPhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        public decimal Balance { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }


        // Foreign Key Relationships
        //category table
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        //user table
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }

        //this for assign a role for user when open account 
        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public ICollection<Transactions> Transactions { get; set; } // Associated transactions

    }
}
