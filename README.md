# 描述
------
該文件為使用Web Request API串接各項技術之文件，內容有:
1. Google第三方登入
2. Google行事曆
3. LINE Notify
4. LINE Pay
5. SignalR聊天室
6. OpenStreetMap地圖

# Google 第三方登入 <img src="https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_92x30dp.png" alt="Google" width="100">
------
## 版本
- 開發環境:ASP.NET
- 專案類型:Web Core 應用程式
- 版本:6.0.0
## 運作邏輯
透過了解[API](https://developers.google.com/identity/protocols/oauth2?hl=zh-tw#libraries)運作邏輯，即可完成API之串接，以下為詳細步驟:
1. 將使用者傳入 [網址](https://accounts.google.com/o/oauth2/v2/auth??scope=scope&include_granted_scopes=true&access_type=online&response_type=code&state=state&redirect_uri=redirect_uri&client_id=client_id)，請將[scope](https://developers.google.com/identity/protocols/oauth2/scopes?hl=zh-tw)改為想要的服務網址、redirect_url改為自己的回傳網址、client_id改為自己的服務ID
2. 解析回傳網址中的**code值**
3. 使用**code**發送Web表單訊息至 [網址](https://oauth2.googleapis.com/token)，body需有code、client_id、client_secret、redirect_url、grant_type
4. 換取Token
5. 解析回傳的**access_token**內容
6. 使用Token發送GET請求至 [網址](https://www.googleapis.com/oauth2/v2/userinfo)，標頭Bearer為Token
7. 成功獲取使用者資料
## 如何使用
1. 下載專案
2. 開啟專案中的Google.sln
3. 打開Controller資料夾中的HomeController
4. 更改[Client_ID] [Client_Secret] [CallbackURL]
5. 執行專案

# Google行事曆 <img src="https://fonts.gstatic.com/s/i/productlogos/calendar_2020q4/v13/192px.svg" alt="Google行事曆" width="50">
------
## 版本
- 開發環境:ASP.NET
- 專案類型:Web 應用程式
- 版本:4.7.2
## 運作邏輯
透過了解[API](https://developers.google.com/identity/protocols/oauth2?hl=zh-tw#libraries)運作邏輯，即可完成API之串接，以下為詳細步驟:
1. 將使用者傳入 [網址](https://accounts.google.com/o/oauth2/v2/auth??scope=scope&include_granted_scopes=true&access_type=online&response_type=code&state=state&redirect_uri=redirect_uri&client_id=client_id)，請將[scope](https://developers.google.com/identity/protocols/oauth2/scopes?hl=zh-tw)改為想要的服務網址、redirect_url改為自己的回傳網址、client_id改為自己的服務ID
2. 解析回傳網址中的**code值**
3. 使用**code**發送Web表單訊息至 [網址](https://oauth2.googleapis.com/token)，body需有code、client_id、client_secret、redirect_url、grant_type
4. 換取Token
5. 解析回傳的**access_token**內容
6. 使用Token發送POST請求至 [網址](https://www.googleapis.com/calendar/v3/calendars/primary/events)，標頭Bearer為Token，body格式為**application/json**需有summary(行事曆名稱)、description(描述)、start與end需有dataTime(時間)及timeZone(時區)、location(地點)、reminders需有useDefault(預設為false)、overrides內含method(模式)與minutes(時間)
7. 成功新增行事曆
## 如何使用
1. 下載專案
2. 開啟專案中的GoogleOauth2.0.sln
3. 打開Controller資料夾中的HomeController
4. 更改[Client_ID] [Client_Secret] [CallbackURL]
5. 執行專案

# LINE Notify <img src="https://tw-developer.gallerycdn.vsassets.io/extensions/tw-developer/linenotify/0.1.6/1681115220777/Microsoft.VisualStudio.Services.Icons.Default" alt="LINE Notify" width="50">
------
## 版本
- 開發環境:ASP.NET
- 專案類型:Web 應用程式
- 版本:4.7.2
## 運作邏輯
透過了解[API](https://notify-bot.line.me/doc/en/)運作邏輯，即可完成API之串接，以下為詳細步驟:
1. 將使用者傳入 [網址](https://notify-bot.line.me/oauth/authorize?response_type=code&client_id=ClientId&redirect_uri=RedirectUri&scope=notify&state=state)，將ClientID改為自己申請到的ID，RedirectUri改為自己的回傳網址
2. 解析回傳網址中的**code值**
3. 使用**code**發送Web表單訊息至 [網址](https://notify-bot.line.me/oauth/token)，body需有code、client_id、client_secret、redirect_uri、grant_type
4. 換取Token
5. 解析回傳的**access_token**內容
6. 使用Token發送POST請求至 [網址](https://notify-api.line.me/api/notify)，標頭Bearer為Token，body格式為**application/x-www-form-urlencoded**需有message
7. 成功發送訊息
## 如何使用
1. 下載專案
2. 開啟專案中的Demo.sln
3. 打開Controller資料夾中的HomeController
4. 更改[ClientID] [ClientSecret] [Callback]
5. 執行專案

# LINE Pay  <img src="https://d.line-scdn.net/linepay/portal/v-240530/portal/assets/img/portal/login-logo-pay.svg?dm=1717031547224" alt="LINE Pay" width="100">
------
## 版本
- 開發環境:ASP.NET
- 專案類型:Web 應用程式
- 版本:4.7.2
## 運作邏輯
透過了解[API]())運作邏輯，即可完成API之串接，以下為詳細步驟:
### 付款
1. 發送POST請求表單至[網址](https://sandbox-api-pay.line.me/v2/payments/request)，標頭需有X-LINE-ChannelId、X-LINE-ChannelSecret，body需有amount(金額)、confirmUrl(回傳網址)、productName(名稱)、orderId(訂單編號)、currency(貨幣類型)
2. 解析回傳的結果**info中的paymentUrl中的web**
3. 將使用者傳入該網址
4. 付款完成
### 驗證 
1. 付款完成後，使用者傳入回傳網址
2. 解析網址中的**transactionId**
3. 發送POST請求表單至[網址](https://sandbox-api-pay.line.me/v2/payments/transactionId/confirm)，請將transactionId改為你抓取到的transactionId，標頭需有X-LINE-ChannelId、X-LINE-ChannelSecret，body格式為**application/json**需有amount(金額)、currency(貨幣類型)
4. 解析回傳結果**returnCode**
5. 成功驗證
## 如何使用
1. 下載專案
2. 開啟專案中的Linepayapi.sln
3. 打開Controller資料夾中的HomeController
4. 更改[X-LINE-ChannelId][X-LINE-ChannelSecret]
5. 執行專案
