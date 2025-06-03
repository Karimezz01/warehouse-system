using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using warehousesystem.models;

namespace warehousesystem.data
{
    
     public class AppDbcontext : DbContext
    {
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SupplyOrder> SupplyOrders { get; set; }
        public DbSet<SupplyOrderDetail> SupplyOrderDetails { get; set; }
        public DbSet<DisbursementOrder> DisbursementOrders { get; set; }
        public DbSet<DisbursementOrderDetail> DisbursementOrderDetails { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<transferitem> TransferItems { get; set; }

       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer("Data Source=DESKTOP-MLPI35O\\SQLEXPRESS01;Initial Catalog=WarehouseDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SupplyOrder>()
                .HasOne(so => so.Warehouse) // إذن التوريد له مخزن واحد
                .WithMany(w => w.SupplyOrders) // المخزن يمكن أن يكون له أذون توريد متعددة
                .HasForeignKey(so => so.WarehouseID); // المفتاح الخارجي

            // علاقة SupplyOrder مع Supplier
            modelBuilder.Entity<SupplyOrder>()
                .HasOne(so => so.Supplier)
                .WithMany(s => s.SupplyOrders)
                .HasForeignKey(so => so.SupplierID);

            // علاقة SupplyOrderDetail مع SupplyOrder
            modelBuilder.Entity<SupplyOrderDetail>()
                .HasOne(sod => sod.SupplyOrder)
                .WithMany(so => so.SupplyOrderDetails)
                .HasForeignKey(sod => sod.SupplyOrderID);

            // علاقة SupplyOrderDetail مع Item
            modelBuilder.Entity<SupplyOrderDetail>()
                .HasOne(sod => sod.Item)
                .WithMany(i => i.SupplyOrderDetails)
                .HasForeignKey(sod => sod.ItemID);

            // علاقة DisbursementOrder مع Warehouse
            modelBuilder.Entity<DisbursementOrder>()
                .HasOne(d => d.Warehouse)
                .WithMany(w => w.DisbursementOrders)
                .HasForeignKey(d => d.WarehouseID);

            // علاقة DisbursementOrder مع Supplier (حسب الوصف)
            modelBuilder.Entity<DisbursementOrder>()
                .HasOne(d => d.Supplier)
                .WithMany(s => s.DisbursementOrders)
                .HasForeignKey(d => d.SupplierID);

            // علاقة DisbursementOrderDetail مع DisbursementOrder
            modelBuilder.Entity<DisbursementOrderDetail>()
                .HasOne(dod => dod.DisbursementOrder)
                .WithMany(d => d.DisbursementOrderDetails)
                .HasForeignKey(dod => dod.DisbursementOrderID);

            // علاقة DisbursementOrderDetail مع Item
            modelBuilder.Entity<DisbursementOrderDetail>()
                .HasOne(dod => dod.Item)
                .WithMany(i => i.DisbursementOrderDetails)
                .HasForeignKey(dod => dod.ItemID);

            // علاقة Inventory مع Warehouse
            modelBuilder.Entity<Inventory>()
                .HasOne(inv => inv.Warehouse)
                .WithMany(w => w.InventoryItems)
                .HasForeignKey(inv => inv.WarehouseID);

            // علاقة Inventory مع Item
            modelBuilder.Entity<Inventory>()
                .HasOne(inv => inv.Item)
                .WithMany(i => i.InventoryItems)
                .HasForeignKey(inv => inv.ItemID);

            // علاقة Inventory مع Supplier
            modelBuilder.Entity<Inventory>()
                .HasOne(inv => inv.Supplier)
                .WithMany(s => s.InventoryItems)
                .HasForeignKey(inv => inv.SupplierID);

           
            modelBuilder.Entity<Inventory>()
                .HasIndex(i => new { i.WarehouseID, i.ItemID, i.ProductionDate, i.ExpiryDate, i.SupplierID })
                .IsUnique();

   
            modelBuilder.Entity<InventoryMovement>()
                .HasOne(im => im.Item)
                .WithMany(i => i.InventoryMovements)
                .HasForeignKey(im => im.ItemID);

            modelBuilder.Entity<InventoryMovement>()
                .HasOne(im => im.SourceWarehouse)
                .WithMany(w => w.SourceMovements)
                .HasForeignKey(im => im.SourceWarehouseID)
                .IsRequired(false); 

            modelBuilder.Entity<InventoryMovement>()
                .HasOne(im => im.DestinationWarehouse)
                .WithMany(w => w.DestinationMovements)
                .HasForeignKey(im => im.DestinationWarehouseID)
                .IsRequired(false); 

            modelBuilder.Entity<InventoryMovement>()
                .HasOne(im => im.Supplier)
                .WithMany(s => s.InventoryMovements)
                .HasForeignKey(im => im.SupplierID)
                .IsRequired(false);
        }
    }
}

