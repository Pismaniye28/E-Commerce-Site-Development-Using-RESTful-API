using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity
{
    public class Image
    {
        public int ImageId { get; set; }
        
        public String ImageUrl{get; set;}

        public String ColorName{get;set;}

        public String ColorCode{get;set;}

        public List<ProductImage> ProductImages { get; set; }
        
    }
}