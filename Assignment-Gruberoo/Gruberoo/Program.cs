using System;

namespace Gruberoo
{
    internal class Program
    {
        static Dictionary<string, Customer> customersByEmail = new Dictionary<string, Customer>();
        static List<Order> allOrders = new List<Order>();

        static void Main(string[] args)
        {
            LoadCustomers();
            LoadOrders();

            Console.WriteLine($"Loaded {customersByEmail.Count} customers.");
            Console.WriteLine($"Loaded {allOrders.Count} orders.");
        }

        // Hayder Zikry S10272315B
        static void LoadCustomers()
        {
            // Feature 2 : Load files (customers and orders)

            string path = "data/customers.csv";
            if (!File.Exists(path))
            {
                Console.WriteLine("Customers file not found.");
                return;
            }

            string[] lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length; i++) // Skip header line
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue; // Skip empty lines
                }

                string[] parts = lines[i].Split(',');

                // Parts
                string name = parts[0].Trim();
                string email = parts[1].Trim();

                customersByEmail[email] = new Customer(name, email);
            }
        }

        static void LoadOrders()
        {
            // Feature 2 : Load files (customers and orders)

            string path = "data/orders.csv";
            if (!File.Exists(path))
            {
                Console.WriteLine("Orders file not found.");
                return;
            }

            string[] lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length; i++) // Skip header line
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue; // Skip empty lines
                }

                string[] parts = lines[i].Split(',');

                // Parts
                int orderId = int.Parse(parts[0].Trim());
                string customerEmail = parts[1].Trim();
                string deliveryDate = parts[3].Trim();
                string deliveryTime = parts[4].Trim();
                string deliveryAddress = parts[5].Trim();
                string createdDateTime = parts[6].Trim();
                double totalAmount = double.Parse(parts[7].Trim());
                string status = parts[8].Trim();

                DateTime deliveryDateTimeObj = DateTime.Parse($"{deliveryDate} {deliveryTime}");
                DateTime createdDateTimeObj = DateTime.Parse(createdDateTime);

                Order order = new Order(orderId, createdDateTimeObj, deliveryDateTimeObj, deliveryAddress, totalAmount, status);

                allOrders.Add(order);
                customersByEmail[customerEmail].AddOrder(order);
            }

        }

        // Seah Hao Bin
    }
}

// Feature 1 : Load files (restaurants and food items)
//


// Feature 3 : List all restaurants and menu items
// Hayder Zikry S10272315B


// Feature 4 : List all orders with basic information
//


// Feature 5 : Create a new order
// Hayder Zikry S10272315B


// Feature 6 : Process an order
// 


// Feature 7 : Modify an existing order
// Hayder Zikry S10272315B


// Feature 8 : Delete an existing order
//
