using Chat_SignalR_Demo.Models;
using Chat_SignalR_Demo.SignalR_Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;

namespace Chat_SignalR_Demo.Hubs
{
    public class NewChatHub:Hub
    {
        private readonly AppDbContext context;

        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }



        public NewChatHub(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext _context)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            context = _context;

        }
       [HubMethodName("login")]
       public async Task Login(string name, string password, bool RememberMe)
       {
           var user = await UserManager.FindByNameAsync(name);
           if (user != null)
           {
               var validPassword = await UserManager.CheckPasswordAsync(user, password);
               if (validPassword)
                {
                    var userPrincipal = await SignInManager.CreateUserPrincipalAsync(user);

                    if (!SignInManager.IsSignedIn(userPrincipal))
                    {
                          await SignInManager.SignInAsync(user, RememberMe);
                    }
                    context.UserConnections.Add(new UserConnection()
                    {
                        ConnectionId = Context.ConnectionId,
                        UserId = user.Id
                    });

                    context.SaveChanges();
                    await Clients.Caller.SendAsync("loggedin", user.Id);
                   return; // Exit the method after successful login
               }
           }

           // Login failed, send a message to the client
           await Clients.Caller.SendAsync("loggedin", null);
       }
        [HubMethodName("sendMessagetogroup")]
        public void SendMessageToGroup(string name, string Gname, string message)
        {
            if (!(Gname == "" || message == ""))
            {
                var conversation = new Conversation()
                {
                    Name = name,
                    Message = message,
                    Kind = kind.SpecificGroup,
                    Gname = Gname,
                    IsSender = Context.UserIdentifier
                    ,
                };
                context.conversation.Add(conversation);
                context.SaveChanges();
                var userConnections = context.UserConnections.Where(x => x.UserId == Context.UserIdentifier).Select(x => x.ConnectionId).ToList();
                bool issender = userConnections.Contains(Context.ConnectionId);
                Clients.GroupExcept(Gname, Context.ConnectionId).SendAsync("gmessage", name, message, Gname, "\t\t(" + DateTime.Now.ToString("dd/MMMM/yyyy : hh/mm") + ")", !issender, conversation.id);
                Clients.Caller.SendAsync("gmessage", name, message, Gname, "\t\t(" + DateTime.Now.ToString("dd/MMMM/yyyy : hh/mm") + ")", issender, conversation.id);

            }
        }
        public void getAllMessages(string name, string sender)
        {
            var user = context.UserConnections.FirstOrDefault(x => x.ConnectionId == sender);
            if (user == null)
            {
                // Optionally log the issue and return or handle it as needed
                return;
            }

            var sent_Messages = context.conversation
                .Where(x => x.Name == name && x.IsSender == user.UserId)
                .ToList();

            Clients.Caller.SendAsync("messages_sent", sent_Messages);
        }

        #region sendMessage to all
        public void sendMessage(string name, string message)
        {
            //broadcasting to all online clients
            if (message != "")
            {
                var user = context.Users.FirstOrDefault(x => x.UserName == name);
                var conversation = new Conversation()
                {
                    Name = name,
                    Message = message,
                    Kind = kind.AllGroups,
                    IsSender = user.Id
                };
                context.conversation.Add(conversation);
                context.SaveChanges();
                if (user != null)
                {
                    var userConnections = context.UserConnections.Where(x => x.UserId == user.Id).Select(x => x.ConnectionId).ToList();
                    bool issender = userConnections.Contains(Context.ConnectionId);
                    Clients.Others.SendAsync("sendNewMessage", name, message, "\t\t(" + DateTime.Now.ToString("dd/MMMM/yyyy : hh/mm") + ")", !issender, conversation.id);//
                    Clients.Caller.SendAsync("sendNewMessage", name, message, "\t\t(" + DateTime.Now.ToString("dd/MMMM/yyyy : hh/mm") + ")", issender, conversation.id);

                }

            }

        }
        [HubMethodName("isOnline")]
        public async Task IsOnline(string ID)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool online = false;

            if (!string.IsNullOrEmpty(ID))
            {
                online = context.UserConnections.Any(x => x.UserId == ID);
            }

            await Clients.All.SendAsync("sendClientState", online);
        }
        [HubMethodName("deleteMessage")]
        public void DeleteMessage(int id)
        {
            var message = context.conversation.FirstOrDefault(x => x.id == id);
            message.Deleted = true;
            context.conversation.Update(message);
            context.SaveChanges();
            Clients.Others.SendAsync("removeMessageSR", message.id);
        }

        #endregion
        [HubMethodName("joinGroup")]
        public void JoinMemberToGroup(string name, string Gname)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, Gname);
            Clients.OthersInGroup(Gname).SendAsync("newMember", name, Gname, "\t\t(" + DateTime.Now.ToString("dd/MMMM/yyyy : hh/mm") + ")");
            Clients.Caller.SendAsync("menewgroup", Gname, "\t\t(" + DateTime.Now.ToString("dd/MMMM/yyyy : hh/mm") + ")");
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                context.UserGroups.Add(new UserGroups()
                {
                    UserId = userId
                    ,
                    Gname = Gname
                });
                context.SaveChanges();
            }
        }
        public async Task someoneTypingMessage(string name)
        {
            await Clients.Others.SendAsync("someBodyTypingMessage", name);
        }

        public override Task OnConnectedAsync()
        {

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                context.UserConnections.Add(new UserConnection()
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = userId
                });

                context.SaveChanges();
                if (context.UserGroups.Any(x => x.UserId == userId))
                {
                    foreach (var item in context.UserGroups.Where(x => x.UserId == userId))
                    {

                        Groups.AddToGroupAsync(Context.ConnectionId, item.Gname);
                    }
                }
            }

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var User = context.UserConnections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (User != null)
            {
                context.UserConnections.Remove(User);
                context.SaveChanges();
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
