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
    public partial class CustomerForm : Form
    {
        private readonly AppDbcontext dbcontext;
        private Customer selectedCustomer;
        public CustomerForm()
        {
            InitializeComponent();
            dbcontext = new AppDbcontext();

        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            RefreshCustomerGrid();
            ClearForm();

        }

        private void ClearForm()
        {

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            comboBox1.SelectedIndex = -1;

            selectedCustomer = null;


        }

        private void RefreshCustomerGrid()
        {

            dataGridView1.DataSource = dbcontext.Customers.Select(c => new
            {
                c.CustomerID,
                c.Name,
                c.Phone,
                c.Fax,
                c.Mobile,
                c.Email,
                c.Website
            }).ToList();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadCustomers()
        {
            comboBox1.DataSource = null;
            comboBox1.DataSource = dbcontext.Customers.ToList();
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "CustomerID";
            comboBox1.SelectedIndex = -1;
        }
        private void ADD_Click(object sender, EventArgs e)
        {
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text)
                )
            {
                MessageBox.Show("Please fill in all fields.");
                return false;
            }
            else
            {
                return true;
            }




        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedCustomer != null)
            {
                MessageBox.Show("Please select a customer to update.");
                return;
            }
            bool changes =
                textBox1.Text != selectedCustomer.Name ||
                textBox2.Text != selectedCustomer.Phone ||

                textBox3.Text != selectedCustomer.Fax ||
                textBox4.Text != selectedCustomer.Mobile ||
                textBox5.Text != selectedCustomer.Email ||
                textBox6.Text != selectedCustomer.Website;
            if (!changes)
            {
                MessageBox.Show("No changes to update.");
                return;
            }
            var customerTOUpdate = dbcontext.Customers.Find(selectedCustomer.CustomerID);
            if (customerTOUpdate != null)
            {
                customerTOUpdate.Name = textBox1.Text;
                customerTOUpdate.Phone = textBox2.Text;
                customerTOUpdate.Fax = textBox3.Text;
                customerTOUpdate.Mobile = textBox4.Text;
                customerTOUpdate.Email = textBox5.Text;
                customerTOUpdate.Website = textBox6.Text;
                dbcontext.SaveChanges();
                LoadCustomers();
                RefreshCustomerGrid();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Customer not found.");
            }
        }

        private void ADD_Click_1(object sender, EventArgs e)
        {

            if (ValidateInputs())
            {
                var customer = new Customer
                {
                    Name = textBox1.Text,
                    Phone = textBox2.Text,
                    Fax = textBox3.Text,
                    Mobile = textBox4.Text,
                    Email = textBox5.Text,
                    Website = textBox6.Text
                };
                dbcontext.Customers.Add(customer);
                dbcontext.SaveChanges();
                LoadCustomers();
                RefreshCustomerGrid();
                ClearForm();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to update.");
                return;
            }

            bool changes =
                textBox1.Text != selectedCustomer.Name ||
                textBox2.Text != selectedCustomer.Phone ||
                textBox3.Text != selectedCustomer.Fax ||
                textBox4.Text != selectedCustomer.Mobile ||
                textBox5.Text != selectedCustomer.Email ||
                textBox6.Text != selectedCustomer.Website;

            if (!changes)
            {
                MessageBox.Show("No changes to update.");
                return;
            }

          
            var customerToUpdate = dbcontext.Customers.Find(selectedCustomer.CustomerID);
            if (customerToUpdate != null)
            {
                // ✅ Update customer fields
                customerToUpdate.Name = textBox1.Text;
                customerToUpdate.Phone = textBox2.Text;
                customerToUpdate.Fax = textBox3.Text;
                customerToUpdate.Mobile = textBox4.Text;
                customerToUpdate.Email = textBox5.Text;
                customerToUpdate.Website = textBox6.Text;

                dbcontext.SaveChanges();

                MessageBox.Show("Customer updated successfully.");
                LoadCustomers();         // Reload comboBox or list
                RefreshCustomerGrid();   // Reload DataGridView
                ClearForm();             // Reset fields
            }
            else
            {
                MessageBox.Show("Customer not found.");
            }
        }
    }
    }

        