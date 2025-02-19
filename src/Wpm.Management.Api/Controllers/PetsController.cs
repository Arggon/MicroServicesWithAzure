using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PetsController(ManagementDbContext dbContext, ILogger<PetsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pets = await dbContext.Pets.Include(p => p.Breed).ToListAsync();
        return Ok(pets);
    }
    
    [HttpGet("{id}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var pet = await dbContext.Pets.Include(p => p.Breed).FirstOrDefaultAsync(p => p.Id == id);
            return pet == null ? NotFound() : Ok(pet);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get pet by id");
            return StatusCode(500);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(NewPet newPet)
    {
        try
        {
            var pet = newPet.ToPet();
            await dbContext.Pets.AddAsync(pet);
            await dbContext.SaveChangesAsync();
        
            return CreatedAtRoute(nameof(GetById), new { id = pet.Id }, pet);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create pet");
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
        
    }
}

public record NewPet(string Name, int Age, int BreedId)
{
    public Pet ToPet() => new Pet
    {
        Name = Name,
        Age = Age,
        BreedId = BreedId
    };
}
