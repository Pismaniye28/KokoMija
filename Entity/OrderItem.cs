using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Entity
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public long Price {get; set;}
        public int Quantity { get; set; }

        public long Quatation{get;set;}

        public long DevelopersCut { get; set; }
        public long StripeCut { get; set; }
        public long? TaxCut { get; set; }
        public long? OtherExpenses { get; set; }
        public long Profit { get; set; }
    }
}