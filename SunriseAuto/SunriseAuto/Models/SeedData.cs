using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SunriseAuto.Data;

namespace SunriseAuto.Models
{
    public class SeedData
    {
        private static List<Type> types;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new SunriseAutoContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SunriseAutoContext>>()))
            {
                /*
                if (!context.Types.Any())
                {
                    context.Types.AddRange(Types.Select(c => c));
                }
                if (!context.Cars.Any())
                {
                    context.Cars.AddRange(
                        new Car
                        {
                            Name = "Nissan Skyline R32",
                            ShortDesc = "",
                            LongDesc = "",
                            ImageName = "R32.jpeg",
                            Price = 7000,
                            Available = true,
                            TypeId = types.ElementAt(0).Id
                        },
                        new Car
                        {
                            Name = "Mazda RX-7 FD3S",
                            ShortDesc = "",
                            LongDesc = "",
                            ImageName = "RX7.jpg",
                            Price = 50000,
                            Available = true,
                            TypeId = types.ElementAt(0).Id
                        },
                        new Car
                        {
                            Name = "Honda S2000",
                            ShortDesc = "",
                            LongDesc = "",
                            ImageName = "s2000.jpeg",
                            Price = 50000,
                            Available = true,
                            TypeId = types.ElementAt(0).Id
                        },
                        new Car
                        {
                            Name = "Nissan Skyline R33",
                            ShortDesc = "",
                            LongDesc = "",
                            ImageName = "R33.jpeg",
                            Price = 50000,
                            Available = true,
                            TypeId = types.ElementAt(0).Id
                        },
                        new Car
                        {
                            Name = "Peugeot Expert III",
                            ShortDesc = "",
                            LongDesc = "",
                            ImageName = "expert 3.jpg",
                            Price = 50000,
                            Available = true,
                            TypeId = types.ElementAt(0).Id
                        }
                    );
                }*/
                context.SaveChanges();
            }
        }
        public static List<Type> Types
        {
            get
            {
                if (types == null)
                {
                    var list = new Type[]
                    {
                        new Type { Name="Coups", ImageName = "sedans.jpg" },
                        new Type { Name = "Trucks", ImageName = "trucks.jpg" }
                    };

                    types = new List<Type>();
                    foreach (Type e in list)
                        types.Add(e);
                }
                return types;
            }
        }
    }
}
