using Data.Abstract;
using Data.Concrete.EfCore;
using Bussines.Abstract;
using Bussines.Concrete;
using Microsoft.Extensions.FileProviders;
using WebUi.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebUi.EmailServices;
using Microsoft.Extensions.DependencyInjection;
using WebUi.Extensions;

var builder = WebApplication.CreateBuilder(args);

var serviceProvider = builder.Services.BuildServiceProvider(validateScopes: true);
var config = serviceProvider.GetRequiredService<IConfiguration>();
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var port = config.GetSection("EmailSender").GetValue<int>("Port");
var host = config.GetSection("EmailSender").GetValue<string>("Host");
var enablessl = config.GetSection("EmailSender").GetValue<bool>("EnableSSL");
var username = config.GetSection("EmailSender").GetValue<string>("UserName");
var password = config.GetSection("EmailSender").GetValue<string>("Password");




// Repository
builder.Services.AddControllersWithViews();
//SqlLite
//builder.Services.AddDbContext<ShopContext>(options=> options.UseSqlite(config.GetConnectionString("SqliteConnection")));
//builder.Services.AddDbContext<ApplicationContext>(options =>options.UseSqlite(config.GetConnectionString("SqliteConnection")));
//SQL Server
 builder.Services.AddDbContext<ShopContext>(options=> options.UseSqlServer(config.GetConnectionString("MsSqlConnection")));
 builder.Services.AddDbContext<ApplicationContext>(options =>options.UseSqlServer(config.GetConnectionString("MsSqlConnection")));

builder.Services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options=>{
    //password
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase=true;
    options.Password.RequireUppercase=true;
    options.Password.RequiredLength=8;
    options.Password.RequireNonAlphanumeric=false;
    //Lockout
    options.Lockout.MaxFailedAccessAttempts=4;
    options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(4);
    options.Lockout.AllowedForNewUsers=true;
    //User
    options.User.RequireUniqueEmail=true;
    options.User.AllowedUserNameCharacters="aąbcćdeęfghijklłmnńoópqrsśtuvwxyzźżabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.SignIn.RequireConfirmedEmail=true;
    options.SignIn.RequireConfirmedPhoneNumber=false;
});
//cookie alanı
builder.Services.ConfigureApplicationCookie(options =>{
    options.LoginPath = "/account/login";
    options.LogoutPath= "/account/logout";
    options.AccessDeniedPath="/account/accessdenied";
    options.SlidingExpiration=true;
    options.ExpireTimeSpan=TimeSpan.FromMinutes(60);
    options.Cookie=new CookieBuilder{
        HttpOnly=true,
        Name=".KokoMija.Security.Cookie",
        SameSite = SameSiteMode.Strict

    };
});
// builder.Services.AddScoped<IProductRepository, EfCoreProductRepository>();
// builder.Services.AddScoped<ICategoryRepository,EfCoreCategoryRepository>();
// builder.Services.AddScoped<ICourserRepository,EfCoreCouserRepository>();
// builder.Services.AddScoped<ICartRepository,EfCoreCartRepository>();
// builder.Services.AddScoped<IPhotoRepository,EfCorePhotoRepository>();
// builder.Services.AddScoped<IOrderRepository,EfCoreOrderRepository>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://kokomija.com",
                                              "http://www.kokomija.com");
                      });
});

// Service 
builder.Services.AddScoped<IEmailSender,SmtpEmailSender>(i=> new SmtpEmailSender(host,port,enablessl,username,password));
builder.Services.AddScoped<ICategoryService,CategoryManager>();
builder.Services.AddScoped<IProductService,ProductManager>();
builder.Services.AddScoped<ICourserService,CourserManager>();
builder.Services.AddScoped<IPhotoService,PhotoManager>();
builder.Services.AddScoped<ICartService,CartManager>();
builder.Services.AddScoped<IOrderService,OrderManager>();
//app builder
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}else
    {
        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            SeedIdentity.Seed(userManager, roleManager,cartService,configuration).Wait();
        }

    }


app.UseStaticFiles();


app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "node_modules")),
        RequestPath = "/node"
    });

app.UseAuthentication();
app.UseRouting();
app.UseCors();
app.UseAuthorization();



app.UseEndpoints(endpoints =>
            {     
                //Cart
                  endpoints.MapControllerRoute(
                    name: "orders", 
                    pattern: "orders",
                    defaults: new {controller="Cart",action="GetOrders"}
                ); 
                  endpoints.MapControllerRoute(
                    name: "cart", 
                    pattern: "cart",
                    defaults: new {controller="Cart",action="Index"}
                ); 
                 endpoints.MapControllerRoute(
                    name: "checkout", 
                    pattern: "checkout",
                    defaults: new {controller="Cart",action="Checkout"}
                ); 
               //account
                 endpoints.MapControllerRoute(
                    name: "accountmanage", 
                    pattern: "account",
                    defaults: new {controller="Account",action="Manage"}
                );

                     endpoints.MapControllerRoute(
                    name: "accountregister", 
                    pattern: "register",
                    defaults: new {controller="Account",action="Register"}
                );

                endpoints.MapControllerRoute(
                    name: "accountlogin", 
                    pattern: "login",
                    defaults: new {controller="Account",action="Login"}
                );
                //Admin
                //courser
               endpoints.MapControllerRoute(
                    name: "admincoursercreate", 
                    pattern: "admin/slider/new",
                    defaults: new {controller="Admin",action="CourserCreate"}
                );
                endpoints.MapControllerRoute(
                    name: "admincourseredit", 
                    pattern: "admin/slider/{id?}",
                    defaults: new {controller="Admin",action="EditCourser"}
                );
                 endpoints.MapControllerRoute(
                    name: "admincourserlist", 
                    pattern: "admin/deletecourser",
                    defaults: new {controller="Admin",action="DeleteCourser"}
                );
                
                     endpoints.MapControllerRoute(
                    name: "admincourserlist", 
                    pattern: "admin/sliderlist",
                    defaults: new {controller="Admin",action="CourserList"}
                );

                //user
                    endpoints.MapControllerRoute(
                    name: "adiminuseredit", 
                    pattern: "admin/user/{id?}",
                    defaults: new {controller="Admin",action="UserEdit"}
                );

                    endpoints.MapControllerRoute(
                    name: "useradminlist", 
                    pattern: "admin/userlist",
                    defaults: new {controller="Admin",action="UserList"}
                );
                //Role -----------------------------------------------------------------

                    endpoints.MapControllerRoute(
                    name : "adminroleedit",
                    pattern:"admin/role/{id?}",
                    defaults: new {controller="Admin", action="RoleEdit"}
                );

                   endpoints.MapControllerRoute(
                    name: "adminrolecreate", 
                    pattern: "admin/rolecreate",
                    defaults: new {controller="Admin",action="RoleCreate"}
                );


                   endpoints.MapControllerRoute(
                    name: "adminrolelist", 
                    pattern: "admin/rolelist",
                    defaults: new {controller="Admin",action="RoleList"}
                );

                //Product--------------------------------------------------------
                endpoints.MapControllerRoute(
                    name: "adminproducts", 
                    pattern: "admin/products",
                    defaults: new {controller="Admin",action="ProductList"}
                );

                endpoints.MapControllerRoute(
                    name: "adminproductcreate", 
                    pattern: "admin/products/create",
                    defaults: new {controller="Admin",action="ProductCreate"}
                );

                endpoints.MapControllerRoute(
                    name: "adminproductedit", 
                    pattern: "admin/products/{id?}",
                    defaults: new {controller="Admin",action="ProductEdit"}
                );
                //Category------------------------------------------------------------
                 endpoints.MapControllerRoute(
                    name: "admincategories", 
                    pattern: "admin/categories",
                    defaults: new {controller="Admin",action="CategoryList"}
                );

                endpoints.MapControllerRoute(
                    name: "admincategorycreate", 
                    pattern: "admin/categories/create",
                    defaults: new {controller="Admin",action="CategoryCreate"}
                );

                endpoints.MapControllerRoute(
                    name: "admincategoryedit", 
                    pattern: "admin/categories/{id?}",
                    defaults: new {controller="Admin",action="CategoryEdit"}
                );
               //shop

                // localhost/search    
                endpoints.MapControllerRoute(
                    name: "search", 
                    pattern: "search/{id?}",
                    defaults: new {controller="Shop",action="search"}
                );

                endpoints.MapControllerRoute(
                    name: "productdetails", 
                    pattern: "{url}",
                    defaults: new {controller="Shop",action="details"}
                );

                endpoints.MapControllerRoute(
                    name: "products",
                    pattern: "products/{category?}",
                    defaults: new { controller = "Shop", action = "List" });
                


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern:"{controller=Home}/{action=Index}/{id?}"
                );
            });
app.MigrateDatabase().Run();