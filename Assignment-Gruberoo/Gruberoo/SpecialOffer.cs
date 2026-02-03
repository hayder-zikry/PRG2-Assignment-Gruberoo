using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class SpecialOffer
    {
        // Attributes
        private string offerCode;
        private string offerDesc;
        private double? discount;   // Nullable to handle "-" case

        // Getters and Setters
        public string OfferCode
        {
            get { return offerCode; }
            set { offerCode = value; }
        }

        public string OfferDesc
        {
            get { return offerDesc; }
            set { offerDesc = value; }
        }

        public double? Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        // Constructor
        public SpecialOffer(string offerCode, string offerDesc, double discount)
        {
            this.offerCode = offerCode;
            this.offerDesc = offerDesc;
            this.discount = discount;
        }

        public SpecialOffer(string offerCode, string offerDesc, string discountText)
        {
            this.offerCode = offerCode;
            this.offerDesc = offerDesc;

            string text = discountText?.Trim();

            if (string.IsNullOrEmpty(text) || text == "-")
            {
                discount = null;
            }
            else if (double.TryParse(text, out double parsedDiscount))
            {
                discount = parsedDiscount;
            }
            else
            {
                discount = null;
            }
        }

        // Methods
        public override string ToString()
        {
            string discountDisplay = discount.HasValue ? $"{discount.Value:F2}%" : "-";
            return $"{offerCode} - {offerDesc} (Discount: {discountDisplay}";
        }
    }
}
