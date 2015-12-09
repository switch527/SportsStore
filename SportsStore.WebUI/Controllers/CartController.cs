using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductRepository repository;
        private IOrderProcessor orderProcessor;
        public CartController(IProductRepository repo, IOrderProcessor proc)
        {
            repository = repo;
            orderProcessor = proc;
        }
        public ViewResult Checkout2()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(new OrderHeader());
            }
            else
                return View("LoginFirst");
        }

        [HttpPost]
        public ViewResult Checkout2(Cart cart, OrderHeader orderHeader)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid)
            {
                //The following orderProcessor.ProcessOrder method might be underlined in red.
                //This will be fixed later when you update the EmailOrderProcessor.cs file and NinjectControllerFactory.cs (see Steps 9 and 10)
                orderProcessor.ProcessOrder(cart, orderHeader,
                                            User.Identity.GetUserId().ToString(),
                                            User.Identity.GetUserName());
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(orderHeader);
            }
        }
        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }
        public RedirectToRouteResult AddToCart(Cart cart, int productId,
        string returnUrl)
        {
            Product product = repository.Products
            .FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                cart.AddItem(product, 1);
            }
            return RedirectToAction("Index", new { returnUrl });
        }
        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId,
        string returnUrl)
        {
            Product product = repository.Products
            .FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                cart.RemoveLine(product);
            }
            return RedirectToAction("Index", new { returnUrl });
        }
        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }
        [HttpPost]
        public ViewResult Checkout(Cart cart, OrderHeader orderHeader, ShippingDetails shippingDetails)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid)
            {
                orderProcessor.ProcessOrder(cart, orderHeader,
                                            User.Identity.GetUserId().ToString(),
                                            User.Identity.GetUserName());
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(shippingDetails);
            }
        }
        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }
    }
}