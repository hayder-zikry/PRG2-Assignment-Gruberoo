using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class Menu
    {
        // Attributes
        private string menuId;
        private string menuName;

        // Association
        private List<FoodItem> foodItems;

        // Getters and Setters
        public string MenuId
        {
            get { return menuId; }
            set { menuId = value; }
        }

        public string MenuName
        {
            get { return menuName; }
            set { menuName = value; }
        }

        public List<FoodItem> FoodItems
        {
            get { return foodItems; }
        }

        // Constructor
        public Menu(string menuId, string menuName)
        {
            this.menuId = menuId;
            this.menuName = menuName;
            foodItems = new List<FoodItem>();
        }

        // Methods
        public void AddFoodItem(FoodItem foodItem)
        {
            if (foodItem != null)
            {
                foodItems.Add(foodItem);
            }
        }

        public bool RemoveFoodItem(FoodItem foodItem)
        {
            return foodItems.Remove(foodItem);
        }

        public void DisplayFoodItems()
        {
            foreach (FoodItem fi in foodItems)
            {
                Console.WriteLine(fi);
            }
        }

        public override string ToString()
        {
            return $"{menuName} ({menuId}) | Items: {foodItems.Count}";
        }
    }
}
