using Domain;
using ShoppingDTO.DTOModels;
using System;
using System.Collections.Generic;

namespace ShoppingDTO.DTOMappers
{
    public class CustomerDTOMapper
    {
        public ShoppingDTO.DTOModels.DTOCustomer MapDTO(Domain.Customer customer)
        {
            return new ShoppingDTO.DTOModels.DTOCustomer
            {
                CustomerID = customer.GUID,
                Name = customer.Name,
                Email = customer.Email,
                IsActive = customer.IsActive,
                Ratings = customer.Ratings,
                DateOfBirth = customer.DateOfBirth,
                Gender = customer.Gender
            };
        }

        public List<ShoppingDTO.DTOModels.DTOCustomer> MapDTOs(List<Domain.Customer> customers)
        {
            List<ShoppingDTO.DTOModels.DTOCustomer> dtoCustomers = new List<ShoppingDTO.DTOModels.DTOCustomer>();

            foreach (var customer in customers)
            {
                dtoCustomers.Add(MapDTO(customer));
            }

            return dtoCustomers;
        }

        public Domain.Customer MapToDomainModel(ShoppingDTO.DTOModels.DTOCustomer dtoCustomer)
        {
            return new Domain.Customer
            {
                GUID = dtoCustomer.CustomerID, // This line seems to be an error, it should be dtoCustomer.CustomerID
                Name = dtoCustomer.Name,
                Email = dtoCustomer.Email,
                IsActive = dtoCustomer.IsActive,
                Ratings = dtoCustomer.Ratings,
                DateOfBirth = dtoCustomer.DateOfBirth,
                Gender = dtoCustomer.Gender,
            };
        }

    }
}
