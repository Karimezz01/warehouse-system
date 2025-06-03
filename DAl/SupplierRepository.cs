using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.models;

namespace warehousesystem.DAl
{
    public class SupplierRepository : BaseRepository<Supplier>
    {
        public Supplier GetSupplierByName(string name)
        {
            return _context.Suppliers.FirstOrDefault(s => s.Name == name);
        }
    }
}
