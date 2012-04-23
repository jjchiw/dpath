using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Listeners;

namespace DPath.Models.Listeners
{
	public class GoalDocumentListener : IDocumentStoreListener
	{
		#region IDocumentStoreListener Members

		public void AfterStore(string key, object entityInstance, Raven.Json.Linq.RavenJObject metadata)
		{
		}

		public bool BeforeStore(string key, object entityInstance, Raven.Json.Linq.RavenJObject metadata)
		{
			var goal = entityInstance as Goal;
			if (goal == null)
				return false;

			goal.OnCourse.Where(x => x.Id == null).ToList().ForEach(x =>
			{
				x.Id = Guid.NewGuid().ToString();
			});

			goal.Astray.Where(x => x.Id == null).ToList().ForEach(x =>
			{
				x.Id = Guid.NewGuid().ToString();
			});

			return true;
		}

		#endregion
	}
}