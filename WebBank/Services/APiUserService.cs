using BankAccountAssignment.API.Modules.UserModules.Model;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebBank.Models;

namespace WebBank.Services
{
    public class APiUserService
    {
        private readonly HttpClient _httpClient;
        private string _APIUrl = "https://localhost:7273/";
        public APiUserService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_APIUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        }
        public async Task<ExpiredUserTokenDTOs> AuthinticateUser(UserLoginDTOs userDetailes)
        {
            HttpResponseMessage response = await _httpClient.
                PostAsJsonAsync($"api/Account/login", userDetailes);
            response.EnsureSuccessStatusCode();

            var contents = await response.Content.ReadAsStringAsync();  
            var APIResponse = JsonConvert.DeserializeObject<ExpiredUserTokenDTOs>(contents);
            return APIResponse;

        }


    }
}
