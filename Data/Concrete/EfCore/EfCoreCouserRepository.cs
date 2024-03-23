using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using KokoMija.Entity;

namespace Data.Concrete.EfCore
{
    public class EfCoreCouserRepository :EfCoreGenericRepository <Courser>,ICourserRepository
    {
          public EfCoreCouserRepository(ShopContext context):base(context)
        {
            
        }
        private ShopContext ShopContext{
            get{return context as ShopContext;}
        }
    }
}