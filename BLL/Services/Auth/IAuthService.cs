using BLL.Services.Auth.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Auth
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDescriptor descriptor);
        Task<string> LoginAsync(LoginDescriptor descriptor);
    }
}
