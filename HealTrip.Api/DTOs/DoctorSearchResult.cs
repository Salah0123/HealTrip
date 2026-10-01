namespace HealTrip.Api.DTOs;

public class DoctorSearchResult
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public int YearsOfExperience { get; set; }

    public string Languages { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string HospitalName { get; set; } = string.Empty;
}
