using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebUi.EmailServices;
using WebUi.Extensions;
using WebUi.Identity;
using WebUi.Models;

namespace WebUi.Controllers
{   
    [AutoValidateAntiforgeryToken]
    public class AccountController:Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        private ICartService _cartService;

        private IEmailSender _emailSender;
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager,IEmailSender emailSender,ICartService cartService)
        {
            _userManager = userManager;
            _signInManager =signInManager;
            _emailSender = emailSender;
            _cartService=cartService;
        }
        [Authorize]
        public IActionResult Manage()
        {
            return View(new UserProfileModel(){
               
            });
        }
        
        [HttpGet]
        public IActionResult Login(string ReturnUrl=null){
            return View(new LoginModel(){
                ReturnUrl = ReturnUrl
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model){
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                ModelState.AddModelError("","Şifre ve ya Email hatalı");
                return View(model);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("","Lütfen Email hesabınıza gelen link ile hesabınızı onaylayın");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user,model.Password,false,false);
            if (result.Succeeded)
            {
                _cartService.InitializeCart(user.Id);
                return Redirect(model.ReturnUrl??"~/");
            }

            ModelState.AddModelError("","Şifre ve ya Email hatalı");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model){
            if (!ModelState.IsValid)
            {
                return View(model);
            }
                var user = new User(){
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                };
            
            var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {   
                await _userManager.AddToRoleAsync(user,"customer");
                //token oluşturma ve mail gönderme
                //token oluşturma
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail","Account",new{
                    userId = user.Id,
                    token = code
                });
                //Email
                await _emailSender.SendEmailAsync(model.Email,
                //Mail başlığı
                "Hesabınızı onaylayınız.",
                $"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:7070{url}'>tıklayınız.</a>"
                );


                return RedirectToAction("Login","Account");
            }
            ModelState.AddModelError("RePassword","Şifreniz aynı olmalıdır!");
            ModelState.AddModelError("","Bilinmeyen bir hata oluştu");
            return View(model);
        }
         [HttpGet]
        public IActionResult Register(){
            return View();
        }
        public async Task<IActionResult>Logout(){

        TempData.Put("message",new AlertMessage(){
        Title="Oturum Kapatıldı!",
        Message="",
        AlertType="warning",
        icon="fa-solid",
        icon2="fa-poo fa-beat-fade"
        });
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token){
            if (userId==null || token == null)
            {
                TempData.Put("message",new AlertMessage(){
                    Title="Geçersiz token",
                    Message="Site güvenliğini sağlayan tokenın süresi doldu ve ya işleme almadı !",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-user-ninja fa-beat-fade"
                });
                
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                   TempData.Put("message",new AlertMessage(){
                    Title="Kullanıcı bulunamadı",
                    Message="Istenilen kullanıcı bulunamadı site sahibiyle iletişime geçebilirsiniz ve ya yeni kullanıcı oluşturabilirsiniz!",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-person-circle-question fa-beat-fade"
                });
                return View();
            }
            var result = await _userManager.ConfirmEmailAsync(user,token);
            if (result.Succeeded)
            {       
                //cart işlemleri
                _cartService.InitializeCart(user.Id);

                //hesap onaylandı
                  TempData.Put("message",new AlertMessage(){
                    Title="Hesabınız onaylandı",
                    Message="Başarılı",
                    AlertType="success",
                    icon="fa-regular",
                    icon2="fa-thumbs-up fa-beat-fade"
                    
                });
                return View();  
            }

                TempData.Put("message",new AlertMessage(){
                Title="Hesabınız onaylanmadı !",
                Message="Hesabınızı onaylayamadık! tekrar deneyin.",
                AlertType="warning",
                icon="fa-solid",
                icon2="fa-skull-crossbones fa-beat-fade"
                });
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword(){
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model){
            if (string.IsNullOrEmpty(model.Email))
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                return View(model);
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword","Account",new{
                    userId = user.Id,
                    token = code
                });
             await _emailSender.SendEmailAsync(model.Email,
                //Mail başlığı
                "Hesabınızın Şifresini Değiştirin.",
                $"Lütfen email hesabınızın şifresini linke tıklayarak değiştirin <a href='https://localhost:7070{url}'>tıklayınız.</a>"
                );
            return View(model);
        }
        [HttpGet]
           public IActionResult ResetPassword(string userId,string token){
            if (userId ==null || token == null)
            {
                     TempData.Put("message",new AlertMessage(){
                    Title="Şifre değişikliği onaylanmadı",
                    Message="Bu şifre değişikliği güvenlik nedeniyle onaylanmadı Code:1",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-user-ninja fa-beat-fade"
                });
                return RedirectToAction("Home","Index");
            }
            var model = new ResetPasswordModel {Token=token};

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
          public async Task<IActionResult> ResetPassword(ResetPasswordModel model){
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                   TempData.Put("message",new AlertMessage(){
                    Title="Şifre değişikliği onaylanmadı",
                    Message="Bu şifre değişikliği güvenlik nedeniyle onaylanmadı Code:2",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-user-ninja fa-beat-fade"
                });
                return RedirectToAction("Home","Index");
            }
            var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
              TempData.Put("message",new AlertMessage(){
                    Title="Hata oluştu",
                    Message="Hata oluştu Error:404 monkeeee",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-user-ninja fa-beat-fade"
                });
            return View(model);
        }

        public IActionResult AccessDenied(){
            return View();
        }
    }
}