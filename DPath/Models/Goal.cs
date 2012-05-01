using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPath.Models
{
	public class Goal
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public List<Achievement> Achievements { get; set; }
		public int Order { get; set; }

		public Goal()
		{
			Id = null;
			Achievements = new List<Achievement>();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Goal)) return false;

			return Id == (obj as Goal).Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}