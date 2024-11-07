using BLL.Services.Auth.Descriptors;
using BLL.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FlowerShopApi.DTOs;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var descriptor = _mapper.Map<RegisterDescriptor>(request);
            await _authService.RegisterAsync(descriptor);

            return Ok("User created successfully!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var descriptor = _mapper.Map<LoginDescriptor>(model);
            string token = await _authService.LoginAsync(descriptor);

            return Ok(token);
        }
    }
}
