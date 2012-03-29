using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPath.Models
{
	public class Goal
	{
		public string Name { get; set; }
		public List<Achievement> OnCourse { get; set; }
		public List<Achievement> Astray { get; set; }

		public Goal()
		{
			OnCourse = new List<Achievement>();
			Astray = new List<Achievement>();
		}
	}
}