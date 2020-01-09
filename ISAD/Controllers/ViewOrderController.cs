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
            foreach (Orders i in _context.Orders) //get all the orders that match the current table number
            {
                if (i.TableNumber.Value == tableNumber)
                {
                    OrderList.Add(new SelectListItem    //put them in a format where the html can use it
                    {
                        Text = i.Name + "'s order costing £" + i.TotalPrice,
                        Value = i.Id.ToString()
                    });
                }
            }
        }
        public IActionResult Index()
        {
            ViewBag.Orders = OrderList; //used to display the orders found that match the table number
            return View();
        }

        public IActionResult EditOrder()
        {
            if (Request.Form.Keys.Contains("table") && Request.Form["table"] != "" ) //If the table number is given
            {
                if (!int.TryParse(Request.Form["table"], out tableNumber))  //check if its valid
                {
                    ViewBag.Error = "Please enter a valid table number"; //error messages are displayed at the top of the page
                }
            }
            else if (Request.Form.Keys.Contains("order")) //if the order button was clicked
            {
                foreach (SelectListItem i in OrderList)     //find the item in the list that was selected
                {
                    if (i.Value == Request.Form["order"])   
                    {
                        selected = i.Text;                          
                        selectedOrderID = Request.Form["order"];
                    }
                }
                orderDetails = _context.OrderDetails.Where(d => d.OrderId == Int32.Parse(Request.Form["order"])).ToList(); //get the details for that product
                DetailsList = new List<SelectListItem> { };
                foreach (OrderDetails d in orderDetails)    //format each product to display all the nesarcary info
                {
                    DetailsList.Add(new SelectListItem { Text = _context.Products.Where(p => p.Id.ToString() == d.ProductId.ToString()).FirstOrDefaultAsync().Result.Name + " x" + d.OrderQuantity,});
                }
            }
            else if (Request.Form.Keys.Contains("remove"))  //if the remove button was clicked
            {
                if (selectedOrderID == "")  //if nothings selected selected
                {
                    ViewBag.Error = "Nothing selected to cancel";  //error messages appear at the top of the page
                }
                else
                {
                    var something = _context.Database.ExecuteSqlRaw("EXEC DeleteOrder @OrderID",    //run the delete procedure with the selected item id
                        new SqlParameter("@OrderID", selectedOrderID));
                }
            }
            else if (Request.Form.Keys.Contains("edit")) //if the remove button was clicked
            {
                if (selectedOrderID == "")  //if nothings selected
                {
                    ViewBag.Error = "Nothing selected to edit";
                }
                else
                {
                    List<SelectListItem> temp = new List<SelectListItem> { };
                    double price = 0;  
                    foreach (OrderDetails d in orderDetails)    //make the list of items in the same format as the order page and count the price
                    {
                        for (int i = 0; i < d.OrderQuantity; i++)
                        {
                            temp.Add(new SelectListItem { Text = _context.Products.Find(d.ProductId).Name, Value = _context.Products.Find(d.ProductId).Id.ToString() });
                            price += (double)_context.Products.Find(d.ProductId).Price;
                        }
                    }
                    var something = _context.Database.ExecuteSqlRaw("EXEC DeleteOrder @OrderID",    //delete the order so a new one can be made
                        new SqlParameter("@OrderID", selectedOrderID));
                    DetailsList = new List<SelectListItem> { };
                    selected = "-1";    //unselect the selected item
                    selectedOrderID = "-1";
                    Response.Redirect("/Order");    //go to the order page
                    return new OrderController(_context).ReviewOrder(temp, price);

                }
            }

            OrderList = new List<SelectListItem> { };
            foreach (Orders i in _context.Orders)//remake the orders list
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