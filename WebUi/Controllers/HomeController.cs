using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bussines.Abstract;
using Data.Abstract;
using WebUi.Models;

namespace WebUi.Controllers;
 // localhost:5000/home
    public class HomeController:Controller
    {      
        private ICategoryService _categoryService;
        private IProductService _productService;
        private ICourserService _courserService;
        private IPhotoService _photoService;
        public HomeController(IProductService productService,ICourserService courserService,IPhotoService photoService,ICategoryService categoryService)
        {
            this._productService=productService;
            this._courserService=courserService;
            this._photoService=photoService;
            this._categoryService=categoryService;
        }
        
        public async Task< IActionResult> Index()
        {
            var categories = await _categoryService.GetAll();
            var courser = await _courserService.GetAll();
            var productViewModel = new ProductListViewModel()
            {
                Products = _productService.GetHomePageProducts(),
                Courser = courser,
                Categories = categories
            };
            return View(productViewModel);
        }

        public IActionResult About()
        {
            return View();
        }

         public IActionResult Contact()
        {
            return View("MyView");
        }
}
