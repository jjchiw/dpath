using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;

namespace DPath.Models
{
	public class User : IUserIdentity
	{
		public string Id { get; set; }
		public Guid Guid { get; set; }
		public string Email { get; set; }
		
		#region IUserIdentity Members

		public string UserName { get; set; }
		public IEnumerable<string> Claims { get; set; }

		#endregion
	}
}