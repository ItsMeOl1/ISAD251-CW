﻿using System;
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
    public class AdminController : Controller
    {
        private readonly ISAD251_OHamiltonContext _context;
        private List<SelectListItem> OrderList = new List<SelectListItem> { };
        private static List<SelectListItem> DetailsList = new List<SelectListItem> { };
        private static List<OrderDetails> orderDetails = new List<OrderDetails> { };
        private static Products selectedProduct;
        private List<SelectListItem> ProductList = new List<SelectListItem> { };
        public AdminController(ISAD251_OHamiltonContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Orders()
        {
            OrderList = new List<SelectListItem> { };
            foreach (Orders i in _context.Orders)
            {
                OrderList.Add(new SelectListItem    //format all the orders for a htlm select list
                {
                    Text = "Table " + i.TableNumber + "'s order for " + i.Name + " costing £" + i.TotalPrice,
                    Value = i.Id.ToString()
                });
            }

            ViewBag.Orders = OrderList;
            return View("Orders");
        }

        public IActionResult ChangeSelected()   //called from the admin orders page
        {
            orderDetails = _context.OrderDetails.Where(d => d.OrderId == Int32.Parse(Request.Form["order"])).ToList();  //get the selected orders details
            DetailsList = new List<SelectListItem> { };
            foreach (OrderDetails d in orderDetails)    //put each detail into a list so it can be displayed
            {
                DetailsList.Add(new SelectListItem { Text = _context.Products.Where(p => p.Id.ToString() == d.ProductId.ToString()).FirstOrDefaultAsync().Result.Name + " x" + d.OrderQuantity });
            }
            OrderList = new List<SelectListItem> { };
            foreach (Orders i in _context.Orders) //remake the orders list
            {
                OrderList.Add(new SelectListItem
                {
                    Text = "Table " + i.TableNumber + "'s  order for " + i.Name + " costing £" + i.TotalPrice,
                    Value = i.Id.ToString()
                });
            }
            ViewBag.Orders = OrderList;
            ViewBag.Details = DetailsList;
            return View("Orders");
        }

        public IActionResult Products() 
        {
            ProductList = new List<SelectListItem> { };
            foreach (Products p in _context.Products)   //get a list of the products
            {
                ProductList.Add(new SelectListItem { Text = p.Name, Value = p.Id.ToString() });
            }
            ViewBag.Items = ProductList;
            return View("Products");
        }

        public IActionResult ChangeItem()   //called from the admin products page
        {
            if (Request.Form.Keys.Contains("save") && Request.Form["save"] != "")   //if the save button was clicked
            {
                if (Request.Form["name"] != "" && Request.Form["price"] != "" && Request.Form["quantity"] != "" && selectedProduct != null)
                {
                    _context.Database.ExecuteSqlRaw("EXEC EditProduct @ID, @Name, @Details, @Price, @Quantity",     //run the sql to edit the selected product with the given details
                       new SqlParameter("@ID", selectedProduct.Id),
                       new SqlParameter("@Name", Request.Form["name"].ToString()),
                       new SqlParameter("@Details", Request.Form["description"].ToString()),
                       new SqlParameter("@Price",  long.Parse(Request.Form["price"])),
                       new SqlParameter("@Quantity", Int32.Parse(Request.Form["quantity"])));
                    ViewBag.Success = "Changes successfully saved";
                }
                else
                {
                    ViewBag.Error = "Please make sure you have inputted a name, price and quantity";
                }
            }
            else if (Request.Form.Keys.Contains("delete") && Request.Form["delete"] != "")  //if the delete button was clicked
            {
                if (Request.Form["name"] != "" && Request.Form["price"] != "" && Request.Form["quantity"] != "" && selectedProduct != null)
                {
                    _context.Database.ExecuteSqlRaw("EXEC DeleteProduct @ID",   //delete the given product with sql
                       new SqlParameter("@ID", selectedProduct.Id));
                    ViewBag.Success = "Changes successfully saved";
                }
                else
                {
                    ViewBag.Error = "Please make sure you have inputted a name, price and quantity";
                }
            }
            else if (Request.Form.Keys.Contains("create") && Request.Form["create"] != "")  //if the create button was clicked
            {
                if (Request.Form["name"] != "" && Request.Form["price"] != "" && Request.Form["quantity"] != "" && selectedProduct != null)
                {   
                    _context.Database.ExecuteSqlRaw("EXEC AddProduct @Name, @Details, @Price, @Quantity",   //add the new item with sql
                      new SqlParameter("@Name", Request.Form["name"].ToString()),
                      new SqlParameter("@Details", Request.Form["description"].ToString()),
                      new SqlParameter("@Price", long.Parse(Request.Form["price"])),
                      new SqlParameter("@Quantity", Int32.Parse(Request.Form["quantity"])));
                    ViewBag.Success = "Changes successfully saved";
                }
                else
                {
                    ViewBag.Error = "Please make sure you have inputted a name, price and quantity";
                }
            }
            else
            {
                selectedProduct = _context.Products.Where(p => p.Id == Int32.Parse(Request.Form["items"])).FirstOrDefault();

            }

            ProductList = new List<SelectListItem> { };
            foreach (Products p in _context.Products)
            {
                ProductList.Add(new SelectListItem { Text = p.Name, Value = p.Id.ToString()});
            }

            ViewBag.Selected = selectedProduct;
            ViewBag.Items = ProductList;
            return View("Products");
        }


    }
}