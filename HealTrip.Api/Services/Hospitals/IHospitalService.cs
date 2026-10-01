using HealTrip.Api.DTOs;

namespace HealTrip.Api.Services.Hospitals;

public interface IHospitalService
{
    Task<IReadOnlyList<HospitalSearchResult>> SearchAsync(
        HospitalSearchRequest request,
        CancellationToken cancellationToken = default);
}
