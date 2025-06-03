using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.DAl;
using warehousesystem.models;

namespace warehousesystem.bussines_logic
{
    public class DisbursementOrderService
    {
        private readonly DisbursementOrderRepository _disbursementOrderRepository;
        private readonly WarehouseService _warehouseService;
        private readonly SupplierService _supplierService;
        private readonly ItemService _itemService;

        public DisbursementOrderService()
        {
            _disbursementOrderRepository = new DisbursementOrderRepository();
            _warehouseService = new WarehouseService();
            _supplierService = new SupplierService();
            _itemService = new ItemService();
        }

        public List<DisbursementOrder> GetAllDisbursementOrders()
        {
            return _disbursementOrderRepository.GetAllDisbursementOrders();
        }

        public DisbursementOrder GetDisbursementOrderById(int id)
        {
            return _disbursementOrderRepository.GetDisbursementOrderById(id);
        }

        public bool AddDisbursementOrder(DisbursementOrder order, List<DisbursementOrderDetail> details)
        {
            if (string.IsNullOrWhiteSpace(order.OrderNumber))
            {
                throw new ArgumentException("رقم إذن الصرف مطلوب.");
            }
            if (order.OrderDate == default(DateTime))
            {
                throw new ArgumentException("تاريخ إذن الصرف مطلوب.");
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
                throw new ArgumentException("يجب أن يحتوي إذن الصرف على تفاصيل أصناف واحدة على الأقل.");
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

            return _disbursementOrderRepository.AddDisbursementOrder(order, details);
        }
    }
}
