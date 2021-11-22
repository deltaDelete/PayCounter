using Microsoft.EntityFrameworkCore;
using System;

namespace PayCounter.DB
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public string DbPath { get; private set; }
        public ApplicationContext()
        {
            var folder = Environment.CurrentDirectory;
            DbPath = $"{folder}{System.IO.Path.DirectorySeparatorChar}paycounter.db";
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }
}
