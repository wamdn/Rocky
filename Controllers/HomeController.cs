using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
            _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _db.Product
                .Include(p => p.Category)
                .Include(p => p.ApplicationType)
                .ToArrayAsync();

            IEnumerable<Category> categories = await _db.Category.ToArrayAsync();

            HomeVM homeVM = new()
            {
                Products = products,
                Categories = categories
            };

            return View(homeVM);
        }

        public async Task<IActionResult> Details([Required] int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            Product product = await _db.Product
                .Include(p => p.Category)
                .Include(p => p.ApplicationType)
                .FirstOrDefaultAsync(p => p.Id == id);

            DetailsVM detailsVM = new()
            {
                Product = product,
                ExistsInCart = false
            };

            var cartProducts = HttpContext.Session.Get<IEnumerable<CartProduct>>(WebConstent.ShoppingCart);
            if (cartProducts?.FirstOrDefault(c => c.Id == id) is not null) detailsVM.ExistsInCart = true;

            return View(detailsVM);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost([Required] int id)
        {
            CartProduct cartProduct = new() { Id = id };

            List<CartProduct> cartProductList = HttpContext.Session.Get<List<CartProduct>>(WebConstent.ShoppingCart) ?? new();
            cartProductList.Add(cartProduct);
            HttpContext.Session.Set(WebConstent.ShoppingCart, cartProductList);

            return RedirectToAction(nameof(Details));
        }

        public IActionResult RemoveFromCart([Required] int id)
        {
            var cartProductList = HttpContext.Session.Get<IEnumerable<CartProduct>>(WebConstent.ShoppingCart);
            if (cartProductList is null) return BadRequest();

            cartProductList = cartProductList.Where(c => c.Id != id);
            HttpContext.Session.Set(WebConstent.ShoppingCart, cartProductList);

            return RedirectToAction(nameof(Details), new { id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
