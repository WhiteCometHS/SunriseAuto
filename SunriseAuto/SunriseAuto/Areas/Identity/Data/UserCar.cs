using SunriseAuto.Models;
using System.ComponentModel.DataAnnotations;

namespace SunriseAuto.Areas.Identity.Data
{
    public class UserCar
    {
        public User User { get; set; }
        public string UserId { get; set; }
        public Car Car { get; set; }
        public int CarId { get; set; }
    }
}
