using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace final.Controllers
{
    public class HomeController : UserController
    {
        [AllowAnonymous]
        //商品檢視
        public ActionResult Index()
        {
            using (Models.ECEEntities db = new Models.ECEEntities())
            {
                var result = (from s in db.Commodity select s).ToList();
                return View(result);
            }           
        }

        //商品詳細檢視
        public ActionResult Details(int id)
        {
            using (Models.ECEEntities db = new Models.ECEEntities())
            {
                var result = (from s in db.Commodity
                              where s.Id == id
                              select s).FirstOrDefault();

                if (result == default(Models.Commodity))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(result);
                }
            }
        }

        //付款成功頁面
        public ActionResult Success(string orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        //訂單付款成功傳送LineNotify訊息
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
    }
}