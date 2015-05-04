using Newtonsoft.Json;

namespace Config.Model
{
	public class Account : Base
	{
		public Account(string id, string name)
			: base(id)
		{
			Name = name;
		}

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "app")]
		public string App { get; set; }

		[JsonProperty(PropertyName = "db")]
		public string Db { get; set; }

		public bool Equals(Account other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && Equals(other.Name, Name) && Equals(other.App, App) && Equals(other.Db, Db);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return ReferenceEquals(this, obj) || Equals(obj as Account);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = base.GetHashCode();
				result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
				result = (result*397) ^ (App != null ? App.GetHashCode() : 0);
				result = (result*397) ^ (Db != null ? Db.GetHashCode() : 0);
				return result;
			}
		}
	}
}