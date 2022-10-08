using Microsoft.AspNetCore.Mvc;

namespace CNJewellerAdmin.Controllers
{
    public class ProductCatalogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
