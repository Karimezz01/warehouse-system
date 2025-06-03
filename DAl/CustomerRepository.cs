using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.models;

namespace warehousesystem.DAl
{
    public class CustomerRepository : BaseRepository<Customer>
    {
        public Customer GetCustomerByName(string name)
        {
            return _context.Customers.FirstOrDefault(c => c.Name == name);
        }
    }

}
