using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Authentication.Forms;
using Raven.Client;

namespace DPath.Models.Repositories
{
	public class UserMapper : IUserMapper
	{
		private IDocumentStore _documentStore;

		public UserMapper(IDocumentStore documentStore)
		{
			_documentStore = documentStore;
		}
		#region IUserMapper Members

		public Nancy.Security.IUserIdentity GetUserFromIdentifier(Guid identifier)
		{
			using (IDocumentSession session = _documentStore.OpenSession())
			{
				var user = session.Query<User>().FirstOrDefault(x => x.Guid == identifier);

				return user;
			}
		}

		#endregion
	}
}