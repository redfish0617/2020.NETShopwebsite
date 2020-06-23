using EC_Back.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EC_Back.Controllers
{
    public class UserController : Controller
    {
        ECEEntities db = new ECEEntities();
        //使用者登入憑證
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //亂數產生商品ID
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int i = rnd.Next();
            ViewBag.ID = i;
            //記下使用者帳號
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = ((FormsIdentity)User.Identity).Name;
            }
            base.OnActionExecuted(filterContext);
        }
    }
}