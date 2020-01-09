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
        private IEnumerable<SelectListItem> ProductList;
        private static List<SelectListItem> OrderingList = new List<SelectListItem> { };
        private static double price = 0; 
        public OrderController(ISAD251_OHamiltonContext context)
        {
            _context = context;
            ProductList = (from p in _context.Products.AsEnumerable()
                                                               select new SelectListItem
                                                               {
                                                                   Text = p.Name,
                                                                   Value = p.Id.ToString()
                                                               }).ToList();
    }
        public IActionResult Index()
        {
            ViewBag.Ordering = OrderingList;
            ViewBag.Products = ProductList;
            return View();
        }

        public IActionResult EditOrder()
        {            
            if (Request.Form.Keys.Contains("Add"))
            {
                if (Request.Form.Keys.Contains("itemOrdered"))
                {
                    for (int i = 0; i < Int32.Parse(Request.Form["orderQuantity"]); i++)
                    {
                        price += (double)_context.Products.Find(Int32.Parse(Request.Form["itemOrdered"])).Price;
                        OrderingList.Add(new SelectListItem { Text = _context.Products.Find(Int32.Parse(Request.Form["itemOrdered"])).Name, Value = Request.Form["itemOrdered"] });
                    }
                }
                else
                {
                    ViewBag.Error = "Please select something to add to your order and try again";
                }
            }
            else if (Request.Form.Keys.Contains("Remove"))
            {
                if (Request.Form.Keys.Contains("itemSelected"))
                {
                    bool found = false;
                    int index = 0;
                    while (!found)
                    {
                        if (OrderingList[index].Value == Request.Form["itemSelected"])
                        {
                            OrderingList.RemoveAt(index);
                            found = true;
                        }
                        index++;
                    }
                    price -= (double)_context.Products.Find(Int32.Parse(Request.Form["itemSelected"])).Price;

                }
                else
                {
                    ViewBag.Error = "Please select something to remove and try again";
                }
            }
            else if (Request.Form.Keys.Contains("Order"))
            {
                if (OrderingList.Count > 0 && Request.Form["name"] != "" && Request.Form["table"] != "")
                {
                    _context.Database.ExecuteSqlRaw("EXEC AddOrder @Name, @TableNumber, @Price",
                       new SqlParameter("@Name", Request.Form["name"].ToString()),
                       new SqlParameter("@TableNumber", Int32.Parse(Request.Form["table"])),
                       new SqlParameter("@Price", price));
                    var order = _context.Orders.Where(o => o.Name == Request.Form["name"].ToString() && o.TableNumber == Int32.Parse(Request.Form["table"]) && o.TotalPrice == price).FirstOrDefault();
                    int ID = order.Id;
                    List<int> types = new List<int> { };
                    List<int> quantities = new List<int> { };
                    foreach (SelectListItem item in OrderingList)
                    {
                        int id = Int32.Parse(item.Value);
                        if (types.Contains(id))
                        {
                            quantities[types.IndexOf(id)]++;
                        }
                        else
                        {
                            types.Add(id);
                            quantities.Add(1);
                        }
                    }
                    //TODO test for quantity
                    for (int i = 0; i < types.Count; i++)
                    {
                        var something = _context.Database.ExecuteSqlRaw("EXEC AddDetails @OrderID, @ProductID, @Quantity",
                            new SqlParameter("@OrderID", ID),
                            new SqlParameter("@ProductID", types[i]),
                            new SqlParameter("@Quantity", quantities[i]));
                    }
                    ViewBag.Success = true;
                    OrderingList = new List<SelectListItem> { };
                    price = 0;
                    
                }
                else if (OrderingList.Count < 0)
                {
                    ViewBag.Error = "Please make sure you have selected something you wish to order";
                }
                else if (!Request.Form.Keys.Contains("name"))
                {
                    ViewBag.Error = "Please make sure you have entered your name";
                }
                else
                {
                    ViewBag.Error = "Please make sure you have entered your table number";
                }
            }

            ViewBag.Price = price;
            ViewBag.Products = ProductList;
            ViewBag.Ordering = OrderingList;
            return View("Index");
        }

        public IActionResult ReviewOrder(List<SelectListItem> orderinglist, double p)
        {
            OrderingList = orderinglist;
            price = p;
            return View("Index");
        }
    }
}