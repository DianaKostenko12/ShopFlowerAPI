using BLL.Services.Auth.Descriptors;

namespace BLL.Services.Auth
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDescriptor descriptor);
        Task<string> LoginAsync(LoginDescriptor descriptor);
    }
}
