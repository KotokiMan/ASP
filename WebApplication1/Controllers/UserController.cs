using Microsoft.AspNetCore.Mvc;
using MimeKit.Cryptography;
using WebApplication1.Core;
using WebApplication1.Helper;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly MyDbContext _context;
        public UserController(MyDbContext context) { _context = context; }

        private string CookRole => Request.Cookies["role"];
        private string CookEmail => Request.Cookies["login"];
        private void IsAuth()
        {
            if (string.IsNullOrEmpty(CookEmail)) RedirectToAction("Login", "Auth");
        }
        public ActionResult Cabinet()
        {
            var accountId = _context.UserAuthBanks.Where(r => r.Email == CookEmail).First().Id;
            var bankAccount = _context.Clients.Where(r => r.UserAuthBankId == accountId).ToList();
            if(bankAccount.Count==0)
            {
                return RedirectToAction("Create");
            }
            var userList = from t in _context.UserAuthBanks.ToList()
                           join u in _context.Clients.ToList() on t.Id equals u.UserAuthBankId
                           join c in _context.Countries.ToList() on u.CountryInfoKey equals c.ID
                           where t.Email == CookEmail
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

        public ActionResult Create() => View();

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
                catch (System.Exception ex)
                {
                    Console.WriteLine("Error" + ex);
                    return RedirectToAction("Index");
                }                
                Account account = new Account()
                {
                    UserId = client.ID,
                    Balance = 0,
                    AccountNumber = new Random().Next(100000000, 999999999).ToString("D9")
                };
                _context.Accounts.Add(account);
                _context.SaveChanges();

                return RedirectToAction("Cabinet");
            }
            return View(obj);
        }

        public IActionResult Balance(int? id)
        {
            var userList = from u in _context.Clients.ToList()
                           join b in _context.Accounts.ToList() on u.ID equals b.UserId
                           where u.ID == id
                           select new UserList
                           {
                               Id = u.ID,
                               Name = u.Name,
                               LastName = u.LastName,
                               IIN = u.IIN,
                               AccountNumber = b.AccountNumber,
                               Balance = b.Balance
                           };
            return View(userList);
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
                return RedirectToAction("Cabinet");
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
            return RedirectToAction("Cabinet");
        }

        public ActionResult Remittance(int? id)
        {
            var dto = from u in _context.Clients.ToList()
                           join b in _context.Accounts.ToList() on u.ID equals b.UserId
                           where u.ID == id
                           select new DtoOperation
                           {
                               MyBalance=b.Balance,
                               NumberAccountRecepient=b.AccountNumber
                           };
           
            return View(dto.First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remittance(DtoOperation dto)
        {
            var entity = new BankOperationHistiry
            {
                NumberAccountRecepient = dto.NumberAccountRecepient,
                NumberAccountSending = dto.NumberAccountSending,
                operationType = Enum.EOperationType.Remittance,
                TransferSum = dto.TransferSum
            };
            _context.BankOperation.Add(entity);
            var userRecepient = _context.Accounts.Where(r => r.AccountNumber == dto.NumberAccountRecepient).First();
            userRecepient.Balance -= dto.TransferSum;
            _context.Accounts.Update(userRecepient);
            var userSending = _context.Accounts.Where(r => r.AccountNumber == dto.NumberAccountSending).First();
            userSending.Balance += dto.TransferSum;
            _context.Accounts.Update(userSending);
            _context.SaveChanges();
            return RedirectToAction("Cabinet");
        }

        public ActionResult Refill(int? id)
        {
            var dto = from u in _context.Clients.ToList()
                      join b in _context.Accounts.ToList() on u.ID equals b.UserId
                      where u.ID == id
                      select new DtoOperation
                      {
                          MyBalance = b.Balance,
                          NumberAccountRecepient = b.AccountNumber
                      };

            return View(dto.First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Refill(DtoOperation dto)
        {
            var entity = new BankOperationHistiry
            {
                NumberAccountRecepient = dto.NumberAccountRecepient,
                operationType = Enum.EOperationType.Refill,
                TransferSum = dto.TransferSum
            };
            _context.BankOperation.Add(entity);
            var userRecepient = _context.Accounts.Where(r => r.AccountNumber == dto.NumberAccountRecepient).First();
            userRecepient.Balance += dto.TransferSum;
            _context.Accounts.Update(userRecepient);
            _context.SaveChanges();
            return RedirectToAction("Cabinet");
        }

        public ActionResult Withdrawal(int? id)
        {
            var dto = from u in _context.Clients.ToList()
                      join b in _context.Accounts.ToList() on u.ID equals b.UserId
                      where u.ID == id
                      select new DtoOperation
                      {
                          MyBalance = b.Balance,
                          NumberAccountRecepient = b.AccountNumber
                      };

            return View(dto.First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Withdrawal(DtoOperation dto)
        {
            var entity = new BankOperationHistiry
            {
                NumberAccountRecepient = dto.NumberAccountRecepient,
                operationType = Enum.EOperationType.Withdrawal,
                TransferSum = dto.TransferSum
            };
            _context.BankOperation.Add(entity);
            var userRecepient = _context.Accounts.Where(r => r.AccountNumber == dto.NumberAccountRecepient).First();
            userRecepient.Balance -= dto.TransferSum;
            _context.Accounts.Update(userRecepient);
            _context.SaveChanges();
            return RedirectToAction("Cabinet");
        }

        public FileStreamResult PdfGeneration()
        {
            var accountId = _context.UserAuthBanks.Where(r => r.Email == CookEmail).First().Id;
            return new PdfHelper(_context).CreateTablePdf(accountId);
            //var bankAccount = _context.Clients.Where(r => r.UserAuthBankId == accountId).ToList();
        }
        
        public FileStreamResult PdfGenerationn(string path)
        {
            return new PdfHelper(_context).CreateTablePdff(path);
            //var bankAccount = _context.Clients.Where(r => r.UserAuthBankId == accountId).ToList();
        }
    }
}
