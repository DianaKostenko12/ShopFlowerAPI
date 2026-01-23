using DAL.Models;

namespace BLL.Services.Users
{
    public interface IUserService
    {
       Task<User> GetUserById(int id);
    }
}
