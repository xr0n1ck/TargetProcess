using Newtonsoft.Json;

namespace Config.Model
{
	public class Server : Base
	{
		public Server(string id, string ip)
			: base(id)
		{
			Ip = ip;
		}

		[JsonProperty(PropertyName = "ip")]
		public string Ip { get; set; }

		[JsonProperty(PropertyName = "roles")]
		public string[] Roles { get; set; }

	}
}