using Chat_SignalR_Demo.DTOs;
using Chat_SignalR_Demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Chat_SignalR_Demo.Controllers
{
    public class AccountController : Controller
    {
        public AppDbContext Context { get; }
        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }

        public AccountController(AppDbContext context, UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            Context = context;
            UserManager = userManager;
            SignInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(UserSignupDTO userSignup)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = userSignup.Name;
                user.PasswordHash = userSignup.Password;
                user.Email = userSignup.email;
                var result = await UserManager.CreateAsync(user, userSignup.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }
                    return View(userSignup);
                }
            }
            return View(userSignup);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {
            if (ModelState.IsValid) 
            {
                var user = await UserManager.FindByNameAsync(userLoginDTO.Name);
                if (user != null) 
                {
                    var validpassword = await UserManager.CheckPasswordAsync(user,userLoginDTO.Password);
                    if (validpassword) 
                    {
                        await SignInManager.SignInAsync(user,userLoginDTO.Remember_me);
                        return RedirectToAction("Index", "Chat");
                    }
                }
            }
            return View(userLoginDTO);
        }
    }

}
