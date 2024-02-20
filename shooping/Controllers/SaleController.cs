using Domain; 
using System;
using System.Linq;
using System.Web.Mvc;
using ShoppingDTO.DTOMappers;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using shopping.Controllers;
using System.Data.Entity.Migrations;
using shooping.Helpers;
using System.Net;

namespace shooping.Controllers
{
    public class SaleController : Controller

    {
        private shop_managementEntities Db;
        private CustomerController customerController;
        private ProductController productController;
        public SaleController()
        {
            customerController = new CustomerController();
            productController = new ProductController();
            Db = new shop_managementEntities();
        }
        ~SaleController()
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
        [Obsolete]
        [Authentication]
        public ActionResult GetSales(string nameSearch, DateTime? dateCreatedStart, DateTime? dateCreatedEnd, string sortOrder, string sortDir, int pageSize, int pageNumber)
        {

            try
            {
                JsonResult result = new JsonResult();
                using (var Db = new shop_managementEntities())
                {
                    IQueryable<Sale> sales = Db.Sales.Include("Customer").Include("Product").Where(s => s.DateDeleted == null).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(nameSearch))
                    {
                        nameSearch = nameSearch.ToLower();
                        sales = sales.Where(s => s.Customer.Name.ToLower().Contains(nameSearch)|| s.Product.Name.ToLower().Contains(nameSearch));
                    }


                    if (dateCreatedStart.HasValue && dateCreatedEnd.HasValue)
                    {
                        var fromdate = dateCreatedStart.Value.Date;
                        var todate = dateCreatedEnd.Value.Date;
                        sales = sales.Where(s => EntityFunctions.TruncateTime(s.DateCreated) >= fromdate && EntityFunctions.TruncateTime(s.DateCreated) <= todate);
                    }



                    List<string> sortorderList = new List<string>()
                {
                    "CustomerName",
                    "ProductName",
                    "Ratings",
                    "InCash",
                    "Quantity",
                    "SaleDate",
                    "TotalPrice"
                };

                    if (string.IsNullOrEmpty(sortDir) || string.IsNullOrEmpty(sortOrder) || !sortorderList.Contains(sortOrder))
                    {
                        sortDir = "asc";
                        sortOrder = "CustomerName";
                    }
                    // for debugging
                    switch (sortOrder.ToLowerInvariant())
                    {
                        case "customername":
                            sales = sortDir == "asc" ? sales.OrderBy(s => s.Customer.Name) : sales.OrderByDescending(s => s.Customer.Name);
                            break;
                        case "productname":
                            sales = sortDir == "asc" ? sales.OrderBy(s => s.Product.Name) : sales.OrderByDescending(s => s.Product.Name);
                            break;
                        case "ratings":
                            sales = sortDir == "asc" ? sales.OrderBy(s => s.Ratings) : sales.OrderByDescending(s => s.Ratings);
                            break;
                        case "inCash":
                            sales = sortDir == "asc" ? sales.OrderBy(s => s.InCash) : sales.OrderByDescending(s => s.InCash);
                            break;
                        case "totalprice":
                            sales = sortDir == "asc" ? sales.OrderBy(s => s.TotalPrice) : sales.OrderByDescending(s => s.TotalPrice);
                            break;
                        case "quantity":
                            sales = sortDir == "asc" ? sales.OrderBy(s => s.Quantity) : sales.OrderByDescending(s => s.Quantity);
                            break;
                        case "saledate":
                            sales = sortDir == "asc" ? sales.OrderBy(s => s.SaleDate) : sales.OrderByDescending(p => p.SaleDate);
                            break;
                        default:
                            sales = sales.OrderBy(s => s.Ratings);
                            break;
                    }


                    if ((Convert.ToBoolean(pageSize) && Convert.ToBoolean(pageNumber) && pageSize <= 0 && pageNumber <= 0))
                    {
                        pageNumber = 1;
                        pageSize = 10;
                    }
                    int totalItems = sales.Count();
                    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    sales = sales.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

                    SaleDTOMapper mapper = new SaleDTOMapper();
                    List<ShoppingDTO.DTOModels.DTOSale> mappedSales = mapper.MapDTOs(sales.ToList());
                    HttpContext.Response.AddHeader("X-TotalItems", totalItems.ToString());
                    // Return the mapped DTOs as JSON
                    result = Json(mappedSales, JsonRequestBehavior.AllowGet);

                }
                return result;
            }
            catch (Exception )
            {
                return new HttpStatusCodeResult(500);
            }

        }
        [HttpPost]
        [Authentication]
        public ActionResult AddSale(ShoppingDTO.DTOModels.DTOSale newSale)
        {
            try
            {
                using (var Db = new shop_managementEntities()) // Using statement to ensure proper disposal
                {
                    if (newSale.SaleID == Guid.Empty)

                    {
                        int customerID = Db.Customers.Where(c => c.GUID == newSale.Customer.CustomerID).Select(c => c.CustomerID).FirstOrDefault();
                        int productID = Db.Products.Where(p => p.GUID == newSale.Product.ProductID).Select(c => c.ProductID).FirstOrDefault();
                        Domain.Sale saleAdd = new SaleDTOMapper().MapToDomainModel(newSale, 0, customerID, productID);

                        Db.Sales.Add(saleAdd);
                        Db.SaveChanges();


                    }


                    return Json(newSale);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }


        [HttpPut]
        [Authentication]
        public ActionResult UpdateSale(ShoppingDTO.DTOModels.DTOSale updatedSale)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    var sale = Db.Sales.FirstOrDefault(s => s.GUID == updatedSale.SaleID);

                    if (sale != null)
                    {
                        // Map the DTO properties to the existing entity
                        int customerID = Db.Customers.Where(c => c.GUID == updatedSale.Customer.CustomerID).Select(c => c.CustomerID).FirstOrDefault();
                        int productID = Db.Products.Where(p => p.GUID == updatedSale.Product.ProductID).Select(c => c.ProductID).FirstOrDefault();
                        Domain.Sale saleUpdate = new SaleDTOMapper().MapToDomainModel(updatedSale, sale.SaleID , customerID, productID);


                        Db.Sales.AddOrUpdate(saleUpdate);
                        Db.SaveChanges();

                        return Json(updatedSale);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpGet]
        [Authentication ]
        public ActionResult GetSale(Guid SaleID)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    var sale = Db.Sales.FirstOrDefault(s => s.GUID == SaleID);

                    if (sale == null)
                    {
                        return HttpNotFound();
                    }

                    SaleDTOMapper mapper = new SaleDTOMapper();
                    ShoppingDTO.DTOModels.DTOSale mappedSale = mapper.MapDTO(sale);

                    return Json(mappedSale, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpDelete]
        [Authentication]
        public ActionResult DeleteSale(Guid SaleID)
        {
            try
            {

                using (var Db = new shop_managementEntities())
                {
                    var sale = Db.Sales.FirstOrDefault(s => s.GUID == SaleID);

                    if (sale != null)
                    {
                        sale.DateDeleted = DateTime.Now;

                        Db.Sales.Remove(sale);
                        Db.SaveChanges();
                    }
                }
                return Json(new { success = true, message = "Customer deleted successfully" });
            }
            catch (Exception )
            {
                return new HttpStatusCodeResult(500);
            }
        }
    }
}