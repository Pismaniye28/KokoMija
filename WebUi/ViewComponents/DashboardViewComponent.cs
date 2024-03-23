using Bussines.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace KokoMija.WebUi.ViewComponents
{
   public class DashboardViewComponent : ViewComponent
{
    private readonly IOrderService _orderService;

    private readonly Dictionary<string, (string Url, string Icon)> _categories;

    public DashboardViewComponent(IOrderService orderService)
    {
       
        _orderService = orderService;
        _categories = new Dictionary<string, (string Url, string Icon)>
        {
            { "Dashboard", ("/Admin/Dashboard", "fa-tachometer-alt") },
            { "Customers", ("/Admin/Customers", "fa-users") },
            { "Logs", ("/Admin/Logs", "fa-file-alt") },
            { "Orders", ("/Admin/Orders", "fa-shopping-cart") }
        };
    }

    public  IViewComponentResult Invoke()
    {
        var currentAction = ViewContext.RouteData.Values["action"].ToString();
        ViewBag.FreshOrders = _orderService.GetNewlyOrders().ToString();
        ViewBag.packingOrders = _orderService.GetpacingOrders().ToString();
        ViewBag.SelectedCategory = _categories.FirstOrDefault(x => x.Value.Url.EndsWith(currentAction)).Key;
        return View(_categories);
    }

}

}
