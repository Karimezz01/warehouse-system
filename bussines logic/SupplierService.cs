using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.DAl;
using warehousesystem.models;

namespace warehousesystem.bussines_logic
{
    public class SupplierService
    {
        private readonly SupplierRepository _supplierRepository;

        public SupplierService()
        {
            _supplierRepository = new SupplierRepository();
        }

        public List<Supplier> GetAllSuppliers()
        {
            return _supplierRepository.GetAll();
        }

        public Supplier GetSupplierById(int id)
        {
            return _supplierRepository.GetById(id);
        }

        public bool AddSupplier(Supplier supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.Name))
            {
                throw new ArgumentException("اسم المورد مطلوب.");
            }
            if (_supplierRepository.GetSupplierByName(supplier.Name) != null)
            {
                throw new InvalidOperationException($"المورد '{supplier.Name}' موجود بالفعل.");
            }

            _supplierRepository.Add(supplier);
            return true;
        }

        public bool UpdateSupplier(Supplier supplier)
        {
            if (supplier.SupplierID <= 0)
            {
                throw new ArgumentException("معرف المورد غير صالح للتعديل.");
            }
            if (string.IsNullOrWhiteSpace(supplier.Name))
            {
                throw new ArgumentException("اسم المورد مطلوب.");
            }

            var existingSupplier = _supplierRepository.GetSupplierByName(supplier.Name);
            if (existingSupplier != null && existingSupplier.SupplierID != supplier.SupplierID)
            {
                throw new InvalidOperationException($"المورد '{supplier.Name}' موجود بالفعل لمورد آخر.");
            }

            _supplierRepository.Update(supplier);
            return true;
        }

        public void DeleteSupplier(int id)
        {
            _supplierRepository.Delete(id);
        }
    }

}
