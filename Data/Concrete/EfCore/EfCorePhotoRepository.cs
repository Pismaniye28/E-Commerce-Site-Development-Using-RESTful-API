using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using Entity;

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
            throw new NotImplementedException();
        }
    }
}