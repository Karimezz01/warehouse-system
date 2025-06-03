using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.models;

namespace warehousesystem.DAl
{
    public class ItemRepository : BaseRepository<Item>
    {
        public Item GetItemByCode(string code)
        {
            return _context.Items.FirstOrDefault(i => i.Code == code);
        }
    }

}
