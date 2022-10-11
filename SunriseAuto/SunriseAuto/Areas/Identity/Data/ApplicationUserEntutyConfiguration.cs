using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SunriseAuto.Areas.Identity.Data;

namespace SunriseAuto.Data
{
    internal class ApplicationUserEntutyConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u=>u.FirstName).HasMaxLength(255);
            builder.Property(u=>u.LastName).HasMaxLength(255);
        }
    }
}