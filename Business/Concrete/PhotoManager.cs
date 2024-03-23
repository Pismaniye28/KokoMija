using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Data.Abstract;
using Entity;

namespace Bussines.Concrete
{
    public class PhotoManager : IPhotoService
    {
        private readonly IUnitOfWork _unitofwork;
        public PhotoManager( IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        
        public string ErrorMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Create(Image entity)
        {
             if (Validation(entity))
            {
                 // iş kuralları uygula
            _unitofwork.Photo.Create(entity);
            _unitofwork.Save();
            return true ;
            }
            return false;
           
        }
        public async Task<Image> GetImageByIdAsync(int imageId)
        {
            return await _unitofwork.Photo.GetImageByIdAsync(imageId);
        }

        public async Task<Image> GetColorByNameAsync(string colorName)
        {
            return await _unitofwork.Photo.GetColorByNameAsync(colorName);
        }

        public async Task<string> GetSizeByNameAsync(string sizeName)
        {
            return await _unitofwork.Photo.GetSizeByNameAsync(sizeName);
        }

        public async Task DeleteAsync(Image entity)
        {
            _unitofwork.Photo.Delete(entity);
            await _unitofwork.SaveChangesAsync();
        }

        public async Task< List<Image>> GetAll()
        {
            return await _unitofwork.Photo.GetAll();
        }

        public async Task<Image> GetById(int id)
        {
            return await _unitofwork.Photo.GetById(id);
        }

        public List<Image> GetListItems()
        {
            return _unitofwork.Photo.GetListItems();
        }

        public void Update(Image entity)
        {
            _unitofwork.Photo.Update(entity);
            _unitofwork.Save();
        }

        public async Task<Image> CreateAsync(Image entity)
        {
            await _unitofwork.Photo.CreateAsync(entity);
            await _unitofwork.SaveAsync();
            return entity;
        }

        public bool Validation(Image entity)
        {
            var IsValid = true;
            if (entity==null)
            {
                IsValid=false;
            }
            return IsValid;
        }

    }
}