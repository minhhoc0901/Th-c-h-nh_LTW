using Microsoft.AspNetCore.Mvc;

namespace MinhHoc.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
