using System.Web.Mvc;
using System.Web;
using NLog;

namespace shooping.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {

            ViewBag.Message = "nada nigga";
            return View();
        }

        public ActionResult Error()

        {
            ViewBag.Message = string.Empty;
            return View();

        }


    }
}
