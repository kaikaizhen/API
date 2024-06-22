using Google.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Google.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private const string oauthurl = "https://accounts.google.com/o/oauth2/v2/auth";

        private const string client_id = "";

        private const string client_secret = "";

        private const string redirect_uri = "";

        private const string scope = "https://www.googleapis.com/auth/userinfo.profile";

        private const string tokenurl = "https://oauth2.googleapis.com/token";


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string Url = $"{oauthurl}?scope={scope}&include_granted_scopes=true&access_type=online&response_type=code&state=state&redirect_uri={redirect_uri}&client_id={client_id}";
            return Redirect(Url);
        }

        [HttpGet]
        [Route("Home/callback")]
        public async Task<ActionResult> Callback(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                string result = await Token(code);
                JObject accesstokenjson = JObject.Parse(result);
                var GoogleAPIKey = (string)accesstokenjson["access_token"];



                if (!string.IsNullOrEmpty(GoogleAPIKey))
                {
                    ViewBag.token = GoogleAPIKey;
                    string userInfoJson = await UserInfo(GoogleAPIKey);
                    JObject jsonData = JObject.Parse(userInfoJson);

                    if (!string.IsNullOrEmpty(userInfoJson))
                    {
                        
                        ViewBag.name = jsonData;
                        
                    }
                    else
                    {
                        ViewBag.token = "編譯Google帳戶資訊失敗";
                    }
                    return View("test");

                }
                else
                {
                    ViewBag.token = "無法擷取回傳資料";
                    return View("test");
                }

            }
            else
            {
                return Content("無法取得code");
            }

        }

        public async Task<string> Token(string code)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var values = new Dictionary<string, string>()
                    {
                        {"code",code },
                        {"client_id",client_id },
                        {"client_secret",client_secret },
                        {"redirect_uri",redirect_uri },
                        {"grant_type","authorization_code" }
                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await httpClient.PostAsync(tokenurl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"交換權杖失敗:{response.StatusCode}");
                        return null;
                    }
                    var responseContext = await response.Content.ReadAsStringAsync();
                    return responseContext;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"網路連線失敗:{ex.Message}");
                    return null;
                }
            }
        }

        public async Task<string> UserInfo(string accessToken)
        {
            using(var httpClient = new HttpClient())
            {
                try
                {
                    // 使用存取權杖來請求使用者資訊
                    var userInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var userInfoResponse = await httpClient.GetAsync(userInfoUrl);

                    if (!userInfoResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"無法獲取使用者資訊: {userInfoResponse.StatusCode}");
                        return null;
                    }

                    var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();
                    return userInfoContent;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"網路連線失敗:{ex.Message}");
                    return null;
                }
            }
        }

        public ActionResult test()
        {
            return View();
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
