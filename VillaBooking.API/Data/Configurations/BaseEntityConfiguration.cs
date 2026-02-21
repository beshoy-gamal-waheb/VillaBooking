using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VillaBooking.API.Models;

namespace VillaBooking.API.Data.Configurations
{
    public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();
        }
    }
}
