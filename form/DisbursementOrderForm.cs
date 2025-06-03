using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using warehousesystem.data;
using warehousesystem.models;

namespace warehousesystem.forms
{
    public partial class DisbursementOrderForm: Form
    {
        private AppDbcontext dbcontext;
        private List<DisbursementOrderDetail> orderDetails = new List<DisbursementOrderDetail>();

        public DisbursementOrderForm()
        {
            InitializeComponent();
            dbcontext = new AppDbcontext();
            LoadWarehouseProducts();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            LoadcCustomers();
            LoadItems();
            comboBox3.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;

            IntializeGrid();
        }

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1 || comboBox3.SelectedIndex == -1)
            {
                label7.Text = "Available Quantity: —";
                return;
            }

            int warehouseId = (int)comboBox1.SelectedValue;
            int itemId = (int)comboBox3.SelectedValue;

            var inventory = dbcontext.Inventory
                .FirstOrDefault(i => i.WarehouseID == warehouseId && i.ItemID == itemId);

            if (inventory != null)
            {
                label7.Text = $"Available Quantity: {inventory.Quantity}";
            }
            else
            {
                label7.Text = "Available Quantity: 0";
            }
        }        

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1) return;

            int warehouseId = (int)comboBox1.SelectedValue;

            var availableItems = dbcontext.Inventory
                .Where(i => i.WarehouseID == warehouseId && i.Quantity > 0)
                .Select(i => i.Item)
                .Distinct()
                .ToList();

            comboBox3.DataSource = availableItems;
            comboBox3.DisplayMember = "ItemName";
            comboBox3.ValueMember = "ItemId";
            comboBox3.SelectedIndex = -1;

            textBox2.Clear();
            label7.Text = "Available Quantity:  —";
        }

        private void IntializeGrid()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ItemId", "Item ID");
            dataGridView1.Columns.Add("ItemName", "Item Name");
            dataGridView1.Columns.Add("Quantity", "Quantity");

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void LoadItems()
        {
            comboBox3.DataSource = dbcontext.Items.ToList();
            comboBox3.DisplayMember = "ItemName";
            comboBox3.ValueMember = "ItemId";
            comboBox3.SelectedIndex = -1;
        }

        private void LoadcCustomers()
        {
            comboBox2.DataSource = dbcontext.Customers.ToList();
            comboBox2.DisplayMember = "CustomerName";
            comboBox2.ValueMember = "CustomerId";
            comboBox2.SelectedIndex = -1;
        }

        private void LoadWarehouseProducts()
        {
            comboBox1.DataSource = dbcontext.Warehouses.ToList();
            comboBox1.DisplayMember = "WarehouseName";
            comboBox1.ValueMember = "WarehouseID";
            comboBox1.SelectedIndex = -1;
        }

        private void DisbursementOrderForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem == null || !decimal.TryParse(textBox2.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Please select an item and enter a valid quantity greater than zero.");
                return;
            }

            int itemId = (int)comboBox3.SelectedValue;
            int warehouseId = (int)comboBox1.SelectedValue;

            if (orderDetails.Any(i => i.ItemID == itemId))
            {
                MessageBox.Show("This item is already added to the order.");
                return;
            }

            if (!CheckInventoryAvailability(itemId, warehouseId, quantity))
            {
                MessageBox.Show("Insufficient quantity in warehouse inventory.");
                return;
            }

            var item = dbcontext.Items.Find(itemId);

            var newItem = new DisbursementOrderDetail
            {
                ItemID = itemId,
                Quantity = quantity,
                Item = item
            };

            orderDetails.Add(newItem);
            refreshGrid();
        }

        private void refreshGrid()
        {
            dataGridView1.Rows.Clear();

            foreach (var item in orderDetails)
            {
                dataGridView1.Rows.Add(
                    item.ItemID,
                    item.Item?.Name ?? "",
                    item.Quantity
                );
            }
        }

        private bool CheckInventoryAvailability(int itemId, int warehouseId, decimal quantity)
        {
            var inventory = dbcontext.Inventory
                                  .FirstOrDefault(i => i.ItemID == itemId && i.WarehouseID == warehouseId);

            return inventory != null && inventory.Quantity >= quantity;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Order number is required.");
                return;
            }

            if (dbcontext.DisbursementOrders.Any(o => o.OrderNumber == textBox1.Text))
            {
                MessageBox.Show("Order number already exists.");
                return;
            }

            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select both warehouse and customer.");
                return;
            }

            if (orderDetails.Count == 0)
            {
                MessageBox.Show("Add at least one item.");
                return;
            }

            var newOrder = new DisbursementOrder
            {
                OrderNumber = textBox1.Text.Trim(),
                OrderDate = dateTimePicker1.Value.Date,
                WarehouseID = (int)comboBox1.SelectedValue,
               SupplierID = (int)comboBox2.SelectedValue,
                DisbursementOrderDetails = orderDetails
            };

            foreach (var item in orderDetails)
            {
                var inventory = dbcontext.Inventory
                                        .FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == newOrder.WarehouseID);

                if (inventory != null)
                {
                    inventory.Quantity -= item.Quantity;
                }
            }

            dbcontext.DisbursementOrders.Add(newOrder);
            dbcontext.SaveChanges();

            MessageBox.Show("Order saved successfully.");
            ClearForm();
        }

        private void ClearForm()
        {
            textBox1.Clear();
            dateTimePicker1.Value = DateTime.Today;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            textBox2.Clear();
            orderDetails.Clear();
            refreshGrid();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearForm();

        }
    }
}

