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
            ViewBag.Products = ProductList;
            return View();
        }
    }
}