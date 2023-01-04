using Microsoft.AspNetCore.Mvc;
using UrlShortner.Core;
using UrlShortner.DataAccess;

namespace UrlShortner.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUrlRepository _urlRepository;
        
        public HomeController(IUrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }

        [HttpGet]
        public IActionResult Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("Index");
            }
            
            var originalUrl = _urlRepository.GetById(id);

            if (string.IsNullOrWhiteSpace(originalUrl))
            { 
                return View("NotFound");
            }

            return Redirect(originalUrl);
        }

        [HttpPost]
        public IActionResult Create(string url)
        {
            var id = IdGenerator.GenerateId();
            _urlRepository.Save(id, url);

            TempData["IndexLink"] = Url.Link("index", new {id});

            return RedirectToAction("Index");
        }
    }
}