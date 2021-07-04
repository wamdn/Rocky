using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ApplicationTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<ApplicationType> appTypes = await _db.ApplicationType.ToListAsync(); 
            return View(appTypes);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType appType)
        {
            if (!ModelState.IsValid) return View();

            _db.ApplicationType.Add(appType);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(
            [Required]
            [Range(1, int.MaxValue)]
            int id)
        {
            ApplicationType? appType = await _db.ApplicationType.FindAsync(id);
            if (appType is null) return NotFound();

            return View(appType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationType appType)
        {
            if (!ModelState.IsValid) return BadRequest();

            _db.ApplicationType.Update(appType);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(
            [Required] 
            [Range(1, int.MaxValue)] 
            int id)
        {
            ApplicationType? category = await _db.ApplicationType.FindAsync(id);
            if (category is null) return NotFound();

            _db.ApplicationType.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
