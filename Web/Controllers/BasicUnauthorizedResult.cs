using System;
using System.Web.Mvc;

namespace Web.Controllers
{
	public class BasicUnauthorizedResult : HttpUnauthorizedResult
	{
		public BasicUnauthorizedResult()
		{
		}

		public BasicUnauthorizedResult(string statusDescription)
			: base(statusDescription)
		{
		}

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null) throw new ArgumentNullException("context");
			context.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic");
			base.ExecuteResult(context);
		}
	}
}