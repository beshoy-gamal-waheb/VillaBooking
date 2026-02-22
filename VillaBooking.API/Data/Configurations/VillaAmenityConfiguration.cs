using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VillaBooking.API.Models;

namespace VillaBooking.API.Data.Configurations
{
    public class VillaAmenityConfiguration :BaseEntityConfiguration<VillaAmenity>
    {
        public override void Configure(EntityTypeBuilder<VillaAmenity> builder)
        {
            builder.Property(va => va.Name).HasMaxLength(100);
            base.Configure(builder);
        }
    }
}
