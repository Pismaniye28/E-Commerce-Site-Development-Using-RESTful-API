using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Entity;
using KokoMija.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using WebUi.Extensions;
using WebUi.Identity;
using WebUi.Models;
using static WebUi.Models.RoleModel;

namespace WebUi.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController: Controller
    {
        private readonly ApplicationContext _context;
        private IProductService _productService;
        private ICategoryService _categoryService;
        private ICourserService _courserService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        private IPhotoService _photoService;

        public AdminController(IProductService productService,
        ICategoryService categoryService ,
        ICourserService courserService,
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager,
        IPhotoService photoService,
        ApplicationContext context)
        {
            _productService = productService;
            _categoryService = categoryService;
            _courserService = courserService;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _photoService = photoService;
        }

        [HttpPost]
        public async Task<IActionResult> UserDelete(string UserId){
            var user = await _userManager.FindByIdAsync(UserId);
            if (user !=null)
            {
                await _userManager.DeleteAsync(user);
                return RedirectToAction("UserList");
            }
              TempData.Put("message",new AlertMessage(){
                    Title="kayıt güncellendi",
                    Message=$"{user.Id} isimli üye silinemedi.",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-vial-circle-check fa-beat-fade"
                    }); 
            return RedirectToAction("UserList");
        }
        [HttpPost]
        public IActionResult UserCreate(UserCreateModel model)
        {
            
            return View();
        }
        [HttpGet]
       public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user!=null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(i=>i.Name);

                ViewBag.Roles = roles;
                return View(new UserDetailsModel(){
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles
                });
            }
            return RedirectToAction("UserList");
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model,string[] selectedRoles)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if(user!=null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        selectedRoles = selectedRoles?? new string[]{};
                        await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles).ToArray<string>());

                        return RedirectToAction("UserList");
                    }
                }
                return Redirect("UserList");
            }

            return View(model);

        }
        [HttpGet]
        public IActionResult UserList()
        {   
            return View(_userManager.Users);
        }
        [HttpGet]
        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user,role.Name)
                                ?members:nonmembers;
                list.Add(user);
            }
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }
       [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if(ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user!=null)
                    {
                        var result = await _userManager.AddToRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                              foreach (var error in result.Errors)
                              { 
                                ModelState.AddModelError("", error.Description);  
                              }  
                        }
                    }
                }
          
                foreach (var userId in model.IdsToDelete ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user!=null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                              foreach (var error in result.Errors)
                              { 
                                ModelState.AddModelError("", error.Description);  
                              }  
                        }
                    }
                }
            }
            return Redirect("/admin/role/"+model.RoleId);
        }
        
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }
        
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]        
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.name));
                if(result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task< IActionResult> ProductList()
        {
            var products = await _productService.GetAll();
            return View(new ProductListViewModel()
            {
                Products = products
                
            });
        }

        public async Task< IActionResult> CategoryList()
        {
            var categories = await _categoryService.GetAll();
            return View(new CategoryListViewModel()
            {
                Categories = categories
            });
        }

        [HttpGet]
        public async Task<IActionResult> ProductCreate()
        { 
           
            return View(new ProductModel(){
                SelectedImages = await _photoService.GetAll()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult ProductCreate(ProductModel model)
        {
           

            if(ModelState.IsValid)
            {
            
                var entity = new Product()
                {
                    ProductName = model.ProductName,
                    Url = model.Url,
                    Price = model.Price,
                    Description = model.Description,
                           
                };      
                if(_productService.Create(entity))
                {                 
                    TempData.Put("message",new AlertMessage(){
                    Title="kayıt güncellendi",
                    Message=$"{entity.ProductName} isimli category eklendi.",
                    AlertType="success",
                    icon="fa-solid",
                    icon2="fa-vial-circle-check fa-beat-fade"
                    }); 
                    return RedirectToAction("ProductList");
                }
                    TempData.Put("message",new AlertMessage(){
                    Title="kayıt güncellendi",
                    Message=$"{_productService.ErrorMessage} isimli ürün eklenemedi!.",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-thumbs-down fa-beat-fade"
                    });       
                return View(model);
            }        
            return View(model);         
        }

         [HttpGet]
        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryCreate(CategoryModel model)
        {
             if(ModelState.IsValid)
            {
                var entity = new Category()
                {
                    Name = model.Name,
                    Url = model.Url            
                };
                
                _categoryService.Create(entity);
                    TempData.Put("message",new AlertMessage(){
                    Title="kayıt güncellendi",
                    Message=$"{entity.Name} isimli category eklendi.",
                    AlertType="success",
                    icon="fa-solid",
                    icon2="fa-vial-circle-check fa-beat-fade"
                    }); 


                return RedirectToAction("CategoryList");
            }
            return View(model);
        }


        [HttpGet]
        public  async Task<IActionResult> ProductEdit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var entity = _productService.GetByIdWithCategories((int)id);

            if(entity==null)
            {
                 return NotFound();
            }

            var model = new ProductModel()
            {
                ProductId = entity.ProductId,
                ProductName = entity.ProductName,
                Url = entity.Url,
                Price = entity.Price,
                Description = entity.Description,
                IsDiscount = entity.IsInDiscount,
                DiscountRate = entity.DiscountRate,
                Quatations = entity.Quatation,
                IsApproved = entity.IsApproved,
                IsHome = entity.IsHome,
                SelectedCategories = entity.ProductCategories.Select(i=>i.Category).ToList(),
                SelectedImages = entity.ProductImages.Select(i=>i.Image).ToList()
            };
            ViewBag.Images= await _photoService.GetAll();
            ViewBag.Categories= await _categoryService.GetAll();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductModel model,int[] categoryIds,int[] selectedImages)
        {
            
            if(ModelState.IsValid)
                {        
                    var entity = await _productService.GetById(model.ProductId);
                    if(entity==null)
                    {
                        return NotFound();
                    }
                    entity.ProductName = model.ProductName;
                    entity.Price = model.Price;
                    entity.Url = model.Url;
                    //entity.ImageUrl = model.ImageUrl;
                    entity.Description = model.Description;
                    entity.IsHome = model.IsHome;
                    entity.IsApproved = model.IsApproved;
                    entity.IsInDiscount = model.IsDiscount;


                    if(_productService.Update(entity,categoryIds,selectedImages))
                    {   
                        TempData.Put("message",new AlertMessage(){
                        Title="kayıt güncellendi",
                        Message=$"{entity.ProductName} isimli ürün güncellendi.",
                        AlertType="success",
                        icon="fa-solid",
                        icon2="fa-vial-circle-check fa-beat-fade"
                        });                 
                        return RedirectToAction("ProductList");
                    }
                        TempData.Put("message",new AlertMessage(){
                        Title="kayıt güncellenemedi",
                        Message=$"{_productService.ErrorMessage} isimli hata oluştu.",
                        AlertType="danger",
                        icon="fa-solid",
                        icon2="fa-thumbs-down fa-beat-fade"
                        });     
             }
            ViewBag.Images= await _photoService.GetAll();
            ViewBag.Categories= await _categoryService.GetAll();
            return View(model);
        }
       
        [HttpGet]
        public IActionResult CategoryEdit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var entity = _categoryService.GetByIdWithProducts((int)id);

            if(entity==null)
            {
                 return NotFound();
            }

            var model = new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select(p=>p.Product).ToList()
            };
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryEdit(CategoryModel model)
        {
            if(ModelState.IsValid)
            {
                var entity = await _categoryService.GetById(model.CategoryId);
                if(entity==null)
                {
                    return NotFound();
                }
                entity.Name = model.Name;
                entity.Url = model.Url;

                _categoryService.Update(entity);

                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.Name} isimli category güncellendi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });
                return RedirectToAction("CategoryList");
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var entity = await _productService.GetById(productId);

            if(entity!=null)
            {
                _productService.Delete(entity);
            }
                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.ProductName} isimli ürün silindi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });

            return RedirectToAction("ProductList");
        }
        
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var entity = await _categoryService.GetById(categoryId);

            if(entity!=null)
            {
                _categoryService.Delete(entity);
            }
                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.Name} isimli category silindi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });
            return RedirectToAction("CategoryList");
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFromCategory(int productId,int categoryId)
        {
            _categoryService.DeleteFromCategory(productId,categoryId);
            return Redirect("/admin/categories/"+categoryId);
        }

        public async Task< IActionResult> CourserList(){
            var afis = await _courserService.GetAll();
            return View(new SliderModel(){
                slider = afis
            });
        }
        public async Task<IActionResult> DeleteCourser(int courserId){
            var entity = await _courserService.GetById(courserId);
            if (entity!=null)
            {
                _courserService.Delete(entity);
            }
                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.CourserImgUrl} isimli afiş silindi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });
                return RedirectToAction("CourserList");
        }
        [HttpGet]
        public async Task<IActionResult> EditCourser(int? id){
              if(id==null)
            {
                return NotFound();
            }
                var entity = await _courserService.GetById((int) id);
                if(entity==null)
            {
                 return NotFound();
            }

                return View(new Courser(){
                    CourserId = entity.CourserId,
                    CourserImgUrl = entity.CourserImgUrl,
                });
        }

            [Authorize]
            [HttpPost]
            public async Task<IActionResult> EditCourser(int? id,IFormFile file)
            {
                var entity = await _courserService.GetById((int)id);
                        if (file != null)
                        {
                            var extantion = Path.GetExtension(file.FileName);
                            var dateImgForSlider = string.Format($"{Guid.NewGuid()}{extantion}");
                            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",dateImgForSlider);
                            var deletepath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",file.FileName);
                            entity.CourserImgUrl= dateImgForSlider;
                            

                            using (var stream = new FileStream(path,FileMode.Create))
                            {
                            await file.CopyToAsync(stream);
                            }
                        }
                        entity.CourserId = (int)id;
                        
                        _courserService.Update(entity);
                        return RedirectToAction("CourserList");

              }

        [HttpGet]
        public IActionResult CourserCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CourserCreate(Courser model,IFormFile file)
        {
                    if (file != null)
                        {
                            var extantion = Path.GetExtension(file.FileName);
                            var dateImgForSlider = string.Format($"{Guid.NewGuid()}{extantion}");
                            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",dateImgForSlider);
                            model.CourserImgUrl= dateImgForSlider;
                            

                            using (var stream = new FileStream(path,FileMode.Create))
                            {
                             await file.CopyToAsync(stream);
                            }
                            
                        }
                     _courserService.Create(model);
                     
            return RedirectToAction("CourserList");
        }
        [HttpGet]
        public async Task< IActionResult> ImageList(ImgListModel model){
            var images= await _photoService.GetAll();
            return View(new ImgListModel(){
                Images = images
            });
        }
        [HttpGet]
        public IActionResult ImageCreate(){
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImageCreate(ImgCreateModel model, IFormFile file){
            if (ModelState.IsValid)
            {   
                    var entity = new Image(){
                        ColorName=model.colorName,
                        ColorCode=model.colorCode,
                         }; 
                    if (file != null)
                    {
                        var extantion = Path.GetExtension(file.FileName);
                        var dateImgForProduct = string.Format($"{Guid.NewGuid()}{extantion}");
                        var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",dateImgForProduct);   
                        entity.ImageUrl=dateImgForProduct;
                        using (var stream = new FileStream(path,FileMode.Create))
                        {
                        await file.CopyToAsync(stream);
                        }
                    }else{
                        return View(model);
                    }
                   
                     _photoService.Create(entity);
                    TempData.Put("message",new AlertMessage(){
                    Title="kayıt güncellendi",
                    Message=$"{entity.ImageUrl} isimli resim eklendi.",
                    AlertType="success",
                    icon="fa-solid",
                    icon2="fa-vial-circle-check fa-beat-fade"
                    }); 
                    return RedirectToAction("ImageList");
            }else{
                return View(model);
                } 
    }
        public async Task<IActionResult> ImageDelete(int imageId){
            var entity = await _photoService.GetById(imageId);
            if (entity!=null)
            {
                _photoService.Delete(entity);
            }
                
                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.ImageId} isimli Image silindi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });
                return RedirectToAction("ImageList");
        }
        [HttpGet]
        public async Task<IActionResult> ImageEdit(int? id){
             if(id==null)
            {
                return NotFound();
            }
                var entity = await _photoService.GetById((int) id);
                if(entity==null)
            {
                 return NotFound();
            }

                return View(new ImgEditModel(){
                    colorCode = entity.ColorCode,
                    colorName = entity.ColorName,
                    imageId = entity.ImageId,
                    imageUrl = entity.ImageUrl
                });
        }
        [HttpPost]
        public async Task<IActionResult> ImageEdit(ImgEditModel model,int? id){
                var entity = await _photoService.GetById((int)id);
                  if(entity==null)
                    {
                        return NotFound();
                    }
                    if (ModelState.IsValid)
                    {    
                        entity.ColorName = model.colorName;
                        entity.ColorCode = model.colorCode;
                        entity.ImageUrl = entity.ImageUrl;
                        entity.ImageId = entity.ImageId;
                    _photoService.Update(entity);
                    return RedirectToAction("ImageList");

        }
        return View(model);
    }
}
}