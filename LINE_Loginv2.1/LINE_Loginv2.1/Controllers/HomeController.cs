using LINE_Loginv2._1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;

namespace LINE_Loginv2._1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public string redirect_url = "{Redirect_Url}";
        public string response_type = "code";
        public string client_id = "{Client_id}";
        public IActionResult Index()
        {
            
            string state = Guid.NewGuid().ToString();
            string scope = "profile%20openid";//使用的服務
            var url = $"https://access.line.me/oauth2/v2.1/authorize?response_type={response_type}&client_id={client_id}&redirect_uri={redirect_url}&state={state}&scope={scope}";
            return Redirect(url);
        }

        [HttpGet]
        [Route("Home/Callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if(!string.IsNullOrEmpty(code))
            {
                var response = await Token(code);
                if(!string.IsNullOrEmpty (response))
                {
                    var data=JObject.Parse(response);
                    string Token = (string)data["access_token"];
                    Console.WriteLine(Token);
                    ViewBag.Response = data;
                    if (!string.IsNullOrEmpty(Token))
                    {
                        var res = await GetInfo(Token);
                        if (!string.IsNullOrEmpty(res))
                        {
                            var info=JObject.Parse (res);
                            var Name = info["name"];
                            var Image= info["picture"];
                            ViewBag.Name=Name;
                            ViewBag.Image=Image;
                        }
                        else
                        {
                            ViewBag.Response = "無法取得用戶資料";
                        }
                    }
                    else
                    {
                        ViewBag.Response = "無法取得Token";
                    }
                }
            }
            return View("Index");
        }

        public async Task<string> Token(string code)
        {
            using(HttpClient Token=new HttpClient())
            {
                var form = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/oauth2/v2.1/token");
                var data = new Dictionary<string,string>
                {
                    {"grant_type","authorization_code"},
                    { "code",code },
                    { "redirect_uri",redirect_url },
                    { "client_id",client_id },
                    { "client_secret","{Client_secret}" }
                };
                form.Content=new FormUrlEncodedContent(data);
                var response = await Token.SendAsync(form);
                response.EnsureSuccessStatusCode();
                var responsejson=await response.Content.ReadAsStringAsync();
                return responsejson;
            }
            return null;
        }

        public async Task<string>GetInfo(string Token)
        {
            using (HttpClient GetInfo = new HttpClient())
            {
                var form = new HttpRequestMessage(HttpMethod.Get, "https://api.line.me/oauth2/v2.1/userinfo");
                GetInfo.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                var response=await GetInfo.SendAsync(form);
                response.EnsureSuccessStatusCode();
                var responsejson= await response.Content.ReadAsStringAsync();
                return responsejson;
            };

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
