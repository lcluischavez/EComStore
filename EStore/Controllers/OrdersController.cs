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
using EStore.Models.OrderViewModels;

namespace EStore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<ActionResult> Index()
        {
            var user = await GetCurrentUserAsync();

            var applicationDbContext = _context.Order

             .Include(o => o.ApplicationUser)
                .Where(o => o.ApplicationUserId == user.Id || user.IsAdmin == true);


            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var user = await GetCurrentUserAsync();

            var incompleteOrder = await _context.Order
                .Where(o => o.ApplicationUserId == user.Id && o.OrderId == id)

                    .Include(o => o.ApplicationUser)
                    .Include(o => o.ProductOrder)
                        .ThenInclude(po => po.Product)
            .FirstOrDefaultAsync();
            if (incompleteOrder != null)
            {
                var orderDetailViewModel = new OrderDetailViewModel();
                orderDetailViewModel.LineItems = incompleteOrder.ProductOrder.GroupBy(po => po.PaintingId)
                        .Select(p => new OrderLineItem
                        {
                            Product = p.FirstOrDefault().Product,
                        });
                orderDetailViewModel.Order = incompleteOrder;
                return View(orderDetailViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int id, ProductOrder productOrder)
        {
            try
            {
                var selectedProductOrder = _context.ProductOrder.FirstOrDefault(po => po.ProductId != 0);
                var user = await GetCurrentUserAsync();
                var userOrder = _context.Order.FirstOrDefault(o => o.ApplicationUser.Id == user.Id && o.IsComplete == false);
                var chosenProduct = _context.Product.FirstOrDefault(p => p.ApplicationUserId == user.Id);
                if (userOrder == null)
                {
                    var newOrder = new Order
                    {
                        IsComplete = false,
                        DateCreated = DateTime.Now,
                        ApplicationUserId = user.Id
                    };
                    _context.Order.Add(newOrder);
                    await _context.SaveChangesAsync();
                    int orderId = newOrder.OrderId;
                    var newProduct = new ProductOrder
                    {
                        OrderId = orderId,
                        ProductId = id,
                    };
                    _context.ProductOrder.Add(newProduct);
                    await _context.SaveChangesAsync();
                    //if (chosenPainting.IsSold == false)
                    //{



                    //};


                    return RedirectToAction("Details", "Orders", new { id = orderId });
                }
                if (userOrder.IsComplete == true)
                {
                    var newOrder = new Order
                    {
                        IsComplete = false,
                        DateCreated = DateTime.Now,
                        ApplicationUserId = user.Id
                    };
                    _context.Order.Add(newOrder);
                    await _context.SaveChangesAsync();
                    int orderId = newOrder.OrderId;
                    var newProduct = new ProductOrder
                    {
                        OrderId = orderId,
                        ProductId = id

                    };
                    _context.ProductOrder.Add(newProduct);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Orders", new { id = orderId });
                }
                else
                {
                    var newProductOrder = new ProductOrder
                    {
                        OrderId = userOrder.OrderId,
                        ProductId = id

                    };
                    _context.ProductOrder.Add(newProductOrder);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Orders", new { id = newProductOrder.OrderId });
                }
            }
            catch (Exception ex)
            {
                return (NotFound());
            }

        }
        // GET: Orders/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Order.FindAsync(id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewData["UserId"] = new SelectList(_context.ApplicationUser, "Id", "Id", order.ApplicationUserId);
        //    return View(order);
        //}

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {


                    var userCurrentOrder = await _context.Order.Where(o => o.OrderId == id)
                        .Include(o => o.ProductOrder).ThenInclude(o => o.Product).FirstOrDefaultAsync();

                    foreach (var p in userCurrentOrder.PaintingOrder)
                    {
                        if (p.Product.IsSold == false)
                        {
                            p.Product.IsSold = true;
                            _context.Update(p);
                        }

                    }

                    await _context.SaveChangesAsync();
                    userCurrentOrder.IsComplete = true;

                    _context.Update(userCurrentOrder);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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

            ViewData["UserId"] = new SelectList(_context.ApplicationUser, "Id", "Id", order.ApplicationUserId);
            return View(order);
        }
        private async void DeletePaintingOrder(int orderId)
        {
            var paintingOrders = await _context.ProductOrder.Where(po => po.OrderId == orderId).ToListAsync();
            foreach (var po in paintingOrders)
            {
                _context.ProductOrder.Remove(po);
            }
        }

        // POST: Orders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var order = await _context.Order
                   .Where(o => o.ApplicationUserId == user.Id || user.IsAdmin == true).FirstOrDefaultAsync(o => o.IsComplete == false || user.IsAdmin == true);
                if (order == null)
                {
                    return RedirectToAction("Index", "Products");
                }


                await DeleteProductOrders(order.OrderId);
                _context.Order.Remove(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {

                return View();
            }
        }

        private async Task DeletePaintingOrders(int orderId)
        {
            var productOrders = await _context.ProductOrder.Where(po => po.OrderId == orderId).ToListAsync();
            foreach (var po in productOrders)
            {
                _context.ProductOrder.Remove(po);
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}