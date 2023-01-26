using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ClientController : Controller
    {
        private readonly MyDbContext _context;
        public ClientController(MyDbContext context) { _context = context; }
        private string CookRole => Request.Cookies["role"];
        private string CookEmail => Request.Cookies["login"];
        public IActionResult Index()
        {
            var userList = from u in _context.Clients.ToList()
                           join c in _context.Countries.ToList() on u.CountryInfoKey equals c.ID
                           orderby c.Country
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

        public IActionResult Balance(int? id)
        {
            var userList = from u in _context.Clients.ToList()
                           join b in _context.Accounts.ToList() on u.ID equals b.UserId
                           where u.ID == id
                           select new UserList
                           {
                               Name = u.Name,
                               LastName = u.LastName,
                               IIN = u.IIN,
                               AccountNumber = b.AccountNumber,
                               Balance = b.Balance
                           };
            return View(userList);
        }

        public IActionResult Create() { return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserList obj)
        {
            if (ModelState.IsValid)
            {
                int countryInfoKey = _context.Countries.Where(c => c.Country == obj.Country && c.City == obj.City).Select(c => c.ID).FirstOrDefault();
                Client client = new Client
                {
                    Name = obj.Name,
                    LastName = obj.LastName,
                    SecondName = obj.SecondName,
                    Address = obj.Address,
                    IIN = obj.IIN,
                    DateOfBirth = obj.DateOfBirth,
                    CountryInfoKey = countryInfoKey,
                    UserAuthBankId = _context.UserAuthBanks.Where(r => r.Email == CookEmail).First().Id
                };
                client.DateOfBirth = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(client.DateOfBirth, "New Zealand Standard Time", "UTC");
                _context.Clients.Add(client);
                try
                {
                    _context.SaveChanges();
                }
                catch (System.Exception ex) {
                    Console.WriteLine("Error" + ex);
                    return RedirectToAction("Index");
                }
                var rnd = new Random();
                Account account = new Account()
                {
                    UserId = client.ID,
                    Balance = rnd.Next(100, 9999),
                    AccountNumber = rnd.Next(1000000, 9999999).ToString()
                };
                _context.Accounts.Add(account);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

       
        
        public IActionResult Country() {

            var u = from c in _context.Clients
                    join ci in _context.Countries on c.CountryInfoKey equals ci.ID
                    group c by ci.Country into g
                    select new CountryCount
                    {
                        Country = g.Key,
                        Count = g.Count()                        
                    };
            


            return View(u);
            //from c in _context.Countries orderby c.Country select c
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = _context.Clients.Find(id);
            var country = _context.Countries.Find(user.CountryInfoKey);
            var userlist = new UserList
            {
                Id = user.ID,
                Name = user.Name,
                LastName = user.LastName,
                SecondName = user.SecondName,
                Address = user.Address,
                IIN = user.IIN,
                DateOfBirth = user.DateOfBirth.Date,
                Country = country.Country,
                City = country.City
            };
            if (userlist == null)
            {
                return NotFound();
            }
            return View(userlist);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserList obj)
        {
            if (ModelState.IsValid)
            {
                int countryInfoKey = _context.Countries.Where(c => c.Country == obj.Country && c.City == obj.City).Select(c => c.ID).FirstOrDefault();
                Client client = new Client
                {
                    ID = obj.Id,
                    Name = obj.Name,
                    LastName = obj.LastName,
                    SecondName = obj.SecondName,
                    Address = obj.Address,
                    IIN = obj.IIN,
                    DateOfBirth = obj.DateOfBirth,
                    CountryInfoKey = countryInfoKey
                };
                client.DateOfBirth = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(client.DateOfBirth, "New Zealand Standard Time", "UTC");
                _context.Clients.Update(client);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Remove(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var user = _context.Clients.Find(id);
            var country = _context.Countries.Find(user.CountryInfoKey);
            var userlist = new UserList
            {
                Id = user.ID,
                Name = user.Name,
                LastName = user.LastName,
                SecondName = user.SecondName,
                Address = user.Address,
                IIN = user.IIN,
                DateOfBirth = user.DateOfBirth.Date,
                Country = country.Country,
                City = country.City
            };
            if (userlist == null)
            {
                return NotFound();
            }
            return View(userlist);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemovePOST(int? id)
        {
            var obj = _context.Clients.Find(id);
            _context.Clients.Remove(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
