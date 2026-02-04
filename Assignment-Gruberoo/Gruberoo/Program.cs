using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

//==========================================================
// Student Number : S10273358K
// Student Name : Michael Jordan
// Partner Name : Scottie Pippen
//==========================================================

namespace Gruberoo
{
    internal class Program
    {
        static Dictionary<string, Customer> customersByEmail = new Dictionary<string, Customer>();
        static List<Order> allOrders = new List<Order>();
        
        static Dictionary<string, Restaurant> restaurantsById = new Dictionary<string, Restaurant>();
        static int foodItemsLoaded = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

            LoadRestaurantsAndFoodItems();
            Console.WriteLine($"{restaurantsById.Count} restaurants loaded!");
            Console.WriteLine($"{foodItemsLoaded} food items loaded!");

            LoadCustomers();
            Console.WriteLine($"{customersByEmail.Count} customers loaded!");
            
            LoadOrders();
            Console.WriteLine($"{allOrders.Count} orders loaded!");
        }

        // Student 1: Hayder Zikry S10272315B
        // Features: 2, 3, 5, 7
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

            for (int i = 1; i < lines.Length; i++)          // Skip header line
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;                               // Skip empty lines
                }

                string[] parts = lines[i].Split(',');

                if (parts.Length < 2)
                {
                    continue;
                }

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

            for (int i = 1; i < lines.Length; i++)          // Skip header line
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;                               // Skip empty lines
                }

                string[] parts = lines[i].Split(',');

                if (parts.Length < 9)
                {
                    continue;
                }

                // Parts
                int orderId = int.Parse(parts[0].Trim());
                string customerEmail = parts[1].Trim();
                string restaurantId = parts[2].Trim();
                string deliveryDate = parts[3].Trim();
                string deliveryTime = parts[4].Trim();
                string deliveryAddress = parts[5].Trim();
                string createdDateTime = parts[6].Trim();
                double totalAmount = double.Parse(parts[7].Trim());
                string status = parts[8].Trim();

                DateTime deliveryDateTimeObj = DateTime.ParseExact(deliveryDate + " " + deliveryTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                DateTime createdDateTimeObj = DateTime.ParseExact(createdDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                Order order = new Order(orderId, createdDateTimeObj, deliveryDateTimeObj, deliveryAddress, totalAmount, status);

                allOrders.Add(order);
                if (customersByEmail.ContainsKey(customerEmail))
                {
                    customersByEmail[customerEmail].AddOrder(order);
                }

                if (restaurantsById.ContainsKey(restaurantId))
                {
                    restaurantsById[restaurantId].OrderQueue.Enqueue(order);
                }
            }

        }

        // Student 2: Seah Hao Bin S10273358K
        // Features: 1, 4, 6, 8
        static void LoadRestaurantsAndFoodItems()
        {
            // Feature 1 : Load files (restaurants and food items)

            restaurantsById.Clear();
            foodItemsLoaded = 0;

            string rPath = "data/restaurants.csv";
            if (!File.Exists(rPath))
            {
                Console.WriteLine("Restaurants file not found.");
                return;
            }

            string[] rLines = File.ReadAllLines(rPath);

            for (int i = 1; i < rLines.Length; i++) // skip header
            {
                if (string.IsNullOrWhiteSpace(rLines[i]))
                    continue;

                string[] parts = rLines[i].Split(',');
                if (parts.Length < 3)
                    continue;

                string rid = parts[0].Trim();
                string name = parts[1].Trim();
                string email = parts[2].Trim();

                Restaurant r = new Restaurant(rid, name, email);

                
                Menu m = new Menu(rid + "-M1", "Main Menu");
                r.AddMenu(m);

                restaurantsById[rid] = r;
            }

            string fPath = "data/fooditems.csv";
            if (!File.Exists(fPath))
            {
                Console.WriteLine("Food items file not found.");
                return;
            }

            string[] fLines = File.ReadAllLines(fPath);

            for (int i = 1; i < fLines.Length; i++) // skip header
            {
                if (string.IsNullOrWhiteSpace(fLines[i]))
                    continue;

                string[] parts = fLines[i].Split(',');
                if (parts.Length < 4)
                    continue;

                string rid = parts[0].Trim();
                string itemName = parts[1].Trim();

                string priceText = parts[parts.Length - 1].Trim();

                string itemDesc = "";
                for (int j = 2; j < parts.Length - 1; j++)
                {
                    if (j > 2)
                    {
                        itemDesc += ",";
                    }
                    itemDesc += parts[j];
                }
                itemDesc = itemDesc.Trim();

                double price;
                if (!double.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                    continue;

                if (restaurantsById.ContainsKey(rid))
                {
                    FoodItem fi = new FoodItem(itemName, itemDesc, price, "-");

                    
                    restaurantsById[rid].Menus[0].AddFoodItem(fi);

                    foodItemsLoaded++;
                }
            }
        }
    }
}

// TODO: Basic Features To Be Implemented

// Feature 3 : List all restaurants and menu items
// Hayder Zikry S10272315B


// Feature 4 : List all orders with basic information
// Seah Hao Bin S10273358K


// Feature 5 : Create a new order
// Hayder Zikry S10272315B


// Feature 6 : Process an order
// Seah Hao Bin S10273358K


// Feature 7 : Modify an existing order
// Hayder Zikry S10272315B


// Feature 8 : Delete an existing order
// Seah Hao Bin S10273358K
