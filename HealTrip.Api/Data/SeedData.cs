using HealTrip.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealTrip.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext db)
    {
        if (await db.Hospitals.AnyAsync())
            return;

        var hospitals = new List<Hospital>
        {
            new()
            {
                Name = "HealTrip Medical Center",
                City = "Riyadh",
                Address = "King Fahd Road, Riyadh",
                HasEmergency = true,
                Specialties = "Cardiology,Internal Medicine,Emergency"
            },
            new()
            {
                Name = "Riyadh Care Hospital",
                City = "Riyadh",
                Address = "Olaya Street, Riyadh",
                HasEmergency = true,
                Specialties = "Cardiology,Neurology,Orthopedics"
            },
            new()
            {
                Name = "Jeddah Health Hospital",
                City = "Jeddah",
                Address = "Prince Sultan Road, Jeddah",
                HasEmergency = true,
                Specialties = "Cardiology,Internal Medicine"
            }
        };

        await db.Hospitals.AddRangeAsync(hospitals);
        await db.SaveChangesAsync();

        var doctors = new List<Doctor>
        {
            new()
            {
                Name = "Dr. Ahmed Hassan",
                Specialty = "Cardiology",
                YearsOfExperience = 12,
                Languages = "Arabic,English",
                City = "Riyadh",
                HospitalId = hospitals[0].Id
            },
            new()
            {
                Name = "Dr. Sara Mohammed",
                Specialty = "Cardiology",
                YearsOfExperience = 9,
                Languages = "Arabic,English",
                City = "Riyadh",
                HospitalId = hospitals[1].Id
            },
            new()
            {
                Name = "Dr. Omar Ali",
                Specialty = "Internal Medicine",
                YearsOfExperience = 15,
                Languages = "Arabic,English",
                City = "Jeddah",
                HospitalId = hospitals[2].Id
            }
        };

        await db.Doctors.AddRangeAsync(doctors);
        await db.SaveChangesAsync();
    }
}