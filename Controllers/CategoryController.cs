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
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _db.Category.ToArrayAsync();
            return View(categories);
        }

        // Create
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View();

            _db.Category.Add(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Edit
        public async Task<IActionResult> Edit(  // Get
            [Required]
            [Range(1, int.MaxValue)]
            int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            var category = await _db.Category.FindAsync(id);
            if (category is null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid) return BadRequest();

            _db.Category.Update(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Delete
        public async Task<IActionResult> Delete(
            [Required]
            [Range(1, int.MaxValue)] 
            int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            var category = await _db.Category.FindAsync(id);
            if (category is null) return NotFound();

            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
