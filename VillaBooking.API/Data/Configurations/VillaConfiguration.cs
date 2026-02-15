using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VillaBooking.API.Models;

namespace VillaBooking.API.Data.Configurations
{
    public class VillaConfiguration : IEntityTypeConfiguration<Villa>
    {
        public void Configure(EntityTypeBuilder<Villa> builder)
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Name)
                .HasMaxLength(50);
            builder.Property(v => v.CreatedDate)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();
        }
    }
}
