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

namespace SunriseAuto.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class TypeController : Controller
    {
        private readonly SunriseAutoContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public TypeController(SunriseAutoContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            //var cultureinfo = new CultureInfo("en");
            //Thread.CurrentThread.CurrentUICulture = cultureinfo;
            //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureinfo.Name);
        }

        // GET: Admin/Type
        public async Task<IActionResult> Index(string searchString)
        {
            var types = from c in _context.Types select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                types = types.Where(s => s.Name!.Contains(searchString));
            }
            return View(await types.ToListAsync());
        }

        // GET: Admin/Type/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Types == null)
            {
                return NotFound();
            }

            var @type = await _context.Types
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@type == null)
            {
                return NotFound();
            }

            return View(@type);
        }

        // GET: Admin/Type/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Models.Type());
            else
                return View(_context.Types.Find(id));
        }

        // POST: Admin/Type/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id,Models.Type @type)
        {
            if (id == 0)
            {
                if (ModelState.IsValid)
                {

                    if (@type.Id == 0)
                    {
                        string RootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(@type.ImageFile.FileName);
                        string extension = Path.GetExtension(@type.ImageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        @type.ImageName = fileName;

                        string path = Path.Combine(RootPath + "/image/type-logo", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await @type.ImageFile.CopyToAsync(fileStream);
                        }
                        _context.Add(@type);
                    }
                    else
                        _context.Update(@type);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                if (id != @type.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", @type.ImageName);
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);

                    string RootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(@type.ImageFile.FileName);
                    string extension = Path.GetExtension(@type.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    @type.ImageName = fileName;

                    string path = Path.Combine(RootPath + "/image/type-logo", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await @type.ImageFile.CopyToAsync(fileStream);
                    }
                    try
                    {
                        _context.Update(@type);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TypeExists(@type.Id))
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
            return View(@type);
        }

        // GET: Admin/Type/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Types == null)
            {
                return NotFound();
            }

            var @type = await _context.Types
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@type == null)
            {
                return NotFound();
            }

            return View(@type);
        }

        // POST: Admin/Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Types == null)
            {
                return Problem("Entity set 'AppDBContext.Cars' is null.");
            }
            var temp = await _context.Types.FindAsync(id);
            if (temp != null)
            {
                //delete image
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image/type-logo", temp.ImageName);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
                //delete record
                _context.Types.Remove(temp);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeExists(int id)
        {
          return (_context.Types?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
