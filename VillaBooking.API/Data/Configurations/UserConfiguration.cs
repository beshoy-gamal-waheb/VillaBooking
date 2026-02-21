using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VillaBooking.API.Models;

namespace VillaBooking.API.Data.Configurations
{
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Name).HasMaxLength(100);
            builder.Property(u => u.Role).HasMaxLength(50);
            base.Configure(builder);
        }
    }
}
