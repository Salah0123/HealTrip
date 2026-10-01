namespace HealTrip.Api.Entities;

public class Doctor
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public int YearsOfExperience { get; set; }

    public string Languages { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public int HospitalId { get; set; }

    public Hospital Hospital { get; set; } = null!;
}
