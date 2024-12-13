using AutoMapper;
using BLL.Services.Users;
using DAL.Exceptions;
using FlowerShopApi.Common.Extensions;
using FlowerShopApi.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetUserById()
        {
            int userId = _httpContextAccessor.HttpContext.User.GetUserId();
            try
            {
                var user = await _userService.GetUserById(userId);
                var userDto = _mapper.Map<UserResponse>(user);
                return Ok(userDto);
            }
            catch (BusinessException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
}
