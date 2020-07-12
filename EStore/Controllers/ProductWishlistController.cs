using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStore.Data;
using EStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EStore.Controllers
{
    public class ProductWishlistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductWishlistsController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
        {
            _userManager = usermanager;
            _context = context;
        }
        // GET: ProductWishlistrs
        public async Task<ActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var productWishlists = await _context.ProductWishlist
                .ToListAsync();

            return View(productWishlists);
        }

        // GET: ProductgWishlists/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: ProductWishlists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int id)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var userCurrentWishlist = _context.Wishlist.Where(o => o.ApplicationUserId == user.Id).FirstOrDefault(o => o.IsComplete == false);

                if (userCurrentWishlist != null)

                {
                    var newProductWishlist = new ProductWishlist
                    {
                        WishlistId = userCurrentWishlist.WishlistId,
                        ProductId = id,
                    };


                    _context.ProductWishlist.Add(newProductWishlist);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Wishlists");
                }
                else
                {
                    var newWishlist = new Wishlist
                    {
                        ApplicationUserId = user.Id,
                        DateCreated = DateTime.Now
                    };
                    _context.Wishlist.Add(newWishlist);
                    var newProductWishlist = new ProductWishlist
                    {
                        Wishlist = newWishlist,
                        ProductId = id
                    };
                    _context.ProductWishlist.Add(newProductWishlist);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Wishlists");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: ProductWishlists/Edit/5 
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductWishlists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var userCurrentWishlist = _context.Wishlist.Where(o => o.ApplicationUserId == user.Id).FirstOrDefault(o => o.IsComplete == false);
                userCurrentWishlist.IsComplete = true;
                if (userCurrentWishlist != null)
                {

                    var newProductWishlist = new ProductWishlist
                    {
                        WishlistId = userCurrentWishlist.WishlistId,
                        ProductId = id,
                    };
                    _context.ProductWishlist.Update(newProductWishlist);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Wishlists");

                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: ProductWishlists/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var productWishlist = _context.ProductWishlist.FirstOrDefault(po => po.ProductId == id);
                _context.ProductWishlist.Remove(productWishlist);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Wishlists");
            }
            catch
            {
                return View();
            }
        }
        private async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

    }
}