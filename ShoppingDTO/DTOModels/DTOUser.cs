using System;

namespace ShoppingDTO.DTOModels
{
    public class DTOUser
    {
        public Guid UserID { get; set; }
     
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CurrentToken { get; set; }

        public System.DateTime LastRequestDate { get; set; }
        public System.DateTime LoggedInDate { get; set; }
        public string Role { get; set; }
    }
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
