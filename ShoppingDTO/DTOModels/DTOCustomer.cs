using System;

namespace ShoppingDTO.DTOModels
{
    public  class DTOCustomer
    {
        public Guid CustomerID { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<byte> Ratings { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
       
        
        
    }
    
}
