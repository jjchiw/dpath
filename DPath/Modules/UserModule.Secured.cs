using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;
using Raven.Client;
using DPath.Models;
using DPath.Helpers;

namespace DPath.Modules
{
	public class UserModule : NancyModule
	{
		private IDocumentStore _documentStore;

		public UserModule(IDocumentStore documentStore) : base("User")
		{
			this.RequiresAuthentication();

			_documentStore = documentStore;

			Post["/"] = parameter =>
			{
				using (var session = documentStore.OpenSession())
				{
					var user = session	.Query<User>()
										.SingleOrDefault(x => x.Email == Context.UserEmail());

					user.UserName = Request.Form.Username;

					session.Store(user);
					session.SaveChanges();

					Context.CurrentUser = user;

					var m = Context.Model("Edit User");
					return View["User/Edit", m];
				}
			};

			Get["/"] = parameter =>
			{
				var m = Context.Model("Edit User");
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