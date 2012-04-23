using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Listeners;

namespace DPath.Models.Listeners
{
	public class PathDocumentListener : IDocumentStoreListener
	{
		#region IDocumentStoreListener Members

		public void AfterStore(string key, object entityInstance, Raven.Json.Linq.RavenJObject metadata)
		{
			
		}

		public bool BeforeStore(string key, object entityInstance, Raven.Json.Linq.RavenJObject metadata)
		{
			var path = entityInstance as Path;
			if (path == null) 
				return false;

			path.Goals.Where(x => x.Id == null).ToList().ForEach(x =>
				{
					x.Id = Guid.NewGuid().ToString();
				});

			return true;
		}

		#endregion
	}
}