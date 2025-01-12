namespace BankAccountAssignment.API.Modules.UserModules.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(UserRegistration registration);
        Task<string> LoginUserAsync(UserLogin login);
        Task<bool> LogoutUserAsync(string token);

    }
}
