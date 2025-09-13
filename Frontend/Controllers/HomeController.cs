using Frontend.HttpClients;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProtectedHttpClient _client;

        public HomeController(ProtectedHttpClient client, ILogger<HomeController> logger)
        {
            _client = client;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Protected()
        {
            var response = await _client.GetProtectedData();

            if (response == null || response.Success == false)
            {
                return RedirectToAction("RefreshTokens", "User");
            }

            return View((object) response.Data);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
