using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain;

namespace MinimalApi.Context
{
    public class LocationContext : DbContext
    {
        public LocationContext(DbContextOptions<LocationContext> options) : base(options)
        {
        }
        public DbSet<Location> Location { get; set; }
    }
}