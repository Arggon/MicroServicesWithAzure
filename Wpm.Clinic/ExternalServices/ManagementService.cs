namespace Wpm.Clinic.Api.ExternalServices;

public class ManagementService(HttpClient client)
{
    public async Task<PetInfo> GetPetInfo(int id)
    {
        var petInfo = await client.GetFromJsonAsync<PetInfo>($"/api/Pets/{id}");
        return petInfo;
    }
}

public record PetInfo(int Id, string Name, int Age, int BreedId);