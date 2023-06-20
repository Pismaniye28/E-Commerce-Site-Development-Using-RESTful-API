using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace WebUi.Models
{
    public class ImgEditModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string colorName { get; set; }
        [Required]
        public string colorCode { get; set; }
        public string imageUrl { get; set; }
        [Required]
        public int imageId { get; set; }

        public List<ProductImage> productimages { get; set; }
        

    }
}