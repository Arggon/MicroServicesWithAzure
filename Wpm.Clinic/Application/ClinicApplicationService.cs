using Microsoft.Extensions.Caching.Memory;
using Wpm.Clinic.Api.Controllers;
using Wpm.Clinic.Api.DataAccess;
using Wpm.Clinic.Api.ExternalServices;

namespace Wpm.Clinic.Api.Application;

public class ClinicApplicationService(ClinicDbContext dbContext, ManagementService managementService, IMemoryCache cache)
{
    public async Task<Consultation> Handle(int patientId)
    {
        var petInfo = await cache.GetOrCreateAsync(patientId, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            return await managementService.GetPetInfo(patientId);
        });

        var newConsultation =
            new Consultation(Guid.NewGuid(), patientId, petInfo.Name, petInfo.Age, DateTime.UtcNow);

        await dbContext.Consultations.AddAsync(newConsultation);
        await dbContext.SaveChangesAsync();

        return newConsultation;
    }
}