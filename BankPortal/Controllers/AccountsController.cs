using Microsoft.AspNetCore.Mvc;
using BankPortal.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BankAccountAssignment.API.Modules.BankAccountModule.Model;

namespace BankPortal.Controllers
{
    public class AccountsController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new System.Uri("https://localhost:7273/api/BankAccounts/");
        }

        // Default page(Accounts) to view account or add a new one
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Show form to add a new account
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // Handle adding a new account
        [HttpPost]
        public async Task<IActionResult> Add(OpenAccountRequest accountRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(accountRequest);
            }

            var response = await _httpClient.PostAsJsonAsync("OpenAccount", accountRequest);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Account created successfully!";

                return RedirectToAction("Index");
            }
            // Add specific error message to ModelState based on the response from the API
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);

            return View(accountRequest);
        }


        ////Search for account
        public async Task<IActionResult> Search(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                ModelState.AddModelError(string.Empty, "Account number is required.");
                return View("Index");
            }

            var response = await _httpClient.GetAsync($"GetAccountDetailsByNumber/{accountNumber}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return View("Index");
            }

            var account = await response.Content.ReadFromJsonAsync<BankAccount>();
            if (account == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return View("Index");
            }

            return View("Details", account); // Display account details
        }

        ////DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(string accountNumber)
        {
            var response = await _httpClient.DeleteAsync($"DeleteAccount/{accountNumber}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Account deleted successfully!";
                return RedirectToAction("Index");
            }

            // Add specific error message to ModelState based on the response from the API
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);

            return RedirectToAction("Details", new { accountNumber });
        }





        [HttpGet]

        public async Task<IActionResult> Edit(string accountNumber)
        {
            var response = await _httpClient.GetAsync($"GetAccountDetailsByNumber/{accountNumber}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound("Account not found.");
            }

            var account = await response.Content.ReadFromJsonAsync<BankAccount>();
            if (account == null)
            {
                return NotFound("Account not found.");
            }

            return View(account); // Returns a form for editing account details
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BankAccount account)
        {
            var modifyRequest = new ModifyAccountDetailsRequest
            {
                HolderName = account.HolderName,
                AssociatedPhoneNumber = account.AssociatedPhoneNumber
                
            };

            var response = await _httpClient.PatchAsJsonAsync($"ModifyAccountDetails/{account.Number}", modifyRequest);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update account.");
                return View(account);
            }
            TempData["SuccessMessage"] = "Account updated successfully!";
            return RedirectToAction("Index");
        }



    }
}
