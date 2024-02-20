using System;

namespace ShoppingDTO.DTOModels
{
    public class DTOProduct
    {
        public Guid ProductID { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<byte> Ratings { get; set; }
        public Nullable<System.DateTime> ExpDate { get; set; }
        public Nullable<bool> IsGrocery { get; set; }
       
    }
   
}
