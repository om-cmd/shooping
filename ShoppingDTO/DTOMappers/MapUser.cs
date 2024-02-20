using ShoppingDTO.DTOModels;
using System;

namespace ShoppingDTO.DTOMappers
{
    public class MapUser
    {
        public Domain.User MapModel(DTOUser user , string hashedPassword)
        {

            return new Domain.User()
            {
               
                UserName = user.UserName,
                DateCreated = DateTime.Now,
                LoggedInDate = DateTime.Now,
                LastRequestDate = DateTime.Now,
                Email = user.Email,
                Password = hashedPassword,
                CurrentToken = user.CurrentToken,
                Role = "ADMIN"
            };
        }

    }
}

