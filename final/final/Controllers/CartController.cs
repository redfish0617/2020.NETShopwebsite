using final.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace final.Controllers
{
    public class CartController : UserController
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

        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        //取得目前購物車頁面
        public ActionResult GetCart()
        {
            return PartialView("_CartPartial");
        }


        //以id加入Product至購物車，並回傳購物車頁面
        public ActionResult AddToCart(int id , string name,string help,int price,int inventory,string owner,[Bind(Include = "Id,Name,Help,Price,Inventory,Owner")]Commodity postback)
        {
            var currentCart = Models.Operation.GetCurrentCart();
            currentCart.AddProduct(id);
            postback.Name = postback.Name;
            postback.Help = postback.Help;
            postback.Price = postback.Price;
            postback.Inventory = postback.Inventory - 1;//扣庫存
            postback.Owner = postback.Owner;
            db.Entry(postback).State = EntityState.Modified;
            db.SaveChanges();
            return PartialView("_CartPartial");
        }

        //清除購物車內指定項目，並回傳購物車頁面
        public ActionResult RemoveFromCart(int id)
        {
            var currentCart = Models.Operation.GetCurrentCart();
            currentCart.RemoveProduct(id);
            return PartialView("_CartPartial");
        }

        //清空購物車，並回傳購物車頁面
        public ActionResult ClearCart()
        {
            var currentCart = Models.Operation.GetCurrentCart();
            currentCart.ClearCart();
            return PartialView("_CartPartial");
        }
    }
}