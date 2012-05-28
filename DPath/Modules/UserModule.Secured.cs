using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;
using Raven.Client;
using DPath.Models;
using DPath.Helpers;
using DPath.Tasks;

namespace DPath.Modules
{
	public class UserModule : RavenModule
	{
		public UserModule() : base("User")
		{
			this.RequiresAuthentication();

			Post["/"] = parameter =>
			{
				var user = (Context.CurrentUser as User);

                user.UserName = Request.Form.Username;

				RavenSession.Store(user);

				Context.CurrentUser = user;

				UpdateUserDenormalizedTask.Execute(user);

				return Response.AsRedirect("/User/");
			};

            Post["/token"] = parameter =>
            {
                var user = (Context.CurrentUser as User);

                user.Token = new Random().Next(1, 10).ToString();

				RavenSession.Store(user);
                    
                return Response.AsJson(new {Token = user.Token}, HttpStatusCode.OK );
            };

			Get["/"] = parameter =>
			{
				var m = Context.Model("Edit User");
                m.Token = (Context.CurrentUser as User).Token;
				return View["User/Edit", m];
			};

			Get[".json"] = parameter =>
			{
				var m = Context.Model("Edit User");
				return Response.AsJson(new { m });
			};

		}
	}
}