using HealTrip.Api.Data;
using HealTrip.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HealTrip.Api.Services.Hospitals;

public class HospitalService : IHospitalService
{
    private readonly ApplicationDbContext _db;

    public HospitalService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<HospitalSearchResult>> SearchAsync(
        HospitalSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Hospitals
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(h =>
                h.City.ToLower() ==
                request.City.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(request.Specialty))
        {
            query = query.Where(h =>
                h.Specialties.ToLower()
                    .Contains(request.Specialty.Trim().ToLower()));
        }

        if (request.HasEmergency.HasValue)
        {
            query = query.Where(h =>
                h.HasEmergency == request.HasEmergency.Value);
        }

        return await query
            .Select(h => new HospitalSearchResult
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                Address = h.Address,
                HasEmergency = h.HasEmergency,
                Specialties = h.Specialties
            })
            .Take(10)
            .ToListAsync(cancellationToken);
    }
}