using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VillaBooking.API.Models;

namespace VillaBooking.API.Data.Configurations
{
    public class VillaAmenitiesConfiguration :BaseEntityConfiguration<VillaAmenities>
    {
        public override void Configure(EntityTypeBuilder<VillaAmenities> builder)
        {
            builder.Property(va => va.Name).HasMaxLength(100);
            base.Configure(builder);
        }
    }
}
