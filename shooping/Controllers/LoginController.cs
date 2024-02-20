using Domain;
using shooping.Helpers;
using ShoppingDTO.DTOMappers;
using ShoppingDTO.DTOModels;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace shooping.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }

        // GET: User
        [HttpPost]
        public ActionResult UserLogin(LoginModel loginModel)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    string hashedPassword = HashPassword(loginModel.Password);
                    var user = Db.Users.FirstOrDefault(u => u.UserName == loginModel.UserName && u.Password == hashedPassword);

                    if (user == null)
                    {
                        HttpContext.Response.StatusCode = 401;
                        return Json("Invalid Username and Password");
                    }
                    else
                    {
                        user.LoggedInDate = DateTime.Now;
                        user.LastRequestDate = DateTime.Now;
                        var token = CommonHelper.Base64Encode(user.GUID);
                        CommonHelper.SetSession(user.GUID, token);
                        

                        HttpContext.Response.AddHeader("X-Auth-Token", token);
                        return Json(new { Message = "Login successful", RedirectUrl = Url.Action("Index", "Home"), UserID = user.GUID, UserName = user.UserName });
                    }
                }
            }
            catch (Exception )
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public ActionResult Register(DTOUser newUser)
        {
            try
            {
                using (var Db = new shop_managementEntities())
                {
                    var user = Db.Users.Where(u => u.UserName == newUser.UserName || u.Email == newUser.Email).FirstOrDefault();

                    if (user != null)
                    {
                        HttpContext.Response.StatusCode = 401;
                        return Json("User Already Exists");
                    }

                    string hashedPassword = HashPassword(newUser.Password);

                    MapUser mapper = new MapUser();
                    Domain.User domainUser = mapper.MapModel(newUser, hashedPassword);


                    Db.Users.Add(domainUser);
                    Db.SaveChanges();

                    return Json(new { Message = "Registration successful", RedirectUrl = Url.Action("Index", "Login") });
                }
            }
            catch (Exception )
            {
                return new HttpStatusCodeResult(500);
            }
        }
        [HttpPost]
        public ActionResult UpdateLastRequest()
        {
            try
            {
                var userId = HttpContext.Session["UserID"] as Guid?;
                if (userId.HasValue)
                {
                    using (var Db = new shop_managementEntities())
                    {
                        var user = Db.Users.FirstOrDefault(u => u.GUID == userId);
                        if (user != null)
                        {
                            user.LastRequestDate = DateTime.Now;//yesle update garxa time lai 
                            Db.SaveChanges();

                            var inactivityTimeoutInHours = 2;
                            if (DateTime.Now.Subtract(user.LastRequestDate).TotalHours > inactivityTimeoutInHours)//yo check hanya time 
                            {
                                
                                CommonHelper.ClearSession();
                                user.LastRequestDate = DateTime.MinValue;//reste hanya time 
                                user.LoggedInDate = DateTime.MinValue;
                                Db.SaveChanges();

                                // Redirect the user to the login page
                                return Json(new { Message = "Session expired. Please login again.", RedirectUrl = Url.Action("Index", "Login") });
                            }

                            return Json(new { Message = "Last request date/time updated successfully" });
                        }
                    }
                }
                return Json(new { Message = "Failed to update last request date/time. User not found." });
            }
            catch (Exception )
            {
                return new HttpStatusCodeResult(500);
            }
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        [HttpPost]
        public ActionResult Logout()
        {
            try
            {
                CommonHelper.ClearSession();

                return Json(new { Message = "Logout successful", RedirectUrl = Url.Action("Index", "Login") });
            }
            catch (Exception )
            {
                return new HttpStatusCodeResult(500);
            }
        }
    }
}