using System.Net;
using RestSharp;

namespace TpRest
{
	public class AuthService
	{
		public string Auth(string login, string password)
		{
			var client = new RestClient("https://sldemo.tpondemand.com")
			             	{
			             		Authenticator = new HttpBasicAuthenticator(login, password)
			             	};
			var request = new RestRequest("api/v1/Context", Method.POST);
			var response = client.Execute<Context>(request);
			return response.StatusCode != HttpStatusCode.OK ? null : response.Data.LoggedUser.Email;
		}
	}

	internal class Context
	{
		public LoggedUser LoggedUser { get; set; }
	}

	internal class LoggedUser
	{
		public string Email { get; set; }
	}
}