using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Text;
using MainApplication.Models;

namespace MainApplication
{
    class MESDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } 
        public DbSet<Workstation> Workstations { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<ProductionOrder> ProductionOrders { get; set; }
        public DbSet<OrderMaterial> OrderMaterials { get; set; }
        public DbSet<ProductionLog> ProductionLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = "Server=tcp:mes-server-nootnoot.database.windows.net,1433;Initial Catalog=MES_Database;Persist Security Info=False;User ID=mesadmin;Password=ProiectMESAN3;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                optionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5, // Încearcă de maxim 5 ori
                        maxRetryDelay: TimeSpan.FromSeconds(30), // Așteaptă până la 30 de secunde între încercări
                        errorNumbersToAdd: null);
                });
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderMaterial>()
                .HasKey(om => new { om.OrderId, om.ItemId });
            
        }
    }
}
