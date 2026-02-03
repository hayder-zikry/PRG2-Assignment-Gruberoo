using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class OrderedFoodItem : FoodItem
    {
        // Attributes
        private int qtyOrdered;
        private double subTotal;

        // Getters and Setters
        public int QtyOrdered
        {
            get { return qtyOrdered; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Quantity ordered must be greater than zero.");
                }
                else
                {
                    qtyOrdered = value;
                    subTotal = CalculateSubTotal();
                }
            }
        }
        public double SubTotal
        {
            get { return subTotal; }
        }

        // Constructor
        public OrderedFoodItem(FoodItem foodItem, int qtyOrdered) : base(foodItem.ItemName, foodItem.ItemDesc, foodItem.ItemPrice, foodItem.Customise)
        {
            this.qtyOrdered = qtyOrdered;
            this.subTotal = CalculateSubTotal();
        }

        // Methods
        public double CalculateSubTotal()
        {
            subTotal = ItemPrice * qtyOrdered;
            return subTotal;
        }

        public override string ToString()
        {
            return $"{ItemName} x {qtyOrdered} = ${subTotal:F2}";
        }
    }
}
