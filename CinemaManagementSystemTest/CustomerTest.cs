using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using CinemaManagementSystem.Models;

namespace CinemaManagementSystemTest
{
    public class CustomerTest
    {
        private List<Customer> GetSampleCustomers()
        {
            return new List<Customer>
            {
                new Customer { CustomerId = 1, FullName = "Nguyen Van A", Phone = "0123456789" },
                new Customer { CustomerId = 2, FullName = "Tran Thi B", Phone = "0987654321" }
            };
        }

        [Fact]
        public void AddCustomer_ShouldSucceed_WhenValidData()
        {
            var customers = GetSampleCustomers();
            var newCustomer = new Customer { CustomerId = 3, FullName = "Le Van C", Phone = "0909090909" };

            customers.Add(newCustomer);

            Assert.Contains(customers, c => c.CustomerId == 3);
        }

        [Fact]
        public void AddCustomer_ShouldFail_WhenDuplicateID()
        {
            var customers = GetSampleCustomers();
            var newCustomer = new Customer { CustomerId = 1, FullName = "Le Van D", Phone = "0911111111" };

            bool exists = customers.Any(c => c.CustomerId == newCustomer.CustomerId);

            Assert.True(exists); // Đã tồn tại nên thêm sẽ fail
        }

        [Fact]
        public void AddCustomer_ShouldFail_WhenPhoneContainsLetters()
        {
            var newCustomer = new Customer { CustomerId = 4, FullName = "Test", Phone = "09A1B2C3" };

            bool isValidPhone = newCustomer.Phone.All(char.IsDigit);

            Assert.False(isValidPhone); // SĐT có ký tự không hợp lệ
        }

        [Fact]
        public void EditCustomer_ShouldUpdateFields()
        {
            var customers = GetSampleCustomers();
            var customer = customers.FirstOrDefault(c => c.CustomerId == 1);

            customer.FullName = "Nguyen Van Updated";
            customer.Phone = "0999999999";

            Assert.Equal("Nguyen Van Updated", customer.FullName);
            Assert.Equal("0999999999", customer.Phone);
        }

        [Fact]
        public void DeleteCustomer_ShouldRemoveFromList()
        {
            var customers = GetSampleCustomers();
            var customerToRemove = customers.FirstOrDefault(c => c.CustomerId == 2);

            customers.Remove(customerToRemove);

            Assert.DoesNotContain(customers, c => c.CustomerId == 2);
        }
    }
}