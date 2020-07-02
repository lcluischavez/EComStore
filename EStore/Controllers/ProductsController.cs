using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EStore.Data;
using EStore.Models;

namespace EStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)

        {

            _context = context;
            _userManager = userManager;

        }


        // GET: Products
        public async Task<ActionResult> Index(string searchString)
        {
            var user = await GetCurrentUserAsync();
            var products = await _context.Product
                //.Where(ti => ti.ApplicationUserId == user.Id)
                .Include(tdi => tdi.ApplicationUser)
                .ToListAsync();

            //if (searchString != null)
            //{
            //    var filteredProducts = _context.Product.Where(s => s.Make.Contains(searchString) || s.Model.Contains(searchString));
            //    return View(filteredProducts);
            //};

            return View(products);
        }


        // GET: Products/Details/1
        public async Task<ActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
               //.Where(p => p.UserId == user.Id)
               .Include(p => p.ApplicationUser)
               .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }



        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                product.ApplicationUserId = user.Id;

                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }



        // GET: Products/Edit/1
        public async Task<ActionResult> Edit(int id)
        {
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == id);
            var loggedInUser = await GetCurrentUserAsync();

            if (product.ApplicationUserId != loggedInUser.Id)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Product product)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                product.ApplicationUserId = user.Id;

                _context.Product.Update(product);
                await _context.SaveChangesAsync();
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }



        // GET: Products/Delete/1
        public async Task<ActionResult> Delete(int id)
        {
            var loggedInUser = await GetCurrentUserAsync();
            var product = await _context.Product
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(c => c.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.ApplicationUserId != loggedInUser.Id)
            {
                return NotFound();
            }

            return View(product);
        }


        // POST: Products/Delete/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                var product = await _context.Product.FindAsync(id);
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

    }
}