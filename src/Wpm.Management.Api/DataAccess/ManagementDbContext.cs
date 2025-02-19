using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Wpm.Management.Api.DataAccess;

public class ManagementDbContext(DbContextOptions<ManagementDbContext> options) : DbContext(options)
{
    public required DbSet<Pet> Pets { get; set; }
    public DbSet<Breed> Breeds { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Breed>().HasData(
            new Breed(1, "Dog"),
            new Breed(2, "Cat"),
            new Breed(3, "Fish")
        );
        
        modelBuilder.Entity<Pet>().HasData(
            new Pet { Id = 1, Name = "Fido", Age = 3, BreedId = 1 },
            new Pet { Id = 2, Name = "Whiskers", Age = 2, BreedId = 2 },
            new Pet { Id = 3, Name = "Goldie", Age = 1, BreedId = 3 }
        );
    }
}

public static class ManagementDbContextExtensions
{
    public static void EnsureDbIsCreated(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetService<ManagementDbContext>();
        context!.Database.EnsureCreated();
    }
}

public class Pet
{ 
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int Age { get; set; }
    public required int BreedId { get; set; }
    public Breed Breed { get; set; }
}

public record Breed(int Id, string Name);