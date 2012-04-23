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

		public Goal()
		{
			Id = null;
			Achievements = new List<Achievement>();
		}
	}
}