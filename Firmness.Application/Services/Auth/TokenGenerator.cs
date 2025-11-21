using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Firmness.Application.Auth;
using Firmness.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Firmness.Application.Services.Auth;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    
    public TokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(Client user, IList<string> roles)
    {
        //Define the Claims 
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), 
            new Claim(JwtRegisteredClaimNames.Email, user.Email!), 
            new Claim(ClaimTypes.Name, user.UserName!), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
        };

        // adding the roles to the claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // creating the security key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
        // expiration define
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes);

        // create the token 
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        // writing the token how a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
