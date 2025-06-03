using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.DAl;
using warehousesystem.models;

namespace warehousesystem.bussines_logic
{
    public class WarehouseService
    {
        private readonly WarehouseRepository _warehouseRepository;

        public WarehouseService()
        {
            _warehouseRepository = new WarehouseRepository();
        }

        public List<Warehouse> GetAllWarehouses()
        {
            return _warehouseRepository.GetAll();
        }

        public Warehouse GetWarehouseById(int id)
        {
            return _warehouseRepository.GetById(id);
        }

        public bool AddWarehouse(Warehouse warehouse)
        {
            // قواعد التحقق من صحة البيانات
            if (string.IsNullOrWhiteSpace(warehouse.Name))
            {
                throw new ArgumentException("اسم المخزن لا يمكن أن يكون فارغًا.");
            }
            if (_warehouseRepository.GetWarehouseByName(warehouse.Name) != null)
            {
                throw new InvalidOperationException($"المخزن '{warehouse.Name}' موجود بالفعل.");
            }

            _warehouseRepository.Add(warehouse);
            return true;
        }

        public bool UpdateWarehouse(Warehouse warehouse)
        {
            if (warehouse.WarehouseID <= 0)
            {
                throw new ArgumentException("معرف المخزن غير صالح للتعديل.");
            }
            if (string.IsNullOrWhiteSpace(warehouse.Name))
            {
                throw new ArgumentException("اسم المخزن لا يمكن أن يكون فارغًا.");
            }

            var existingWarehouse = _warehouseRepository.GetWarehouseByName(warehouse.Name);
            if (existingWarehouse != null && existingWarehouse.WarehouseID != warehouse.WarehouseID)
            {
                throw new InvalidOperationException($"المخزن '{warehouse.Name}' موجود بالفعل لمخزن آخر.");
            }

            _warehouseRepository.Update(warehouse);
            return true;
        }

        public void DeleteWarehouse(int id)
        {
        

            _warehouseRepository.Delete(id);
        }
    }
}