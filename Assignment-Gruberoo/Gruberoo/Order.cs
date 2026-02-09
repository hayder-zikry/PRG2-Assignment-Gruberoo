using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class Order
    {
        // Attributes
        private int orderId;
        private DateTime orderDateTime;
        private double orderTotal;
        private string orderStatus;
        private DateTime deliveryDateTime;
        private string deliveryAddress;
        private string orderPaymentMethod;
        private bool orderPaid;

        // Association
        private List<OrderedFoodItem> orderedFoodItems = new List<OrderedFoodItem>();
        private SpecialOffer? specialOffer;

        // Status Constants
        public const string STATUS_PENDING = "Pending";
        public const string STATUS_PREPARING = "Preparing";
        public const string STATUS_DELIVERED = "Delivered";
        public const string STATUS_CANCELLED = "Cancelled";
        public const string STATUS_REJECTED = "Rejected";

        // Getters and Setters
        public int OrderId
        {
            get { return orderId; }
        }
        public DateTime OrderDateTime
        {
            get { return orderDateTime; }
        }
        public double OrderTotal
        {
            get { return orderTotal; }
        }
        public string OrderStatus
        {
            get { return orderStatus; }
            set { orderStatus = value; }
        }
        public DateTime DeliveryDateTime
        {
            get { return deliveryDateTime; }
            set { deliveryDateTime = value; }
        }
        public string DeliveryAddress
        {
            get { return deliveryAddress; }
            set { deliveryAddress = value; }
        }
        public string OrderPaymentMethod
        {
            get { return orderPaymentMethod; }
            set { orderPaymentMethod = value; }
        }
        public bool OrderPaid
        {
            get { return orderPaid; }
            set { orderPaid = value; }
        }
        public List<OrderedFoodItem> OrderedFoodItems
        {
            get { return orderedFoodItems; }
        }
        public SpecialOffer? SpecialOffer
        {
            get { return specialOffer; }
            set { specialOffer = value; }
        }


        // Constructor
        public Order(int orderId, DateTime deliveryDateTime, string deliveryAddress)
        {
            this.orderId = orderId;
            this.orderDateTime = DateTime.Now;
            this.deliveryDateTime = deliveryDateTime;
            this.deliveryAddress = deliveryAddress;
            this.orderStatus = STATUS_PENDING;
            this.orderPaymentMethod = "";
            this.orderPaid = false;
            this.orderTotal = 0;
        }
        public Order(int orderId, DateTime createdDateTime, DateTime deliveryDateTime, string deliveryAddress, double orderTotal, string orderStatus)
        {
            this.orderId = orderId;
            this.orderDateTime = createdDateTime;
            this.deliveryDateTime = deliveryDateTime;
            this.deliveryAddress = deliveryAddress;
            this.orderTotal = orderTotal;
            this.orderStatus = orderStatus;
            this.orderPaymentMethod = "";
            this.orderPaid = false;
        }


        // Methods
        public double CalculateOrderTotal()
        {
            double total = 0;
            
            foreach (OrderedFoodItem ofi in orderedFoodItems)
            {
                total += ofi.CalculateSubTotal();
            }

            if (specialOffer != null && specialOffer.Discount.HasValue && specialOffer.Discount.Value > 0)
            {
                total = total * (1 - (specialOffer.Discount.Value / 100.0));
            }

            const double DELIVERY_FEE = 5.00;
            total += DELIVERY_FEE;

            orderTotal = Math.Round(total, 2);
            return orderTotal;
        }
        public void AddOrderedFoodItem(OrderedFoodItem orderedFoodItem)
        {
            if (orderedFoodItem == null)
            {
                return;
            }
            orderedFoodItems.Add(orderedFoodItem);
            CalculateOrderTotal();
        }
        public bool RemoveOrderedFoodItem(OrderedFoodItem orderedFoodItem)
        {
            bool removed = orderedFoodItems.Remove(orderedFoodItem);
            if (removed)
            {
                CalculateOrderTotal();
            }
            return removed;
        }
        public void DisplayOrderedFoodItems()
        {
            if (orderedFoodItems.Count == 0)
            {
                Console.WriteLine("No ordered food items.");
                return;
            }

            int i = 1;
            foreach (OrderedFoodItem ofi in orderedFoodItems)
            {
                Console.WriteLine($"{i}. {ofi}");
                i++;
            }
        }
        public override string ToString()
        {
            return $"Order {orderId} | Delivery: {deliveryDateTime:dd/MM/yyyy HH:mm} | Total: ${orderTotal:F2} | Status: {orderStatus}";
        }
    }
}
