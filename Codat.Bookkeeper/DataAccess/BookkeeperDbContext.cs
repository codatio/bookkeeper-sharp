using Codat.Bookkeeper.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Codat.Bookkeeper.DataAccess
{
    public class BookkeeperDbContext : DbContext
    {
        public BookkeeperDbContext(DbContextOptions<BookkeeperDbContext> options) : base(options) { }

        public DbSet<Invoice> Invoices { get; private set; }
        public DbSet<Payment> Payments { get; private set; }
        public DbSet<User> Users { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding
            var seedData = GetSeedData();

            modelBuilder.Entity<Invoice>().HasData(seedData.Invoices);
            modelBuilder.Entity<Payment>().HasData(seedData.Payments);

            modelBuilder.Entity<User>()
                .HasData(new User()
                {
                    UserId = 1,
                    Name = "Bob",
                    HashedPassword = "37e4392dad1ad3d86680a8c6b06ede92", // P@55w0rd
                    TenantId = 1
                },
                new User()
                {
                    UserId = 2,
                    Name = "Alice",
                    HashedPassword = "37e4392dad1ad3d86680a8c6b06ede92", // P@55w0rd
                    TenantId = 2
                });
        }

        private static SeedData GetSeedData()
        {
            var seedDataPath = Path.Combine(Environment.CurrentDirectory, "invoicesSeed.json");
            var invoices = JsonConvert.DeserializeObject<List<Invoice>>(File.ReadAllText(seedDataPath));
            var seedData = new SeedData();

            foreach (var invoice in invoices)
            {
                if (invoice.Payments.Any())
                {
                    foreach (var payment in invoice.Payments)
                    {
                        payment.InvoiceId = invoice.InvoiceId;
                        seedData.Payments.Add(payment);
                    }
                }

                invoice.Payments = null;
                seedData.Invoices.Add(invoice);
            }

            return seedData;
        }

        private sealed class SeedData
        {
            public List<Invoice> Invoices { get; } = new List<Invoice>();

            public List<Payment> Payments { get; } = new List<Payment>();
        }
    }
}