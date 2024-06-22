using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Web.Util;

namespace Linepayapi.Controllers
{
    public class HomeController : Controller
    {
        // GET: LinePayment
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MakePayment()
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://sandbox-api-pay.line.me/v2/payments/request");

                // 設定 Headers
                request.Headers.Add("X-LINE-ChannelId", "[ID]");
                request.Headers.Add("X-LINE-ChannelSecret", "[ChannelSecret]");

                // 設定請求內容為 JSON
                var requestData = new
                {
                    amount = "555",
                    /*productImageUrl = "http://placehold.it/84*84",*/
                    confirmUrl = "http://localhost:55156/Home/Callback",
                    productName = "面交",
                    orderId = "1",
                    currency = "TWD"
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var  jsonObject=JObject.Parse(responseBody);
                var paymentUrlWeb = (string)jsonObject["info"]["paymentUrl"]["web"];

                return Redirect(paymentUrlWeb);


            }
        }
        [HttpGet]
        [Route("Home/Callback")]
        public async Task<ActionResult> Callback(string transactionId)
        {
            if (!string.IsNullOrEmpty(transactionId))
            {
                var result = await Confirm(transactionId);
                if (result == "0000")
                {
                    ViewBag.result = "交易完成";

                }
                else
                {
                    ViewBag.result = "交易失敗";

                }
            }
            else
            {
                ViewBag.result = "找不到交易id";
            }
            return View("Index");
            
        }

        public async Task<string> Confirm(string transactionId)
        {
            using (var httpclient = new HttpClient())
            {
                var confirm = new HttpRequestMessage(HttpMethod.Post, $"https://sandbox-api-pay.line.me/v2/payments/{transactionId}/confirm");

                confirm.Headers.Add("X-LINE-ChannelId", "[ID]");
                confirm.Headers.Add("X-LINE-ChannelSecret", "[ChannelSecret]");

                var confirmData = new
                {
                    amount = "100", //驗證金額
                    currency = "TWD" //貨幣種類
                };

                var jsonContent = JsonConvert.SerializeObject(confirmData);
                confirm.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var respons = await httpclient.SendAsync(confirm);
                respons.EnsureSuccessStatusCode();
                var responsBody = await respons.Content.ReadAsStringAsync();
                var responsObject = JObject.Parse(responsBody);
                var result = (string)responsObject["returnCode"];
                return result;

            }
            
        }
    }
}