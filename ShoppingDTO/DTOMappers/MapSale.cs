using Domain;
using System;
using System.Collections.Generic;

namespace ShoppingDTO.DTOMappers
{
    public class SaleDTOMapper
    {
        private readonly CustomerDTOMapper customerDTOMapper;
        private readonly ProductDTOMapper productDTOMapper;

        public SaleDTOMapper()
        {
            customerDTOMapper = new CustomerDTOMapper();
            productDTOMapper = new ProductDTOMapper();
        }

        public ShoppingDTO.DTOModels.DTOSale MapDTO(Domain.Sale sale)
        {
            return new ShoppingDTO.DTOModels.DTOSale
            {
                SaleID = sale.GUID,
                Quantity = sale.Quantity,
                TotalPrice = sale.TotalPrice,
                SaleDate = sale.SaleDate,
                Ratings = sale.Ratings,
                InCash = sale.InCash,
                Customer = MapCustomerToken(sale.Customer),
                Product = MapProductToken(sale.Product)
            };
        }

        public List<ShoppingDTO.DTOModels.DTOSale> MapDTOs(List<Domain.Sale> sales)
        {
            List<ShoppingDTO.DTOModels.DTOSale> dtoSales = new List<ShoppingDTO.DTOModels.DTOSale>();

            foreach (var sale in sales)
            {
                dtoSales.Add(MapDTO(sale));
            }

            return dtoSales;
        }

        private ShoppingDTO.DTOModels.DTOCustomerToken MapCustomerToken(Domain.Customer customer)
        {
            return new ShoppingDTO.DTOModels.DTOCustomerToken
            {
                CustomerID = customer.GUID,
                Name = customer.Name
            };
        }

        private ShoppingDTO.DTOModels.DTOProductToken MapProductToken(Domain.Product product)
        {
            return new ShoppingDTO.DTOModels.DTOProductToken
            {
                ProductID = product.GUID,
                Name = product.Name
            };
        }

       
        public Domain.Sale MapToDomainModel(ShoppingDTO.DTOModels.DTOSale dtoSale ,int saleID,int customerID , int productID )
        {
            
            return new Domain.Sale
            {
                SaleID = saleID,
                Quantity = dtoSale.Quantity,
                TotalPrice = dtoSale.TotalPrice,
                SaleDate = dtoSale.SaleDate,
                Ratings = dtoSale.Ratings,
                InCash = dtoSale.InCash,
                CustomerID = customerID, 
                ProductID = productID
            };
        }
       

    }

}
