using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using final.Models;

namespace final.Controllers
{
    public class UserController : Controller
    {
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

        //記下使用者帳號
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {                     
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = ((FormsIdentity)User.Identity).Name;
            }
            base.OnActionExecuted(filterContext);
        }
    }
}