using System;
using System.Web;
using System.Web.Mvc;

namespace shooping
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var header = filterContext.HttpContext.Request.Headers["X-Auth-Token"];
            var sessionToken = HttpContext.Current.Session["Token"] as string ?? "";

            if (string.IsNullOrEmpty(sessionToken))
            {
                filterContext.Result = new RedirectResult("~/Login");
                return;
            }

            if (!IsValidToken(sessionToken))
            {
                filterContext.Result = new RedirectResult("~/Login");
                return;
            }

            UpdateLastRequestDateTime(sessionToken);
            // Check token expiration
            var loggedInDateTime = GetLoggedInDateTime(sessionToken);
            var authKeyExpireInHour = GetAuthKeyExpireInHour();
            if (loggedInDateTime.HasValue && DateTime.Now.Subtract(loggedInDateTime.Value).TotalHours > authKeyExpireInHour)
            {
                filterContext.Result = new RedirectResult("~/Login");
                return;
            }

            var lastRequestDateTime = GetLastRequestDateTime(sessionToken);
            var inactivityTimeoutInHours = 2;
            if (lastRequestDateTime.HasValue && DateTime.Now.Subtract(lastRequestDateTime.Value).TotalHours > inactivityTimeoutInHours)
            {
                // Log out the user
                HttpContext.Current.Session.Remove("Token");
                filterContext.Result = new RedirectResult("~/Login");
                return;
            }
        }
        private void UpdateLastRequestDateTime(string token)
        {
            HttpContext.Current.Session["LastRequestDateTime_" + token] = DateTime.Now;
        }

        private DateTime? GetLastRequestDateTime(string token)
        {
           
            if (HttpContext.Current.Session["LastRequestDateTime_" + token] is DateTime lastRequestDateTime)
            {
                return lastRequestDateTime;
            }
            return null;
        }

        private bool IsValidToken(string token)
        {
            return !string.IsNullOrEmpty(token); 
        }

        private DateTime? GetLoggedInDateTime(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null; 
            }
            else
            {
                DateTime? LoggedInDate = null; 
                return LoggedInDate;
            }
        }

        private int GetAuthKeyExpireInHour()
        {
            int defaultExpireInHour = 24; 
            return defaultExpireInHour;
        }
    }
}
