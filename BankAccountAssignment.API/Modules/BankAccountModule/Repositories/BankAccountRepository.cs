using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BankAccountAssignment.API.Modules.BankAccountModule.Model;
using BankAccountAssignment.API.Modules.BankAccountModule.Model.TransactionModel;
using BankAccountAssignment.API.Modules.Data;
using BankAccountAssignment.API.Modules.JWT;
using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;

namespace BankAccountAssignment.API.Modules.BankAccountModule.Repositories
{
    public class BankAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BankAccountRepository(ApplicationDbContext context, JwtTokenService jwtTokenService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> IsUserExistAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
        public async Task<IEnumerable<BankAccount>> SearchAccountsByHolderNameAsync(string holderName)
        {
            // Normalize the holderName to handle case-insensitive search
            var normalizedHolderName = holderName.ToLower();

            return await _context.BankAccounts
                .Where(a => a.HolderName.ToLower().Contains(normalizedHolderName))
                .ToListAsync();
        }


        public async Task<int?> GetCategoryIdAsync(string categoryName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());
            return category?.CategoryId;
        }

        public async Task<bool> UserHasAccountInCategoryAsync(int userId, int categoryId)
        {
            return await _context.BankAccounts.AnyAsync(a => a.UserID == userId && a.CategoryID == categoryId);
        }

        public async Task<BankAccount> CreateNewBankAccountAsync(int userId, int categoryId, OpenAccountRequest request)
        {

            try
            {
                string accountNumber = await GenerateUniqueBankAccountNumberAsync();
                var bankAccount = new BankAccount
                {
                    Number = accountNumber,
                    HolderName = request.HolderName,
                    AssociatedPhoneNumber = request.AssociatedPhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    UserID = userId,
                    RoleId = 1, // default 'Customer' role ID
                    CategoryID = categoryId,
                    Balance = 0,
                    CreationDate = DateTime.Now

                };

                _context.BankAccounts.Add(bankAccount);
                await _context.SaveChangesAsync();

                return bankAccount;
            }
            catch
            {
                return null;
            }
        }

        public async Task<BankAccount> GetBankAccountByNumberAsync(string accountNumber)
        {
            return await _context.BankAccounts.FirstOrDefaultAsync(a => a.Number == accountNumber);
        }

        public async Task<bool> UpdateAccountDetailsAsync(string accountNumber, ModifyAccountDetailsRequest request)
        {
            var account = await GetBankAccountByNumberAsync(accountNumber);
            if (account == null)
                return false;

            account.HolderName = request.HolderName;
            account.AssociatedPhoneNumber = request.AssociatedPhoneNumber;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TransactionResult> DepositToAccountAsync(string accountNumber, TransactionRequest request)
        {
            var account = await GetBankAccountByNumberAsync(accountNumber);
            if (account == null)
            {
                return new TransactionResult(false, "Account not found.");
            }

            var transaction = new Transactions
            {
                ID = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                StatusId = 2,
                Number = account.Number,
                TransactionAmount = request.Amount,
                TransactionTypeId = 1,
                Beneficiary = request.Beneficiary
            };

            account.Balance += request.Amount;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new TransactionResult(true, "Deposit successful.", account.Balance);
        }

        public async Task<TransactionResult> WithdrawFromAccountAsync(string accountNumber, WithdrawRequest request)
        {
            var account = await GetBankAccountByNumberAsync(accountNumber);
            if (account == null)
            {
                return new TransactionResult(false, "Account not found.");
            }

            if (account.Balance < request.Amount)
            {
                return new TransactionResult(false, "Insufficient funds.");
            }

            var transaction = new Transactions
            {
                ID = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                StatusId = 2,
                Number = account.Number,
                TransactionAmount = -request.Amount,
                TransactionTypeId = 2,
                Beneficiary = request.Beneficiary
            };

            account.Balance -= request.Amount;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new TransactionResult(true, "Withdrawal successful.", account.Balance);
        }

        public async Task<Transactions> GetTransactionByIdAsync(Guid id)
        {
            return await _context.Transactions.Include(t => t.Status)
                .Include(t => t.TransactionType).FirstOrDefaultAsync(t => t.ID == id);
        }

        public async Task<IEnumerable<Transactions>> GetAccountTransactionsAsync(
            string accountNumber,
            DateTime? fromDate,
            DateTime? toDate,
            string transactionType)
        {
            var transactionsQuery = _context.Transactions
                .Include(t => t.Status)
                .Include(t => t.TransactionType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                transactionsQuery = transactionsQuery.Where(t => t.Number == accountNumber);
            }

            if (fromDate.HasValue)
            {
                transactionsQuery = transactionsQuery.Where(t => t.Timestamp >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                transactionsQuery = transactionsQuery.Where(t => t.Timestamp <= toDate.Value);
            }

            if (!string.IsNullOrEmpty(transactionType))
            {
                // Mapping transaction type string to TransactionTypeId if provided
                int? transactionTypeId = transactionType.ToLower() switch
                {
                    "deposit" => 1, // Map "Deposit" to TransactionTypeId = 1
                    "withdraw" => 2, // Map "Withdraw" to TransactionTypeId = 2
                    _ => throw new ArgumentException("Invalid transaction type. Please use 'Deposit' or 'Withdraw'.")
                };

                transactionsQuery = transactionsQuery.Where(t => t.TransactionTypeId == transactionTypeId.Value);
            }

            return await transactionsQuery.ToListAsync();
        }

        private async Task<string> GenerateUniqueBankAccountNumberAsync()
        {
            var datePart = DateTime.UtcNow.ToString("yyMMdd");
            var sequence = await _context.GetBankAccountSeqNumber();
            return datePart + sequence.ToString("D4");
        }
        public async Task<bool> DeleteBankAccountAsync(string accountNumber)
        {
            // Find the bank account by account number
            var account = await _context.BankAccounts
                                           .FirstOrDefaultAsync(acc => acc.Number == accountNumber);

            if (account == null)
            {
                // If no account is found, return false
                return false;
            }

            // Remove the account from the DbSet
            _context.BankAccounts.Remove(account);

            try
            {
                // Commit the transaction to persist the deletion
                await _context.SaveChangesAsync();
                return true; // Indicate that the deletion was successful
            }
            catch (Exception)
            {
                // Handle potential errors (e.g., foreign key constraint violations)
                return false;
            }
        }


    }


}
