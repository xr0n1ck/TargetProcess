using System;
using Newtonsoft.Json;

namespace Config.Model
{
	[Serializable]
	public class Base
	{
		public Base(string id)
		{
			Id = id;
		}

		[JsonIgnore]
		public string Id { get; set; }

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		public static TD FromJson<TD>(string json) where TD : Base
		{
			return JsonConvert.DeserializeObject<TD>(json);
		}

		public bool Equals(Base other)
		{
			if (ReferenceEquals(null, other)) return false;
			return ReferenceEquals(this, other) || Equals(other.Id, Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == typeof (Base) && Equals((Base) obj);
		}

		public override int GetHashCode()
		{
			return (Id != null ? Id.GetHashCode() : 0);
		}
	}
}