using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPath.Models.Helpers
{
	public class UserDenormalized<T> where T : IUserDenormalized
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }

		public static implicit operator UserDenormalized<T>(T doc)
		{
			return new UserDenormalized<T>
			{
				Id = doc.Id,
				UserName = doc.UserName,
				Email = doc.UserName
			};
		}
	}
}
