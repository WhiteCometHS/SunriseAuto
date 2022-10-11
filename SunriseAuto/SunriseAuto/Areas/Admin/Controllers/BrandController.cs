using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunriseAuto.Data;
using SunriseAuto.Models;

namespace SunriseAuto.Areas.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly SunriseAutoContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BrandController(SunriseAutoContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        // GET: Admin/Brand
        public async Task<IActionResult> Index(string searchString)
        {
            var brands = from c in _context.Brands select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                brands = brands.Where(s => s.Name!.Contains(searchString));
            }
            return View(await brands.ToListAsync());
        }

        // GET: Admin/Brand/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Admin/Brand/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Brand());
            else
                return View(_context.Brands.Find(id));
        }

        // POST: Admin/Brand/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Brand brand)
        {
            if (id == 0)
            {
                if (ModelState.IsValid)
                {

                    if (brand.Id == 0)
                    {
                        string RootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(brand.ImageFile.FileName);
                        string extension = Path.GetExtension(brand.ImageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        brand.ImageName = fileName;

                        string path = Path.Combine(RootPath + "/image/brand-logo", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await brand.ImageFile.CopyToAsync(fileStream);
                        }
                        _context.Add(brand);
                    }
                    else
                        _context.Update(brand);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                if (id != brand.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", brand.ImageName);
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);

                    string RootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(brand.ImageFile.FileName);
                    string extension = Path.GetExtension(brand.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    brand.ImageName = fileName;

                    string path = Path.Combine(RootPath + "/image/brand-logo", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await brand.ImageFile.CopyToAsync(fileStream);
                    }
                    try
                    {
                        _context.Update(brand);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BrandExists(brand.Id))
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
            return View(brand);
        }

        // GET: Admin/Brand/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Admin/Brand/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Brands == null)
            {
                return Problem("Entity set 'AppDBContext.Cars' is null.");
            }
            var temp = await _context.Brands.FindAsync(id);
            if (temp != null)
            {
                //delete image
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image/brand-logo", temp.ImageName);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
                //delete record
                _context.Brands.Remove(temp);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
          return (_context.Brands?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
