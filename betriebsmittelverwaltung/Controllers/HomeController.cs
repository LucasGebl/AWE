using betriebsmittelverwaltung.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace betriebsmittelverwaltung.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
          return View();
        }

        public IActionResult Baustellenverwaltung()
        {
            //ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Bestandsverwaltung()
        {
            //ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Auftragsverwaltung()
        {
            //ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Retourenverwaltung()
        {
           // ViewData["Message"] = "Your contact page.";

            return View();
        }
        public IActionResult Nutzerverwaltung()
        {
          //  ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Impressum()
        {
           // ViewData["Message"] = "Your contact page.";

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
