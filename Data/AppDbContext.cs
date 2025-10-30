// File: Data/AppDbContext.cs
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SalesTrackingSystem.Models;

namespace SalesTrackingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=SalesTrackingConnection") { }

        public DbSet<SaleRecord> Sales { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Expense> Expenses { get; set; }   

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Salary–Employee 1-many relationship
            modelBuilder.Entity<Salary>()
                .HasRequired(s => s.Employee)
                .WithMany()
                .HasForeignKey(s => s.EmployeeId)
                .WillCascadeOnDelete(false);
        }
    }
}
