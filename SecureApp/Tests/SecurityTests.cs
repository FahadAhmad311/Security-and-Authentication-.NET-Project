using Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SecureApp.Models;
using SecureApp.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class SecurityTests
{
    /// <summary>
    /// Validates user input to ensure incorrect formats fail validation.
    /// </summary>
    [Fact]
    public void ValidateUserInput_ShouldFailIfInvalid()
    {
        var user = new User { Email = "invalidemail" };
        var context = new ValidationContext(user);
        var results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(user, context, results, true);
        Assert.False(isValid);
    }

    /// <summary>
    /// Ensures valid user input passes validation.
    /// </summary>
    [Fact]
    public void ValidateUserInput_ShouldPassIfValid()
    {
        var user = new User { Email = "test@example.com", Password = "SecurePass123" };
        var context = new ValidationContext(user);
        var results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(user, context, results, true);
        Assert.True(isValid);
    }

    /// <summary>
    /// Tests JWT token generation to verify validity.
    /// </summary>
    [Fact]
    public void GenerateJwtToken_ShouldProduceValidToken()
    {
        var key = Encoding.UTF8.GetBytes("YourSecureSecretKey");
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "test@example.com"),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: System.DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Assert.NotNull(tokenString);
    }

    /// <summary>
    /// Ensures role-based authorization is correctly applied.
    /// </summary>
    [Fact]
    public void RoleAuthorization_ShouldDenyAccessForNonAdmins()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "user@example.com"),
            new Claim(ClaimTypes.Role, "User")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        bool hasAdminRole = principal.IsInRole("Admin");
        Assert.False(hasAdminRole);
    }
}
