using System.Web.Mvc;

namespace Web.Controllers
{
	public class HomeController : BootstrapBaseController
	{
		public ActionResult Index()
		{
			return RedirectToAction("Index", "Accounts");
		}
	}
}