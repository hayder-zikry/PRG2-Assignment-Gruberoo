using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace Gruberoo
{
    internal class Program
    {
        static Dictionary<string, Customer> customersByEmail = new Dictionary<string, Customer>();
        static List<Order> allOrders = new List<Order>();
        
        static Dictionary<string, Restaurant> restaurantsById = new Dictionary<string, Restaurant>();
        static int foodItemsLoaded = 0;

        static Dictionary<int, string> orderCustomerEmail = new Dictionary<int, string>();
        static Dictionary<int, string> orderRestaurantId = new Dictionary<int, string>();

        static Stack<Order> refundStack = new Stack<Order>();

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

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
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

                orderCustomerEmail[orderId] = customerEmail;
                orderRestaurantId[orderId] = restaurantId;
                restaurantsById[restaurantId].AddToOrderQueue(order);

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

        // Feature 4 : List all orders with basic information
        // Seah Hao Bin S10273358K

        static void ListAllOrdersBasicInfo()
        {

            if (allOrders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }

            Console.WriteLine("All Orders");
            Console.WriteLine("==========");

            Console.WriteLine("{0,-11}{1,-15}{2,-17}{3,-21}{4,-11}{5,-12}",
                "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
            Console.WriteLine("{0,-11}{1,-15}{2,-17}{3,-21}{4,-11}{5,-12}",
                "--------", "----------", "-------------", "------------------", "------", "---------");

            foreach (Order o in allOrders)
            {
                string custName = "Unknown";
                if (orderCustomerEmail.ContainsKey(o.OrderId))
                {
                    string email = orderCustomerEmail[o.OrderId];
                    if (customersByEmail.ContainsKey(email))
                    {
                        custName = customersByEmail[email].CustomerName;
                    }
                }

                string restName = "Unknown";
                if (orderRestaurantId.ContainsKey(o.OrderId))
                {
                    string rid = orderRestaurantId[o.OrderId];
                    if (restaurantsById.ContainsKey(rid))
                    {
                        restName = restaurantsById[rid].RestaurantName;
                    }
                }

                Console.WriteLine("{0,-10} {1,-14} {2,-16} {3,-20} ${4,-9} {5}",
                    o.OrderId,
                    custName,
                    restName,
                    o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"),
                    o.OrderTotal.ToString("0.00"),
                    o.OrderStatus);
            }
        }

        // Feature 6 : Process an order
        // Seah Hao Bin S10273358K

        static void ProcessOrder()
        {
      
            Console.WriteLine();
            Console.WriteLine("Process Order");
            Console.WriteLine("=============");

            Console.Write("Enter Restaurant ID: ");
            string rid = Console.ReadLine().Trim();

            if (!restaurantsById.ContainsKey(rid))
            {
                Console.WriteLine("Invalid Restaurant ID.");
                return;
            }

            Restaurant r = restaurantsById[rid];

            if (r.OrderQueue.Count == 0)
            {
                Console.WriteLine("No orders to process.");
                return;
            }

            // Loop through the queue once (avoid infinite loop if we re-enqueue)
            int count = r.OrderQueue.Count;

            for (int i = 0; i < count; i++)
            {
                Order o = r.OrderQueue.Dequeue();

                // Customer name (using your mapping)
                string custName = "Unknown";
                if (orderCustomerEmail.ContainsKey(o.OrderId))
                {
                    string email = orderCustomerEmail[o.OrderId];
                    if (customersByEmail.ContainsKey(email))
                        custName = customersByEmail[email].CustomerName;
                }

                Console.WriteLine();
                Console.WriteLine($"Order {o.OrderId}:");
                Console.WriteLine($"Customer: {custName}");
                Console.WriteLine("Ordered Items:");
                o.DisplayOrderedFoodItems();
                Console.WriteLine($"Delivery date/time: {o.DeliveryDateTime:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Total Amount: ${o.OrderTotal:0.00}");
                Console.WriteLine($"Order Status: {o.OrderStatus}");
                Console.WriteLine();

                Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
                string action = Console.ReadLine().Trim().ToUpper();

                if (action == "C")
                {
                    if (o.OrderStatus == Order.STATUS_PENDING)
                    {
                        o.OrderStatus = Order.STATUS_PREPARING;
                        Console.WriteLine($"Order {o.OrderId} confirmed. Status: {o.OrderStatus}");
                        r.OrderQueue.Enqueue(o); // still active
                    }
                    else
                    {
                        Console.WriteLine($"Cannot confirm. Order status is {o.OrderStatus}.");
                        r.OrderQueue.Enqueue(o); // keep order
                    }
                }
                else if (action == "R")
                {
                    if (o.OrderStatus == Order.STATUS_PENDING)
                    {
                        o.OrderStatus = Order.STATUS_REJECTED;
                        refundStack.Push(o);
                        Console.WriteLine($"Order {o.OrderId} rejected. Status: {o.OrderStatus}");
                        Console.WriteLine($"Refund required for Order {o.OrderId}.");
                        // rejected = archived (do not enqueue back)
                    }
                    else
                    {
                        Console.WriteLine($"Cannot reject. Order status is {o.OrderStatus}.");
                        r.OrderQueue.Enqueue(o);
                    }
                }
                else if (action == "S")
                {
                    if (o.OrderStatus == Order.STATUS_CANCELLED)
                    {
                        Console.WriteLine($"Order {o.OrderId} skipped (Cancelled).");
                        // cancelled = move on (do not enqueue back)
                    }
                    else
                    {
                        Console.WriteLine($"Cannot skip. Order status is {o.OrderStatus} (must be Cancelled).");
                        r.OrderQueue.Enqueue(o);
                    }
                }
                else if (action == "D")
                {
                    if (o.OrderStatus == Order.STATUS_PREPARING)
                    {
                        o.OrderStatus = Order.STATUS_DELIVERED;
                        Console.WriteLine($"Order {o.OrderId} delivered. Status: {o.OrderStatus}");
                        // delivered = archived (do not enqueue back)
                    }
                    else
                    {
                        Console.WriteLine($"Cannot deliver. Order status is {o.OrderStatus} (must be Preparing).");
                        r.OrderQueue.Enqueue(o);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Order not processed.");
                    r.OrderQueue.Enqueue(o);
                }
            }
        }
    }
}

// TODO: Basic Features To Be Implemented

// Feature 3 : List all restaurants and menu items
// Hayder Zikry S10272315B


// Feature 5 : Create a new order
// Hayder Zikry S10272315B


// Feature 7 : Modify an existing order
// Hayder Zikry S10272315B


// Feature 8 : Delete an existing order
// Seah Hao Bin S10273358K
