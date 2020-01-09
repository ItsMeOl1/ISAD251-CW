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
    public class ViewOrderController : Controller
    {
        private readonly ISAD251_OHamiltonContext _context;
        private List<SelectListItem> OrderList = new List<SelectListItem> { };
        private static List<SelectListItem> DetailsList = new List<SelectListItem> { };
        private static List<OrderDetails> orderDetails = new List<OrderDetails> { };
        private static string selected;
        private static string selectedOrderID;
        private static int tableNumber = -1;
        public ViewOrderController(ISAD251_OHamiltonContext context)
        {
            _context = context;
            OrderList = new List<SelectListItem> { };
            foreach (Orders i in _context.Orders)
            {
                if (i.TableNumber.Value == tableNumber)
                {
                    OrderList.Add(new SelectListItem
                    {
                        Text = i.Name + "'s order costing £" + i.TotalPrice,
                        Value = i.Id.ToString()
                    });
                }
            }
        }
        public IActionResult Index()
        {
            ViewBag.Orders = OrderList;
            return View();
        }

        public IActionResult EditOrder()
        {
            if (Request.Form.Keys.Contains("table") && Request.Form["table"] != "" )
            {
                if (!int.TryParse(Request.Form["table"], out tableNumber))
                {
                    ViewBag.Error = "Please enter a valid table number";
                }
            }
            else if (Request.Form.Keys.Contains("order"))
            {
                foreach (SelectListItem i in OrderList)
                {
                    if (i.Value == Request.Form["order"])
                    {
                        selected = i.Text;
                        selectedOrderID = Request.Form["order"];
                    }
                }
                orderDetails = _context.OrderDetails.Where(d => d.OrderId == Int32.Parse(Request.Form["order"])).ToList();
                DetailsList = new List<SelectListItem> { };
                foreach (OrderDetails d in orderDetails)
                {
                    DetailsList.Add(new SelectListItem { Text = _context.Products.Where(p => p.Id.ToString() == d.ProductId.ToString()).FirstOrDefaultAsync().Result.Name + " x" + d.OrderQuantity,});
                }
            }
            else if (Request.Form.Keys.Contains("remove"))
            {
                if (selectedOrderID == "")
                {
                    ViewBag.Error = "Nothing selected to cancel";
                }
                else
                {
                    var something = _context.Database.ExecuteSqlRaw("EXEC DeleteOrder @OrderID",
                        new SqlParameter("@OrderID", selectedOrderID));
                }
            }
            else if (Request.Form.Keys.Contains("edit"))
            {
                if (selectedOrderID == "")
                {
                    ViewBag.Error = "Nothing selected to edit";
                }
                else
                {
                    List<SelectListItem> temp = new List<SelectListItem> { };
                    double price = 0;
                    foreach (OrderDetails d in orderDetails)
                    {
                        for (int i = 0; i < d.OrderQuantity; i++)
                        {
                            temp.Add(new SelectListItem { Text = _context.Products.Find(d.ProductId).Name, Value = _context.Products.Find(d.ProductId).Id.ToString() });
                            price += (double)_context.Products.Find(d.ProductId).Price;
                        }
                    }
                    var something = _context.Database.ExecuteSqlRaw("EXEC DeleteOrder @OrderID",
                        new SqlParameter("@OrderID", selectedOrderID));
                    DetailsList = new List<SelectListItem> { };
                    selected = "-1";
                    selectedOrderID = "-1";
                    Response.Redirect("/Order");
                    return new OrderController(_context).ReviewOrder(temp, price);

                }
            }

            OrderList = new List<SelectListItem> { };
            foreach (Orders i in _context.Orders)
            {
                if (i.TableNumber.Value == tableNumber)
                {
                    OrderList.Add(new SelectListItem
                    {
                        Text = i.Name + "'s order costing £" + i.TotalPrice,
                        Value = i.Id.ToString()
                    });
                }
            }
            ViewBag.Selected = selected;
            ViewBag.Orders = OrderList;
            ViewBag.Details = DetailsList;
            return View("Index");
        }
    }
}