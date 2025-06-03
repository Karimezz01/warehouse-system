using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using warehousesystem.data;
using warehousesystem.models;

namespace warehousesystem.DAl
{
    public class SupplyOrderRepository
    {
        private readonly AppDbcontext _context;

        public SupplyOrderRepository()
        {
            _context = new AppDbcontext();
        }

        public List<SupplyOrder> GetAllSupplyOrders()
        {
            return _context.SupplyOrders
                           .Include(so => so.Warehouse)
                           .Include(so => so.Supplier)
                           .Include(so => so.SupplyOrderDetails)
                               .ThenInclude(sod => sod.Item)
                           .ToList();
        }

        public SupplyOrder GetSupplyOrderById(int id)
        {
            return _context.SupplyOrders
                           .Include(so => so.Warehouse)
                           .Include(so => so.Supplier)
                           .Include(so => so.SupplyOrderDetails)
                               .ThenInclude(sod => sod.Item)
                           .FirstOrDefault(so => so.SupplyOrderID == id);
        }

        public void AddSupplyOrder(SupplyOrder order, List<SupplyOrderDetail> details)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SupplyOrders.Add(order);
                    _context.SaveChanges(); 

                    foreach (var detail in details)
                    {
                        detail.SupplyOrderID = order.SupplyOrderID; 
                        _context.SupplyOrderDetails.Add(detail);

                        var inventoryItem = _context.Inventory.FirstOrDefault(
                            i => i.WarehouseID == order.WarehouseID &&
                                 i.ItemID == detail.ItemID &&
                                 i.ProductionDate == detail.ProductionDate &&
                                 i.ExpiryDate == detail.ExpiryDate &&
                                 i.SupplierID == order.SupplierID); // ربط بالمورد الأصلي للدفعة

                        if (inventoryItem == null)
                        {
                            _context.Inventory.Add(new Inventory
                            {
                                WarehouseID = order.WarehouseID,
                                ItemID = detail.ItemID,
                                Quantity = detail.Quantity,
                                ProductionDate = detail.ProductionDate,
                                ExpiryDate = detail.ExpiryDate,
                                SupplierID = order.SupplierID
                            });
                        }
                        else
                        {
                            // إذا تم العثور على دفعة مطابقة، قم بتحديث الكمية
                            inventoryItem.Quantity += detail.Quantity;
                            _context.Inventory.Update(inventoryItem);
                        }
                    }
                    _context.SaveChanges();
                    transaction.Commit(); // تأكيد المعاملة
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // التراجع عن المعاملة في حالة الخطأ
                    throw new Exception("حدث خطأ أثناء إضافة إذن التوريد وتحديث المخزون.", ex);
                }
            }
        }

        
        public void UpdateSupplyOrder(SupplyOrder order, List<SupplyOrderDetail> newDetails)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    
                    _context.SupplyOrders.Update(order);

                    var existingDetails = _context.SupplyOrderDetails.Where(d => d.SupplyOrderID == order.SupplyOrderID).ToList();
                    _context.SupplyOrderDetails.RemoveRange(existingDetails);
                    _context.SaveChanges();

                    foreach (var detail in newDetails)
                    {
                        detail.SupplyOrderID = order.SupplyOrderID;
                        _context.SupplyOrderDetails.Add(detail);

                        
                    }
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("حدث خطأ أثناء تعديل إذن التوريد.", ex);
                }
            }
        }

        public void DeleteSupplyOrder(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = _context.SupplyOrders.Include(so => so.SupplyOrderDetails)
                                                     .FirstOrDefault(so => so.SupplyOrderID == id);
                    if (order == null) return;

                    foreach (var detail in order.SupplyOrderDetails)
                    {
                        var inventoryItem = _context.Inventory.FirstOrDefault(
                            i => i.WarehouseID == order.WarehouseID &&
                                 i.ItemID == detail.ItemID &&
                                 i.ProductionDate == detail.ProductionDate &&
                                 i.ExpiryDate == detail.ExpiryDate &&
                                 i.SupplierID == order.SupplierID); 

                        if (inventoryItem != null)
                        {
                            inventoryItem.Quantity -= detail.Quantity;
                            if (inventoryItem.Quantity < 0)
                            {
                                throw new InvalidOperationException("لا يمكن حذف الإذن: الكمية في المخزون ستصبح سالبة.");
                            }
                            else if (inventoryItem.Quantity == 0)
                            {
                                _context.Inventory.Remove(inventoryItem); 
                            }
                            else
                            {
                                _context.Inventory.Update(inventoryItem);
                            }
                        }
                    }

                    _context.SupplyOrderDetails.RemoveRange(order.SupplyOrderDetails);
                    _context.SupplyOrders.Remove(order);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("حدث خطأ أثناء حذف إذن التوريد.", ex);
                }
            }
        }
    }

}
