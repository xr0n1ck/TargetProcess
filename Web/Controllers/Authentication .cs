using System;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TpRest;

namespace Web.Controllers
{
	public class BasicAuthentication : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null) throw new ArgumentNullException("filterContext");

			if (!Authenticate(filterContext.HttpContext))
			{
				filterContext.Result = new BasicUnauthorizedResult();
			}
			else
			{
				if (AuthorizeCore(filterContext.HttpContext))
				{
					var cachePolicy = filterContext.HttpContext.Response.Cache;
					cachePolicy.SetProxyMaxAge(new TimeSpan(0));
					cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
				}
				else
				{
					filterContext.Result = new BasicUnauthorizedResult();
				}
			}
		}

		private static bool Authenticate(HttpContextBase context)
		{
			if (!context.Request.Headers.AllKeys.Contains("Authorization")) return false;

			var authHeader = context.Request.Headers["Authorization"];

			IPrincipal principal;
			if (TryGetPrincipal(authHeader, out principal))
			{
				HttpContext.Current.User = principal;
				return true;
			}
			return false;
		}

		private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
		{
			validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
		}

		private static bool TryGetPrincipal(string authHeader, out IPrincipal principal)
		{
			var creds = ParseAuthHeader(authHeader);
			if (creds != null)
			{
				if (TryGetPrincipal(creds[0], creds[1], out principal)) return true;
			}

			principal = null;
			return false;
		}

		private static string[] ParseAuthHeader(string authHeader)
		{
			if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic")) return null;
			var base64Credentials = authHeader.Substring(6);
			var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials)).Split(new[] {':'});
			if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0]))
				return null;
			return credentials;
		}

		private static bool TryGetPrincipal(string userName, string password, out IPrincipal principal)
		{
			var auth = new AuthService();
			var email = auth.Auth(userName, password);
			if (email == null)
			{
				principal = null;
				return false;
			}
			principal = new GenericPrincipal(new GenericIdentity(email), new string[0]);
			return true;
		}
	}
}