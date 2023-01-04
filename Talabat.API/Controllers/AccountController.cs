using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.API.Extensions;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities.Identity;

namespace Talabat.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) return BadRequest(new ApiResponse(404));
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if(!result.Succeeded) return Unauthorized(new ApiResponse(401));
            var userDto = new UserDto()
            {
                Email = loginDto.Email,
                DisplayName = user.DisplayName,
                Token = await _tokenService.CreateToken(user, _userManager),
            };
            return Ok(userDto);
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            if (CheckEmailExistAsync(registerDto.Email).Result.Value)
                return new BadRequestObjectResult(new ApiValidationErrorResponse() { Errors = new[] {"Email address already in use"}});
            var user = new AppUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.Email.Split("@")[0],
                DisplayName = registerDto.DisplayName,
                Address = new UserAddress()
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    City = registerDto.City,
                    Country = registerDto.Country,
                    Street = registerDto.Street,
                    ZipCode = registerDto.ZipCode,
                }
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            var userDto = new UserDto()
            {
                Email = registerDto.Email,
                DisplayName = user.DisplayName,
                Token = await _tokenService.CreateToken(user, _userManager),
            };
            return Ok(userDto);
        }

        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExistAsync([FromQuery] string email)
            => await _userManager.FindByEmailAsync(email) != null;

        [Authorize]
        [HttpGet("getCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            var userDto = new UserDto()
            {
                Email = email,
                DisplayName = user.DisplayName,
                Token = await _tokenService.CreateToken(user, _userManager)
            };
            return Ok(userDto);
        }

        [Authorize]
        [HttpGet("getUserAddress")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindByEmailWithAddressAsync(User);
            var addressDto = _mapper.Map<UserAddress, AddressDto>(user.Address);
            return Ok(addressDto);
        }

        [Authorize]
        [HttpPut("updateUserAddress")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        {
            var user = await _userManager.FindByEmailWithAddressAsync(User);
            user.Address = _mapper.Map<AddressDto, UserAddress>(addressDto);
            var result = await _userManager.UpdateAsync(user);
            if(result.Succeeded)
                return Ok(new ApiResponse(200, "User address updated successfully"));
            return BadRequest(new ApiResponse(400, "An error occured while updating address"));
        }
    }
}
