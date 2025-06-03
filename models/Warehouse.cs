using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
namespace warehousesystem.models
{
    public class Warehouse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WarehouseID { get; set; }

        [Required(ErrorMessage = "اسم المخزن مطلوب.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string ResponsiblePerson { get; set; }

        public virtual ICollection<SupplyOrder> SupplyOrders { get; set; }
        public virtual ICollection<DisbursementOrder> DisbursementOrders { get; set; }
        public virtual ICollection<Inventory> InventoryItems { get; set; }
        public virtual ICollection<InventoryMovement> SourceMovements { get; set; }
        public virtual ICollection<InventoryMovement> DestinationMovements { get; set; }

        public Warehouse()
        {
            SupplyOrders = new HashSet<SupplyOrder>();
            DisbursementOrders = new HashSet<DisbursementOrder>();
            InventoryItems = new HashSet<Inventory>();
            SourceMovements = new HashSet<InventoryMovement>();
            DestinationMovements = new HashSet<InventoryMovement>();
        }
    }
}