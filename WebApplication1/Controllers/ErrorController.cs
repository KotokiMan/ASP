using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error(ErrorModel dto)
        {
            return View(dto);
        }
    }
}
