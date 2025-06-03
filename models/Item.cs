using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehousesystem.models
{
  public  class Item
    {
        public int ItemID { get; set; }

        [Required(ErrorMessage = "كود الصنف مطلوب.")]
        [MaxLength(50)]
        [StringLength(50)]
        public string Code { get; set; }

        [Required(ErrorMessage = "اسم الصنف مطلوب.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string UnitOfMeasure { get; set; }

        public  virtual ICollection<SupplyOrderDetail> SupplyOrderDetails { get; set; }
        public virtual ICollection<DisbursementOrderDetail> DisbursementOrderDetails { get; set; }
        public virtual ICollection<Inventory> InventoryItems { get; set; }
        public virtual ICollection<InventoryMovement> InventoryMovements { get; set; }

        public Item()
        {
            SupplyOrderDetails = new HashSet<SupplyOrderDetail>();
            DisbursementOrderDetails = new HashSet<DisbursementOrderDetail>();
            InventoryItems = new HashSet<Inventory>();
            InventoryMovements = new HashSet<InventoryMovement>();
        }
    }
}