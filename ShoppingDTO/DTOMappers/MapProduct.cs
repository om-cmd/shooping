using Domain;
using ShoppingDTO.DTOModels;
using System;
using System.Collections.Generic;

namespace ShoppingDTO.DTOMappers
{
    public class ProductDTOMapper
    {
        public ShoppingDTO.DTOModels.DTOProduct MapDTO(Domain.Product product )
        {
            return new ShoppingDTO.DTOModels.DTOProduct
            {
                ProductID = product.GUID,
                Name = product.Name,
                Price = product.Price,
                IsGrocery = product.IsGrocery,
                Ratings = product.Ratings,
                ExpDate = product.ExpDate
            };
        }
        public List<ShoppingDTO.DTOModels.DTOProduct> MapDTOs(List<Domain.Product> products)
        {
            List<ShoppingDTO.DTOModels.DTOProduct> dtoProducts = new List<ShoppingDTO.DTOModels.DTOProduct>();

            foreach (var product in products)
            {
                dtoProducts.Add(MapDTO(product));
            }
            return dtoProducts;
        }

        public Domain.Product MapToDomainModel(ShoppingDTO.DTOModels.DTOProduct dtoProduct)
        {
            return new Domain.Product
            {
                GUID = dtoProduct.ProductID,
                Name = dtoProduct.Name,
                Price = dtoProduct.Price,
                IsGrocery = dtoProduct.IsGrocery,
                Ratings = dtoProduct.Ratings,
                ExpDate = dtoProduct.ExpDate
            };
        }

    }
}
