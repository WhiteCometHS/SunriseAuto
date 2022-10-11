using System;
using System.Collections.Generic;
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
    public class ReservationController : Controller
    {
        private readonly SunriseAutoContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ReservationController(SunriseAutoContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            var cultureinfo = new CultureInfo("en");
            Thread.CurrentThread.CurrentUICulture = cultureinfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureinfo.Name);
        }
        public async Task<IActionResult> Index()
        {
            if (_context.Reservations != null)
            {
                var reservs = await _context.Reservations.ToListAsync();
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
        public async Task<IActionResult> SetOrdered(int id)
        {

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            car.Status = Status.Ordered;
            _context.Update(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Reservation");
        }
        public async Task<IActionResult> SetReady(int id)
        {

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            car.Status = Status.Ready;
            _context.Update(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Reservation");
        }
        public async Task<IActionResult> SetInUse(int id)
        {

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            car.Status = Status.InUse;
            _context.Update(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Reservation");
        }
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
    }
}
