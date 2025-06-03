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
    public partial class WarehouseForm : Form
    {
        private readonly AppDbcontext dbcontext;
        private readonly Warehouse warehouse;
        public WarehouseForm()
        {
            dbcontext = new AppDbcontext();
            InitializeComponent();
        }

        private void WarehouseForm_Load(object sender, EventArgs e)
        {
            LoadWarehouses();

        }

        private void LoadWarehouses()
        {
            var warehouses = dbcontext.Warehouses.ToList();
            dataGridView1.DataSource = warehouses.Select(w => new
            {
                w.WarehouseID,
                w.Name,
                w.Address,
                w.ResponsiblePerson,
            }).ToList();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


            comboBox1.DataSource = null;
            comboBox1.DataSource = warehouses;
            comboBox1.DisplayMember = "WarehouseName";
            comboBox1.ValueMember = "WarehouseId";
            comboBox1.SelectedIndex = -1;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            var newWarehouse = new Warehouse
            {
                Name = textBox1.Text,
                Address = textBox2.Text,
                ResponsiblePerson = textBox3.Text
            };

            dbcontext.Warehouses.Add(newWarehouse);
            dbcontext.SaveChanges();

            MessageBox.Show("Warehouse added successfully.");
            clearform();
            LoadWarehouses();
        }

        private void clearform()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return false;
            }

            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (warehouse == null)
            {
                MessageBox.Show("Please select a warehouse to update.");
                return;
            }

            if (!ValidateInputs()) return;

            bool hasChanges =
                textBox1.Text != warehouse.Name ||
                textBox2.Text != warehouse.Address ||
                textBox3.Text != warehouse.ResponsiblePerson;

            if (!hasChanges)
            {
                MessageBox.Show("No changes detected.");
                return;
            }

            var warehouseToUpdate = dbcontext.Warehouses
                .FirstOrDefault(w => w.WarehouseID == warehouse.WarehouseID);

            if (warehouseToUpdate != null)
            {
                warehouseToUpdate.Name = textBox1.Text;
                warehouseToUpdate.Address = textBox2.Text;
                warehouseToUpdate.ResponsiblePerson = textBox3.Text;

                dbcontext.SaveChanges();

                MessageBox.Show("Warehouse updated successfully.");
                clearform();
                LoadWarehouses();
            }
            else
            {
                MessageBox.Show("Warehouse not found in database.");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
