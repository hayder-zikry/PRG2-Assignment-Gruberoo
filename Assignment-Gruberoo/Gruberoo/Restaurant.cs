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
        private Menu menu;
        private List<Order> orders;
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

        public Menu Menu
        {
            get { return menu; }
            set { menu = value; }
        }

        public List<Order> Orders
        {
            get { return orders; }
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

            orders = new List<Order>();
            specialOffers = new List<SpecialOffer>();
        }

        // Methods
        public void DisplayOrders()
        {
            foreach (Order o in orders)
            {
                Console.WriteLine(o);
            }
        }

        public void DisplaySpecialOffers()
        {
            foreach (SpecialOffer so in specialOffers)
            {
                Console.WriteLine(so);
            }
        }

        public void DisplayMenu()
        {
            if (menu != null)
            {
                menu.DisplayFoodItems();
            }
        }

        public void AddMenu(Menu menu)
        {
            this.menu = menu;
        }

        public bool RemoveMenu(Menu menu)
        {
            if (this.menu == menu)
            {
                this.menu = null;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{restaurantName} ({restaurantId}) | Email: {restaurantEmail}";
        }
    }
}
