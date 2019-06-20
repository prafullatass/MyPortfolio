using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Models;

namespace MyPortfolio.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options){    }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Sector>  Sectors { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Stock>  Stocks { get; set; }
        public DbSet<Transaction>  Transactions { get; set; }
        public DbSet<UserAgency>  UserAgencies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ApplicationUser user = new ApplicationUser
            {
                FirstName = "Admina",
                LastName = "Straytor",
                StreetAddress = "123 Infinity Way",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            base.OnModelCreating(modelBuilder);
            var passwordHash = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHash.HashPassword(user, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(user);

            modelBuilder.Entity<Country>().HasData(
              new Country()
              {
                  CountryId = 1,
                  Name = "USA",
                  Currency = "$"
              });


            modelBuilder.Entity<Agency>().HasData(
                new Agency()
                {
                    AgencyId = 1,
                    Name = "RobinHood",
                    CountryId = 1
                });

            modelBuilder.Entity<UserAgency>().HasData(
                 new UserAgency()
                 {
                     UserAgencyId = 1,
                     UserId = user.Id,
                     AgencyId = 1,
                     AccountNo = 123123
                 });
            modelBuilder.Entity<Sector>().HasData(
                 new Sector()
                 {
                     SectorId = 1,
                     Name = "Finantial"
                 });
            modelBuilder.Entity<Stock>().HasData(
               new Stock()
               {
                   StockId = 1,
                   Name = "Bank of America co-op",
                   Ticker = "BAC",
                   SectorId = 1,
                   CountryId = 1
               });
            modelBuilder.Entity<Transaction>().HasData(
                new Transaction()
                {
                    TransactionId = 1,
                    StockId = 1,
                    UserAgencyId = 1,
                    Date = DateTime.Now,
                    Rate = 25.80,
                    Qty = 10,
                    
                    BuyOrSell = true
                });
        }
    }
}
