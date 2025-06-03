using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.DAl;
using warehousesystem.models;

namespace warehousesystem.bussines_logic
{
    public class SupplyOrderService
    {
        private readonly SupplyOrderRepository _supplyOrderRepository;
        private readonly WarehouseService _warehouseService; // لضمان وجود المخزن
        private readonly SupplierService _supplierService;   // لضمان وجود المورد
        private readonly ItemService _itemService;           // لضمان وجود الأصناف

        public SupplyOrderService()
        {
            _supplyOrderRepository = new SupplyOrderRepository();
            _warehouseService = new WarehouseService();
            _supplierService = new SupplierService();
            _itemService = new ItemService();
        }

        public List<SupplyOrder> GetAllSupplyOrders()
        {
            return _supplyOrderRepository.GetAllSupplyOrders();
        }

        public SupplyOrder GetSupplyOrderById(int id)
        {
            return _supplyOrderRepository.GetSupplyOrderById(id);
        }

        public bool AddSupplyOrder(SupplyOrder order, List<SupplyOrderDetail> details)
        {
            if (string.IsNullOrWhiteSpace(order.OrderNumber))
            {
                throw new ArgumentException("رقم الإذن مطلوب.");
            }
            if (order.OrderDate == default(DateTime))
            {
                throw new ArgumentException("تاريخ الإذن مطلوب.");
            }
            if (_warehouseService.GetWarehouseById(order.WarehouseID) == null)
            {
                throw new InvalidOperationException("المخزن المحدد غير موجود.");
            }
            if (_supplierService.GetSupplierById(order.SupplierID) == null)
            {
                throw new InvalidOperationException("المورد المحدد غير موجود.");
            }
            if (details == null || !details.Any())
            {
                throw new ArgumentException("يجب أن يحتوي إذن التوريد على تفاصيل أصناف واحدة على الأقل.");
            }

            foreach (var detail in details)
            {
                if (_itemService.GetItemById(detail.ItemID) == null)
                {
                    throw new InvalidOperationException($"الصنف بمعرف {detail.ItemID} غير موجود.");
                }
                if (detail.Quantity <= 0)
                {
                    throw new ArgumentException($"كمية الصنف '{detail.Item?.Name}' يجب أن تكون أكبر من صفر.");
                }
            }

            _supplyOrderRepository.AddSupplyOrder(order, details);
            return true;
        }

        public void DeleteSupplyOrder(int id)
        {
            _supplyOrderRepository.DeleteSupplyOrder(id);
        }
    }
}
