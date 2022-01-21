using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _db.Product
                .Include(p => p.Category)
                .Include(p => p.ApplicationType)
                .ToArrayAsync();

            return View(products);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Category[] categorys = await _db.Category.ToArrayAsync();
            IEnumerable<SelectListItem> categoryDropDown = categorys.Select(c =>
                new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            ApplicationType[] applicationTypes = await _db.ApplicationType.ToArrayAsync();
            IEnumerable<SelectListItem> AppTypeDropDown = applicationTypes.Select(at => 
                new SelectListItem 
                { 
                    Text = at.Name,
                    Value = at.Id.ToString()
                });

            // ViewBag.CategoryDropDown = categoryDropDown;
            // ViewData["CategoryDropDown"] = categoryDropDown;

            var productVM = new ProductVM
            {
                Product = new Product(),
                CategorySelectList = categoryDropDown,
                ApplicationTypeSelectList = AppTypeDropDown
            };

            if (id is null) return View(productVM);

            Product? productNullable = await _db.Product.FirstOrDefaultAsync(p => p.Id == id);
            if (productNullable is null) return NotFound();
            productVM.Product = productNullable;

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                Category[] categorys = await _db.Category.ToArrayAsync();
                IEnumerable<SelectListItem> categoryDropDown = categorys.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    });

                ApplicationType[] applicationTypes = await _db.ApplicationType.ToArrayAsync();
                IEnumerable<SelectListItem> AppTypeDropDown = applicationTypes.Select(at =>
                    new SelectListItem
                    {
                        Text = at.Name,
                        Value = at.Id.ToString()
                    });

                productVM.CategorySelectList = categoryDropDown;
                productVM.ApplicationTypeSelectList = AppTypeDropDown;
                return View(productVM);
            }

            Product product = productVM.Product;

            if (HttpContext.Request.Form.Files.Any())
            {
                // Save new image
                IFormFile upladedImage = HttpContext.Request.Form.Files[0];

                string imageBaseName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(upladedImage.FileName);
                string imageName = imageBaseName + extension;
                string imagePath = Path.Combine(_env.WebRootPath, WebConstent.ProductImagePath, imageName);

                using var image = new FileStream(
                    imagePath, 
                    FileMode.Create, 
                    FileAccess.Write, 
                    FileShare.None, 
                    4096, 
                    true);

                await upladedImage.CopyToAsync(image);

                product.Image = imageName;
            }

            if (product.Id == default)
            {
                // Create Product
                _db.Product.Add(product);
            }
            else
            {
                // Delete previous image
                Product? productBeforeUpdate = await _db.Product.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);
                if (string.IsNullOrWhiteSpace(product.Image))
                {
                    product.Image = productBeforeUpdate.Image;
                }
                else if (!string.IsNullOrWhiteSpace(productBeforeUpdate?.Image))
                {
                    string imagePath = Path.Combine(_env.WebRootPath, WebConstent.ProductImagePath, productBeforeUpdate.Image);
                    System.IO.File.Delete(imagePath);
                }

                // Edit Product
                _db.Product.Update(product);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete([Required] int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            Product? product = await _db.Product
                .Include(p => p.Category)
                .Include(p => p.ApplicationType)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost([Required] int id)
        {
            if (!ModelState.IsValid) return BadRequest();
            
            Product? product = await _db.Product.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();

            // Delete image
            if (!string.IsNullOrWhiteSpace(product.Image))
            {
                string imagePath = Path.Combine(_env.WebRootPath, WebConstent.ProductImagePath, product.Image);
                if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
            }

            _db.Product.Remove(product);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
