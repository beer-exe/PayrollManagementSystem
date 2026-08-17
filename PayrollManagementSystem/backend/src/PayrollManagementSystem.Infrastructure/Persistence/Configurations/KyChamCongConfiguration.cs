using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Infrastructure.Persistence.Configurations
{
    public class KyChamCongConfiguration : IEntityTypeConfiguration<KyChamCong>
    {
        public void Configure(EntityTypeBuilder<KyChamCong> builder)
        {
            builder.ToTable("ky_cham_congs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TrangThai)
                .HasConversion<string>()
                .IsRequired();
                
            builder.HasMany(x => x.ChamCongs)
                .WithOne(x => x.KyChamCong)
                .HasForeignKey(x => x.IdKyChamCong)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
