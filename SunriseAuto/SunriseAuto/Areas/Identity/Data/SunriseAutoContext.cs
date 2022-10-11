using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SunriseAuto.Areas.Identity.Data;
using SunriseAuto.Models;

namespace SunriseAuto.Data;

public class SunriseAutoContext : IdentityDbContext<User>
{
    public SunriseAutoContext(DbContextOptions<SunriseAutoContext> options): base(options)
    {

    }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Models.Type> Types { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<UserCar> UserCars { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserCar>()
     .HasKey(t => new { t.UserId, t.CarId });
        modelBuilder.Entity<UserCar>()
            .HasOne(pt => pt.User)
            .WithMany(p => p.Favs)
            .HasForeignKey(pt => pt.UserId);
        modelBuilder.Entity<UserCar>()
            .HasOne(pt => pt.Car)
            .WithMany(t => t.Favs)
            .HasForeignKey(pt => pt.CarId);
        
    }

    //protected override void OnModelCreating(ModelBuilder builder)
    //{
    //base.OnModelCreating(builder);
    // Customize the ASP.NET Identity model and override the defaults if needed.
    // For example, you can rename the ASP.NET Identity table names and more.
    // Add your customizations after calling base.OnModelCreating(builder);


    //builder.ApplyConfiguration(new ApplicationUserEntutyConfiguration());
    //}
}
