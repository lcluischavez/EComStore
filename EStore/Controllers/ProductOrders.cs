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
    public class ProductOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductOrdersController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
        {
            _userManager = usermanager;
            _context = context;
        }
        // GET: ProductOrders
        public async Task<ActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var productOrders = await _context.ProductOrder
                .ToListAsync();

            return View(productOrders);
        }

        // GET: ProductgOrders/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: ProductOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int id)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var userCurrentOrder = _context.Order.Where(o => o.ApplicationUserId == user.Id).FirstOrDefault(o => o.IsComplete == false);

                if (userCurrentOrder != null)

                {
                    var newProductOrder = new ProductOrder
                    {
                        OrderId = userCurrentOrder.OrderId,
                        ProductId = id,
                    };


                    _context.ProductOrder.Add(newProductOrder);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Orders");
                }
                else
                {
                    var newOrder = new Order
                    {
                        ApplicationUserId = user.Id,
                        DateCreated = DateTime.Now
                    };
                    _context.Order.Add(newOrder);
                    var newProductOrder = new ProductOrder
                    {
                        Order = newOrder,
                        ProductId = id
                    };
                    _context.ProductOrder.Add(newProductOrder);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Orders");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: ProductOrders/Edit/5 
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var userCurrentOrder = _context.Order.Where(o => o.ApplicationUserId == user.Id).FirstOrDefault(o => o.IsComplete == false);
                userCurrentOrder.IsComplete = true;
                if (userCurrentOrder != null)
                {

                    var newProductOrder = new ProductOrder
                    {
                        OrderId = userCurrentOrder.OrderId,
                        ProductId = id,
                    };
                    _context.ProductOrder.Update(newProductOrder);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Orders");

                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: ProductOrders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var productOrder = _context.ProductOrder.FirstOrDefault(po => po.ProductId == id);
                _context.ProductOrder.Remove(productOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Orders");
            }
            catch
            {
                return View();
            }
        }
        private async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

    }
}