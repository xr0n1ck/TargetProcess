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
	public class AccountRepository
	{

		private static readonly ILog Log = LogManager.GetLogger(typeof(AccountRepository));

		private static readonly string AccountsFolder =
			Path.Combine(Path.GetFullPath(ConfigurationManager.AppSettings["GitFolder"]), "accounts");

		public IEnumerable<Account> All()
		{
			GitClient.Fetch();
			var accountFiles = Directory.GetFiles(AccountsFolder);
			return accountFiles.Select(accountFile =>
										{
											try
											{
												var account = Base.FromJson<Account>(File.ReadAllText(accountFile));
												account.Id = ParseId(accountFile);
												return account;
											}
											catch (Exception e)
											{
												Log.ErrorFormat("Fail to parse {0} account config file {1}", accountFile, e);
												return null;
											}
										}).Where(a => a != null && a.Name != null);
		}

		public Account Get(string hostName)
		{
			hostName = hostName.ToLower();
			GitClient.Fetch();
			var accountFile = GetAccountConfigFileByHost(hostName);
			try
			{
				var account = Base.FromJson<Account>(File.ReadAllText(accountFile));
				account.Id = ParseId(accountFile);
				return account;
			}
			catch (Exception e)
			{
				Log.ErrorFormat("Fail to parse {0} account config file {1}", accountFile, e);
				return null;
			}
		}

        public Account Set_db(string hostName, string db)
        {
            hostName = hostName.ToLower();
            GitClient.Fetch();
            var accountFile = GetAccountConfigFileByHost(hostName);
            try
            {
                var account = Base.FromJson<Account>(File.ReadAllText(accountFile));
                account.Db = db;
                File.WriteAllText(accountFile, account.ToJson());
                GitClient.Push(account.Name + " db change", new Action<string>(Commit => Console.WriteLine("Commit")));
                return account;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Fail to write {0} account config file {1}", accountFile, e);
                return null;
            }
        }

		private static string GetAccountConfigFileByHost(string hostname)
		{
			return GetAccountConfigFile(HostToId(hostname));
		}

		private static string GetAccountConfigFile(string id)
		{
			return Path.Combine(AccountsFolder, id + ".json.cfg");
		}

		private static string HostToId(string name)
		{
			return name.Substring(0, name.IndexOf('.')).ToLower();
		}

		private static string ParseId(string accountFile)
		{
			return new FileInfo(accountFile).Name.Replace(".json.cfg", "");
		}

	}
}