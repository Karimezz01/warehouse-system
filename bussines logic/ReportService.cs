using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using warehousesystem.data;

namespace warehousesystem.bussines_logic
{
    public class ReportService
    {
        private readonly AppDbcontext _context;

        public ReportService()
        {
            _context = new AppDbcontext();
        }

        public List<dynamic> GetWarehouseReport(int warehouseId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Inventory
                                .Include(i => i.Item)
                                .Include(i => i.Supplier)
                                .Where(i => i.WarehouseID == warehouseId)
                                .AsQueryable();

            

            return query.Select(i => new
            {
                ItemName = i.Item.Name,
                ItemCode = i.Item.Code,
                Quantity = i.Quantity,
                UnitOfMeasure = i.Item.UnitOfMeasure,
                ProductionDate = i.ProductionDate,
                ExpiryDate = i.ExpiryDate,
                SupplierName = i.Supplier.Name
            }).ToList<dynamic>();
        }

        public List<dynamic> GetItemReport(int itemId, List<int> warehouseIds, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Inventory
                                .Include(i => i.Item)
                                .Include(i => i.Warehouse)
                                .Include(i => i.Supplier)
                                .Where(i => i.ItemID == itemId)
                                .AsQueryable();

            if (warehouseIds != null && warehouseIds.Any())
            {
                query = query.Where(i => warehouseIds.Contains(i.WarehouseID));
            }


            return query.Select(i => new
            {
                WarehouseName = i.Warehouse.Name,
                ItemName = i.Item.Name,
                ItemCode = i.Item.Code,
                Quantity = i.Quantity,
                UnitOfMeasure = i.Item.UnitOfMeasure,
                ProductionDate = i.ProductionDate,
                ExpiryDate = i.ExpiryDate,
                SupplierName = i.Supplier.Name
            }).ToList<dynamic>();
        }

        public List<dynamic> GetItemMovementReport(List<int> warehouseIds, DateTime startDate, DateTime endDate)
        {
            var supplyMovements = _context.SupplyOrderDetails
                .Include(sod => sod.SupplyOrder)
                .Include(sod => sod.Item)
                .Include(sod => sod.SupplyOrder.Warehouse)
                .Include(sod => sod.SupplyOrder.Supplier)
                .Where(sod => sod.SupplyOrder.OrderDate >= startDate && sod.SupplyOrder.OrderDate <= endDate &&
                              (warehouseIds == null || !warehouseIds.Any() || warehouseIds.Contains(sod.SupplyOrder.WarehouseID)))
                .Select(sod => new
                {
                    Date = sod.SupplyOrder.OrderDate,
                    MovementType = "توريد",
                    OrderNumber = sod.SupplyOrder.OrderNumber,
                    ItemName = sod.Item.Name,
                    Quantity = sod.Quantity,
                    WarehouseName = sod.SupplyOrder.Warehouse.Name,
                    SourceDestination = "", 
                    SupplierName = sod.SupplyOrder.Supplier.Name,
                    ProductionDate = sod.ProductionDate,
                    ExpiryDate = sod.ExpiryDate
                });

            var disbursementMovements = _context.DisbursementOrderDetails
                .Include(dod => dod.DisbursementOrder)
                .Include(dod => dod.Item)
                .Include(dod => dod.DisbursementOrder.Warehouse)
                .Include(dod => dod.DisbursementOrder.Supplier)
                .Where(dod => dod.DisbursementOrder.OrderDate >= startDate && dod.DisbursementOrder.OrderDate <= endDate &&
                              (warehouseIds == null || !warehouseIds.Any() || warehouseIds.Contains(dod.DisbursementOrder.WarehouseID)))
                .Select(dod => new
                {
                    Date = dod.DisbursementOrder.OrderDate,
                    MovementType = "صرف",
                    OrderNumber = dod.DisbursementOrder.OrderNumber,
                    ItemName = dod.Item.Name,
                    Quantity = dod.Quantity,
                    WarehouseName = dod.DisbursementOrder.Warehouse.Name,
                    SourceDestination = "",
                    SupplierName = dod.DisbursementOrder.Supplier.Name,
                    ProductionDate = (DateTime?)null, 
                    ExpiryDate = (DateTime?)null
                });

            var transferMovements = _context.InventoryMovements
                .Include(im => im.Item)
                .Include(im => im.SourceWarehouse)
                .Include(im => im.DestinationWarehouse)
                .Include(im => im.Supplier)
                .Where(im => im.MovementDate >= startDate && im.MovementDate <= endDate &&
                             (warehouseIds == null || !warehouseIds.Any() ||
                              (im.SourceWarehouseID.HasValue && warehouseIds.Contains(im.SourceWarehouseID.Value)) ||
                              (im.DestinationWarehouseID.HasValue && warehouseIds.Contains(im.DestinationWarehouseID.Value))))
                .Select(im => new
                {
                    Date = im.MovementDate,
                    MovementType = "تحويل",
                    OrderNumber = "", // لا يوجد رقم إذن للتحويلات
                    ItemName = im.Item.Name,
                    Quantity = im.Quantity,
                    WarehouseName = (im.SourceWarehouse != null ? im.SourceWarehouse.Name : "N/A") + " -> " + (im.DestinationWarehouse != null ? im.DestinationWarehouse.Name : "N/A"),
                    SourceDestination = (im.SourceWarehouse != null ? im.SourceWarehouse.Name : "N/A") + " -> " + (im.DestinationWarehouse != null ? im.DestinationWarehouse.Name : "N/A"),
                    SupplierName = im.Supplier != null ? im.Supplier.Name : "N/A",
                    ProductionDate = im.ProductionDate,
                    ExpiryDate = im.ExpiryDate
                });

            var allMovements = supplyMovements.ToList<dynamic>()
                .Concat(disbursementMovements.ToList<dynamic>())
                .Concat(transferMovements.ToList<dynamic>())
                .OrderBy(m => m.Date)
                .ToList();

            return allMovements;
        }

        public List<dynamic> GetOldItemsInWarehouse(int warehouseId, int daysInWarehouse)
        {
            DateTime thresholdDate = DateTime.Today.AddDays(-daysInWarehouse);

            var query = _context.Inventory
                                .Include(i => i.Item)
                                .Include(i => i.Warehouse)
                                .Include(i => i.Supplier)
                                .Where(i => i.WarehouseID == warehouseId &&
                                            i.ProductionDate.HasValue &&
                                            i.ProductionDate.Value <= thresholdDate)
                                .AsQueryable();

            return query.Select(i => new
            {
                ItemName = i.Item.Name,
                ItemCode = i.Item.Code,
                WarehouseName = i.Warehouse.Name,
                Quantity = i.Quantity,
                ProductionDate = i.ProductionDate,
                ExpiryDate = i.ExpiryDate,
                SupplierName = i.Supplier.Name,
                DaysInWarehouse = EF.Functions.DateDiffDay(i.ProductionDate.Value, DateTime.Today) // حساب الأيام
            }).ToList<dynamic>();
        }

        public List<dynamic> GetExpiringItems(int daysRemaining, int? warehouseId = null)
        {
            DateTime expiryThreshold = DateTime.Today.AddDays(daysRemaining);

            var query = _context.Inventory
                                .Include(i => i.Item)
                                .Include(i => i.Warehouse)
                                .Include(i => i.Supplier)
                                .Where(inv => inv.ExpiryDate.HasValue && inv.ExpiryDate.Value <= expiryThreshold)
                                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(inv => inv.WarehouseID == warehouseId.Value);
            }

            return query.Select(i => new
            {
                ItemName = i.Item.Name,
                ItemCode = i.Item.Code,
                WarehouseName = i.Warehouse.Name,
                Quantity = i.Quantity,
                ProductionDate = i.ProductionDate,
                ExpiryDate = i.ExpiryDate,
                SupplierName = i.Supplier.Name,
                DaysUntilExpiry = EF.Functions.DateDiffDay(DateTime.Today, i.ExpiryDate.Value) // حساب الأيام المتبقية
            }).ToList<dynamic>();
        }
    }

}
