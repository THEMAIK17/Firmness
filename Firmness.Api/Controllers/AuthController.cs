using Firmness.Api.DTOs.Auth;
using Firmness.Application.Services.Auth;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<Client> _userManager;
    private readonly SignInManager<Client> _signInManager;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthController(
        UserManager<Client> userManager, 
        SignInManager<Client> signInManager, 
        ITokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        // search the client by email
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Unauthorized("Usuario o contraseña incorrectos.");
        }

        // verify the password
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Usuario o contraseña incorrectos.");
        }

        // get the roles of the user
        var roles = await _userManager.GetRolesAsync(user);

        // generate the token
        var token = _tokenGenerator.GenerateToken(user, roles);

        
        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName, 
            Roles = roles
        });
    }
}