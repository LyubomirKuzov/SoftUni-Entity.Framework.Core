using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuizSystem.Services;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuizSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IQuizService quizService;



        public HomeController(IQuizService quizService)
        {
            this.quizService = quizService;
        }

        public IActionResult Index()
        {
            var userQuizzes = this.quizService.GetQuizesByUserName(this.User?.Identity?.Name);

            return View(userQuizzes);
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
