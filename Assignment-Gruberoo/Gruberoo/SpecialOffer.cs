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
        private double discount;

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

        public double Discount
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
            
            if (discountText.Trim() == "-")
            {
                discount = -1;
            }
            else
            {
                discount = Convert.ToDouble(discountText);
            }
        }

        // Methods
        public override string ToString()
        {
            string discountDisplay;

            if (discount < 0)
            {
                discountDisplay = "-";
            }
            else
            {
                discountDisplay = $"{discount}%";
            }

            return $"{offerCode} - {offerDesc} (Discount: {discountDisplay})";
        }
    }
}
