using DAL.Exceptions;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace BLL.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound, $"No user found with ID {id}.");
            }
            return user;
        }
    }
}
