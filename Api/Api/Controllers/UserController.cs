﻿using Api.Data;
using Api.Dtos;
using Api.Entities;
using Api.Mapping;
using Api.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, AppDbContext context,IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterDto registerDto) 
        {
            if (!ModelState.IsValid)return BadRequest(ModelState);

            var user = registerDto.ToEntity();

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Buyer");

            _emailService.SendEmail(new EmailDto { To = registerDto.Email, Subject = "Welcome to Nftfy – Registration Successful!", Body = "<h1>Welcome to Nftfy!</h1><p>We’re excited to have you on board. Your account has been successfully created, and you’re now part of a community where opportunities await.</p>" });

            return Ok("User created successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto) 
        {
            if (!ModelState.IsValid)return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Message = "User not found with this email",
                });
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Message = "Invalid Password."
                });
            }
            var token = GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Message = "Login Success.",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUserDetail()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(currentUserId!);

            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    Message = "User not found"
                });
            }

            return Ok(new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            });
        }

        private string GenerateToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII
            .GetBytes(_configuration.GetSection("JWTSetting").GetSection("securityKey").Value!);

            var roles = _userManager.GetRolesAsync(user).Result;

            List<Claim> claims =
            [
                new (JwtRegisteredClaimNames.Email,user.Email??""),
                new (JwtRegisteredClaimNames.Name,user.FirstName??""),
                new (JwtRegisteredClaimNames.NameId,user.Id ??""),
                new (JwtRegisteredClaimNames.Aud,
                _configuration.GetSection("JWTSetting").GetSection("validAudience").Value!),
                new (JwtRegisteredClaimNames.Iss,_configuration.GetSection("JWTSetting").GetSection("validIssuer").Value!)
            ];


            foreach (var role in roles)

            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
