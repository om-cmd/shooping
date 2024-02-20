//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

//namespace shooping.Models
//{
//    public class Customer
//    {
//        public int? CustomerID { get; set; }

//        [Required(ErrorMessage = "Name is required.")]
//        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Name should include alphabets only.")]   
//      public string Name { get; set; }


//      [Required(ErrorMessage = "Email address is required.")]
//      [EmailAddress(ErrorMessage = "Invalid email address.")]
//      [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address format.")]
//      public string Email { get; set; }
//  }
//}