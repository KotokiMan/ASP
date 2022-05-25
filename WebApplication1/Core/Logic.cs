/*using System;
using System.Globalization;
using WebApplication1.Models;

namespace WebApplication1.Core
{
    class Logic
    {
        static void log()
        {
            var p = new Logic();
            p.Menu();
        }

        private void Menu()
        {
            Console.WriteLine("1. Добавить Пользователя");
            Console.WriteLine("2. Просмотр пользователей");
            Console.WriteLine("3. Переводы");
            Console.WriteLine("4. Страны");
            Console.WriteLine("5. Добавить страну");
            Console.WriteLine("6. Выход");
            Console.WriteLine("7. Статистика");
            switch (Console.ReadLine())
            {
                case "1":
                    AddUser();
                    Menu();
                    break;
                case "2":
                    ViewUsers();
                    Menu();
                    break;
                case "3":
                    BankOperation();
                    Menu();
                    break;
                case "4":
                    readCountry();
                    Menu();
                    break;
                case "5":
                    AddCountry();
                    Menu();
                    break;
                case "6":
                    break;
                case "7":
                    statistic();
                    Menu();
                    break;
                default:
                    Menu();
                    break;
            }

        }

        private void BankOperation()
        {
            Console.WriteLine("1. Перевод");
            Console.WriteLine("2. Пополнение");
            Console.WriteLine("3. Снятие");
            Console.WriteLine("4. Назад");
            switch (Console.ReadLine())
            {
                case "1":
                    Transfer();
                    BankOperation();
                    break;
                case "2":
                    AddBalance();
                    BankOperation();
                    break;
                case "3":
                    Withdraw();
                    BankOperation();
                    break;
                case "4":
                    Menu();
                    break;
                default:
                    BankOperation();
                    break;
            }

        }
        private void Read(MyDbContext context)
        {

            var users = context.Clients.ToList();
            var account = context.Accounts.ToList();
            var userAccount = from u in users
                              join a in account on u.ID equals a.UserId
                              select new
                              {
                                  u.ID,
                                  u.Name,
                                  u.LastName,
                                  u.IIN,
                                  a.AccountNumber,
                                  a.Balance
                              };
            Console.WriteLine("Users list:");
            Console.WriteLine($"ID\tName\tLastName\tIIN\tAccountNumber\tBalance");
            foreach (var u in userAccount)
            {
                Console.WriteLine($"{u.ID}\t{u.Name}\t{u.LastName}\t{u.IIN}\t{u.AccountNumber}\t\t{u.Balance}");
            }

        }
        private void Withdraw()
        {
            using (var context = new MyDbContext())
            {
                Read(context);
                Console.WriteLine("Введите номер счета");
                var accountNumber = Console.ReadLine();
                Console.WriteLine("Введите сумму");
                var sum = Convert.ToDouble(Console.ReadLine());
                var account = context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
                if (account.Balance >= sum)
                {
                    account.Balance -= sum;
                    context.SaveChanges();
                    Console.WriteLine("Снятие прошло успешно");
                }
                else
                {
                    Console.WriteLine("Недостаточно средств");
                }
            }
        }

        private void AddBalance()
        {
            using (var context = new MyDbContext())
            {
                Read(context);
                Console.WriteLine("Введите номер счета");
                var accountNumber = Console.ReadLine();
                Console.WriteLine("Введите сумму");
                var sum = Convert.ToDouble(Console.ReadLine());
                var account = context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
                account.Balance += sum;
                context.SaveChanges();
                Console.WriteLine("Пополнение прошло успешно");
            }
        }

        private void Transfer()
        {
            using (var context = new MyDbContext())
            {
                Read(context);
                Console.WriteLine("Введите номер счета отправителя");
                var accountNumber = Console.ReadLine();
                Console.WriteLine("Введите номер счета получателя");
                var accountNumber2 = Console.ReadLine();
                Console.WriteLine("Введите сумму");
                var sum = Convert.ToDouble(Console.ReadLine());
                var account = context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
                var account2 = context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber2);
                if (account.Balance >= sum)
                {
                    account.Balance -= sum;
                    account2.Balance += sum;
                    context.SaveChanges();
                    Console.WriteLine("Перевод прошел успешно");
                }
                else
                {
                    Console.WriteLine("Недостаточно средств");
                }

            }
        }

        private void Accounts(int id)
        {
            using (var context = new MyDbContext())
            {
                var rnd = new Random();
                var account = new Account()
                {
                    AccountNumber = rnd.Next(1000000, 9999999).ToString(),
                    Balance = rnd.Next(100, 1000),
                    UserId = id
                };
                context.Accounts.Add(account);
                context.SaveChanges();
            }
        }

        private void AddUser()
        {
            using (var context = new MyDbContext())
            {
                Console.WriteLine("Введите имя");
                var name = Console.ReadLine();
                Console.WriteLine("Введите фамилию");
                var surname = Console.ReadLine();
                Console.WriteLine("Введите отчество");
                var patronymic = Console.ReadLine();
                Console.WriteLine("Введите адрес");
                var address = Console.ReadLine();
                Console.WriteLine("Введите ИИН");
                var iin = Console.ReadLine();

                var birthdate = inputDoB();
                var users = context.Countries.ToList();
                Console.WriteLine("Users list:");
                foreach (UCountry Country in users)
                {
                    Console.WriteLine($"{Country.ID}  {Country.Country}  {Country.City}");
                }

                var user = new Client()
                {
                    Name = name,
                    LastName = surname,
                    SecondName = patronymic,
                    Address = address,
                    IIN = iin,
                    DateOfBirth = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(birthdate, "New Zealand Standard Time", "UTC"),
                    CountryInfoKey = users[int.Parse(Console.ReadLine()) - 1].ID

                };
                context.Clients.Add(user);
                context.SaveChanges();
                Accounts(user.ID);
            }
        }
        DateTime inputDoB()
        {
            DateTime dob; // date of birth
            string input;

            do
            {
                Console.WriteLine("Введите дату рождения в формате дд.ММ.гггг (день.месяц.год):");
                input = Console.ReadLine();
            }
            while (!DateTime.TryParseExact(input, "dd.MM.yyyy", null, DateTimeStyles.None, out dob));

            return dob;
        }
        private void ViewUsers()
        {
            using (var context = new MyDbContext())
            {
                var users = context.Clients.ToList();
                var account = context.Accounts.ToList();
                var userAccount = from u in users
                                  join a in account on u.ID equals a.UserId
                                  select new
                                  {
                                      u.ID,
                                      u.Name,
                                      u.LastName,
                                      u.SecondName,
                                      u.Address,
                                      u.IIN,
                                      u.DateOfBirth,
                                      context.Countries.Find(u.CountryInfoKey).Country,
                                      context.Countries.Find(u.CountryInfoKey).City,
                                      a.AccountNumber,
                                      a.Balance
                                  };
                Console.WriteLine("Users list:");
                Console.WriteLine($"ID\tName\tLastName\tSecondName\tAddress\tIIN\tCountry\tCity\t\tAccountNumber\tBalance");
                foreach (var u in userAccount)
                {
                    Console.WriteLine($"{u.ID}\t{u.Name}\t{u.LastName}\t{u.SecondName}\t{u.Address}\t{u.IIN}\t{u.Country}  {u.City} \t{u.AccountNumber} \t\t{u.Balance}");
                }
            }
        }

        private void AddCountry()
        {
            using (var context = new MyDbContext())
            {
                Console.WriteLine("Введите название страны");
                var country = Console.ReadLine();
                Console.WriteLine("Введите название города");
                var city = Console.ReadLine();
                var Country = new UCountry()
                {
                    Country = country,
                    City = city
                };
                context.Countries.Add(Country);
                context.SaveChanges();
            }
        }
        private void readCountry()
        {
            using (var context = new MyDbContext())
            {
                var users = context.Countries.ToList();
                Console.WriteLine("Users list:");
                foreach (UCountry Country in users)
                {
                    Console.WriteLine($"{Country.ID}  {Country.Country}  {Country.City}");
                }
            }
        }
        private void statistic()
        {
            using (var context = new MyDbContext())
            {
                var users = context.Clients.ToList();
                var account = context.Accounts.ToList();
                var userAccount = from u in users
                                  join a in account on u.ID equals a.UserId
                                  select new
                                  {
                                      u.ID,
                                      u.Name,
                                      u.LastName,
                                      u.IIN,
                                      context.Countries.Find(u.CountryInfoKey).Country,
                                      context.Countries.Find(u.CountryInfoKey).City,
                                      a.Balance
                                  };
                var CountrySelect = from u in userAccount
                                    group u by u.Country into g
                                    select new
                                    {
                                        Country = g.Key,
                                        Balance = g.Sum(x => x.Balance)
                                    };
                Console.WriteLine("Users list:");

                foreach (var u in CountrySelect)
                {
                    Console.WriteLine($"{u.Country}\t\t{u.Balance}");
                }
            }
        }
    }
}*/