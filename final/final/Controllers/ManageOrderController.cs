using final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace final.Controllers
{
    public class ManageOrderController : UserController
    {
        //建立資料庫實體
        ECEEntities db = new ECEEntities();
        
        //DB dispose
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
        public ActionResult Index()
        {
            string userid = ((FormsIdentity)User.Identity).Name;
            var result = (from i in db.Order where i.Buyer == userid select i).ToList();
            return View(result);
        }

        //訂單資訊
        [HttpGet]
        [Authorize]
        public ActionResult Detail(int id)
        {            
            var result = (from i in db.Order where i.Id == id select i).FirstOrDefault();
            if (result != default(Order))
            {
                return View(result);             
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //訂單取消
        [HttpPost]
        [Authorize]
        public ActionResult Detail([Bind(Include = "Id,Buyer,Price,State,Content,ReceiverAddress")]Order postback,int id)
        {
            if (postback.State == "false")
            {
                Order order = db.Order.Find(id);
                db.Order.Remove(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else {
                return Content("error");
            }
        }

        //傳送LineNotify訊息
        public ActionResult SendMessage(string Message = "")
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://notify-api.line.me");
                //設定Notify權杖位子
                //23MRZze2HTzxfnf7eASgNF4ciiWkk75q698vYQNvxUW / 8mQ1CesOnXFIyTjPZGyncp7nPb7uZM5I7Vy0ytyVEUW 為Notify權杖碼
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

        //訂單搜尋
        [HttpPost]
        [Authorize]
        public ActionResult Index(int id)
        {
            //儲存查詢出來的Id
            int searchId;

            using (Models.ECEEntities db = new Models.ECEEntities())
            {
                searchId = (from s in db.Order
                            where s.Id == id
                            select s.Id).FirstOrDefault();

            }

            //如果有存在UserId
            if (searchId != 0)
            {
                using (Models.ECEEntities db = new ECEEntities())
                {
                    var result = (from s in db.Order
                                  where s.Id == searchId
                                  select s).ToList();

                    return View(result);
                }
               
            }
            else
            {
                //回傳 空結果 至Index()的View
                return View("Index", new List<Models.Order>());
            }
        }
    }
}