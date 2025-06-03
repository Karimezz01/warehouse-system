using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using warehousesystem.models;

namespace warehousesystem.DAl
{
    public class InventoryRepository : BaseRepository<Inventory>
    {
        // الحصول على المخزون لصنف معين في مخزن معين
        public List<Inventory> GetInventoryByWarehouseAndItem(int warehouseId, int itemId)
        {
            return _context.Inventory
                           .Include(i => i.Item)
                           .Include(i => i.Warehouse)
                           .Include(i => i.Supplier)
                           .Where(i => i.WarehouseID == warehouseId && i.ItemID == itemId)
                           .ToList();
        }

        // الحصول على جميع الأصناف في مخزن معين
        public List<Inventory> GetInventoryByWarehouse(int warehouseId)
        {
            return _context.Inventory
                           .Include(i => i.Item)
                           .Include(i => i.Warehouse)
                           .Include(i => i.Supplier)
                           .Where(i => i.WarehouseID == warehouseId)
                           .ToList();
        }
    }

}