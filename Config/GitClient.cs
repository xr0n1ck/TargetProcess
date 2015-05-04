using System;
using System.Configuration;
using System.IO;
using NGit.Api;

namespace Config
{
	public class GitClient
	{
		private static readonly object Lock = new object();

		private readonly Git _git;
		private readonly string _rep;

		private GitClient(string rep, Git git)
		{
			_rep = rep;
			_git = git;
		}

		public string Rep
		{
			get { return _rep; }
		}

		public void Commit(string message)
		{
			_git.Add().AddFilepattern(".").Call();
			_git.Add().SetUpdate(true).AddFilepattern(".").Call();
			_git.Commit().SetMessage(message).Call();
		}

		public void Push()
		{
			_git.Push().Call();
		}

		public void Close()
		{
			_git.GetRepository().Close();
		}

		private static GitClient Connect(string rep, string url, string home)
		{
			// workaround, see https://github.com/mono/ngit/issues/14
			Environment.SetEnvironmentVariable("HOME", home);
			Git nativeGit;
			if (Directory.Exists(rep))
			{
				nativeGit = Git.Open(rep);
				nativeGit.Reset().SetMode(ResetCommand.ResetType.HARD).Call();
				nativeGit.Fetch().Call();
				nativeGit.Rebase().SetUpstream("origin/master").Call();
			}
			else
			{
				nativeGit = Git.CloneRepository()
					.SetURI(url)
					.SetNoCheckout(false)
					.SetDirectory(rep).Call();
			}
			return new GitClient(rep, nativeGit);
		}

		private static GitClient ConnectFromConfig()
		{
			var gitFolder = Path.GetFullPath(ConfigurationManager.AppSettings["GitFolder"]);
			var gitUrl = Path.GetFullPath(ConfigurationManager.AppSettings["GitUrl"]);
			var home = ConfigurationManager.AppSettings["GitUserHome"];
			return Connect(gitFolder, gitUrl, home);
		}

		public static void Fetch()
		{
			lock (Lock)
			{
				var git = ConnectFromConfig();
				git.Close();
			}
		}

		public static void Push(string message, Action<string> action)
		{
			lock (Lock)
			{
				var git = ConnectFromConfig();
				action(git.Rep);
				git.Commit(message);
				git.Push();
				git.Close();
			}
		}
	}
}