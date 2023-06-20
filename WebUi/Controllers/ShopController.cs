using Bussines.Abstract;
using Data.Abstract;
using KokoMija.Entity;
using Microsoft.AspNetCore.Mvc;
using WebUi.Models;


namespace WebUi.Controllers
{
  public class ShopController:Controller
    {  
        private IProductService _productService;
        private ICourserService _courserService;

        private IPhotoService _photoService;
    
        public ShopController(IProductService productService,ICourserService courserService,IPhotoService photoService)
        {
            this._productService=productService;
            this._courserService=courserService;
            this._photoService=photoService;
        }
        // localhost/products/telefon?page=1
        public IActionResult List(string category,int page=1)
        {
            const int pageSize=2;
            var productViewModel = new ProductListViewModel()
            {
                
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = category
                },
                Products = _productService.GetProductsByCategory(category,page,pageSize)
              
            };
            
            return View(productViewModel);
        }

        public IActionResult Details(string url)
        {
            if (url==null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetails(url);
            if(product==null)
            {
                return NotFound();
            }
            return View(new ProductDetailModel{
                Product = product,
                // Images = product.ProductImages.Select(i=>i.Img).ToList(),
                Categories = product.ProductCategories.Select(i=>i.Category).ToList()
            });
        }

        public async Task< IActionResult> Search(string q,int page=1)
        {
            if (q == null)
            {
                var count = await _productService.GetAll();
                const int pageSize=2;
                var allViewModel = new ProductListViewModel()
                {
                    PageInfo = new PageInfo()
                    {
                        TotalItems= count.Count(),
                        CurrentPage = page,
                        ItemsPerPage = pageSize,
                    },
                    Products= await _productService.GetAll(),
                    
                    
                };
                return View(allViewModel);
            }
                
                var productViewModel = new ProductListViewModel()
                {
                    Products = _productService.GetSearchResult(q)
                };
                if (_productService.GetSearchResult(q).Count == 0)
                {
                    return View("_noproduct");
                }
            

            return View(productViewModel);
        }

       
        public IActionResult DiscountedProducts(){
            return View();
        }
        }
    }
