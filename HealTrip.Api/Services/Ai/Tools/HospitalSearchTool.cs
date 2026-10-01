using HealTrip.Api.DTOs;
using HealTrip.Api.Services.Hospitals;
using System.Text.Json;

namespace HealTrip.Api.Services.Ai.Tools;

public class HospitalSearchTool
{
    private readonly IHospitalService _hospitalService;

    public HospitalSearchTool(IHospitalService hospitalService)
    {
        _hospitalService = hospitalService;
    }

    public async Task<string> ExecuteAsync(
        string arguments,
        CancellationToken cancellationToken = default)
    {
        var request = JsonSerializer.Deserialize<HospitalSearchRequest>(
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

        var hospitals = await _hospitalService.SearchAsync(
            request,
            cancellationToken);

        return JsonSerializer.Serialize(new
        {
            success = true,
            count = hospitals.Count,
            results = hospitals
        });
    }
}
