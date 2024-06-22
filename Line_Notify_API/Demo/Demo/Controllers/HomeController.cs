using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text;
using Demo.Models;
using Newtonsoft.Json.Linq;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        private const string LineNotifyApiUrl = "https://notify-api.line.me/api/notify";
        public string LineNotifyApiKey = "";
        private const string LineNotifyAuthorizeUrl = "https://notify-bot.line.me/oauth/authorize";
        private const string LineNotifyTokenUrl = "https://notify-bot.line.me/oauth/token";
        private const string ClientId = "[ClientID]";
        private const string ClientSecret = "[ClientSecret]";
        private const string RedirectUri = "[Callback]"; //回傳網址
        DataBasesEntities db = new DataBasesEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        

        [HttpGet]
        public ActionResult Authorize()
        {
            var authorizeUrl = $"{LineNotifyAuthorizeUrl}?response_type=code&client_id={ClientId}&redirect_uri={RedirectUri}&scope=notify&state=state";
            return Redirect(authorizeUrl);
        }
        

        [HttpGet]
        [Route("Home/Callback")]
        public async Task<ActionResult> Callback(string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                // 在這裡，您可以對授權碼（code）值進行處理，例如交換為存取令牌等操作
                Console.WriteLine("Authorization Code: " + code);
                ViewBag.code = code;

                // 調用 GetAccessToken 方法並傳遞授權碼作為參數
                string accessToken = await GetAccessToken(code);

                JObject accessTokenJson = JObject.Parse(accessToken);
                LineNotifyApiKey = (string)accessTokenJson["access_token"];

                if (!string.IsNullOrEmpty(LineNotifyApiKey))
                {
                    // 如果成功獲取存取令牌，您可以進一步處理它，例如儲存到資料庫中或使用它來呼叫其他 API
                    if (ModelState.IsValid)
                    {
                        var temp = db.member
                             .Where(m => m.account == "admin")
                             .FirstOrDefault();
                        temp.apikey = LineNotifyApiKey;

                        db.SaveChanges();
                    }

                    ViewBag.token = LineNotifyApiKey;

                    // 返回您想要顯示給用戶的視圖或重定向到其他頁面
                    return RedirectToAction("Send"); // 或者 return View("ViewName");
                }
                else
                {
                    // 如果無法獲取存取令牌，您可以返回相應的錯誤頁面或其他處理方式
                    return Content("Failed to get access token");
                }
            }
            else
            {
                // 如果沒有獲取到授權碼，可以返回錯誤頁面或者重新導向到其他地方
                return Content("Authorization Code Not Found");
            }
        }

        public async Task<string> GetAccessToken(string code)
        {

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var requestBody = new FormUrlEncodedContent(new[]
                    {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret)
            });

                    var response = await httpClient.PostAsync(LineNotifyTokenUrl, requestBody);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to get access token. Status code: {response.StatusCode}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // 處理可能的異常
                    Console.WriteLine($"Error occurred while getting access token: {ex.Message}");
                    return null;
                }
            }
        }



        [HttpGet]
        public ActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage(string message)
        {
            var keyapi = db.member
                   .Where(m => m.account == "admin")
                   .Select(m => m.apikey)
                   .FirstOrDefault();

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {keyapi}");

                var content = new StringContent($"message={message}", Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await client.PostAsync(LineNotifyApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // 發送成功
                    TempData["Message"] = "訊息已發送！";
                }
                else
                {
                    // 發送失敗
                    TempData["Message"] = $"訊息發送失敗。+{LineNotifyApiKey}";
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response status: {response.StatusCode}");
                    Console.WriteLine($"Response content: {responseContent}");


                }
            }

            return RedirectToAction("Send");
        }
    }
}