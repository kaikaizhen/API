using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalRChat.Models;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalRChat
{
    public class ChatHub : Hub
    {
        keydata DB=new keydata();
        DatabaseEntities1 Ms=new DatabaseEntities1();

        public void Connect(string userId,string connectionId)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(connectionId))
            {
                var existingUser = DB.Table.FirstOrDefault(u => u.user == userId);
                if (existingUser == null)
                {
                    var key = new Table
                    {
                        user = userId,
                        key = connectionId
                    };
                    DB.Table.Add(key);
                }
                else
                {
                    existingUser.key = connectionId; // 更新連結id
                }
                DB.SaveChanges();
            }
        }

        public void Send(string userId,string toUserId, string fromUserName, string message)
        {
            
            if (!string.IsNullOrEmpty(toUserId) && !string.IsNullOrEmpty(fromUserName) && !string.IsNullOrEmpty(message))
            {
                var toUsername = DB.Table.FirstOrDefault(u => u.user == toUserId);
                
                if (toUsername != null)
                {
                    var toUser = toUsername.key;
                    if (!string.IsNullOrEmpty(toUser))
                    {
                        Clients.Client(toUser).addNewMessageToPage(fromUserName, message);
                    }
                }
                var newmessage = new Messages
                {
                    SenderUserId = userId,
                    ReceiverUserId = toUserId,
                    MessageText = message,
                    SendTime = DateTime.Now
                };

                Ms.Messages.Add(newmessage);
                Ms.SaveChanges();
            }
           
        }

        public  Task GetPreviousData(string userId,string reuserId)
        {
            
           
            var num = Ms.Messages.FirstOrDefault(u => u.SenderUserId == userId || u.SenderUserId==reuserId);
            if (num != null) { 
            // 抓取以前聊天資料
            var previousData = (from b in Ms.Messages
                               where (b.SenderUserId == userId && b.ReceiverUserId == reuserId) ||(b.SenderUserId==reuserId && b.ReceiverUserId==userId)
                               select b.MessageText).ToList();

            // 將資料傳送給客戶端
             Clients.Client(Context.ConnectionId).initializeData(previousData);

            
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userkey = Context.ConnectionId;
            var user = DB.Table.FirstOrDefault(u => u.key == userkey);
            if (user != null)
            {
                user.key = "";
                DB.SaveChanges();
            }
            
            return base.OnDisconnected(stopCalled);
        }

        public void disconnection(string userId)
        {
            var User = DB.Table.FirstOrDefault(u=>u.user==userId);
            
            if (User !=null)
            {
                User.key = null;
                DB.SaveChanges();
            }
        }
    }
}
