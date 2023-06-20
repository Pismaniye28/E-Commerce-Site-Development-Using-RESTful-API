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
        void Delete(Image entity);

        Task<Image> GetById(int id);

       Task< List<Image>> GetAll();
        
    }
}