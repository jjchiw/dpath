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
        private IDocumentSession _session;

		public UserModule(IDocumentStore documentStore) : base("User")
		{
			this.RequiresAuthentication();

            Before.AddItemToEndOfPipeline(ctx =>
            {
                _session = _documentStore.OpenSession();
                return null;
            });

            After.AddItemToEndOfPipeline(ctx =>
            {
                if(_session != null)
                    _session.Dispose();
            });

			_documentStore = documentStore;

			Post["/"] = parameter =>
			{
				var user = (Context.CurrentUser as User);

                user.UserName = Request.Form.Username;

				_session.Store(user);
				_session.SaveChanges();

				Context.CurrentUser = user;

				var m = Context.Model("Edit User");
				return View["User/Edit", m];
			};

            Post["/token"] = parameter =>
            {
                var user = (Context.CurrentUser as User);

                user.Token = new Random().Next(1, 10).ToString();

                _session.Store(user);
                _session.SaveChanges();
                    
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