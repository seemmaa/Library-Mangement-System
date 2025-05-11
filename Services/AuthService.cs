using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using LibraryManagementSystem.Services.Interfaces;

namespace LibraryManagementSystem.Services
{
    public class AuthService:IAuth
    {
        private readonly LibraryContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            LibraryContext context,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, string Message)> SignupAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return (false, "Email already registered.");

            if (!await _roleManager.RoleExistsAsync("Member"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("Member"));
                if (!roleResult.Succeeded)
                    return (false, "Failed to create role.");
            }

            
            var generatedId = Guid.NewGuid();

            var identityUser = new IdentityUser
            {
                Id = generatedId.ToString(), 
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

           
            var user2 = new User
            {
                Id = generatedId, 
                Email = model.Email,
                Password = model.Password, 
                Role = "Member",
                IsActive = true
            };
            _context.Users.Add(user2);
            await _context.SaveChangesAsync();

            var roleResultAssign = await _userManager.AddToRoleAsync(identityUser, "Member");
            if (!roleResultAssign.Succeeded)
                return (false, "Failed to assign role.");

            return (true, "User registered successfully.");
        }


        public async Task<(bool IsSuccess, string TokenOrMessage)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogWarning("Login failed: No user found with email {Email} or invalid password", email);
                return (false, "Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            _logger.LogInformation("Login successful for user {Email}", email);
            return (true, token);
        }

        private string GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email ?? "")
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
