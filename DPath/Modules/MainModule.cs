using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using DPath.Helpers;

namespace DPath.Modules
{
	public class MainModule : NancyModule
	{
		public MainModule()
		{
			Get["/"] = parameters =>
			{
				var m = Context.Model("Home");

				return View["Views/Home", m];
			};

			Get["/path/{path}"] = parameters =>
			{
				var m = Context.Model("Home");

				return "yaya" + parameters.path;
			};
		}
	}
}