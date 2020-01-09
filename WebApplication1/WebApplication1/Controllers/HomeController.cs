using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public readonly ExamContext _context;

        public HomeController(ExamContext context)
        {
            _context = context;
        }

        public IActionResult Scan()
        {
            ViewBag.Results = null;
            return View();
        }

        [HttpPost]
        public IActionResult Scan(string url, int depth)
        {
            ViewBag.Results = ScanHelper.GetResultFromUrlWithDepth(url, depth);
            var pages=ScanHelper.GetSavedPages();
            foreach (var page in pages)
            {
                _context.Add(page);
            }

            _context.SaveChanges();
            return View();
        }

        public IActionResult Results()
        {
            
            return View();
        }

        #region useless

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
#endregion
    }
}
