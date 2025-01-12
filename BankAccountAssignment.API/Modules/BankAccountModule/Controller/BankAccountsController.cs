using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccountAssignment.API.Modules.BankAccountModule.Repositories;
using BankAccountAssignment.API.Modules.UserModules.Services;
using BankAccountAssignment.API.Modules.BankAccountModule.Model;
using BankAccountAssignment.API.Modules.BankAccountModule.Model.TransactionModel;
using Microsoft.AspNetCore.Authorization;
using BankAccountAssignment.API.Exceptions;
namespace BankAccountAssignment.API.Modules.BankAccountModule.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly BankAccountRepository _repository;

        public BankAccountsController(BankAccountRepository repository)
        {
            _repository = repository;
        }

        //[Authorize]
        [HttpPost("OpenAccount")]
        public async Task<ActionResult<BankAccount>> OpenAccount([FromBody] OpenAccountRequest request)
        {/*
            var userIdClaim = User.FindFirst("UserId")?.Value; //find the Id from the JWT userId claim
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid or missing user ID.");
            }
            */

            if (!await _repository.IsUserExistAsync(request.userId))
            {
                return NotFound($"User with ID {request.userId} does not exist.");
            }

            var categoryId = await _repository.GetCategoryIdAsync(request.CategoryName);
            if (categoryId == null)
            {
                return BadRequest("Invalid category name.");
            }


            if (await _repository.UserHasAccountInCategoryAsync(request.userId, categoryId.Value))
            {
                return Conflict("User already has an account in this category.");
            }

            var bankAccount = await _repository.CreateNewBankAccountAsync(request.userId, categoryId.Value, request);
            if (bankAccount == null)
            {
                return StatusCode(500, "Error creating account.");
            }

            return CreatedAtAction(nameof(GetAccountDetailsByNumber), new { accountNumber = bankAccount.Number }, bankAccount);
        }

        [HttpDelete("DeleteAccount/{accountNumber}")]
        public async Task<IActionResult> DeleteAccount(string accountNumber)
        {
            var result = await _repository.DeleteBankAccountAsync(accountNumber);
            if (!result)
            {
                return NotFound(new { message = "Account not found." });
            }

            return Ok(new { message = "Account deleted successfully." });
        }
        //[Authorize]
        [HttpGet("GetAccountDetailsByNumber/{accountNumber}")]
        public async Task<ActionResult<BankAccount>> GetAccountDetailsByNumber(string accountNumber)
        {
            var account = await _repository.GetBankAccountByNumberAsync(accountNumber);
            if (account == null)
            {
                throw new NotFoundException(nameof(GetAccountDetailsByNumber), accountNumber);

            }

            return Ok(account);
        }
        [HttpGet("SearchAccountsByHolderName")]
        public async Task<ActionResult<IEnumerable<BankAccount>>> SearchAccountsByHolderName([FromQuery] string holderName)
        {
            if (string.IsNullOrEmpty(holderName))
            {
                return BadRequest("Holder name cannot be empty.");
            }

            var accounts = await _repository.SearchAccountsByHolderNameAsync(holderName);
            if (accounts == null || !accounts.Any())
            {
                return NotFound($"No accounts found for holder name '{holderName}'.");
            }

            return Ok(accounts);
        }


        [HttpPatch("ModifyAccountDetails/{accountNumber}")]
        public async Task<IActionResult> ModifyAccountDetails(string accountNumber, [FromBody] ModifyAccountDetailsRequest request)
        {
            var updated = await _repository.UpdateAccountDetailsAsync(accountNumber, request);
            if (!updated)
            {
                return NotFound("Account not found.");
            }

            return NoContent();
        }


        [HttpPatch("Deposit/{accountNumber}")]
        public async Task<IActionResult> Deposit(string accountNumber, [FromBody] TransactionRequest request)
        {
            var result = await _repository.DepositToAccountAsync(accountNumber, request);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message, newBalance = result.NewBalance });
        }


        [HttpPatch("Withdraw/{accountNumber}")]
        public async Task<IActionResult> Withdraw(string accountNumber, [FromBody] WithdrawRequest request)
        {
            var result = await _repository.WithdrawFromAccountAsync(accountNumber, request);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message, newBalance = result.NewBalance });
        }


        [HttpGet("GetTransactionByID/{id}")]
        public async Task<ActionResult<Transactions>> GetTransactionByID(Guid id)
        {
            var transaction = await _repository.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                throw new NotFoundException(nameof(GetTransactionByID), id.ToString());
            }

            return Ok(transaction);
        }


        [HttpGet("GetAccountTransactions")]
        public async Task<ActionResult<IEnumerable<Transactions>>> GetAccountTransactions(
            [FromQuery] string accountNumber,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string transactionType = null)
        {
            var transactions = await _repository.GetAccountTransactionsAsync(accountNumber, fromDate, toDate, transactionType);

            if (transactions == null || !transactions.Any())
            {
                throw new NotFoundException(nameof(GetAccountTransactions), accountNumber);
            }

            return Ok(transactions);
        }



    }
}
