using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ISAD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISAD.Controllers
{
    public class OrderController : Controller
    {
        private readonly ISAD251_OHamiltonContext _context;
        public OrderController(ISAD251_OHamiltonContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<SelectListItem> ProductList = (from p in _context.Products.AsEnumerable()
                                                       select new SelectListItem
                                                       {
                                                           Text = p.Name,
                                                           Value = p.Id.ToString()
                                                       }).ToList();
            List<SelectListItem> OrderingList = new List<SelectListItem> { };
            ViewBag.Products = ProductList;
            ViewBag.Ordering = OrderingList;
            return View();
        }

        public IActionResult AddToOrder()
        {
            List<SelectListItem> moreOrders = new List<SelectListItem> { };
            Console.WriteLine(Request.Form["orderQuantity"]);
            for (int i = 0; i < Int32.Parse(Request.Form["orderQuantity"]);  i++)
            {
                
                moreOrders.Add(new SelectListItem { Text = _context.Products.Find(Int32.Parse(Request.Form["itemOrdered"])).Name, Value = Request.Form["itemOrdered"]});
            }
            if (ViewBag.Ordering is null)
            {
                ViewBag.Ordering = moreOrders;
            }
            else
            {
                ViewBag.Ordering.Concat(moreOrders);
            }

            ViewBag.Products = (from p in _context.Products.AsEnumerable()
                                select new SelectListItem
                                {
                                    Text = p.Name,
                                    Value = p.Id.ToString()
                                }).ToList();
            return View("Index");
        }
    }
}