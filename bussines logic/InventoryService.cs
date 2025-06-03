using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.DAl;
using warehousesystem.data;
using warehousesystem.models;

namespace warehousesystem.bussines_logic
{
    public class InventoryService
    {
        private readonly InventoryRepository _inventoryRepository;
        private readonly InventoryMovementRepository _movementRepository;
        private readonly WarehouseService _warehouseService;
        private readonly ItemService _itemService;
        private readonly SupplierService _supplierService;

        public InventoryService()
        {
            _inventoryRepository = new InventoryRepository();
            _movementRepository = new InventoryMovementRepository();
            _warehouseService = new WarehouseService();
            _itemService = new ItemService();
            _supplierService = new SupplierService();
        }

        // 7) تحويل مجموعة اصناف من مخزن لأخر
        public bool TransferItems(int sourceWarehouseId, int destinationWarehouseId, int itemId, decimal quantity, DateTime? productionDate, DateTime? expiryDate, int supplierId)
        {
            if (sourceWarehouseId == destinationWarehouseId)
            {
                throw new ArgumentException("لا يمكن تحويل الأصناف إلى نفس المخزن.");
            }
            if (quantity <= 0)
            {
                throw new ArgumentException("الكمية المحولة يجب أن تكون أكبر من صفر.");
            }
            if (_warehouseService.GetWarehouseById(sourceWarehouseId) == null)
            {
                throw new InvalidOperationException("المخزن المصدر غير موجود.");
            }
            if (_warehouseService.GetWarehouseById(destinationWarehouseId) == null)
            {
                throw new InvalidOperationException("المخزن الوجهة غير موجود.");
            }
            if (_itemService.GetItemById(itemId) == null)
            {
                throw new InvalidOperationException("الصنف المحدد غير موجود.");
            }
            if (_supplierService.GetSupplierById(supplierId) == null)
            {
                throw new InvalidOperationException("المورد المحدد غير موجود.");
            }

            // استخدام معاملة لضمان تكامل عملية التحويل
            using (var context = new AppDbcontext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. إنقاص الكمية من المخزن المصدر
                        var sourceInventory = context.Inventory.FirstOrDefault(
                            i => i.WarehouseID == sourceWarehouseId &&
                                 i.ItemID == itemId &&
                                 i.ProductionDate == productionDate &&
                                 i.ExpiryDate == expiryDate &&
                                 i.SupplierID == supplierId);

                        if (sourceInventory == null || sourceInventory.Quantity < quantity)
                        {
                            throw new InvalidOperationException("الكمية المطلوبة للتحويل غير متوفرة في المخزن المصدر.");
                        }

                        sourceInventory.Quantity -= quantity;
                        if (sourceInventory.Quantity == 0)
                        {
                            context.Inventory.Remove(sourceInventory);
                        }
                        else
                        {
                            context.Inventory.Update(sourceInventory);
                        }

                        // 2. زيادة الكمية في المخزن الوجهة
                        var destinationInventory = context.Inventory.FirstOrDefault(
                            i => i.WarehouseID == destinationWarehouseId &&
                                 i.ItemID == itemId &&
                                 i.ProductionDate == productionDate &&
                                 i.ExpiryDate == expiryDate &&
                                 i.SupplierID == supplierId);

                        if (destinationInventory == null)
                        {
                            context.Inventory.Add(new Inventory
                            {
                                WarehouseID = destinationWarehouseId,
                                ItemID = itemId,
                                Quantity = quantity,
                                ProductionDate = productionDate,
                                ExpiryDate = expiryDate,
                                SupplierID = supplierId
                            });
                        }
                        else
                        {
                            destinationInventory.Quantity += quantity;
                            context.Inventory.Update(destinationInventory);
                        }

                        // 3. تسجيل حركة التحويل في جدول InventoryMovement
                        context.InventoryMovements.Add(new InventoryMovement
                        {
                            MovementDate = DateTime.Now,
                            ItemID = itemId,
                            Quantity = quantity,
                            MovementType = "Transfer",
                            SourceWarehouseID = sourceWarehouseId,
                            DestinationWarehouseID = destinationWarehouseId,
                            ProductionDate = productionDate,
                            ExpiryDate = expiryDate,
                            SupplierID = supplierId
                        });

                        context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("حدث خطأ أثناء تحويل الأصناف: " + ex.Message, ex);
                    }
                }
            }
        }
    }

}
