namespace HealTrip.Api.DTOs;

public class HospitalSearchRequest
{
    public string? City { get; set; }

    public string? Specialty { get; set; }

    public bool? HasEmergency { get; set; }
}
