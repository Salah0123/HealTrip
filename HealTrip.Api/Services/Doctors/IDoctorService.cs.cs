using HealTrip.Api.DTOs;

namespace HealTrip.Api.Services.Doctors;

public interface IDoctorService
{
    Task<IReadOnlyList<DoctorSearchResult>> SearchAsync(
        DoctorSearchRequest request,
        CancellationToken cancellationToken = default);
}