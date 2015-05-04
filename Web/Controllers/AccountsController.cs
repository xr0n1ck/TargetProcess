using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Config.Dao;
using log4net;
using Config.Model;
namespace Web.Controllers
{
	public class AccountsController : BootstrapBaseController
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(AccountsController));

		private readonly AccountRepository _accountRepository = new AccountRepository();
        private readonly ServerRepository _serverRepository = new ServerRepository();

		[BasicAuthentication]
		public ActionResult Index(string filter = "")
		{
		   var accounts = _accountRepository.All();
           var servers = _serverRepository.All();
           IEnumerable<Server> serverlist = servers;
           ViewBag.Servers = serverlist;
           ViewBag.filter = filter;
           return View(accounts);
		}

		[BasicAuthentication]
		public ActionResult Tp3(string id)
		{
			Information("TP3 acess provided for " + id + ", button will appear in the next few minutes.");
			return RedirectToAction("Index");
		}

        [BasicAuthentication]
        [HttpPost]
        public ActionResult Change_db(string name, string db)
        {
            var account = _accountRepository.Set_db(name, db);
            return RedirectToAction("Index");
            //return account.Db;
        }
	}
}