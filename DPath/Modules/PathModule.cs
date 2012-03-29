using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;

namespace DPath.Modules
{
	public class PathModule : NancyModule
	{
		public PathModule() : base("paths")
		{
			Get["/{id}"] = parameters =>
			{
				return "path " + parameters.path;
			};
		}
	}
}