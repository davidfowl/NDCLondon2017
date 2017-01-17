using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUpload.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _environment;
        public HomeController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm(Name = "data")]IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var path = Path.Combine(_environment.ContentRootPath, file.FileName);
                using (var output = System.IO.File.Create(path))
                {
                    await stream.CopyToAsync(output);
                }
            }

            return RedirectToAction(nameof(Index));
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

        public IActionResult Error()
        {
            return View();
        }
    }
}
