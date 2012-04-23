﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPath.Models.ViewModels
{
	public class PathView
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public int TotalOncourse { get; set; }
		public int TotalAstray { get; set; }
		public List<GoalView> Goals { get; set; }

		public string Description { get; set; }

		public DateTime DateCreated { get; set; }
		public string PrettyDate { get; set; }

		public DateTime LastUpdated { get; set; }

		public string PrettyLastUpdatedDate { get; set; }
	}
}