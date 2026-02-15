using Microsoft.EntityFrameworkCore;

namespace VillaBooking.API.Data.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {

    }
}
