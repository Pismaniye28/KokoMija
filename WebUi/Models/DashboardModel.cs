using System.Collections.Generic;
using WebUi.Models; // Import the namespace where your OrderListModel is defined
using KokoMija.Entity;
using WebUi.Identity; // Import the namespace where your User and Order entities are defined

namespace WebUi.Models
{
    public class DashboardModel
    {
        public List<OrderListModel> CompletedOrders { get; set; }
        public List<User> RecentUsers { get; set; }
        public int NewlyOrders {get;set;}
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
    }
}
