using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core;
using WebApplication1.Enum;
using WebApplication1.Helper;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        private readonly MyDbContext _context;
        public AuthController(MyDbContext context) { _context = context; }

        public ActionResult Login()
        {
            var cookies = Request.Cookies["login"];
            if (cookies != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(DtoReg dto)
        {            
            if(!_context.UserAuthBanks.Any(r => r.Email == dto.Email && r.Password==new RegHelper().GetHash(dto.Password)))
            {
                return RedirectToAction("Error", "Error", new ErrorModel { ErrorMessage = "Email не зарегистрирован в системе" });
            }
            var user = _context.UserAuthBanks.Where(r => r.Email == dto.Email && r.Password == new RegHelper().GetHash(dto.Password)).First();
            if (!user.IsEmailValidate) return RedirectToAction("Validate", "Auth", user);
            Response.Cookies.Append("login", user.Email);
            Response.Cookies.Append("role", user.AccessLevel.ToString());
            return RedirectToAction("Index","Home");
        }


        public ActionResult Register()
        {
            var cookies = Request.Cookies["login"];
            if (cookies != null)
            {
                return RedirectToAction("Index", "Home");
            }
            DtoReg dto = new DtoReg();
            return View(dto);
        }


        [HttpPost]
        public IActionResult Register(DtoReg dto)
        {
            if (_context.UserAuthBanks.Any(r => r.Email == dto.Email)) return RedirectToAction("Error", "Error", new ErrorModel { ErrorMessage = "Email зарегистрирован в системе" });
            if (!new RegHelper().IsEmail(dto.Email)) return RedirectToAction("Error", "Error", new ErrorModel { ErrorMessage = "Email указан не правильно" });
            if (dto.AutoPass)
            {
                dto.Password = new RegHelper().CreateRandomPassword();
                dto.Password2 = dto.Password;
            }
                  
            if (!dto.Password.Equals(dto.Password2)) return RedirectToAction("Error", "Error", new ErrorModel { ErrorMessage = "Пароли не совпадают" });
            if (!new RegHelper().IsPass(dto.Password)) return RedirectToAction("Error", "Error", new ErrorModel { ErrorMessage = "Пароль должен содержать более 8 символов верхнего нижнего регистра, цифр и спецсимвола" });
            var role = EAccessLevel.User;
            var user = new UserAuthBank
            {
                Email = dto.Email,
                Password = new RegHelper().GetHash(dto.Password),
                AccessLevel = role,
                IsEmailValidate = false,
                ValidateCode = new Random().Next(0, 1000000).ToString("D6")
            };
            _context.UserAuthBanks.Add(user);
            _context.SaveChanges();
            new RegHelper().EmailValidate(user.Email, user.ValidateCode, dto.Password);
            return RedirectToAction("Validate", "Auth", user);
        }

        public ActionResult Validate(UserAuthBank inf)
        {
            return View(new DtoReg { Email = inf.Email});
        }
        [HttpGet]
        public void CodeSend(string? Email)
        {
            var user = _context.UserAuthBanks.Where(r => r.Email == Email).First();
            user.ValidateCode = new Random().Next(0, 1000000).ToString("D6");
            _context.UserAuthBanks.Update(user);
            _context.SaveChanges();
            new RegHelper().EmailValidate(user.Email, user.ValidateCode);
        }

        [HttpPost]
        public IActionResult Validate(DtoReg dto)
        {
            var user = _context.UserAuthBanks.Where(r => r.Email == dto.Email).First();
            if (user.ValidateCode == dto.Code)
            {
                user.IsEmailValidate = true;
            }
            else
            {
                return RedirectToAction("Error", "Error", new ErrorModel { ErrorMessage = "Неправильный код" });
            }
            _context.UserAuthBanks.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Login","Auth");
        }

        public ActionResult LogOut()
        {
            Response.Cookies.Delete("role");
            Response.Cookies.Delete("login");
            return RedirectToAction("Login", "Auth");
        }


    }
}
