using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunriseAuto.Data;
using SunriseAuto.Models;

namespace SunriseAuto.Controllers
{
    public class CarController : Controller
    {
        private readonly SunriseAutoContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CarController(SunriseAutoContext context,IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            var cultureinfo = new CultureInfo("en");
            Thread.CurrentThread.CurrentUICulture = cultureinfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureinfo.Name);
        }

        // GET: Car
        public async Task<IActionResult> Index(string searchString, int typeId)
        {
            List<Car> cars = await _context.Cars.Where(s => s.Status.Equals(Status.Available)).ToListAsync();

            if (typeId == 0)
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    cars = cars.Where(s => s.Name!.Contains(searchString)).ToList();
                }
            }
            else
            {
                ViewData["TypeId"] = typeId;
                cars = cars.Where(s => s.TypeId==typeId).ToList();
                if (!String.IsNullOrEmpty(searchString))
                {
                    cars = cars.Where(s => s.Name!.Contains(searchString)).ToList();
                }
            }
            ViewData["Cars"] = cars;
            ViewData["Types"] = await _context.Types.ToListAsync();
            ViewData["UserCars"] = await _context.UserCars.ToListAsync();
            return View();
        }

        public async Task<IActionResult> Brands(string searchString, int brandId)
        {
            List<Car> cars = await _context.Cars.Where(s => s.Status.Equals(Status.Available)).ToListAsync();
            if (brandId == 0)
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    cars = cars.Where(s => s.Name!.Contains(searchString)).ToList();
                }
            }
            else
            {
                ViewData["BrandId"] = brandId;
                cars = cars.Where(s => s.BrandId == brandId).ToList();
                if (!String.IsNullOrEmpty(searchString))
                {
                    cars = cars.Where(s => s.Name!.Contains(searchString)).ToList();
                }
                
            }
            ViewData["Cars"] = cars;
            ViewData["Brands"] = await _context.Brands.ToListAsync();
            return View();
        }

        // GET: Car/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            ViewData["Car"] = car;
            return View();
        }

        private bool CarExists(int id)
        {
          return (_context.Cars?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
