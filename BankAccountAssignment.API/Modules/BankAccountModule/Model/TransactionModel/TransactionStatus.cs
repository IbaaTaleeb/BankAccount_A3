using System.ComponentModel.DataAnnotations;

namespace BankAccountAssignment.API.Modules.BankAccountModule.Model.TransactionModel
{
    public class TransactionStatus
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
    public enum TransactionStatusEnum {
        Pending = 1,
        Completed = 2,
        Failed = 3
    }
}
