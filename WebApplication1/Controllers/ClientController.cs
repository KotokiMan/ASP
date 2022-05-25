using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ClientController : Controller
    {
        private readonly MyDbContext _context;
       

        public ClientController(MyDbContext context)
        {
            _context = context;
        }        



        public IActionResult Index()
        { 
            var userList = from u in _context.Clients.ToList()
                           join c in _context.Countries.ToList() on u.CountryInfoKey equals c.ID
                           select new UserList
                           {
                               Id = u.ID,
                               Name = u.Name,
                               LastName = u.LastName,
                               SecondName = u.SecondName,
                               Address = u.Address,
                               IIN = u.IIN,
                               DateOfBirth = u.DateOfBirth.Date,
                               Country = c.Country,
                               City = c.City
                           };
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd"));
            return View(userList);
        }
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Client obj)
        {
            
            if (ModelState.IsValid)
            {
                obj.DateOfBirth = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.DateOfBirth, "New Zealand Standard Time", "UTC");
                _context.Clients.Add(obj);
                _context.SaveChanges();
                var rnd = new Random();
                Account account = new Account()
                {
                    UserId = obj.ID,
                    Balance = rnd.Next(100, 9999),
                    AccountNumber = rnd.Next(1000000, 9999999).ToString()
                };
                _context.Accounts.Add(account);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }        
    }
}
