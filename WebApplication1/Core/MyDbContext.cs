using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Core
{

    public class MyDbContext : DbContext
    {
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=my;Username=postgres;Password=root");
        //}



        public DbSet<UCountry> Countries { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<UserAuthBank> UserAuthBanks { get; set; }
        public DbSet<BankOperationHistiry> BankOperation { get; set; }
    }
}
