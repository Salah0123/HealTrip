using HealTrip.Api.DTOs;
using HealTrip.Api.Services.Doctors;
using System.Text.Json;

namespace HealTrip.Api.Services.Ai.Tools;

public class DoctorSearchTool
{
    private readonly IDoctorService _doctorService;

    public DoctorSearchTool(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task<string> ExecuteAsync(
        string arguments,
        CancellationToken cancellationToken = default)
    {
        var request = JsonSerializer.Deserialize<DoctorSearchRequest>(
            arguments,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (request is null)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "Invalid search parameters."
            });
        }

        var doctors = await _doctorService.SearchAsync(
            request,
            cancellationToken);

        return JsonSerializer.Serialize(new
        {
            success = true,
            count = doctors.Count,
            results = doctors
        });
    }
}