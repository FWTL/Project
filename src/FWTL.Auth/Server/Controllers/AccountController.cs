using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Auth.Server.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult SendCode()
        {
            return View();
        }
    }
}