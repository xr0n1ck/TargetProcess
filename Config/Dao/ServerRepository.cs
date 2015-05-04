using System.Configuration;
using System.IO;
using System.Collections.Generic;
using Config.Model;


using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading;
using Config.Model;
using log4net;
using RestSharp;
using TpRest;

namespace Config.Dao
{
	public class ServerRepository
	{
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerRepository));
		private static readonly string ServersFolder =
			Path.Combine(Path.GetFullPath(ConfigurationManager.AppSettings["GitFolder"]), "servers");

		public Server GetServer(string id, bool fetch = true)
		{
			if (fetch)
				GitClient.Fetch();
			var serverFile = Path.Combine(ServersFolder, id + ".json.cfg");
			return Base.FromJson<Server>(File.ReadAllText(serverFile));
		}

        public IEnumerable<Server> All()
        {
            GitClient.Fetch();
            var serverFiles = Directory.GetFiles(ServersFolder);
            return serverFiles.Select(serverFile =>
            {
                try
                {
                    var server = Base.FromJson<Server>(File.ReadAllText(serverFile));
                    server.Id = ParseId(serverFile);
                    return server;
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Fail to parse {0} server config file {1}", serverFile, e);
                    return null;
                }
            }).Where(a => a != null);
        }

        private static string ParseId(string serverFile)
        {
            return new FileInfo(serverFile).Name.Replace(".json.cfg", "");
        }
	}
}