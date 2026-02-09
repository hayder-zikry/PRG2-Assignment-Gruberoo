using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class Restaurant
    {
        // Attributes
        private string restaurantId;
        private string restaurantName;
        private string restaurantEmail;

        // Association
        private List<Menu> menus;
        private Queue<Order> orderQueue;
        private List<SpecialOffer> specialOffers;

        // Getters and Setters
        public string RestaurantId
        {
            get { return restaurantId; }
            set { restaurantId = value; }
        }

        public string RestaurantName
        {
            get { return restaurantName; }
            set { restaurantName = value; }
        }

        public string RestaurantEmail
        {
            get { return restaurantEmail; }
            set { restaurantEmail = value; }
        }

        public List<Menu> Menus
        {
            get { return menus; }
        }

        public Queue<Order> OrderQueue
        {
            get { return orderQueue; }
        }

        public List<SpecialOffer> SpecialOffers
        {
            get { return specialOffers; }
        }

        // Constructor
        public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
        {
            this.restaurantId = restaurantId;
            this.restaurantName = restaurantName;
            this.restaurantEmail = restaurantEmail;

            orderQueue = new Queue<Order>();
            menus = new List<Menu>();
            specialOffers = new List<SpecialOffer>();
        }

        // Methods
        public void DisplayOrders()
        {
            if (orderQueue.Count == 0)
            {
                Console.WriteLine("No orders in the queue.");
                return;
            }

            Console.WriteLine($"Orders Queue for {restaurantName} {restaurantId}");
            Console.WriteLine("===================================");

            int i = 1;
            foreach (Order o in orderQueue)
            {
                Console.WriteLine($"Order {i}: {o}");
                i++;
            }
        }

        public void AddToOrderQueue(Order order)
        {
            if (order != null) orderQueue.Enqueue(order);
        }

        public void DisplaySpecialOffers()
        {
            if (specialOffers.Count == 0)
            {
                Console.WriteLine("No special offers available.");
                return;
            }

            Console.WriteLine($"Special Offers for {restaurantName} {restaurantId}");
            Console.WriteLine("===================================");

            foreach (SpecialOffer offer in specialOffers)
            {
                Console.WriteLine(offer);
            }
        }

        public void DisplayMenu()
        {
            if (menus.Count == 0)
            {
                Console.WriteLine("No menus available.");
                return;
            }

            Console.WriteLine($"Menus for {restaurantName} {restaurantId}");
            Console.WriteLine("===================================");

            foreach (Menu m in menus)
            {
                Console.WriteLine(m);
                m.DisplayFoodItems();
                Console.WriteLine();
            }
        }

        public void AddMenu(Menu menu)
        {
            if (menu == null)
            {
                return;
            }

            foreach (Menu m in menus)
            {
                if (m.MenuId == menu.MenuId)
                {
                    return;
                }
            }

            menus.Add(menu);
        }

        public bool RemoveMenu(Menu menu)
        {
            if (menu == null)
            {
                return false;
            }
            return menus.Remove(menu);
        }

        public override string ToString()
        {
            return $"{restaurantId} - {restaurantName} ({restaurantEmail})";
        }
    }
}
