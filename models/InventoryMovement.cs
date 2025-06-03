using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehousesystem.models
{
 public   class InventoryMovement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovementID { get; set; }

        [Required]
        public DateTime MovementDate { get; set; }

        [Required]
        public int ItemID { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Required]
        [MaxLength(50)]
        public string MovementType { get; set; }
        public int? SourceWarehouseID { get; set; }
        public int? DestinationWarehouseID { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int? SupplierID { get; set; }
        [ForeignKey("ItemID")]
        public Item Item { get; set; }

        [ForeignKey("SourceWarehouseID")]
        public Warehouse SourceWarehouse { get; set; }

        [ForeignKey("DestinationWarehouseID")]
        public Warehouse DestinationWarehouse { get; set; }

        [ForeignKey("SupplierID")]
        public Supplier Supplier { get; set; }
    }

}
