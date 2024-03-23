using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete.EfCore
{
    public class EfCorePhotoRepository : EfCoreGenericRepository<Image>, IPhotoRepository
    {
         public EfCorePhotoRepository(ShopContext context):base(context)
        {
            
        }
        private ShopContext ShopContext{
            get{return context as ShopContext;}
        }
        
        public List<Image> GetListItems()
        {
            return ShopContext.Images.ToList();
        }
        public async Task<Image> GetImageByIdAsync(int imageId)
        {
            return await ShopContext.Images.FirstOrDefaultAsync(i => i.ImageId == imageId);
        }

        public async Task<Image> GetColorByNameAsync(string colorName)
        {
            return await ShopContext.Images.FirstOrDefaultAsync(c => c.ColorName == colorName);
        }

        public async Task<string> GetSizeByNameAsync(string sizeName)
            {
                if (sizeName.Equals("Small", StringComparison.OrdinalIgnoreCase) && await ShopContext.Images.AnyAsync(i => i.SizeSmall))
                {
                    return "Small";
                }
                else if (sizeName.Equals("Medium", StringComparison.OrdinalIgnoreCase) && await ShopContext.Images.AnyAsync(i => i.SizeMedium))
                {
                    return "Medium";
                }
                else if (sizeName.Equals("Large", StringComparison.OrdinalIgnoreCase) && await ShopContext.Images.AnyAsync(i => i.SizeLarge))
                {
                    return "Large";
                }
                else if (sizeName.Equals("XSmall", StringComparison.OrdinalIgnoreCase) && await ShopContext.Images.AnyAsync(i => i.SizeXSmall))
                {
                    return "XSmall";
                }
                else if (sizeName.Equals("XLarge", StringComparison.OrdinalIgnoreCase) && await ShopContext.Images.AnyAsync(i => i.SizeXLarge))
                {
                    return "XLarge";
                }

                return null;
            }
    }
}