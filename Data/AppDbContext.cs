// File: Data/AppDbContext.cs
using System.Data.Entity;
using SalesTrackingSystem.Models;

namespace SalesTrackingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=SalesTrackingConnection")
        {
        }

        public DbSet<SaleRecord> Sales { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Employee> Employee { get; set; }

        // public DbSet<Expense> Expenses { get; set; }
    }
}