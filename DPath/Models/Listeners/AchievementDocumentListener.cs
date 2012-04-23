using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Listeners;

namespace DPath.Models.Listeners
{
	public class AchievementDocumentListener : IDocumentStoreListener
	{

		#region IDocumentStoreListener Members

		public void AfterStore(string key, object entityInstance, Raven.Json.Linq.RavenJObject metadata)
		{
			
		}

		public bool BeforeStore(string key, object entityInstance, Raven.Json.Linq.RavenJObject metadata)
		{
			var achievement = entityInstance as Achievement;
			if (achievement == null)
				return false;

			if (achievement.Id == null)
				achievement.Id = Guid.NewGuid().ToString();

			if (achievement.DateCreated == DateTime.MinValue)
				achievement.DateCreated = DateTime.Now;

			return true;
		}

		#endregion
	}
}