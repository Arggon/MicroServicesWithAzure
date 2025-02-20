using Wpm.Clinic.Api.Controllers;
using Wpm.Clinic.Api.DataAccess;
using Wpm.Clinic.Api.ExternalServices;

namespace Wpm.Clinic.Api.Application;

public class ClinicApplicationService(ClinicDbContext dbContext, ManagementService managementService)
{
    public async Task<Consultation> Handle(int patientId)
    {
        var petInfo = await managementService.GetPetInfo(patientId);

        var newConsultation =
            new Consultation(Guid.NewGuid(), patientId, petInfo.Name, petInfo.Age, DateTime.UtcNow);

        await dbContext.Consultations.AddAsync(newConsultation);
        await dbContext.SaveChangesAsync();

        return newConsultation;
    }
}