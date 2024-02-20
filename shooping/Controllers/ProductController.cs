

using Domain;
using shooping.Helpers;
using ShoppingDTO.DTOMappers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using shooping;


namespace shopping.Controllers
{
    public class ProductController : Controller
    {
        private shop_managementEntities Db;

        public ProductController()
        {
            Db = new shop_managementEntities();
        }

        ~ProductController()
        {
            this.Db.Dispose();
        }

        public ActionResult Index()
        {
            if (!CommonHelper.CheckSession())
            {
                return RedirectToAction("Login", "LoginController"); // Redirect to login if session is not valid
            }
            return View();
        }

        [HttpGet]
        [Authentication]
        public ActionResult GetProducts(string nameSearch, DateTime? dateCreatedStart, DateTime? dateCreatedEnd, string sortOrder, string sortDir, int pageSize, int pageNumber)
        {
            try
            {
                JsonResult result = new JsonResult();
                using (var Db = new shop_managementEntities())
                {
                    IQueryable<Product> products = Db.Products.Where(x => x.IsGrocery == true).AsQueryable();

                    // Search by product name
                    if (!string.IsNullOrWhiteSpace(nameSearch))
                    {
                        nameSearch = nameSearch.ToLower();
                        products = products.Where(p => p.Name.ToLower().Contains(nameSearch));
                    }

                    // Search by creation date
                    if (dateCreatedStart.HasValue && dateCreatedEnd.HasValue)
                    {
                        var fromdate = dateCreatedStart.Value.Date;
                        var todate = dateCreatedEnd.Value.Date;
                        products = products.Where(c => DbFunctions.TruncateTime(c.DateCreated) >= fromdate && DbFunctions.TruncateTime(c.DateCreated) <= todate);
                    }

                    List<string> sortorderList = new List<string>()
                    {
                        "Name",
                        "Ratings",
                        "IsGrocery",
                        "Price",
                        "ExpDate"
                    };

                    if (string.IsNullOrEmpty(sortDir) || string.IsNullOrEmpty(sortOrder) || !sortorderList.Contains(sortOrder))
                    {
                        sortDir = "asc";
                        sortOrder = "Name";
                    }

                    // for debugging
                    switch (sortOrder.ToLowerInvariant())
                    {
                        case "Name":
                            products = sortDir == "asc" ? products.OrderBy(p => p.Name) : products.OrderByDescending(p => p.Name);
                            break;
                        case "Ratings":
                            products = sortDir == "asc" ? products.OrderBy(p => p.Ratings) : products.OrderByDescending(p => p.Ratings);
                            break;
                        case "IsGrocery":
                            products = sortDir == "asc" ? products.OrderBy(p => p.IsGrocery) : products.OrderByDescending(p => p.IsGrocery);
                            break;
                        case "Price":
                            products = sortDir == "asc" ? products.OrderBy(p => p.Price) : products.OrderByDescending(p => p.Price);
                            break;
                        case "ExpDate":
                            products = sortDir == "asc" ? products.OrderBy(p => p.ExpDate) : products.OrderByDescending(p => p.ExpDate);
                            break;
                        default:
                            products = products.OrderBy(p => p.Name);
                            break;
                    }

                    if (pageSize <= 0 || pageNumber <= 0)
                    {
                        pageNumber = 1;
                        pageSize = 10;
                    }

                    int totalItems = products.Count();
                    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    products = products.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

                    ProductDTOMapper mapper = new ProductDTOMapper();
                    List<ShoppingDTO.DTOModels.DTOProduct> mappedProducts = mapper.MapDTOs(products.ToList());
                    HttpContext.Response.AddHeader("X-TotalItems", totalItems.ToString());

                    // Return the mapped DTOs as JSON
                    result = Json(mappedProducts, JsonRequestBehavior.AllowGet);
                }

                return result;
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(400);
            }
        }

        [HttpPost]
        [Authentication]
        public ActionResult AddProduct(ShoppingDTO.DTOModels.DTOProduct newProduct)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    if (newProduct.ProductID == Guid.Empty)
                    {
                        ProductDTOMapper mapper = new ProductDTOMapper();
                        Domain.Product domainProduct = mapper.MapToDomainModel(newProduct);

                        Db.Products.Add(domainProduct); // Map DTO to Domain model
                        Db.SaveChanges();
                    }

                    return Json(newProduct);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(400);
            }
        }

      

        [HttpPut]
        [Authentication]
        public ActionResult UpdateProduct(ShoppingDTO.DTOModels.DTOProduct updatedProduct)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    var product = Db.Products.SingleOrDefault(x => x.GUID == updatedProduct.ProductID);

                    if (product != null)
                    {
                        ProductDTOMapper mapper = new ProductDTOMapper();
                        Domain.Product domainProduct = mapper.MapToDomainModel(updatedProduct);

                        Db.SaveChanges();

                        return Json(updatedProduct);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(400);
            }
        }

        [HttpGet]
        [Authentication]
        public ActionResult GetProduct(Guid ProductID)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    var product = Db.Products.FirstOrDefault(c => c.GUID == ProductID);

                    if (product == null)
                    {
                        return HttpNotFound();
                    }

                    ProductDTOMapper mapper = new ProductDTOMapper();
                    ShoppingDTO.DTOModels.DTOProduct mappedProduct = mapper.MapDTO(product);

                    return Json(mappedProduct, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception )
            {
                return new HttpStatusCodeResult(400);
            }
        }

        [HttpDelete]
        [Authentication]
        public ActionResult DeleteProduct(Guid ProductID)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    var product = Db.Products.FirstOrDefault(c => c.GUID == ProductID);

                    if (product != null)
                    {
                        product.DateDeleted = DateTime.Now;
                        product.IsGrocery = false;
                        Db.SaveChanges();
                    }
                }

                return Json(new { success = true, message = "Product deleted successfully" });
            }
            catch (Exception )
            {
                return new HttpStatusCodeResult(400);
            }
        }
    }
}
