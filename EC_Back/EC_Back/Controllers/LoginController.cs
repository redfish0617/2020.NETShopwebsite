using EC_Back.Models;
using EC_Back.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EC_Back.Controllers
{
    public class LoginController : UserController
    {
        //使用者資料庫建立
        private ECEEntities db = new ECEEntities();

        //DBDispose
        protected override void Dispose(bool Disposeing)
        {
            if (Disposeing)
            {
                db.Dispose();
            }
            base.Dispose(Disposeing);
        }

        //登入頁面Get
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //登入頁面Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var id = db.User.Find(login.UserID);
                //後端資料驗證
                if (id == null)
                {
                    ModelState.AddModelError(nameof(login.UserID), "查無此帳號");
                    return View(login);
                }
                else if (id.Password != login.Password)
                {
                    ModelState.AddModelError(nameof(login.Password), "密碼錯誤");
                    return View(login);
                }
                //建立連線Cookie
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(login.UserID, login.KeepLogin, 30);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                if (login.KeepLogin)
                {
                    cookie.Expires = DateTime.Now.AddDays(7);
                }
                Response.Cookies.Add(cookie);
                return Redirect(login.ReturnUrl ?? "/");
            }
            return View(login);
        }

        //登出
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    }
}
