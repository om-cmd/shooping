
using Domain;
using System;
using System.Linq;
using System.Web.Mvc;
using ShoppingDTO.DTOMappers;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using shooping.Helpers;
using NLog;

namespace shooping.Controllers
{
    public class CustomerController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private shop_managementEntities Db;

        public CustomerController()
        {
            Db = new shop_managementEntities();
        }

        ~CustomerController()
        {
            this.Db.Dispose();
        }
       
        public ActionResult Index()
        {
            if (!CommonHelper.CheckSession())
            {
                logger.Error("Invalid session detected in Index action");
                return RedirectToAction("Login", "LoginController"); // Redirect to login if session is not valid
            }
            return View();
        }

        [HttpGet]
        [Obsolete]
        [Authentication]
        public ActionResult GetCustomers(string nameSearch, DateTime? dateCreatedStart, DateTime? dateCreatedEnd, string sortOrder, string sortDir, int pageSize, int pageNumber)
        {
            try
            {
                JsonResult result = new JsonResult();
                using (var Db = new shop_managementEntities())
                {
                    IQueryable<Customer> customers = Db.Customers.Where(s => s.DateDeleted == null).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(nameSearch))
                    {
                        nameSearch = nameSearch.ToLower();
                        customers = customers.Where(c => c.Name.ToLower().Contains(nameSearch));
                    }

                    // Date Range
                    if (dateCreatedStart.HasValue && dateCreatedEnd.HasValue)
                    {
                        var fromdate = dateCreatedStart.Value.Date;               
                        var todate = dateCreatedEnd.Value.Date;
                        customers = customers.Where(c => EntityFunctions.TruncateTime(c.DateCreated) >= fromdate && EntityFunctions.TruncateTime(c.DateCreated) <= todate);
                    }
                   

                     List<string> sortorderList = new List<string>()
                    {
                        "Name",
                        "Email",
                        "IsActive",
                        "DateOfBirth",
                        "Ratings",
                        "Gender"
                    };

                    if ( string.IsNullOrEmpty(sortDir) || string.IsNullOrEmpty(sortOrder) || !sortorderList.Contains(sortOrder))
                    {
                        sortDir = "asc";
                        sortOrder = "Name";
                    }

                    switch (sortOrder.ToLowerInvariant())
                    {
                        case "name":
                            customers = sortDir == "asc" ? customers.OrderBy(c => c.Name) : customers.OrderByDescending(c => c.Name);
                            break;
                        case "email":
                            customers = sortDir == "asc" ? customers.OrderBy(c => c.Email) : customers.OrderByDescending(c => c.Email);
                            break;
                        case "isactive":
                            customers = sortDir == "asc" ? customers.OrderBy(c => c.IsActive) : customers.OrderByDescending(c => c.IsActive);
                            break;
                        case "dateofbirth":
                            customers = sortDir == "asc" ? customers.OrderBy(c => c.DateOfBirth) : customers.OrderByDescending(c => c.DateOfBirth);
                            break;
                        case "ratings":
                            customers = sortDir == "asc" ? customers.OrderBy(c => c.Ratings) : customers.OrderByDescending(c => c.Ratings);
                            break;
                        case "gender":
                            customers = sortDir == "asc" ? customers.OrderBy(c => c.Gender) : customers.OrderByDescending(c => c.Gender);
                            break;
                        default:
                            customers = customers.OrderBy(c => c.Name);
                            break;
                    }

                    if ((Convert.ToBoolean(pageSize) && Convert.ToBoolean(pageNumber) && pageSize <= 0 && pageNumber <= 0))
                    {
                        pageNumber = 1;
                        pageSize = 10;
                    }

                    int totalItems = customers.Count();
                    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    customers = customers.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

                    CustomerDTOMapper mapper = new CustomerDTOMapper();
                    List<ShoppingDTO.DTOModels.DTOCustomer> mappedCustomers = mapper.MapDTOs(customers.ToList());
                    HttpContext.Response.AddHeader("X-TotalItems", totalItems.ToString());
                    result = Json(mappedCustomers, JsonRequestBehavior.AllowGet);
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
        public ActionResult AddCustomer(ShoppingDTO.DTOModels.DTOCustomer newCustomer)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    if (newCustomer.CustomerID == Guid.Empty)
                    {

                        CustomerDTOMapper mapper = new CustomerDTOMapper();
                        Domain.Customer domainCustomer = mapper.MapToDomainModel(newCustomer);

                        Db.Customers.Add(domainCustomer); // Map DTO to Domain model
                        Db.SaveChanges();
                    }
                }

                return Json(newCustomer);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500); // Bad Request
            }
        }


        [HttpPut]
        [Authentication]
        public ActionResult UpdateCustomer(ShoppingDTO.DTOModels.DTOCustomer updatedCustomer)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    var customer = Db.Customers.SingleOrDefault(x => x.GUID == updatedCustomer.CustomerID);

                    if (customer != null)
                    {

                        CustomerDTOMapper mapper = new CustomerDTOMapper();
                        Domain.Customer domainCustomer = mapper.MapToDomainModel(updatedCustomer);

                        Db.SaveChanges();
                        return Json(updatedCustomer);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500); // Bad Request
            }
        }

        [HttpGet]
        [Authentication]
        public ActionResult GetCustomer(Guid CustomerID)
        {
            using (var Db = new shop_managementEntities())
            {
                var customer = Db.Customers.FirstOrDefault(c => c.GUID == CustomerID);

                if (customer == null)
                {
                    return HttpNotFound(); // Not Found
                }

                CustomerDTOMapper mapper = new CustomerDTOMapper();
                ShoppingDTO.DTOModels.DTOCustomer mappedCustomer = mapper.MapDTO(customer);

                return Json(mappedCustomer, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        [Authentication]
        public ActionResult DeleteCustomer(Guid CustomerID)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    var customer = Db.Customers.FirstOrDefault(c => c.GUID == CustomerID);

                    if (customer != null)
                    {
                        customer.DateDeleted = DateTime.Now;
                        customer.IsActive = false;
                        Db.SaveChanges();
                    }
                }

                return Json(new { success = true, message = "Customer deleted successfully" });
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }


    }
}

