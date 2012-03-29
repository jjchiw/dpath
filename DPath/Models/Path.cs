using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPath.Models
{
	public class Path
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public List<Goal> Goals { get; set; }
		public User User { get; set; }

		public Path()
		{
			Goals = new List<Goal>();
		}
	}
}