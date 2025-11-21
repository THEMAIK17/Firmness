using AutoMapper;
using Firmness.Api.DTOs.Clients;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class ClientsController : ControllerBase
{
    private readonly UserManager<Client> _userManager;
    private readonly IMapper _mapper;

    public ClientsController(UserManager<Client> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: api/Clients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
    {
        
        var users = await _userManager.Users.ToListAsync();
            
        // Filter or mapping
        var dtos = _mapper.Map<IEnumerable<ClientDto>>(users);
            
        return Ok(dtos);
    }
        
    // POST: api/Clients 
    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient(CreateClientDto createDto)
    {
        var user = _mapper.Map<Client>(createDto);
            
       
        var result = await _userManager.CreateAsync(user, createDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        await _userManager.AddToRoleAsync(user, "Client");
        
        var returnDto = _mapper.Map<ClientDto>(user);
        return CreatedAtAction(nameof(GetClients), new { id = user.Id }, returnDto);
    }
    // PUT: api/Clients/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(string id, UpdateClientDto updateDto)
    {
        var client = await _userManager.FindByIdAsync(id);
        if (client == null)
        {
            return NotFound($"Client with ID {id} not found.");
        }
        
        _mapper.Map(updateDto, client);
        
        var result = await _userManager.UpdateAsync(client);

        if (!result.Succeeded)
        {
            
            return BadRequest(result.Errors);
        }

        return NoContent(); 
    }

    // DELETE: api/Clients/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(string id)
    {
        var client = await _userManager.FindByIdAsync(id);
        if (client == null)
        {
            return NotFound($"Client with ID {id} not found.");
        }
        
        if (client.Email == "admin@firmeza.com")
        {
            return BadRequest("Cannot delete the default Administrator account.");
        }
     
        var result = await _userManager.DeleteAsync(client);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent(); // 204 No Content
    }
    
}