using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunriseAuto.Data;
using SunriseAuto.Models;

namespace SunriseAuto.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VehicleController : Controller
    {
        private readonly SunriseAutoContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public VehicleController(SunriseAutoContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            var cultureinfo = new CultureInfo("en");
            Thread.CurrentThread.CurrentUICulture = cultureinfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureinfo.Name);
        }

        // GET: Admin/Vehicle
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var cars = from c in _context.Cars
                           select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(s => s.Name.Contains(searchString)
                                       || s.ShortDesc.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    cars = cars.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    cars = cars.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    cars = cars.OrderByDescending(s => s.Date);
                    break;
                case "Price":
                    cars = cars.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    cars = cars.OrderByDescending(s => s.Price);
                    break;
                default:
                    cars = cars.OrderBy(s => s.Name);
                    break;
            }


            /*var cars = from c in _context.Cars select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(s => s.Name!.Contains(searchString));
            }*/
            int pageSize = 3;
            return View(await PaginatedList<Car>.CreateAsync(cars.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Admin/Vehicle/Details/5
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

            return View(car);
        }

        // GET: Admin/Vehicle/Create
        public async Task<IActionResult> AddOrEdit(int id=0)
        {
            var vm = new VehicleViewModel();
            vm.Types = await _context.Types.ToListAsync();
            vm.Brands = await _context.Brands.ToListAsync();
            if (id == 0)
            {
                vm.Car = new Car();
            }
            else
            {
                if (_context.Cars == null)
                {
                    return NotFound();
                }
                var car = await _context.Cars.FindAsync(id);
                if (car == null)
                {
                    return NotFound();
                }
                vm.Car = car;
            }
            return View(vm);
            //List<Models.Type> types = await _context.Types.ToListAsync();
            //ViewData["Types"] = await _context.Types.ToListAsync();
            //ViewData["Car"] = new Car();
        }

        // POST: Admin/Vehicle/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, VehicleViewModel vm)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (id == 0)
            {
                if (ModelState.IsValid)
                {
                    string RootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(vm.Car.ImageFile.FileName);
                    string extension = Path.GetExtension(vm.Car.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    vm.Car.ImageName = fileName;

                    string path = Path.Combine(RootPath + "/image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await vm.Car.ImageFile.CopyToAsync(fileStream);
                    }


                    vm.Car.Date = DateTime.Now;
                    _context.Add(vm.Car);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                
            }
            else
            {
                if (id != vm.Car.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", vm.Car.ImageName);
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);

                    string RootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(vm.Car.ImageFile.FileName);
                    string extension = Path.GetExtension(vm.Car.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    vm.Car.ImageName = fileName;

                    string path = Path.Combine(RootPath + "/image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await vm.Car.ImageFile.CopyToAsync(fileStream);
                    }
                    try
                    {
                        _context.Update(vm.Car);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CarExists(vm.Car.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            vm.Types = await _context.Types.ToListAsync();
            return View(vm);
        }

        // GET: Admin/Vehicle/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }
            var vm = new VehicleViewModel();
            vm.Types = await _context.Types.ToListAsync();
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            vm.Car = car;
            return View(vm);
        }

        // POST: Admin/Vehicle/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehicleViewModel vm)
        {
            if (id != vm.Car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", vm.Car.ImageName);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);

                string RootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(vm.Car.ImageFile.FileName);
                string extension = Path.GetExtension(vm.Car.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                vm.Car.ImageName = fileName;

                string path = Path.Combine(RootPath + "/image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await vm.Car.ImageFile.CopyToAsync(fileStream);
                }
                try
                {
                    _context.Update(vm.Car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(vm.Car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            vm.Types = await _context.Types.ToListAsync();
            return View(vm);
        }

        // GET: Admin/Vehicle/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(car);
        }

        // POST: Admin/Vehicle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cars == null)
            {
                return Problem("Entity set 'AppDBContext.Cars'  is null.");
            }
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                //delete image
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", car.ImageName);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
                //delete record
                _context.Cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
          return (_context.Cars?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
