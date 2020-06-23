using final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace final.Controllers
{
    public class AccountController : UserController
    {
        //建立db實體與Dispose
        ECEEntities db = new ECEEntities();

        //DB Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl) //會員登入，回傳登入頁面
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //登入 回傳資料表
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var email = db.Buyer.Find(loginViewModel.Email);
                if (email == null)
                {
                    ModelState.AddModelError(nameof(email.Id),"查無此信箱");
                    return View(loginViewModel);
                }
                else if(loginViewModel.Password !=email.Password)
                {
                    ModelState.AddModelError(nameof(email.Password), "密碼錯誤");
                    return View(loginViewModel);
                }
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(loginViewModel.Email,loginViewModel.RememberMe, 30);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                if (loginViewModel.RememberMe)
                {
                    cookie.Expires = DateTime.Now.AddDays(7);
                }
                Response.Cookies.Add(cookie);
                return Redirect(loginViewModel.ReturnUrl ?? "/");
            }
            return View(loginViewModel);
        }

        //登出
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register() //會員註冊，回傳註冊頁面
        {
            return View();
        }

        //會員註冊，回傳資料到資料庫        
        [HttpPost]
        public ActionResult Register(Buyer model)
        {
            
            if (ModelState.IsValid)
            {
                db.Buyer.Add(model);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }
    }
}