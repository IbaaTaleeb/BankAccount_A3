using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BankAccountAssignment.API.Modules.BankAccountModule.Model;

namespace BankAccountAssignment.API.Modules.BankAccountModule.Model.TransactionModel
{
    public class Transactions
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [MaxLength(100)]
        public string Beneficiary { get; set; }

        [Required]
        public decimal TransactionAmount { get; set; }

        // ForignKey for TransactionStatus table
        [Required]
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public TransactionStatus Status { get; set; }

        // Foreign key to BankAccount table
        [Required]
        public string Number { get; set; }
        [ForeignKey("Number")]
        public BankAccount BankAccount { get; set; }


        // ForignKey for TransactionType table
        [Required]
        public int TransactionTypeId { get; set; }
        [ForeignKey("TransactionTypeId")]
        public TransactionType TransactionType { get; set; }


    }
}
