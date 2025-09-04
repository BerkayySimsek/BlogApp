using BlogApp.Entity;
using BlogApp.Models;

namespace BlogApp.Services.Abstract
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<bool> RegisterAsync(RegisterViewModel model);
        Task<User?> GetUserProfileAsync(string username);
    }
}
