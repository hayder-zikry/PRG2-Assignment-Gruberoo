using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace Gruberoo
{
    internal class Program
    {
        // lookup tables for quick access
        static Dictionary<string, Customer> customersByEmail = new Dictionary<string, Customer>();
        static Dictionary<string, Restaurant> restaurantsById = new Dictionary<string, Restaurant>();

        // orders list
        static List<Order> allOrders = new List<Order>();

        static Dictionary<int, List<OrderedFoodItem>> orderItemsById = new Dictionary<int, List<OrderedFoodItem>>();

        static Dictionary<int, string> orderCustomerEmail = new Dictionary<int, string>();
        static Dictionary<int, string> orderRestaurantId = new Dictionary<int, string>();

        // tracking refund for cancelled/rejected orders
        static Stack<Order> refundStack = new Stack<Order>();

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

            RunMainMenu();
        }

        static void RunMainMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("===== Gruberoo Food Delivery System =====");
                Console.WriteLine("1. List all restaurants and menu items");
                Console.WriteLine("2. List all orders");
                Console.WriteLine("3. Create a new order");
                Console.WriteLine("4. Process an order");
                Console.WriteLine("5. Modify an existing order");
                Console.WriteLine("6. Delete an existing order");
                Console.WriteLine("7. Bulk process pending orders for a current day");
                Console.WriteLine("8. Display total order amount");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                string input = (Console.ReadLine() ?? "").Trim();

                // validate user input for main menu
                if (!int.TryParse(input, out int option))
                {
                    Console.WriteLine("Invalid option. Please enter a number.");
                    continue;
                }

                if (option == 0)
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }
                else if (option == 1)
                {
                    ListAllRestaurantsAndMenuItems();   // Feature 3
                }
                else if (option == 2)
                {
                    ListAllOrdersBasicInfo();           // Feature 4
                }
                else if (option == 3)
                {
                    CreateNewOrder();                   // Feature 5
                }
                else if (option == 4)
                {
                    ProcessOrder();                     // Feature 6
                }
                else if (option == 5)
                {
                    ModifyOrder();                      // Feature 7
                }
                else if (option == 6)
                {
                    DeleteOrder();                      // Feature 8
                }
                else if (option == 7)
                {
                    BulkProcessPendingOrdersForDay();   // Advanced Feature A
                }
                else if (option == 8)
                {
                    DisplayTotalOrderAmount();          // Advanced Feature B
                }
                else
                {
                    Console.WriteLine("Invalid option. Please choose from the menu.");
                }
            }
        }



        // Student 1: Hayder Zikry S10272315B
        // Features: 2, 3, 5, 7

        // === Feature Methods ===
        // Feature 2: LoadCustomers(), LoadOrders()
        // Feature 3: ListAllRestaurantsAndMenuItems()
        // Feature 5: CreateNewOrder()
        // Feature 7: ModifyOrder()
        // Advanced Feature (A): BulkProcessPendingOrdersForDay()
        static void LoadCustomers()
        {
            // Feature 2 : Load files (customers and orders)
            // Hayder Zikry S10272315B

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
            // Hayder Zikry S10272315B

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

                string itemsText = "";

                // parse fixed columns first since items in order.csv may have commas
                int orderId = int.Parse(parts[0].Trim());
                string customerEmail = parts[1].Trim();
                string restaurantId = parts[2].Trim();
                string deliveryDate = parts[3].Trim();
                string deliveryTime = parts[4].Trim();
                string deliveryAddress = parts[5].Trim();
                string createdDateTime = parts[6].Trim();
                double totalAmount = double.Parse(parts[7].Trim());
                string status = parts[8].Trim();

                // rejoin items columns
                if (parts.Length >= 10)
                {
                    itemsText = string.Join(",", parts, 9, parts.Length - 9).Trim();

                    if (itemsText.StartsWith("\"") && itemsText.EndsWith("\""))
                        itemsText = itemsText.Substring(1, itemsText.Length - 2);
                }


                List<OrderedFoodItem> loadedItems = new List<OrderedFoodItem>();

                // parse items with | (e.g. "Beef Burger, 1|Grilled Steak, 1")
                if (!string.IsNullOrWhiteSpace(itemsText))
                {
                    string[] itemChunks = itemsText.Split('|');

                    foreach (string chunk in itemChunks)
                    {
                        string[] pair = chunk.Split(',');

                        if (pair.Length < 2) continue;

                        string itemName = pair[0].Trim();
                        int qty;
                        if (!int.TryParse(pair[1].Trim(), out qty)) continue;

                        // find the item price from the restaurant menu
                        double price = 0;
                        string desc = "-";

                        if (restaurantsById.ContainsKey(restaurantId))
                        {
                            Restaurant rr = restaurantsById[restaurantId];
                            foreach (Menu mm in rr.Menus)
                            {
                                foreach (FoodItem fi in mm.FoodItems)
                                {
                                    if (fi.ItemName.Equals(itemName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        price = fi.ItemPrice;
                                        desc = fi.ItemDesc;
                                        break;
                                    }
                                }
                            }
                        }

                        FoodItem temp = new FoodItem(itemName, desc, price, "-");
                        OrderedFoodItem ofi = new OrderedFoodItem(temp, qty);
                        loadedItems.Add(ofi);
                    }
                }

                // create datetime values
                DateTime deliveryDateTimeObj = DateTime.ParseExact(deliveryDate + " " + deliveryTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                DateTime createdDateTimeObj = DateTime.ParseExact(createdDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                Order order = new Order(orderId, createdDateTimeObj, deliveryDateTimeObj, deliveryAddress, totalAmount, status);

                // attach items into the Order object so DisplayOrderedFoodItems() works
                if (loadedItems.Count > 0)
                {
                    foreach (OrderedFoodItem ofi in loadedItems)
                    {
                        order.OrderedFoodItems.Add(ofi);
                    }
                }

                allOrders.Add(order);

                // save relationships for later lookups
                orderCustomerEmail[orderId] = customerEmail;
                orderRestaurantId[orderId] = restaurantId;

                // attach order to customer
                if (customersByEmail.ContainsKey(customerEmail))
                {
                    customersByEmail[customerEmail].AddOrder(order);
                }

                // add only active orders to restaurant menu
                if (restaurantsById.ContainsKey(restaurantId))
                {
                    string statusClean = order.OrderStatus.Trim();

                    if (statusClean == Order.STATUS_PENDING ||
                        statusClean == Order.STATUS_CANCELLED ||
                        statusClean == Order.STATUS_PREPARING)
                    {
                        restaurantsById[restaurantId].OrderQueue.Enqueue(order);
                    }
                }
            }
        }

        static void ListAllRestaurantsAndMenuItems()
        {
            // Feature 3 : List all restaurants and menu items
            // Hayder Zikry S10272315B

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

                // print all menu items for each restaurant
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
            // Hayder Zikry S10272315B

            Console.WriteLine();
            Console.WriteLine("Create New Order");
            Console.WriteLine("================");

            // get valid customer email
            string customerEmail;
            while (true)
            {
                Console.Write("Enter Customer Email (0 to cancel): ");
                customerEmail = Console.ReadLine().Trim();

                if (customerEmail == "0") return;

                if (!customersByEmail.ContainsKey(customerEmail))
                {
                    Console.WriteLine("Invalid Customer Email. Try again.");
                    continue;
                }
                break;
            }

            // get valid restaurant id
            string restaurantId;
            while (true)
            {
                Console.Write("Enter Restaurant ID (0 to cancel): ");
                restaurantId = Console.ReadLine().Trim();

                if (restaurantId == "0") return;

                if (!restaurantsById.ContainsKey(restaurantId))
                {
                    Console.WriteLine("Invalid Restaurant ID. Try again.");
                    continue;
                }
                break;
            }

            // get valid delivery date
            DateTime deliveryDate;
            while (true)
            {
                Console.Write("Enter Delivery Date (dd/mm/yyyy) (0 to cancel): ");
                string deliveryDateText = Console.ReadLine().Trim();

                if (deliveryDateText == "0") return;

                if (!DateTime.TryParseExact(deliveryDateText, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out deliveryDate))
                {
                    Console.WriteLine("Invalid date format. Try again.");
                    continue;
                }
                break;
            }

            // get valid delivery time
            DateTime deliveryTime;
            while (true)
            {
                Console.Write("Enter Delivery Time (hh:mm) (0 to cancel): ");
                string deliveryTimeText = Console.ReadLine().Trim();

                if (deliveryTimeText == "0") return;

                if (!DateTime.TryParseExact(deliveryTimeText, "HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out deliveryTime))
                {
                    Console.WriteLine("Invalid time format. Try again.");
                    continue;
                }
                break;
            }

            DateTime deliveryDateTime = new DateTime(
                deliveryDate.Year, deliveryDate.Month, deliveryDate.Day,
                deliveryTime.Hour, deliveryTime.Minute, 0
            );

            // get an address while not allowing null
            string deliveryAddress;
            while (true)
            {
                Console.Write("Enter Delivery Address (0 to cancel): ");
                deliveryAddress = Console.ReadLine();

                if (deliveryAddress.Trim() == "0") return;

                if (string.IsNullOrWhiteSpace(deliveryAddress))
                {
                    Console.WriteLine("Address cannot be empty. Try again.");
                    continue;
                }
                break;
            }

            // generate next order id
            int maxId = 0;
            foreach (Order o in allOrders)
                if (o.OrderId > maxId) maxId = o.OrderId;

            int newOrderId = maxId + 1;

            // create order
            Order newOrder = new Order(newOrderId, deliveryDateTime, deliveryAddress);
            newOrder.OrderStatus = Order.STATUS_PENDING;

            // display list of food items for the selected restaurant for customer convenience
            Restaurant r = restaurantsById[restaurantId];
            List<FoodItem> availableItems = new List<FoodItem>();
            foreach (Menu m in r.Menus)
                foreach (FoodItem fi in m.FoodItems)
                    availableItems.Add(fi);

            if (availableItems.Count == 0)
            {
                Console.WriteLine("No food items available for this restaurant.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Available Food Items:");
            for (int i = 0; i < availableItems.Count; i++)
                Console.WriteLine($"{i + 1}. {availableItems[i].ItemName} - ${availableItems[i].ItemPrice:0.00}");

            // collect ordered items
            List<OrderedFoodItem> selectedOrderedItems = new List<OrderedFoodItem>();

            while (true)
            {
                Console.Write("Enter item number (0 to finish, -1 to restart): ");
                string itemNoText = Console.ReadLine().Trim();

                int itemNo;
                if (!int.TryParse(itemNoText, out itemNo))
                {
                    Console.WriteLine("Please enter a valid number.");
                    continue;
                }

                if (itemNo == 0) break;

                if (itemNo == -1)
                {
                    // restart item selection
                    selectedOrderedItems.Clear();
                    newOrder.OrderedFoodItems.Clear();
                    Console.WriteLine("Item selection restarted.");
                    continue;
                }

                if (itemNo < 1 || itemNo > availableItems.Count)
                {
                    Console.WriteLine("Invalid item number.");
                    continue;
                }

                // get valid quantity
                int qty;
                while (true)
                {
                    Console.Write("Enter quantity: ");
                    string qtyText = Console.ReadLine().Trim();

                    if (!int.TryParse(qtyText, out qty) || qty <= 0)
                    {
                        Console.WriteLine("Invalid quantity. Try again.");
                        continue;
                    }
                    break;
                }

                FoodItem chosen = availableItems[itemNo - 1];
                OrderedFoodItem ofi = new OrderedFoodItem(chosen, qty);

                newOrder.AddOrderedFoodItem(ofi);
                selectedOrderedItems.Add(ofi);

                Console.WriteLine($"Added: {ofi.ItemName} - {ofi.QtyOrdered}");
            }

            if (selectedOrderedItems.Count == 0)
            {
                Console.WriteLine("No items selected. Order not created.");
                return;
            }

            // special request option
            while (true)
            {
                Console.Write("Add special request? [Y/N]: ");
                string sr = Console.ReadLine().Trim().ToUpper();

                if (sr == "N") break;
                if (sr != "Y")
                {
                    Console.WriteLine("Invalid input. Enter Y or N.");
                    continue;
                }

                Console.Write("Enter special request: ");
                string request = Console.ReadLine();

                foreach (OrderedFoodItem ofi in selectedOrderedItems)
                    ofi.Customise = request;

                break;
            }

            // calculate total and show order summary and total
            double finalTotal = newOrder.CalculateOrderTotal();
            const double DELIVERY_FEE = 5.00;
            double itemsTotal = Math.Round(finalTotal - DELIVERY_FEE, 2);

            Console.WriteLine();
            Console.WriteLine("Your Order Summary:");
            newOrder.DisplayOrderedFoodItems();
            Console.WriteLine($"Order Total: ${itemsTotal:0.00} + ${DELIVERY_FEE:0.00} (delivery) = ${finalTotal:0.00}");

            // payment confirmation
            while (true)
            {
                Console.Write("Proceed to payment? [Y/N]: ");
                string pay = Console.ReadLine().Trim().ToUpper();

                if (pay == "N") return;
                if (pay != "Y")
                {
                    Console.WriteLine("Invalid input. Enter Y or N.");
                    continue;
                }
                break;
            }

            // get a valid payment method (CC/PP/CD)
            string method;
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Payment method:");
                Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
                method = Console.ReadLine().Trim().ToUpper();

                if (method != "CC" && method != "PP" && method != "CD")
                {
                    Console.WriteLine("Invalid payment method. Try again.");
                    continue;
                }
                break;
            }

            newOrder.OrderPaymentMethod = method;
            newOrder.OrderPaid = true;
            newOrder.OrderStatus = Order.STATUS_PENDING;

            // add order to collections
            allOrders.Add(newOrder);
            customersByEmail[customerEmail].AddOrder(newOrder);
            restaurantsById[restaurantId].OrderQueue.Enqueue(newOrder);

            orderCustomerEmail[newOrderId] = customerEmail;
            orderRestaurantId[newOrderId] = restaurantId;

            // create items string for CSV output
            string itemsText = "";
            for (int i = 0; i < selectedOrderedItems.Count; i++)
            {
                OrderedFoodItem ofi = selectedOrderedItems[i];
                if (i > 0) itemsText += "|";
                itemsText += $"{ofi.ItemName}, {ofi.QtyOrdered}";
            }
            itemsText = "\"" + itemsText + "\"";

            // append the new order to the orders.csv
            string path = "data/orders.csv";
            string deliveryDateOut = deliveryDateTime.ToString("dd/MM/yyyy");
            string deliveryTimeOut = deliveryDateTime.ToString("HH:mm");
            string createdOut = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string totalOut = finalTotal.ToString("0.00", CultureInfo.InvariantCulture);

            string line = $"{newOrderId},{customerEmail},{restaurantId},{deliveryDateOut},{deliveryTimeOut},{deliveryAddress},{createdOut},{totalOut},{newOrder.OrderStatus},{itemsText}";
            using (StreamWriter sw = new StreamWriter(path, true))
                sw.WriteLine(line);

            // Confirmation
            Console.WriteLine();
            Console.WriteLine($"Order {newOrderId} created successfully! Status: {newOrder.OrderStatus}");
        }

        static void ModifyOrder()
        {
            // Feature 7 : Modify an existing order
            // Hayder Zikry S10272315B

            Console.WriteLine();
            Console.WriteLine("Modify Order");
            Console.WriteLine("============");

            // validate customer email
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();

            if (!customersByEmail.ContainsKey(email))
            {
                Console.WriteLine("Invalid customer email.");
                return;
            }

            // get pending orders from customer
            List<Order> pendingOrders = new List<Order>();
            foreach (Order o in allOrders)
            {
                if (orderCustomerEmail.ContainsKey(o.OrderId) &&
                    orderCustomerEmail[o.OrderId] == email &&
                    o.OrderStatus == Order.STATUS_PENDING)
                {
                    pendingOrders.Add(o);
                }
            }

            Console.WriteLine("Pending Orders:");
            if (pendingOrders.Count == 0)
            {
                Console.WriteLine("(none)");
                return;
            }

            foreach (Order o in pendingOrders)
            {
                Console.WriteLine(o.OrderId);
            }

            // validate order id selection
            Console.Write("Enter Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid Order ID.");
                return;
            }

            Order selected = null;
            foreach (Order o in pendingOrders)
            {
                if (o.OrderId == orderId)
                {
                    selected = o;
                    break;
                }
            }

            if (selected == null)
            {
                Console.WriteLine("Order ID not found.");
                return;
            }

            // display order details
            Console.WriteLine("Order Items:");
            selected.DisplayOrderedFoodItems();

            Console.WriteLine("Address:");
            Console.WriteLine(selected.DeliveryAddress);

            Console.WriteLine("Delivery Date/Time:");
            Console.WriteLine(selected.DeliveryDateTime.ToString("dd/MM/yyyy, HH:mm"));

            // validate modification choice
            Console.WriteLine();
            string choice;
            while (true)
            {
                Console.Write("Modify: [1] Items [2] Address [3] Delivery Time (0 to cancel): ");
                choice = Console.ReadLine().Trim();

                if (choice == "0") return;

                if (choice == "1" || choice == "2" || choice == "3")
                    break;

                Console.WriteLine("Invalid option. Please enter 1, 2, 3 or 0.");
            }


            double oldTotal = selected.OrderTotal;

            // update delivery address
            if (choice == "2")
            {
                string newAddr;
                while (true)
                {
                    Console.Write("Enter new Address: ");
                    newAddr = Console.ReadLine() ?? "";

                    if (string.IsNullOrWhiteSpace(newAddr))
                    {
                        Console.WriteLine("Address cannot be empty.");
                        continue;
                    }

                    break;
                }

                selected.DeliveryAddress = newAddr;
                Console.WriteLine($"Order {orderId} updated. New Address applied.");
            }


            // update delivery time
            else if (choice == "3")
            {
                Console.Write("Enter new Delivery Time (hh:mm): ");
                string timeText = Console.ReadLine().Trim();

                if (!DateTime.TryParseExact(timeText, "HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newTime))
                {
                    Console.WriteLine("Invalid time format.");
                    return;
                }

                DateTime oldDT = selected.DeliveryDateTime;
                selected.DeliveryDateTime = new DateTime(
                    oldDT.Year, oldDT.Month, oldDT.Day,
                    newTime.Hour, newTime.Minute, 0
                );

                Console.WriteLine();
                Console.WriteLine($"Order {orderId} updated. New Delivery Time: {timeText}");
            }

            // update ordered items
            else if (choice == "1")
            {
                // get restaurantId for this order
                if (!orderRestaurantId.ContainsKey(orderId))
                {
                    Console.WriteLine("Restaurant for this order not found.");
                    return;
                }

                string rid = orderRestaurantId[orderId];
                if (!restaurantsById.ContainsKey(rid))
                {
                    Console.WriteLine("Restaurant not found.");
                    return;
                }

                Restaurant rr = restaurantsById[rid];

                // display food item list for the restaurant
                List<FoodItem> availableItems = new List<FoodItem>();
                foreach (Menu m in rr.Menus)
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

                // clear previous items from list
                selected.OrderedFoodItems.Clear();

                // add new items to list
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

                    selected.OrderedFoodItems.Add(ofi);
                }

                if (selected.OrderedFoodItems.Count == 0)
                {
                    Console.WriteLine("No items selected. Changes not applied.");
                    return;
                }

                Console.WriteLine($"Items updated for Order {orderId}.");
            }


            // recalculate the total and request for payment if total increased from previous total
            double newTotal = selected.CalculateOrderTotal();

            if (newTotal > oldTotal)
            {
                Console.Write($"Order total increased from ${oldTotal:0.00} to ${newTotal:0.00}. Proceed to payment? [Y/N]: ");
                if (Console.ReadLine().Trim().ToUpper() != "Y")
                {
                    Console.WriteLine("Payment cancelled. Changes not applied.");
                    return;
                }
                selected.OrderPaid = true;
            }

            // display full updated order details after modifying
            Console.WriteLine();
            Console.WriteLine("Updated Order Details:");
            Console.WriteLine("Order Items:");
            selected.DisplayOrderedFoodItems();
            Console.WriteLine("Address:");
            Console.WriteLine(selected.DeliveryAddress);
            Console.WriteLine("Delivery Date/Time:");
            Console.WriteLine(selected.DeliveryDateTime.ToString("dd/MM/yyyy, HH:mm"));
            Console.WriteLine($"Total Amount: ${selected.OrderTotal:0.00}");
        }

        static void BulkProcessPendingOrdersForDay()
        {
            // Advanced Feature (A) : Bulk processing of unprocessed orders for a current day
            // Hayder Zikry S10272315B

            Console.WriteLine();
            Console.WriteLine("Bulk Processing - Pending Orders (Chosen Day)");
            Console.WriteLine("=============================================");

            // ask for the target date for bulk processing
            DateTime targetDate;
            while (true)
            {
                Console.Write("Enter date to bulk process (dd/MM/yyyy) (0 to cancel): ");
                string dateText = (Console.ReadLine() ?? "").Trim();

                if (dateText == "0") return;

                if (!DateTime.TryParseExact(dateText, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out targetDate))
                {
                    Console.WriteLine("Invalid date format. Try again.");
                    continue;
                }
                break;
            }

            int totalPendingInQueues = 0;

            // count the no. of pending orders in queues for that day
            foreach (Restaurant r in restaurantsById.Values)
            {
                foreach (Order o in r.OrderQueue)
                {
                    if (o.OrderStatus == Order.STATUS_PENDING && o.DeliveryDateTime.Date == targetDate.Date)
                    {
                        totalPendingInQueues++;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Total Pending orders in queues for {targetDate:dd/MM/yyyy}: {totalPendingInQueues}");

            if (totalPendingInQueues == 0)
            {
                Console.WriteLine("No pending orders to bulk process for this day.");
                return;
            }

            int processedCount = 0;
            int preparingCount = 0;
            int rejectedCount = 0;

            DateTime now = DateTime.Now;

            foreach (Restaurant r in restaurantsById.Values)
            {
                int qCount = r.OrderQueue.Count;

                for (int i = 0; i < qCount; i++)
                {
                    Order o = r.OrderQueue.Dequeue();

                    bool isPendingTargetDay =
                        (o.OrderStatus == Order.STATUS_PENDING) &&
                        (o.DeliveryDateTime.Date == targetDate.Date);

                    if (isPendingTargetDay)
                    {
                        processedCount++;

                        // reject if delivery time is within 1hr from now
                        double minsToDelivery = (o.DeliveryDateTime - now).TotalMinutes;

                        if (minsToDelivery < 60)
                        {
                            o.OrderStatus = Order.STATUS_REJECTED;
                            rejectedCount++;

                            refundStack.Push(o);
                        }
                        else
                        {
                            o.OrderStatus = Order.STATUS_PREPARING;
                            preparingCount++;
                            r.OrderQueue.Enqueue(o);
                        }
                    }
                    else
                    {
                        r.OrderQueue.Enqueue(o);
                    }
                }
            }

            // calculate percentage of orders auto-processed against all orders
            double percentage = 0;
            if (allOrders.Count > 0)
                percentage = (processedCount * 100.0) / allOrders.Count;

            Console.WriteLine();
            Console.WriteLine("Summary Statistics");
            Console.WriteLine("------------------");
            Console.WriteLine($"Number of orders processed: {processedCount}");
            Console.WriteLine($"Preparing: {preparingCount}");
            Console.WriteLine($"Rejected: {rejectedCount}");
            Console.WriteLine($"Percentage auto-processed vs all orders: {percentage:0.00}%");
        }


        // Student 2: Seah Hao Bin S10273358K
        // Features: 1, 4, 6, 8

        // === Feature Methods ===
        // Feature 1: LoadRestaurantsAndFoodItems()
        // Feature 4: ListAllOrdersBasicInfo()
        // Feature 6: ProcessOrder()
        // Feature 8: DeleteOrder()

        static void LoadRestaurantsAndFoodItems()
        {
            // Feature 1 : Load files (restaurants and food items)
            // Seah Hao Bin S10273358K

            restaurantsById.Clear();
            foodItemsLoaded = 0;

            // Load restaurants.csv and create Restaurant objects
            string rPath = "data/restaurants.csv";
            if (!File.Exists(rPath))
            {
                Console.WriteLine("Restaurants file not found.");
                return;
            }

            string[] rLines = File.ReadAllLines(rPath);

            for (int i = 1; i < rLines.Length; i++) // skip header
            {
                if (string.IsNullOrWhiteSpace(rLines[i])) continue;

                string[] parts = rLines[i].Split(',');
                if (parts.Length < 3) continue;

                string rid = parts[0].Trim();
                string name = parts[1].Trim();
                string email = parts[2].Trim();

                // Each restaurant starts with one default menu
                Restaurant r = new Restaurant(rid, name, email);
                Menu m = new Menu(rid + "-M1", "Main Menu");
                r.AddMenu(m);

                restaurantsById[rid] = r;
            }

            // Load fooditems.csv and add each FoodItem into the restaurant's menu
            string fPath = "data/fooditems.csv";
            if (!File.Exists(fPath))
            {
                Console.WriteLine("Food items file not found.");
                return;
            }

            string[] fLines = File.ReadAllLines(fPath);

            for (int i = 1; i < fLines.Length; i++) // skip header
            {
                if (string.IsNullOrWhiteSpace(fLines[i])) continue;

                string[] parts = fLines[i].Split(',');
                if (parts.Length < 4) continue;

                string rid = parts[0].Trim();
                string itemName = parts[1].Trim();

                // Last column = price, middle columns = description (may contain commas)
                string priceText = parts[parts.Length - 1].Trim();

                string itemDesc = "";
                for (int j = 2; j < parts.Length - 1; j++)
                {
                    if (j > 2) itemDesc += ",";
                    itemDesc += parts[j];
                }
                itemDesc = itemDesc.Trim();

                double price;
                if (!double.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                    continue;

                // Add item into the first menu created earlier
                if (restaurantsById.ContainsKey(rid))
                {
                    FoodItem fi = new FoodItem(itemName, itemDesc, price, "-");
                    restaurantsById[rid].Menus[0].AddFoodItem(fi);
                    foodItemsLoaded++;
                }
            }
        }

        static void ListAllOrdersBasicInfo()
        {
            // Feature 4 : List all orders with basic information
            // Seah Hao Bin S10273358K

            if (allOrders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }

            // Table header (basic fields)
            Console.WriteLine("All Orders");
            Console.WriteLine("==========");

            Console.WriteLine("{0,-11}{1,-15}{2,-17}{3,-21}{4,-11}{5,-12}",
                "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
            Console.WriteLine("{0,-11}{1,-15}{2,-17}{3,-21}{4,-11}{5,-12}",
                "--------", "----------", "-------------", "------------------", "------", "---------");

            foreach (Order o in allOrders)
            {
                // Map order -> customer name
                string custName = "Unknown";
                if (orderCustomerEmail.ContainsKey(o.OrderId))
                {
                    string email = orderCustomerEmail[o.OrderId];
                    if (customersByEmail.ContainsKey(email))
                        custName = customersByEmail[email].CustomerName;
                }

                // Map order -> restaurant name
                string restName = "Unknown";
                if (orderRestaurantId.ContainsKey(o.OrderId))
                {
                    string rid = orderRestaurantId[o.OrderId];
                    if (restaurantsById.ContainsKey(rid))
                        restName = restaurantsById[rid].RestaurantName;
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

        static void ProcessOrder()
        {
            // Feature 6 : Process an order
            // Seah Hao Bin S10273358K

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

            // Loop through the queue once (avoid infinite loop if re-enqueue)
            int count = r.OrderQueue.Count;

            for (int i = 0; i < count; i++)
            {
                Order o = r.OrderQueue.Dequeue();

                string statusClean = o.OrderStatus.Trim();

                // Customer name (using mapping)
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
                        Console.WriteLine();
                        Console.WriteLine($"Order {o.OrderId} confirmed. Status: {o.OrderStatus}");
                        r.OrderQueue.Enqueue(o); // still active
                    }
                    else
                    {
                        Console.WriteLine();
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
                        Console.WriteLine();
                        Console.WriteLine($"Order {o.OrderId} rejected. Status: {o.OrderStatus}");
                        Console.WriteLine($"Refund required for Order {o.OrderId}.");
                        // rejected = archived (do not enqueue back)
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Cannot reject. Order status is {o.OrderStatus}.");
                        r.OrderQueue.Enqueue(o);
                    }
                }
                else if (action == "S")
                {
                    if (o.OrderStatus == Order.STATUS_CANCELLED)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Order {o.OrderId} skipped (Cancelled).");
                        // cancelled = move on (do not enqueue back)
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Cannot skip. Order status is {o.OrderStatus} (must be Cancelled).");
                        r.OrderQueue.Enqueue(o);
                    }
                }
                else if (action == "D")
                {
                    if (o.OrderStatus == Order.STATUS_PREPARING)
                    {
                        o.OrderStatus = Order.STATUS_DELIVERED;
                        Console.WriteLine();
                        Console.WriteLine($"Order {o.OrderId} delivered. Status: {o.OrderStatus}");
                        // delivered = archived (do not enqueue back)
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Cannot deliver. Order status is {o.OrderStatus} (must be Preparing).");
                        r.OrderQueue.Enqueue(o);
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid input. Order not processed.");
                    r.OrderQueue.Enqueue(o);
                }
            }
        }

        static void DeleteOrder()
        {
            // Feature 8 : Delete an existing order
            // Seah Hao Bin S10273358K

            Console.WriteLine();
            Console.WriteLine("Delete Order");
            Console.WriteLine("============");

            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();

            if (!customersByEmail.ContainsKey(email))
            {
                Console.WriteLine("Invalid customer email.");
                return;
            }

            // Collect pending orders for this customer
            List<Order> pendingOrders = new List<Order>();

            foreach (Order o in allOrders)
            {
                if (orderCustomerEmail.ContainsKey(o.OrderId) &&
                    orderCustomerEmail[o.OrderId] == email &&
                    o.OrderStatus == Order.STATUS_PENDING)
                {
                    pendingOrders.Add(o);
                }
            }

            Console.WriteLine("Pending Orders:");
            if (pendingOrders.Count == 0)
            {
                Console.WriteLine("(none)");
                return;
            }

            foreach (Order o in pendingOrders)
            {
                Console.WriteLine(o.OrderId);
            }

            Console.Write("Enter Order ID: ");
            string input = Console.ReadLine().Trim();

            int orderId;
            if (!int.TryParse(input, out orderId))
            {
                Console.WriteLine("Invalid Order ID.");
                return;
            }

            // Find the selected pending order
            Order selected = null;
            foreach (Order o in pendingOrders)
            {
                if (o.OrderId == orderId)
                {
                    selected = o;
                    break;
                }
            }

            if (selected == null)
            {
                Console.WriteLine("Order ID not found in pending orders.");
                return;
            }

            // Display basic order information (like sample)
            string custName = customersByEmail[email].CustomerName;

            Console.WriteLine();
            Console.WriteLine($"Customer: {custName}");
            Console.WriteLine("Ordered Items:");
            selected.DisplayOrderedFoodItems();
            Console.WriteLine($"Delivery date/time: {selected.DeliveryDateTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Total Amount: ${selected.OrderTotal:0.00}");
            Console.WriteLine($"Order Status: {selected.OrderStatus}");

            Console.Write("Confirm deletion? [Y/N]: ");
            string ans = Console.ReadLine().Trim().ToUpper();

            if (ans != "Y")
            {
                Console.WriteLine("Deletion cancelled.");
                return;
            }

            // Update status + refund stack + message
            selected.OrderStatus = Order.STATUS_CANCELLED;
            refundStack.Push(selected);

            Console.WriteLine();
            Console.WriteLine($"Order {selected.OrderId} cancelled. Refund of ${selected.OrderTotal:0.00} processed.");
        }

        static void DisplayTotalOrderAmount()
        {
            // Advanced Feature (B) : Display total order amount
            // Seah Hao Bin S10273358K

            const double DELIVERY_FEE = 5.00;
            const double COMMISSION_RATE = 0.30;

            Console.WriteLine();
            Console.WriteLine("Total Order Amount");
            Console.WriteLine("==================");

            double grandDeliveredNet = 0;
            double grandRefunds = 0;

            // Make output order stable (R001, R002, ...)
            List<Restaurant> sortedRestaurants = new List<Restaurant>(restaurantsById.Values);
            sortedRestaurants.Sort((a, b) => string.Compare(a.RestaurantId, b.RestaurantId, StringComparison.Ordinal));

            foreach (Restaurant r in sortedRestaurants)
            {
                double deliveredNet = 0;
                double refunds = 0;

                // Scan all orders and group by restaurant
                foreach (Order o in allOrders)
                {
                    if (!orderRestaurantId.ContainsKey(o.OrderId))
                        continue;

                    if (orderRestaurantId[o.OrderId] != r.RestaurantId)
                        continue;

                    string status = (o.OrderStatus ?? "").Trim();

                    // Successful orders
                    if (status == Order.STATUS_DELIVERED)
                    {
                        double net = o.OrderTotal - DELIVERY_FEE;   // less delivery fee per order
                        if (net < 0) net = 0;
                        deliveredNet += net;
                    }
                    // Refunded orders (Rejected / Cancelled)
                    else if (status == Order.STATUS_REJECTED || status == Order.STATUS_CANCELLED)
                    {
                        refunds += o.OrderTotal;
                    }
                }

                grandDeliveredNet += deliveredNet;
                grandRefunds += refunds;

                Console.WriteLine();
                Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");
                Console.WriteLine($"Total Order Amount (less delivery fee): ${deliveredNet:0.00}");
                Console.WriteLine($"Total Refunds: ${refunds:0.00}");
            }

            double gruberooEarns = (grandDeliveredNet * COMMISSION_RATE) - grandRefunds;

            Console.WriteLine();
            Console.WriteLine("Overall Summary");
            Console.WriteLine("===============");
            Console.WriteLine($"Total Order Amount (less delivery fee): ${grandDeliveredNet:0.00}");
            Console.WriteLine($"Total Refunds: ${grandRefunds:0.00}");
            Console.WriteLine($"Final Amount Gruberoo Earns: ${gruberooEarns:0.00}");

        }
    }
}