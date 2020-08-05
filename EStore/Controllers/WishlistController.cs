using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EStore.Data;
using EStore.Models;
using EStore.Models.WishlistViewModels;

namespace EStore.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Wishlist
        public async Task<ActionResult> Index()
        {
            var user = await GetCurrentUserAsync();

            var applicationDbContext = _context.Wishlist

             .Include(o => o.ApplicationUser)
                .Where(o => o.ApplicationUserId == user.Id || user.IsAdmin == true);


            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Wishlist/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var user = await GetCurrentUserAsync();

            var incompleteWishlist = await _context.Wishlist
                .Where(o => o.ApplicationUserId == user.Id && o.WishlistId == id)

                    .Include(o => o.ApplicationUser)
                    .Include(o => o.ProductWishlist)
                        .ThenInclude(po => po.Product)
            .FirstOrDefaultAsync();
            if (incompleteWishlist != null)
            {
                var wishlistDetailViewModel = new WishlistDetailViewModel();
                wishlistDetailViewModel.LineItems = incompleteWishlist.ProductWishlist.GroupBy(po => po.ProductId)
                        .Select(p => new WishlistLineItem
                        {
                            Product = p.FirstOrDefault().Product,
                        });
                wishlistDetailViewModel.Wishlist = incompleteWishlist;
                return View(wishlistDetailViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Wishlists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Wishlists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int id, ProductWishlist productWishlist)
        {
            try
            {
                var selectedProductWishlist = _context.ProductWishlist.FirstOrDefault(po => po.ProductId != 0);
                var user = await GetCurrentUserAsync();
                var userWishlist = _context.Wishlist.FirstOrDefault(o => o.ApplicationUser.Id == user.Id && o.IsComplete == false);
                var chosenProduct = _context.Product.FirstOrDefault(p => p.ApplicationUserId == user.Id);
                if (userWishlist == null)
                {
                    var newWishlist = new Wishlist
                    {
                        IsComplete = false,
                        DateCreated = DateTime.Now,
                        ApplicationUserId = user.Id
                    };
                    _context.Wishlist.Add(newWishlist);
                    await _context.SaveChangesAsync();
                    int wishlistId = newWishlist.WishlistId;
                    var newProduct = new ProductWishlist
                    {
                        WishlistId = wishlistId,
                        ProductId = id,
                    };
                    _context.ProductWishlist.Add(newProduct);
                    await _context.SaveChangesAsync();
                    //if (chosenPainting.IsSold == false)
                    //{



                    //};


                    return RedirectToAction("Details", "Wishlists", new { id = wishlistId });
                }
                if (userWishlist.IsComplete == true)
                {
                    var newWishlist = new Wishlist
                    {
                        IsComplete = false,
                        DateCreated = DateTime.Now,
                        ApplicationUserId = user.Id
                    };
                    _context.Wishlist.Add(newWishlist);
                    await _context.SaveChangesAsync();
                    int wishlistId = newWishlist.WishlistId;
                    var newProduct = new ProductWishlist
                    {
                        WishlistId = wishlistId,
                        ProductId = id

                    };
                    _context.ProductWishlist.Add(newProduct);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Wishlists", new { id = wishlistId });
                }
                else
                {
                    var newProductWishlist = new ProductWishlist
                    {
                        WishlistId = userWishlist.WishlistId,
                        ProductId = id

                    };
                    _context.ProductWishlist.Add(newProductWishlist);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Wishlists", new { id = newProductWishlist.WishlistId });
                }
            }
            catch (Exception ex)
            {
                return (NotFound());
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Wishlist wishlist)
        {
            if (id != wishlist.WishlistId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {


                    var userCurrentWishlist = await _context.Wishlist.Where(o => o.WishlistId == id)
                        .Include(o => o.ProductWishlist).ThenInclude(o => o.Product).FirstOrDefaultAsync();

                    foreach (var p in userCurrentWishlist.ProductWishlist)
                    {
                        if (p.Product.IsSold == false)
                        {
                            p.Product.IsSold = true;
                            _context.Update(p);
                        }

                    }

                    await _context.SaveChangesAsync();
                    userCurrentWishlist.IsComplete = true;

                    _context.Update(userCurrentWishlist);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishlistExists(wishlist.WishlistId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Edit", "Orders");
            }

            ViewData["UserId"] = new SelectList(_context.ApplicationUser, "Id", "Id", wishlist.ApplicationUserId);
            return View(wishlist);
        }
        private async void DeletePaintingWishlist(int wishlistId)
        {
            var paintingWishlists = await _context.ProductWishlist.Where(po => po.WishlistId == wishlistId).ToListAsync();
            foreach (var po in paintingWishlists)
            {
                _context.ProductWishlist.Remove(po);
            }
        }

        // POST: Wishlist/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Delete()
        //{
        //    try
        //    {
        //        var user = await GetCurrentUserAsync();
        //        var order = await _context.Order
        //           .Where(o => o.ApplicationUserId == user.Id || user.IsAdmin == true).FirstOrDefaultAsync(o => o.IsComplete == false || user.IsAdmin == true);
        //        if (order == null)
        //        {
        //            return RedirectToAction("Index", "Products");
        //        }


        //        await DeleteProductOrders(order.OrderId);
        //        _context.Order.Remove(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Index", "Products");
        //    }
        //    catch (Exception ex)
        //    {

        //        return View();
        //    }
        //}

        private async Task DeletePaintingWishlists(int wishlistId)
        {
            var productWishlists = await _context.ProductWishlist.Where(po => po.WishlistId == wishlistId).ToListAsync();
            foreach (var po in productWishlists)
            {
                _context.ProductWishlist.Remove(po);
            }
        }

        private bool WishlistExists(int id)
        {
            return _context.Wishlist.Any(e => e.WishlistId == id);
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}