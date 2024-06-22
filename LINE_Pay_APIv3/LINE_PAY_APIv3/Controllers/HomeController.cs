using LINE_PAY_APIv3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using LINE_PAY_APIv3.Dtos;
using static LINE_PAY_APIv3.Dtos.LineMessage;

namespace LINE_PAY_APIv3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public string requestUrl = "https://sandbox-api-pay.line.me/v3/payments/request";
        


        public static string CalculateHmacSha256(string data)
        {
            var key = "{ChannelSecret}";
            var data1 = $"{key}{data}";
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(key);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);
            byte[] dataBytes = encoding.GetBytes(data1);
            byte[] hashmessage = hmacsha256.ComputeHash(dataBytes);
            return Convert.ToBase64String(hashmessage);
        }

        public static string Nonce()
        {
            string nonce = Guid.NewGuid().ToString();
            return nonce;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SendPaymentRequest()
        {
            var body = new PaymentRequestDto
            {
                amount = 100,
                currency = "TWD",
                orderId = "MKSI_S_20180904_1000003",
                packages = new List<PackageDto>
                    {
                        new PackageDto
                        {
                            id="1",
                            amount=100,
                            products=new List<LinePayProductDto>
                            {
                                new LinePayProductDto
                                {
                                    id="PEN-B-001",
                                    name="Pen Brown",
                                    quantity=2,
                                    price=50
                                }
                            }
                        }
                    },
                redirectUrls = new RedirectUrls
                {
                    confirmUrl = "{ConfirmUrl}",
                    cancelUrl = "{CancelUrl}"
                }
            };
            var content = JsonConvert.SerializeObject(body);
            //ViewBag.nonce = Nonce();
            var nonce = Nonce();
            //ViewBag.test = CalculateHmacSha256("/v3/payments/request" + content+nonce);
            var hmac= CalculateHmacSha256("/v3/payments/request" + content + nonce);
            
            using (HttpClient test = new HttpClient())
            {
                var form = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                test.DefaultRequestHeaders.Add("X-LINE-ChannelId", "{ChannelId}");
                test.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
                test.DefaultRequestHeaders.Add("X-LINE-Authorization",hmac );

                
                
                form.Content = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await test.SendAsync(form);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonData = JObject.Parse(responseBody);
                var Url = (string)jsonData["info"]["paymentUrl"]["web"];
                ViewBag.test = Url;
                return Redirect(Url);
            };
        }

        [HttpGet]
        [Route("Home/authorize")]
        public async Task<IActionResult> authorize(string transactionId, string orderId)
        {
            if (!string.IsNullOrEmpty(transactionId))
            {
                var response = await ConFirm(transactionId);
                if (!string.IsNullOrEmpty(response))
                {
                    ViewBag.response = response;
                }
                else
                {
                    ViewBag.response = "錯誤";
                }
            }
            return View("index");
        }

        public async Task<string> ConFirm(string transactionId)
        {
            var data = new PaymentConfirmDto
            {
                amount = 100,
                currency = "TWD"
            };
            var contentjson = JsonConvert.SerializeObject(data);
            var nonce = Nonce();
            var hmac = CalculateHmacSha256($"/v3/payments/{transactionId}/confirm"+contentjson+nonce);


            using (HttpClient ConFirm = new HttpClient())
            {
                var form = new HttpRequestMessage(HttpMethod.Post, $"https://sandbox-api-pay.line.me/v3/payments/{transactionId}/confirm");
                ConFirm.DefaultRequestHeaders.Add("X-LINE-ChannelId", "{ChannelId}");
                ConFirm.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
                ConFirm.DefaultRequestHeaders.Add("X-LINE-Authorization", hmac);



                form.Content = new StringContent(contentjson, Encoding.UTF8, "application/json");
                var response = await ConFirm.SendAsync(form);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            return null;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
