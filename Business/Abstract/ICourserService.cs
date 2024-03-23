using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Bussines.Abstract
{
    public interface ICourserService : IValidator<Courser>
    { 
     bool Create(Courser entity);
     Task<Courser> CreateAsync(Courser entity);
     void Delete(Courser entity);
     void Update(Courser entity);
     Task< List<Courser>>GetAll();
     Task<Courser> GetById(int id);

    }
}