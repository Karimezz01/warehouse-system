using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehousesystem.models
{
   public class transferitem
    {
        [Key]
        public int TransferID { get; set; } // Primary Key for the transfer record

        [Required]
        public DateTime TransferDate { get; set; } // When the transfer occurred

        [Required]
        public int ItemID { get; set; } // Which item is being transferred
        [ForeignKey("ItemID")]
        public Item Item { get; set; } // Navigation property to the Item

        [Required]
        public int Quantity { get; set; } // How much of the item is being transferred

        [Required]
        public int SourceWarehouseID { get; set; } // From which warehouse
        [ForeignKey("SourceWarehouseID")]
        public Warehouse SourceWarehouse { get; set; } // Navigation property to the Source Warehouse

        [Required]
        public int DestinationWarehouseID { get; set; } // To which warehouse
        [ForeignKey("DestinationWarehouseID")]
        public Warehouse DestinationWarehouse { get; set; } // Navigation property to the Destination Warehouse

        public string Notes { get; set; } // Optional notes about the transfer
        public DateTime? ProductionDate { get; internal set; }
        public DateTime? ExpiryDate { get; internal set; }
        public int SupplierID { get; internal set; }
    }
}