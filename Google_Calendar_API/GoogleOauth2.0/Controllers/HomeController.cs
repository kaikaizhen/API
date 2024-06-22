using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoogleOauth2._0.Controllers
{
    public class HomeController : Controller
    {
        private const string oauthurl = "https://accounts.google.com/o/oauth2/v2/auth";//登入驗證
        
        private const string client_id = "[ClientID]";

        private const string client_secret = "[ClientSecret]";

        private const string redirect_uri = "[CallBack]";

        private const string scope = "https://www.googleapis.com/auth/calendar.events"; //使用哪個服務

        private const string tokenurl = "https://oauth2.googleapis.com/token";

        private const string calendarsurl = "https://www.googleapis.com/calendar/v3/calendars/primary/events";//使用哪個服務
        // GET: Home
        public ActionResult Index()
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
                JObject accesstokenjson= JObject.Parse(result);
                var GoogleAPIKey = (string)accesstokenjson["access_token"];



                if (!string.IsNullOrEmpty(GoogleAPIKey))
                {
                    ViewBag.token = GoogleAPIKey;
                    string events = await AddCalendarEvent(GoogleAPIKey);
                    JObject jsonData=JObject.Parse(events);
                    string jsonDataView = jsonData.ToString();
                    if (!string.IsNullOrEmpty(events))
                    {
                        ViewBag.token = events;
                    }
                    else
                    {
                        ViewBag.token = "編譯日歷json失敗";
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
                    
                    var response=await httpClient.PostAsync(tokenurl,content);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"交換權杖失敗:{response.StatusCode}");
                        return null;
                    }
                    var responseContext=await response.Content.ReadAsStringAsync();
                    return responseContext;
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"網路連線失敗:{ex.Message}");
                    return null;
                }
           }
        }

        public async Task<string> AddCalendarEvent(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var apiUrl = "https://www.googleapis.com/calendar/v3/calendars/primary/events";

                    var jsonEvent = new
                    {
                        summary = "測試",
                        description = "這是個測試",
                        start = new
                        {
                            dateTime = "2024-04-27T10:00:00",
                            timeZone = "Asia/Taipei"
                        },
                        end = new
                        {
                            dateTime = "2024-04-27T11:00:00",
                            timeZone = "Asia/Taipei"
                        },
                        location = "台北市北投區復興公園",
                        reminders = new
                        {
                            useDefault = false, // 不使用預設提醒
                            overrides = new[]
                            {
                                new { method = "popup", minutes = 30 } // 30分鐘前彈出提醒
                            }
                        }

                    };

                    var jsonEventString = JsonConvert.SerializeObject(jsonEvent);

                    var content = new StringContent(jsonEventString, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"新增日曆活動失敗:{response.StatusCode}");
                        return null;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"發生錯誤:{ex.Message}");
                    return null;
                }
            }
        }

        public ActionResult test()
        {
            return View();
        }
    }
}