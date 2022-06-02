using System;
using System.Collections.Generic;
using System.Text;

namespace funcformrecognizer
{
   public class InvoiceData
    {
      
            public string BillingAddress { get; set; }
            public string BillingAddressRecipient { get; set; }
            public string CustomerName { get; set; }
            public string DueDate { get; set; }
            public string InvoiceDate { get; set; }
            public string InvoiceId { get; set; }
            public double InvoiceTotal { get; set; }
            public string PurchaseOrder { get; set; }
            public string RemittanceAddressRecipient { get; set; }
            public string ShippingAddress { get; set; }
            public string ShippingAddressRecipient { get; set; }
            public string SubTotal { get; set; }
            public double TotalTax { get; set; }
            public string VendorAddress { get; set; }
            public string VendorName { get; set; }
 
        

    }

}
