using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace WebUi.Models
{
    public class ImgCreateModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string colorName { get; set; }
        [Required]
        public string colorCode { get; set; }
        
    }
}