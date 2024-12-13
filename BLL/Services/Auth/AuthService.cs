using BLL.Services.Auth.Descriptors;
using DAL.Exceptions;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public AuthService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task RegisterAsync(RegisterDescriptor descriptor)
        {
            var existsUser = await _userManager.FindByNameAsync(descriptor.Username);
            if (existsUser != null)
            {
                throw new BusinessException(HttpStatusCode.Conflict, "User already exists!");
            }

            var registerUser = new User()
            {
                UserName = descriptor.Username,
                FirstName = descriptor.FirstName,
                LastName = descriptor.LastName,
                Email = descriptor.Email,
                PhoneNumber = descriptor.Phone,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            IdentityResult result = await _userManager.CreateAsync(registerUser, descriptor.Password);
            if (!result.Succeeded)
            {
                string message = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BusinessException(HttpStatusCode.InternalServerError, message);
            }

            await _userManager.AddToRoleAsync(registerUser, "Customer");
        }

        public async Task<string> LoginAsync(LoginDescriptor descriptor)
        {
            User user = await _userManager.FindByNameAsync(descriptor.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, descriptor.Password))
            {
                throw new BusinessException(HttpStatusCode.Unauthorized, "Wrong email or password");
            }

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            JwtSecurityToken accessToken = GenerateAccessToken(authClaims);

            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        private JwtSecurityToken GenerateAccessToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            return new JwtSecurityToken(
                issuer: _configuration.GetSection("AppSettings:ValidIssuer").Value,
                audience: _configuration.GetSection("AppSettings:ValidAudience").Value,
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}
