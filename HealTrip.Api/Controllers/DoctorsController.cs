using HealTrip.Api.DTOs;
using HealTrip.Api.Services.Doctors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealTrip.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<DoctorSearchResult>>> Search(
        [FromQuery] DoctorSearchRequest request,
        CancellationToken cancellationToken)
    {
        var doctors = await _doctorService.SearchAsync(
            request,
            cancellationToken);

        return Ok(doctors);
    }
}
