using Microsoft.AspNetCore.Mvc;
using WebBank.Models;
using WebBank.Services;

namespace WebBank.Controllers
{
    public class AuthController : Controller
    {
        private readonly APiUserService _apiService;

        public AuthController()
        {
            _apiService = new APiUserService();
        }

        public IActionResult Login()
        {
            UserLoginDTOs obj = new UserLoginDTOs();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDTOs obj)
        {
            ExpiredUserTokenDTOs objResponse = new ExpiredUserTokenDTOs();
            objResponse = await _apiService.AuthinticateUser(obj);

            if (objResponse != null && objResponse.Token.ToString() != "")
            {
                HttpContext.Session.SetString("APIToken", objResponse.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                HttpContext.Session.SetString("APIToken", "");
            }
            return View(objResponse);
        }



        public IActionResult Logout()
        {
            return View();
        }
    }
}
