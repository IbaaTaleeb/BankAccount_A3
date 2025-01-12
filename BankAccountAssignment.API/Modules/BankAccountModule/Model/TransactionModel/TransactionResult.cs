namespace BankAccountAssignment.API.Modules.BankAccountModule.Model.TransactionModel
{
    public class TransactionResult
    {
        public bool Success { get; }
        public string Message { get; }
        public decimal? NewBalance { get; }

        public TransactionResult(bool success, string message, decimal? newBalance = null)
        {
            Success = success;
            Message = message;
            NewBalance = newBalance;
        }
    }
}
