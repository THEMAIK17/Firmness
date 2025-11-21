using System.ComponentModel.DataAnnotations;

namespace Firmness.Api.DTOs.Auth;

public class LoginDto
{
    [Microsoft.Build.Framework.Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Microsoft.Build.Framework.Required]
    public string Password { get; set; } = string.Empty;
}