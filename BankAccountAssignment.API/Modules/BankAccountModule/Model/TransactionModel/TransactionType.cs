using System.ComponentModel.DataAnnotations;

namespace BankAccountAssignment.API.Modules.BankAccountModule.Model.TransactionModel
{
    public class TransactionType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
    public enum TransactionTypeEnum {
        Deposit = 1,
        Withdraw = 2
    }
}
