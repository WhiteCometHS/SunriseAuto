using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunriseAuto.Areas.Identity.Data;
using SunriseAuto.Data;
using SunriseAuto.Models;

namespace SunriseAuto.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize]
    public class FavController : Controller
    {
        private readonly SunriseAutoContext _context;
        private readonly UserManager<User> _userManager;

        public FavController(SunriseAutoContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> List(string searchString)
        {
            List<Car> temp= await _context.Cars.ToListAsync();
            var list = await _context.UserCars.ToListAsync();
            List<Car> cars= new List<Car>();
            foreach (Car c in temp)
            {
                foreach (var i in list)
                {
                    if (i.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)) && i.CarId == c.Id)
                    {
                        cars.Add(c);
                    }
                }
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(s => s.Name!.Contains(searchString)).ToList();
            }
            return View(cars);
        }
        public async Task<IActionResult> Add(int id)
        {
            Car car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            UserCar userCar = new UserCar()
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CarId = id,
                User = await _userManager.GetUserAsync(HttpContext.User),
                Car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id)
            };
            car.Favs.Add(userCar);
            _context.Update(car);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Index", new { Area = "", Controller = "Car", Action = "Index" });
        }
        public async Task<IActionResult> Delete(int id)
        {
            Car car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            UserCar userCar = await _context.UserCars.FirstOrDefaultAsync(m => m.CarId == id && m.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            car.Favs.Remove(userCar);
            _context.Update(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }
    }
}
