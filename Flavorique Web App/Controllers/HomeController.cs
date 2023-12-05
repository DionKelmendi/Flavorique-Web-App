using System.Diagnostics;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flavorique_Web_App.Controllers
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

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProvaMailer()
        {
            var reciever = "dionkelmendi@hotmail.com";
            var subject = "Prova";
            var message = "Hello!!!!";

            await _emailSender.SendEmailAsync(reciever, subject, message);

            return RedirectToAction("Index");
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