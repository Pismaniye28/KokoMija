using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Data.Abstract;
using KokoMija.Entity;

namespace Bussines.Concrete
{
    public class CourserManager : ICourserService
    {
        private readonly IUnitOfWork _unitofwork;
        public CourserManager( IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public string ErrorMessage { get; set; }

        public async Task< List<Courser>>GetAll(){
            return await _unitofwork.Slider.GetAll();
        }

        public void Delete(Courser entity){
            _unitofwork.Slider.Delete(entity);
            _unitofwork.Save();
        }

        public async Task< Courser> GetById(int id){
            return await _unitofwork.Slider.GetById(id);
        }
          public bool Create(Courser entity)
        {

            if (Validation(entity))
            {
                 // iş kuralları uygula
            _unitofwork.Slider.Create(entity);
            _unitofwork.Save();
            return true ;
            }
            return false;
           
        }

        public void Update(Courser entity)
        {
            
            _unitofwork.Slider.Update(entity);
            _unitofwork.Save();
        }


        public bool Validation(Courser entity)
        {
    
            var isValid = true;
            if (entity==null)
            {
                isValid = false;
            }
            return isValid;
        }

        public async Task<Courser> CreateAsync(Courser entity)
        {
            await _unitofwork.Slider.CreateAsync(entity);
            await _unitofwork.SaveAsync();
            return entity;
        }
        
    }
}