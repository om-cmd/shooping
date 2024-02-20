using System;
using System.Web;

namespace shooping.Helpers
{
    public static class CommonHelper
    {
        public static void SetSession(Guid UserID, string token)
        {
            HttpContext.Current.Session["UserId"] = UserID;
            HttpContext.Current.Session["Token"] = token;
        }
        public static bool CheckSession()
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                HttpContext.Current.Session.Abandon();
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void ClearSession()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
        public static void UpdateLastRequestDatee()
        {
            HttpContext.Current.Session["LastRequestDate"] = DateTime.Now;
        }

        public static bool IsLastRequestExpired()
        {
            if (HttpContext.Current.Session["LastRequestDate"] == null)
                return true;

            DateTime lastRequestDate = (DateTime)HttpContext.Current.Session["LastRequestDate"];
            return DateTime.Now.Subtract(lastRequestDate).TotalHours >= 2;
        }

        public static string Base64Encode(Guid UserID)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(UserID.ToString());
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

}