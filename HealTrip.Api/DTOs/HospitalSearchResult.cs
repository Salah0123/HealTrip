namespace HealTrip.Api.DTOs;

public class HospitalSearchResult
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public bool HasEmergency { get; set; }

    public string Specialties { get; set; } = string.Empty;
}
