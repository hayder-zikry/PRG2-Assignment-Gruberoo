using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Gruberoo
{
    internal class Customer
    {
        // Attributes
        private string customerName;
        private string emailAddress;

        // Association
        private List<Order> orders;

        // Getters and Setters
        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; }
        }

        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }

        public List<Order> Orders
        {
            get { return orders; }
        }

        // Constructor
        public Customer(string customerName, string emailAddress)
        {
            this.customerName = customerName;
            this.emailAddress = emailAddress;

            orders = new List<Order>();
        }

        // Methods
        public void AddOrder(Order order)
        {
            if (order != null)
            {
                orders.Add(order);
            }
        }

        public bool RemoveOrder(Order order)
        {
            return orders.Remove(order);
        }

        public void DisplayAllOrders()
        {
            foreach (Order o in orders)
            {
                Console.WriteLine(o);
            }
        }

        public override string ToString()
        {
            return $"{customerName} | Email: {emailAddress}";
        }
    }
}
