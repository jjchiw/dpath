using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DPath.Models.Helpers;

namespace DPath.Models
{
	public class Path
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public List<Goal> Goals { get; set; }
		public UserDenormalized<User> User { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime LastUpdated { get; set; }
		public List<string> SubscribedUsers { get; set; }

		public Path()
		{
			Goals = new List<Goal>();
		}
	}
}