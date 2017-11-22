namespace GuessTheAnimal.Web.Controllers
{
    using System.Diagnostics;

    using GuessTheAnimal.Web.Models;

    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}