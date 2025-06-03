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
    public partial class ItemsForm : Form
    {
        private readonly AppDbcontext dbcontext;
        private Item newitem;
        public ItemsForm()
        {
            InitializeComponent();
            dbcontext = new AppDbcontext();

        }

        private void ItemsForm_Load(object sender, EventArgs e)
        {
            Loaditems();
        }
        private void Loaditems()
        {
            var items = dbcontext.Items;
            dataGridView1.DataSource = items.Select(i => new
            {
                i.ItemID,
                i.Code,
                i.Name,
                i.UnitOfMeasure
            }).ToList();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Populate ComboBox
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[]
            {
                "Units",
                "Boxes",
                "Packs",
                "Bottles",
                "Cases"
            }); comboBox1.SelectedIndex = -1;

            comboBox2.DataSource = null;
            comboBox2.DataSource = dbcontext.Items.ToList();
            comboBox2.DisplayMember = "Code";
            comboBox2.ValueMember = "ItemID";
            comboBox2.SelectedIndex = -1;
            textBox1.Clear();
            textBox2.Clear();
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return false;
            }

            return true;
        }
        private void ClearForm()
        {
            textBox1.Clear();
            textBox2.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            var newItem = new Item
            {
                Code = textBox1.Text,
                Name = textBox2.Text,
                UnitOfMeasure = comboBox1.SelectedItem?.ToString() ?? "Units"
            };

           dbcontext.Items.Add(newItem);
            dbcontext.SaveChanges();

            MessageBox.Show("Item added successfully");
            ClearForm();
            Loaditems();
        
    }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select an item to update.");
                return;
            }
            int selectedItemId = (int)comboBox2.SelectedValue;
            var itemToUpdate = dbcontext.Items.Find(selectedItemId);
            if (itemToUpdate != null)
            {


                itemToUpdate.Code = textBox1.Text;
                itemToUpdate.Name = textBox2.Text;
                itemToUpdate.UnitOfMeasure = comboBox1.SelectedItem?.ToString() ?? "Units";
                dbcontext.SaveChanges();
                MessageBox.Show("Item updated successfully");
                ClearForm();
                Loaditems();

            }
            else
            {
                MessageBox.Show("Item not found.");
            }


        }
    }
}
