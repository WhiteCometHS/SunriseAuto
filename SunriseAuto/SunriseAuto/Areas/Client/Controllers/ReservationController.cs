using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace SunriseAuto.Areas.Client
{
    [Area("Client")]
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly SunriseAutoContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ReservationController(SunriseAutoContext context, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            var cultureinfo = new CultureInfo("en");
            Thread.CurrentThread.CurrentUICulture = cultureinfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureinfo.Name);
        }

        // GET: Client/Reservation
        public async Task<IActionResult> Index()
        {
            if(_context.Reservations != null)
            {
                var reservs = await _context.Reservations.Where(r => r.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier))).ToListAsync();
                var cars = new List<Car>();
                foreach (Reservation r in reservs)
                {
                    cars.Add(await _context.Cars.FirstOrDefaultAsync(m => m.Id == r.CarId));
                }
                ViewData["Cars"] = cars;
                return View(reservs);
            }
            else
            {
                return Problem("Entity set 'SunriseAutoContext.Reservation'  is null.");
            }
        }

        public async Task<IActionResult> Add(int CarId)
        {
            ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["CarId"] = CarId;
            return View(new Reservation());
        }

        // GET: Client/Reservation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            else
            {
                Car car=await _context.Cars.FirstOrDefaultAsync(m => m.Id == reservation.CarId);
                Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == car.BrandId);
                ViewData["Car"] = car;
                ViewData["Brand"] = brand.Name;
                ViewData["User"] = await _userManager.FindByIdAsync(reservation.UserId);
            }

            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int CarId, Reservation reservation)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                Car car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == CarId);
                car.Status = Status.NotAccepted;
                _context.Update(car);
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Account/Manage/Index", new { Area = "Identity" });
            }
            return View(reservation);
        }

        // GET: Client/Reservation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // POST: Client/Reservation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Duration,Date,CarId,UserId")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            return View(reservation);
        }

        // GET: Client/Reservation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Client/Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'SunriseAutoContext.Reservation'  is null.");
            }
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                Car car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == reservation.CarId);
                _context.Reservations.Remove(reservation);
                car.Status = Status.Available;
                _context.Update(car);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return (_context.Reservations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> AddToHistory(int id)
        {

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            car.Status = Status.Available;
            _context.Update(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Reservation");
        }
    }
}
