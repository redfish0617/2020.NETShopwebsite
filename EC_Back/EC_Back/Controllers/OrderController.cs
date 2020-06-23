using EC_Back.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EC_Back.Controllers
{
    public class OrderController : UserController
    {
        //DB建立
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

        //訂單首頁
        [Authorize]
        public ActionResult OrderIndex()
        {
            //顯示狀態
            ViewBag.Result = TempData["Result"];
            List<Order> result = new List<Order>();
            //取得訂單明細
            result = (from i in db.Order select i).ToList();           
            return View(result);
        }

        //訂單明細表單
        [HttpGet]
        [Authorize]
        public ActionResult StateCommit(int id)
        {
            var result = (from i in db.Order where i.Id == id select i).FirstOrDefault();
            if (result != default(Order))
            {
                return View(result);
            }
            else
            {
                TempData["Result"] = "錯誤";
                return RedirectToAction("OrderIndex");
            }
        }

        //訂單出貨
        [HttpPost]
        [Authorize]
        public ActionResult StateCommit([Bind(Include = "Id,Buyer,Price,State,Content,ReceiverAddress")]Order postback)
        {
            //當表單送出
            //發現狀態為False
            if (postback.State == "false")
            {
                //出貨                
                postback.State = "true";
                db.Entry(postback).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Result"] = string.Format("訂單{0}出貨成功!!", postback.Id);
                return RedirectToAction("OrderIndex");
            }
            //狀態為已出貨
            else if(postback.State == "true")
            {
                TempData["Result"] = string.Format("訂單{0}已完成出貨", postback.Id);
                return RedirectToAction("OrderIndex");
            }
            else
            {
                TempData["Result"] = string.Format("找不到該筆訂單，請重新確認");
                return RedirectToAction("OrderIndex");
            }
        }

        //訂單出貨成功傳送LineNotify訊息
        public ActionResult SendMessage(string Message = "")
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://notify-api.line.me");
                //設定Notify權杖位子
                //23MRZze2HTzxfnf7eASgNF4ciiWkk75q698vYQNvxUW 為Notify權杖碼
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "23MRZze2HTzxfnf7eASgNF4ciiWkk75q698vYQNvxUW");
                //接收前端訊息
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("message", Message) });
                using (HttpResponseMessage message = httpClient.PostAsync("/api/notify", content).Result)
                {
                    //傳送訊息
                    return new HttpStatusCodeResult(message.StatusCode);
                }
            }
        }

        //訂單刪除
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var result = (from i in db.Order where i.Id ==id select i).FirstOrDefault();
            if (result != default(Order))
            {
                Order order = db.Order.Find(id);
                db.Order.Remove(order);
                db.SaveChanges();
                TempData["Result"] = string.Format("訂單刪除成功");
                return RedirectToAction("OrderIndex");
            }
            else
            {
                TempData["Result"] = string.Format("查無訂單!");
                return RedirectToAction("OrderIndex");
            }
        }
    }
}