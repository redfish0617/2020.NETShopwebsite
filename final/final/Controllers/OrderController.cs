using final.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace final.Controllers
{
    public class OrderController : UserController
    {
        //Order頁面
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            int id = int.Parse($"{DateTime.Now:MMddHHmmss}");
            ViewBag.OrderId = id;
            
            return View();
        }

        //宣告LinePay訂單交易所需的字典
        private static Dictionary<string, object> OrderTransactions = new Dictionary<string, object>();

        //設定LinePay的GateWay
        private async Task<string> RequestGateway(string path, object param)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-LINE-ChannelId", "1654190618"); 
                client.DefaultRequestHeaders.Add("X-LINE-ChannelSecret", "8c33b655cf36b77f3f759694dd402d32"); 
                var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://sandbox-api-pay.line.me/v2/payments/" + path, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        //LinePay ReserveAPI(付款)
        [HttpPost]
        public async Task<ActionResult> Reserve(Ship postback, int ItemId, bool capture = true)
        {
            try
            {
                //建立訂單資訊
                var currentcart = Operation.GetCurrentCart();
                var param = new
                {
                    productName = $"訂單編號{postback.OrderID}",
                    productImageUrl = "https://i.imgur.com/DqZUcGd.gif",
                    amount = currentcart.TotalAmount,
                    currency = "TWD",
                    confirmUrl = "http://localhost:50535/Order/Confirm",
                    orderId = $"{postback.OrderID}"
                };               

                //呼叫ReserveAPI
                var responseJson = await RequestGateway("request", param);
                dynamic responseObj = JObject.Parse(responseJson);

                //接收回傳訊息
                if (responseObj.returnCode == "0000")
                {
                    //回傳的交易ID傳回字典
                    OrderTransactions[responseObj.info.transactionId.ToString()] = param;
                    //建立訂單資訊
                    using (ECEEntities db = new ECEEntities())
                    {
                        var content = (from i in db.Commodity select i).FirstOrDefault();
                        string userid = ((FormsIdentity)User.Identity).Name;
                        //建立Order物件
                        var order = new Order()
                        {
                            Id = postback.OrderID,
                            Buyer = userid,
                            Price = currentcart.TotalAmount,
                            Content = $"{currentcart.Content}",
                            ReceiverAddress = postback.ReceiverAddress,
                            State = "false"
                        };
                        //加其入Order資料表後，儲存變更
                        db.Order.Add(order);
                        db.SaveChanges();
                    }
                }
                //取得回傳的Json檔中的付款網址並前往
                string PaymentUrl = responseObj.info.paymentUrl.web;
                return Redirect(PaymentUrl);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        //LinePay ConfirmAPI
        [HttpGet]
        public async Task<ActionResult> Confirm(string transactionId, Ship postback)
        {
            //建立Body
            dynamic transaction = OrderTransactions[transactionId];
            var param = new
            {
                amount = transaction.amount,
                currency = "TWD"
            };
            //呼叫Confirm API
            var responseJson = await RequestGateway($"{transactionId}/confirm", param);
            dynamic responseObj = JObject.Parse(responseJson);
            //成功回商品首頁
            if (responseObj.returnCode == "0000")
            {               
                return RedirectToAction("Success","Home",new { orderId = transaction.orderId});
            }
            else
            {
                return Content("Error");
            }
        }

        //傳送LineNotify訊息
        public ActionResult SendMessage(string Message = "")
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://notify-api.line.me");
                //設定Notify權杖位子
                //23MRZze2HTzxfnf7eASgNF4ciiWkk75q698vYQNvxUW / 8hGKfd5tDo30bfIN6485YWuMpoUAcfqOTLcbtABwHR5 為Notify權杖碼
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