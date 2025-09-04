using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using BlogApp.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            return await _userRepository.Users
                .FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            var exists = await _userRepository.Users
                .AnyAsync(x => x.UserName == model.UserName || x.Email == model.Email);

            if (exists) return false;

            _userRepository.CreateUser(new User
            {
                UserName = model.UserName,
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                Image = "avatar.jpg"
            });

            return true;
        }

        public async Task<User?> GetUserProfileAsync(string username)
        {
            return await _userRepository.Users
                .Include(x => x.Posts)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.Post)
                .FirstOrDefaultAsync(x => x.UserName == username);
        }
    }
}
