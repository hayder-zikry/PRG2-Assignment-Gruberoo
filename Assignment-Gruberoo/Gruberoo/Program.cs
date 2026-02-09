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

                if (customersByEmail.ContainsKey(customerEmail))
                {
                    customersByEmail[customerEmail].AddOrder(order);
                }

                orderRestaurantId[orderId] = restaurantId;

                if (restaurantsById.ContainsKey(restaurantId))
                {
                    restaurantsById[restaurantId].OrderQueue.Enqueue(order);
                }

            }
        }

        static void ListAllRestaurantsAndMenuItems()
        {
            // Feature 3 : List all restaurants and menu items

            Console.WriteLine();
            Console.WriteLine("All Restaurants and Menu Items");
            Console.WriteLine("==============================");
            int i = 1;

            foreach (Restaurant r in restaurantsById.Values)
            {
                if (i > 1)
                {
                    Console.WriteLine();
                }

                Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");

                foreach (Menu m in r.Menus)
                {
                    foreach (FoodItem fi in m.FoodItems)
                    {
                        Console.WriteLine($"    - {fi.ItemName}: {fi.ItemDesc} - ${fi.ItemPrice:0.00}");
                    }
                }
                i++;
            }
        }


        static void CreateNewOrder()
        {
            // Feature 5 : Create a new order

            Console.WriteLine();
            Console.WriteLine("Create New Order");
            Console.WriteLine("================");

            // 1) Customer Email
            Console.Write("Enter Customer Email: ");
            string customerEmail = Console.ReadLine().Trim();

            if (!customersByEmail.ContainsKey(customerEmail))
            {
                Console.WriteLine("Invalid Customer Email.");
                return;
            }

            // 2) Restaurant ID
            Console.Write("Enter Restaurant ID: ");
            string restaurantId = Console.ReadLine().Trim();

            if (!restaurantsById.ContainsKey(restaurantId))
            {
                Console.WriteLine("Invalid Restaurant ID.");
                return;
            }

            // 3) Delivery Date
            Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
            string deliveryDateText = Console.ReadLine().Trim();

            DateTime deliveryDate;
            if (!DateTime.TryParseExact(deliveryDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out deliveryDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            // 4) Delivery Time
            Console.Write("Enter Delivery Time (hh:mm): ");
            string deliveryTimeText = Console.ReadLine().Trim();

            DateTime deliveryTime;
            if (!DateTime.TryParseExact(deliveryTimeText, "HH:mm", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out deliveryTime))
            {
                Console.WriteLine("Invalid time format.");
                return;
            }

            DateTime deliveryDateTime = new DateTime(
                deliveryDate.Year, deliveryDate.Month, deliveryDate.Day,
                deliveryTime.Hour, deliveryTime.Minute, 0
            );

            // 5) Address
            Console.Write("Enter Delivery Address: ");
            string deliveryAddress = Console.ReadLine();

            // 6) New Order ID = max + 1
            int maxId = 0;
            foreach (Order o in allOrders)
            {
                if (o.OrderId > maxId) maxId = o.OrderId;
            }
            int newOrderId = maxId + 1;

            // Create order (status must be Pending)
            Order newOrder = new Order(newOrderId, deliveryDateTime, deliveryAddress);
            newOrder.OrderStatus = Order.STATUS_PENDING;

            // 7) Show list of available items for selected restaurant
            Restaurant r = restaurantsById[restaurantId];

            List<FoodItem> availableItems = new List<FoodItem>();
            foreach (Menu m in r.Menus)
            {
                foreach (FoodItem fi in m.FoodItems)
                {
                    availableItems.Add(fi);
                }
            }

            if (availableItems.Count == 0)
            {
                Console.WriteLine("No food items available for this restaurant.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Available Food Items:");
            for (int i = 0; i < availableItems.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableItems[i].ItemName} - ${availableItems[i].ItemPrice:0.00}");
            }

            // Track ordered items so items column can be made for CSV
            List<OrderedFoodItem> selectedOrderedItems = new List<OrderedFoodItem>();

            while (true)
            {
                Console.Write("Enter item number (0 to finish): ");
                string itemNoText = Console.ReadLine().Trim();

                int itemNo;
                if (!int.TryParse(itemNoText, out itemNo))
                {
                    Console.WriteLine("Please enter a valid number.");
                    continue;
                }

                if (itemNo == 0) break;

                if (itemNo < 1 || itemNo > availableItems.Count)
                {
                    Console.WriteLine("Invalid item number.");
                    continue;
                }

                Console.Write("Enter quantity: ");
                string qtyText = Console.ReadLine().Trim();

                int qty;
                if (!int.TryParse(qtyText, out qty) || qty <= 0)
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                FoodItem chosen = availableItems[itemNo - 1];
                OrderedFoodItem ofi = new OrderedFoodItem(chosen, qty);

                newOrder.AddOrderedFoodItem(ofi);
                selectedOrderedItems.Add(ofi);
            }

            if (selectedOrderedItems.Count == 0)
            {
                Console.WriteLine("No items selected. Order not created.");
                return;
            }

            // 8) Special request
            Console.Write("Add special request? [Y/N]: ");
            string sr = Console.ReadLine().Trim().ToUpper();

            if (sr == "Y")
            {
                Console.Write("Enter special request: ");
                string request = Console.ReadLine();

                foreach (OrderedFoodItem ofi in selectedOrderedItems)
                {
                    ofi.Customise = request;
                }
            }

            // 9) Calculate total
            double finalTotal = newOrder.CalculateOrderTotal();

            // For display, show items total and delivery
            const double DELIVERY_FEE = 5.00;
            double itemsTotal = Math.Round(finalTotal - DELIVERY_FEE, 2);
            Console.WriteLine();
            Console.WriteLine($"Order Total: ${itemsTotal:0.00} + ${DELIVERY_FEE:0.00} (delivery) = ${finalTotal:0.00}");

            // 10) Payment
            Console.Write("Proceed to payment? [Y/N]: ");
            string pay = Console.ReadLine().Trim().ToUpper();

            if (pay != "Y")
            {
                return; // exit feature
            }

            Console.WriteLine();
            Console.WriteLine("Payment method:");
            Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
            string method = Console.ReadLine().Trim().ToUpper();

            if (method != "CC" && method != "PP" && method != "CD")
            {
                Console.WriteLine("Invalid payment method.");
                return;
            }

            newOrder.OrderPaymentMethod = method;
            newOrder.OrderPaid = true;
            newOrder.OrderStatus = Order.STATUS_PENDING;

            // 11) Add to in-memory structures
            allOrders.Add(newOrder);
            customersByEmail[customerEmail].AddOrder(newOrder);
            restaurantsById[restaurantId].OrderQueue.Enqueue(newOrder);

            orderCustomerEmail[newOrderId] = customerEmail;
            orderRestaurantId[newOrderId] = restaurantId;

            // 12) Make Items text for CSV
            // E.g. "Chicken Rice, 2|Beef Burger, 1"
            string itemsText = "";
            for (int i = 0; i < selectedOrderedItems.Count; i++)
            {
                OrderedFoodItem ofi = selectedOrderedItems[i];
                if (i > 0) itemsText += "|";
                itemsText += $"{ofi.ItemName}, {ofi.QtyOrdered}";
            }
            itemsText = "\"" + itemsText + "\"";

            // 13) Append to orders.csv
            string path = "data/orders.csv";
            string deliveryDateOut = deliveryDateTime.ToString("dd/MM/yyyy");
            string deliveryTimeOut = deliveryDateTime.ToString("HH:mm");
            string createdOut = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string totalOut = finalTotal.ToString("0.00", CultureInfo.InvariantCulture);

            string line = $"{newOrderId},{customerEmail},{restaurantId},{deliveryDateOut},{deliveryTimeOut},{deliveryAddress},{createdOut},{totalOut},{newOrder.OrderStatus},{itemsText}";

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(line);
            }

            // 14) Confirmation
            Console.WriteLine();
            Console.WriteLine($"Order {newOrderId} created successfully! Status: {newOrder.OrderStatus}");
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

// Feature 7 : Modify an existing order
// Hayder Zikry S10272315B


// Feature 8 : Delete an existing order
// Seah Hao Bin S10273358K
