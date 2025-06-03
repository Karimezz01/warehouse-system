using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.DAl;
using warehousesystem.models;

namespace warehousesystem.bussines_logic
{
    public class CustomerService
    {
        private readonly CustomerRepository _customerRepository;

        public CustomerService()
        {
            _customerRepository = new CustomerRepository();
        }

        public List<Customer> GetAllCustomers()
        {
            return _customerRepository.GetAll();
        }

        public Customer GetCustomerById(int id)
        {
            return _customerRepository.GetById(id);
        }

        public bool AddCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                throw new ArgumentException("اسم العميل مطلوب.");
            }
            _customerRepository.Add(customer);
            return true;
        }

        public bool UpdateCustomer(Customer customer)
        {
            if (customer.CustomerID <= 0)
            {
                throw new ArgumentException("معرف العميل غير صالح للتعديل.");
            }
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                throw new ArgumentException("اسم العميل مطلوب.");
            }
            _customerRepository.Update(customer);
            return true;
        }

        public void DeleteCustomer(int id)
        {
            _customerRepository.Delete(id);
        }
    }

}
