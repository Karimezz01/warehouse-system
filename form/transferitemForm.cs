using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using warehousesystem.data;
using warehousesystem.models;

namespace warehousesystem.forms
{
    public partial class transferitemForm: Form
    { private readonly AppDbcontext dbcontext;
        private List<Inventory> products = new List<Inventory>();
        private List  <transferitem> transferitems = new List<transferitem>();
        public transferitemForm()
        {
            InitializeComponent();
            dbcontext = new AppDbcontext();
            LoadWarehouses();
            Intializproductsegrid();
            Intializetransfer();
        }

        private void Intializetransfer()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("ItemName", "Item");
            dataGridView1.Columns.Add("Quantity", "Quantity");
            dataGridView1.Columns.Add("Supplier", "Supplier");
            dataGridView1.Columns.Add("ProductionDate", "Production Date");
            dataGridView1.Columns.Add("ExpiryDate", "Expiry Date");
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Intializproductsegrid()
        {
            dataGridView2.Columns.Clear();
            dataGridView2.Columns.Add("ItemName", "Item");
            dataGridView2.Columns.Add("SupplierName", "Supplier");
            dataGridView2.Columns.Add("ProductionDate", "Production Date");
            dataGridView2.Columns.Add("ExpiryDate", "Expiry Date");
            dataGridView2.Columns.Add("CurrentQuantity", "Available Quantity");
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.MultiSelect = false;

        }

        private void LoadWarehouses()
        {
            var warehouses = dbcontext.Warehouses.ToList();

            comboBox1.DataSource = warehouses;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "WarehouseID";
            comboBox1.SelectedIndexChanged += (s, e) => LoadAvailable();

            comboBox2.DataSource = warehouses.ToList();
            comboBox2.DisplayMember = "Name";
            comboBox2.ValueMember = "WarehouseID";
            comboBox2.SelectedIndex = -1;
        }

        private void LoadAvailable()
        {
            products.Clear();
            dataGridView2.Rows.Clear();

            if (comboBox1.SelectedIndex != -1)
            {
                int fromId = (int)comboBox1.SelectedValue;
                products = dbcontext.Inventory
                    .Where(i => i.WarehouseID == fromId && i.Quantity > 0)
                    .Include(i => i.Item)
                    .Include(i => i.Supplier)
                    .ToList();

                foreach (var b in products)
                {
                    dataGridView2.Rows.Add(
                        b.Item?.Name,
                        b.Supplier?.Name,
                        b.ProductionDate.HasValue ? b.ProductionDate.Value.ToShortDateString() : "",
                        b.ExpiryDate.HasValue ? b.ExpiryDate.Value.ToShortDateString() : "",
                        b.Quantity
                    );
                }
            }
        }

        private void transferitemForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a batch to transfer.");
                return;
            }

            if (!decimal.TryParse(textBox2.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Enter a valid quantity.");
                return;
            }

            int selectedIndex = dataGridView2.SelectedRows[0].Index;
            var batch = products[selectedIndex];

            if (batch.Quantity < quantity)
            {
                MessageBox.Show("Not enough quantity in selected batch.");
                return;
            }

            transferitems.Add(new transferitem
            {
              
                ItemID = batch.ItemID,
                Quantity = (int)quantity,
                SourceWarehouseID = (int)comboBox1.SelectedValue,
                DestinationWarehouseID = (int)comboBox2.SelectedValue,
                TransferDate = DateTime.Now,
                Notes = textBox1.Text

            });

            RefreshTransferItemsGrid();
            textBox2.Clear();
        }

        private void RefreshTransferItemsGrid()
        {
            throw new NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select source and destination warehouses.");
                return;
            }

            int fromId = (int)comboBox1.SelectedValue;
            int toId = (int)comboBox2.SelectedValue;

            if (fromId == toId)
            {
                MessageBox.Show("Source and destination must be different.");
                return;
            }

            if (transferitems.Count == 0)
            {
                MessageBox.Show("Add at least one item to transfer.");
                return;
            }

            var transferOrder = new transferitem
            {
                TransferDate = dateTimePicker3.Value.Date,
                SourceWarehouseID = fromId,
                DestinationWarehouseID = toId,
                Notes = textBox1.Text

            };

            foreach (var item in transferitems)
            {
                var fromBatch = dbcontext.Inventory.FirstOrDefault(i =>
                    i.ItemID == item.ItemID &&
                    i.WarehouseID == fromId &&
                    i.ProductionDate == item.ProductionDate &&
                    i.ExpiryDate == item.ExpiryDate &&
                    i.SupplierID == item.SupplierID);

                if (fromBatch == null || fromBatch.Quantity < item.Quantity)
                {
                    MessageBox.Show($"Not enough quantity for item {item.Item?.Name} in source batch.");
                    return;
                }

                fromBatch.Quantity -= item.Quantity;

                var toBatch = dbcontext.Inventory.FirstOrDefault(i =>
                    i.ItemID == item.ItemID &&
                    i.WarehouseID == toId &&
                    i.ProductionDate == item.ProductionDate &&
                    i.ExpiryDate == item.ExpiryDate &&
                    i.SupplierID == item.SupplierID);

                if (toBatch == null)
                {
                    toBatch = new Inventory
                    {
                        ItemID = item.ItemID,
                        WarehouseID = toId,
                        SupplierID = item.SupplierID,
                        ProductionDate = item.ProductionDate,
                        ExpiryDate = item.ExpiryDate,
                        Quantity = 0

                    };
                    dbcontext.Inventory.Add(toBatch);
                }

                toBatch.Quantity += item.Quantity;
            }

            dbcontext.TransferItems.Add(transferOrder);
            dbcontext.SaveChanges();

            MessageBox.Show("Transfer completed successfully.");

            clearform();
        }

        private void clearform()
        {
            textBox2.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            transferitems.Clear();
            dataGridView2.Rows.Clear();
            dataGridView1.Rows.Clear();
            dateTimePicker3.Value = DateTime.Today;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearform();
        }
    }
}
