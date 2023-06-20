using System.ComponentModel.DataAnnotations;
using Entity;

namespace KokoMija.Entity
{
    public class Product
    {
        public int ProductId {get; set;}
        public String ProductName {get; set;}
        public double? Price{get; set;}
        public String Description{get; set;}

        public String Url {get; set;}
        public bool IsApproved{get; set;}

        public bool IsHome{get; set;}

        public bool IsInDiscount{get;set;}

        public double? DiscountRate{get;set;}

        public double? Quatation{get;set;}

        public DateTime DateAdded { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }
        public Category Category { get; set; }
        public List<ProductImage> ProductImages{get;set;}
        public Image Image { get; set; }
    }

    
}