using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat_SignalR_Demo.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly AppDbContext context;

        public ChatController(AppDbContext _context)
        {
            context = _context;
        }
        public IActionResult Index()
        {
            return View(context.conversation.ToList());
        }
   


    }
}
