using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers;

public class BreedsController(ManagementDbContext dbContext, ILogger<BreedsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var breeds = await dbContext.Breeds.ToListAsync();
        return Ok(breeds);
    }

    [HttpGet("{id}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var breed = await dbContext.Breeds.FindAsync(id);
            return breed == null ? NotFound() : Ok(breed);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get breed by id");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewBreed newBreed)
    {
        try
        {
            var breed = newBreed.ToBreed();
            await dbContext.Breeds.AddAsync(breed);
            await dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetById), new { id = breed.Id }, newBreed);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create breed");
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}

public record NewBreed(string Name)
{
    public Breed ToBreed()
    {
        return new Breed(0, Name);
    }
}