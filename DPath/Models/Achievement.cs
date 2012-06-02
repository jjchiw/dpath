using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPath.Models.Helpers;

namespace DPath.Models
{
	public class Achievement
	{
		public string Id { get; set; }
		public UserDenormalized<User> User { get; set; }
		public string Comment { get; set; }
		public DateTime DateCreated { get; set; }
		public Resolution Resolution { get; set; }

		public Achievement()
		{
			Id = null;
			DateCreated = DateTime.MinValue;
		}
	}

	public enum Resolution
	{
		OnCourse, Astray
	}
}
