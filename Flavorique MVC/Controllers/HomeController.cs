using System.Diagnostics;
using System.Net;
using Flavorique_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Flavorique_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var userRole = await GetUserRole();

            if (userRole.Equals("Admin")) {
                return View();
            }
            else
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

        }

        public async Task<IActionResult> Mail()
        {
            var userRole = await GetUserRole();

            if (userRole.Equals("Admin"))
            {
                return View();
            }
            else
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }
        }

        public async Task<IActionResult> ProvaMailer()
        {
            var reciever = "dionkelmendi@hotmail.com";
            var subject = "Prova";
            var message = "Hello!!!!";

            await _emailSender.SendEmailAsync(reciever, subject, message);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<string> GetUserRole() {

            var role = "User";

            using (var handler = new HttpClientHandler())
            {
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(new Uri("https://localhost:7147"), new Cookie(".AspNetCore.Identity.Application", Request.Cookies[".AspNetCore.Identity.Application"]));

                handler.CookieContainer = cookieContainer;

                using (var client = new HttpClient(handler))
                {
                    {
                        var response = await client.GetAsync("https://localhost:7147/api/Account/user/role");
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        role = apiResponse;
                    }
                }
            }
            return role;
        }
    }
}