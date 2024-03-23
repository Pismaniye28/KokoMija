using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace Data.Abstract
{
    public interface IPhotoRepository : IRepository<Image>
    {
        List<Image> GetListItems();
        Task<Image> GetImageByIdAsync(int imageId);
        Task<Image> GetColorByNameAsync(string colorName);
        Task<string> GetSizeByNameAsync(string sizeName);
    }
}