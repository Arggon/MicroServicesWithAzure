using Microsoft.AspNetCore.Mvc;
using Wpm.Clinic.Api.Application;

namespace Wpm.Clinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultationController(ClinicApplicationService applicationService) : ControllerBase
{
    [HttpGet("start/{patientId}")]
    public async Task<IActionResult> Start(int patientId)
    {
        var result = await applicationService.Handle(patientId);
        return Ok(result);
    }
}

public record StartConsultationCommand(int PatientId);