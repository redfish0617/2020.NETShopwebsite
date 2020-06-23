using EC_Back.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EC_Back.Controllers
{
    public class CommodityController : UserController
    {
        //商品DB建立
        ECEEntities db = new ECEEntities();

        //DBDispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //商品頁面Get
        [Authorize]
        public ActionResult CommodIndex()
        {
            List<Commodity> result = new List<Commodity>();
            //商品情況(CRUD)文字呈現
            ViewBag.Result = TempData["Result"];
            //取得登入的Userid
            string userid = ((FormsIdentity)User.Identity).Name;
            //尋找資料與登入的userid符合的商品DB資料
            result = (from i in db.Commodity where i.Owner == userid select i).ToList();
            return View(result);
        }

        //建立商品頁面Get
        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        //建立商品頁面Post
        [Authorize]
        [HttpPost]
        public ActionResult Create(Commodity post)
        {
            if (ModelState.IsValid)
            {
                //建立商品db
                db.Commodity.Add(post);
                db.SaveChanges();
                TempData["Result"] = string.Format("商品{0}建立成功", post.Name);
                //建立後回首頁
                return RedirectToAction("CommodIndex");
            }           
            ViewBag.Result = "資料輸入錯誤";
            //資料錯誤回首頁
            return View(post);
        }

        //更改商品資料Get
        [HttpGet]
        [Authorize]
        public ActionResult Update(int id)
        {
            var result = (from i in db.Commodity where i.Id == id select i).FirstOrDefault();
            //確認ID無誤進入更改頁面
            if (result != default(Commodity))
            {
                return View(result);
            }
            //錯誤
            else
            {
                TempData["Result"] = "錯誤";
                return RedirectToAction("CommodIndex");
            }
        }

        //更改商品資料Post
        [HttpPost]
        [Authorize]
        public ActionResult Update([Bind(Include = "Id,Name,Help,Price,Inventory,Owner")]Commodity postback)
        {
            //輸入更改資料無誤進行更改
            if (ModelState.IsValid)
            {
                db.Entry(postback).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Result"] = string.Format("商品{0}編輯成功", postback.Name);
                return RedirectToAction("CommodIndex");
            }
            //更改資料錯誤回首頁
            else
            {
                TempData["Result"] = string.Format("找不到該筆資料，請重新確認資料");
                return View(postback);
            }
        }

        //刪除商品資料    
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, [Bind(Include = "Id,Name,Help,Price,Inventory,Owner")]Commodity postback)
        {
            var result = (from i in db.Commodity where i.Id == id select i).FirstOrDefault();
            //確認刪除資料無誤進行刪除
            if (result != default(Commodity))
            {
                Commodity commodity = db.Commodity.Find(id);
                db.Commodity.Remove(commodity);
                db.SaveChanges();
                TempData["Result"] = string.Format("商品{0}刪除成功", postback.Name);
                return RedirectToAction("CommodIndex");
            }
            else
            {
                TempData["Result"] = string.Format("找不到該筆資料，請重新確認資料");
                return RedirectToAction("CommodIndex");
            }
        }
    }
}