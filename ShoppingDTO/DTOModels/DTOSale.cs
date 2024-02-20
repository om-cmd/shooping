using System;

namespace ShoppingDTO.DTOModels
{
    public class DTOSale
    {
        public Guid SaleID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
       
        public Nullable<System.DateTime> SaleDate { get; set; }
        public Nullable<bool> InCash { get; set; }
        public Nullable<decimal> Ratings { get; set; }

        public DTOCustomerToken Customer { get; set; }

        public DTOProductToken Product { get; set; }


    }
    public class DTOCustomerToken
    {
        public Guid CustomerID { get; set; }
        public string Name { get; set; }
    }

    public class DTOProductToken
    {
        public Guid ProductID { get; set; }
        public string Name { get; set; }
    }
}
