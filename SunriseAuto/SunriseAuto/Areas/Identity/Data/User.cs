using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using SunriseAuto.Models;

namespace SunriseAuto.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    [StringLength(100)]
    public string FirstName { get; set; }
    [StringLength(100)]
    public string LastName { get; set; }
    [StringLength(250)]
    public string Address { get; set; }
    public ICollection<UserCar>? Favs { get; set; }
    public User()
    {
        Favs = new List<UserCar>();
    }
}

