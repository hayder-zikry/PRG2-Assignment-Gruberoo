using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class FoodItem
    {
        // Attributes
        private string itemName;
        private string itemDesc;
        private double itemPrice;
        private string customise;

        // Getters and Setters
        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }
        public string ItemDesc
        {
            get { return itemDesc; }
            set { itemDesc = value; }
        }
        public double ItemPrice
        {
            get { return itemPrice; }
            set { itemPrice = value; }
        }
        public string Customise
        {
            get { return customise; }
            set { customise = value; }
        }

        // Constructor
        public FoodItem(string itemName, string itemDesc, double itemPrice, string customise)
        {
            this.itemName = itemName;
            this.itemDesc = itemDesc;
            this.itemPrice = itemPrice;
            this.customise = "-";
        }

        // Methods
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(customise) || customise == "-")
                return $"{itemName} : {itemDesc} - ${itemPrice:F2}";
            else
                return $"{itemName} : {itemDesc} - ${itemPrice:F2} | Customisation: {customise}";

        }

    }
}
