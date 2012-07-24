using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPath.Models.ViewModels
{
	public class AchievementView
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string GravatarUrl { get; set; }
		public string Comment { get; set; }
		public string Resolution { get; set; }
		public string CssResolution
		{
			get
			{
				if(Resolution == DPath.Models.Resolution.Astray.ToString())
					return "achievement-alert alert-error";
				return "achievement-alert alert-success";
			}
		}
		public DateTime DateCreated { get; set; }
		public string PrettyDate {get; set;}

		public bool IsOnCourse { get; set; }
	}
}