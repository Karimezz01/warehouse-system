using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.models;

namespace warehousesystem.DAl
{
  public  class WarehouseRepository : BaseRepository<Warehouse>
    {
        public Warehouse GetWarehouseByName(string name)
        {
            return _context.Warehouses.FirstOrDefault(w => w.Name == name);
        }
    
}
}
