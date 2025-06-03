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
    public class DisbursementOrderRepository
    {
        private readonly AppDbcontext _context;

        public DisbursementOrderRepository()
        {
            _context = new AppDbcontext();
        }

        public List<DisbursementOrder> GetAllDisbursementOrders()
        {
            return _context.DisbursementOrders
                           .Include(d => d.Warehouse)
                           .Include(d => d.Supplier)
                           .Include(d => d.DisbursementOrderDetails)
                               .ThenInclude(dd => dd.Item)
                           .ToList();
        }

        public DisbursementOrder GetDisbursementOrderById(int id)
        {
            return _context.DisbursementOrders
                           .Include(d => d.Warehouse)
                           .Include(d => d.Supplier)
                           .Include(d => d.DisbursementOrderDetails)
                               .ThenInclude(dd => dd.Item)
                           .FirstOrDefault(d => d.DisbursementOrderID == id);
        }

        public bool AddDisbursementOrder(DisbursementOrder order, List<DisbursementOrderDetail> details)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.DisbursementOrders.Add(order);
                    _context.SaveChanges();

                    foreach (var detail in details)
                    {
                        detail.DisbursementOrderID = order.DisbursementOrderID;
                        _context.DisbursementOrderDetails.Add(detail);
                        var inventoryItem = _context.Inventory.FirstOrDefault(
                            i => i.WarehouseID == order.WarehouseID &&
                                 i.ItemID == detail.ItemID &&
                                 i.SupplierID == order.SupplierID); 

                        if (inventoryItem == null || inventoryItem.Quantity < detail.Quantity)
                        {
                            transaction.Rollback();
                            throw new InvalidOperationException($"كمية الصنف '{detail.Item.Name}' غير كافية في المخزن '{order.Warehouse.Name}'.");
                        }

                        inventoryItem.Quantity -= detail.Quantity;
                        if (inventoryItem.Quantity == 0)
                        {
                            _context.Inventory.Remove(inventoryItem);
                        }
                        else
                        {
                            _context.Inventory.Update(inventoryItem);
                        }
                    }
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("حدث خطأ أثناء إضافة إذن الصرف وتحديث المخزون.", ex);
                }
            }
        }
    }

}
