using System;
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
	}
}