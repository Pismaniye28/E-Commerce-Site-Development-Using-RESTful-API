using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Abstract
{
    public interface IUnitOfWork:IDisposable
    {
         ICourserRepository Slider{get;}
         IPhotoRepository Photo{get;}
         ICartRepository Carts {get;}
         ICategoryRepository Categories {get;}
         IOrderRepository Orders {get;}
         IProductRepository Products {get;} 
         void Save();

         Task<int> SaveAsync();
    }
}