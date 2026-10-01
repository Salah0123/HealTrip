using HealTrip.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealTrip.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Hospital> Hospitals => Set<Hospital>();

    public DbSet<Doctor> Doctors => Set<Doctor>();
}
