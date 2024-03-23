using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace Bussines.Abstract
{
    public interface IPhotoService : IValidator<Image>
    {
        public List<Image> GetListItems();
        bool Create(Image entity);
        Task<Image> CreateAsync(Image entity);
        void Update(Image entity);
        Task DeleteAsync(Image entity);

        Task<Image> GetById(int id);
        Task<Image> GetImageByIdAsync(int imageId);
        Task<Image> GetColorByNameAsync(string colorName);
        Task<string> GetSizeByNameAsync(string sizeName);

       Task< List<Image>> GetAll();
        
    }
}