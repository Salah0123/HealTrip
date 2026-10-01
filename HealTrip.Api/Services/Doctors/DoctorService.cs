using HealTrip.Api.Data;
using HealTrip.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HealTrip.Api.Services.Doctors;

public class DoctorService : IDoctorService
{
    private readonly ApplicationDbContext _db;

    public DoctorService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DoctorSearchResult>> SearchAsync(
        DoctorSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Doctors
            .AsNoTracking()
            .Include(d => d.Hospital)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Specialty))
        {
            query = query.Where(d =>
                d.Specialty.ToLower() ==
                request.Specialty.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(d =>
                d.City.ToLower() ==
                request.City.Trim().ToLower());
        }

        return await query
            .Select(d => new DoctorSearchResult
            {
                Id = d.Id,
                Name = d.Name,
                Specialty = d.Specialty,
                YearsOfExperience = d.YearsOfExperience,
                Languages = d.Languages,
                City = d.City,
                HospitalName = d.Hospital.Name
            })
            .Take(10)
            .ToListAsync(cancellationToken);
    }
}
