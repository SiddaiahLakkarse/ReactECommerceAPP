using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(UserManager<AppUser> users, IConfiguration configuration) : ControllerBase
{
    public sealed record RegisterRequest(string Email, string Password, string DisplayName);
    public sealed record LoginRequest(string Email, string Password);

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new AppUser { Id = Guid.NewGuid(), UserName = request.Email, Email = request.Email, DisplayName = request.DisplayName };
        var result = await users.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(result.Errors.Select(x => x.Description));
        return Ok(new { token = CreateToken(user), user = new { user.Id, user.Email, user.DisplayName } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await users.FindByEmailAsync(request.Email);
        if (user is null || !await users.CheckPasswordAsync(user, request.Password)) return Unauthorized();
        return Ok(new { token = CreateToken(user), user = new { user.Id, user.Email, user.DisplayName } });
    }

    private string CreateToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "development-only-secret-key-change-me-32-chars"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(ClaimTypes.Name, user.Email ?? user.UserName ?? string.Empty) };
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(configuration["Jwt:Issuer"] ?? "ECommerce", configuration["Jwt:Audience"] ?? "ECommerce.Client", claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: credentials));
    }
}