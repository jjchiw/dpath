using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DPath.Models;
using DPath.Models.ViewModels;
using Nancy;
using Raven.Client;

namespace DPath.Tasks
{
    public class PathTasks
    {
        public static Path GetPath(NancyModule module, dynamic parameters, Raven.Client.IDocumentSession session)
        {
            var pathId = String.Format("{0}/{1}", module.ModulePath, parameters.id.Value as string);

            pathId = pathId.Replace("api/", "");

            var path = session.Load<Path>(pathId);

            return path;
        }

        public static dynamic AddResolution(NancyModule module, dynamic parameters, IDocumentSession session, User user, string comment)
        {
            Path path = GetPath(module, parameters as DynamicDictionary, session);
            if (path == null)
                return null;

            if (path.SubscribedUsers == null)
                path.SubscribedUsers = new List<string>();

            var isSubscribed = path.SubscribedUsers.SingleOrDefault(x => x == user.Id);
            if (isSubscribed == null)
                path.SubscribedUsers.Add(user.Id);

            var goalId = parameters.goalId.Value as string;
            var resolution = parameters.resolution.Value as string;

            var goal = path.Goals.SingleOrDefault(x => x.Id == goalId);

            var enumResolution = Resolution.OnCourse;
            if (resolution == "astray")
                enumResolution = Resolution.Astray;

            var achievement = new Achievement
            {
                Id = Guid.NewGuid().ToString(),
                Comment = comment,
                User = user,
                DateCreated = DateTime.UtcNow,
                Resolution = enumResolution
            };

            path.LastUpdated = DateTime.UtcNow;
            goal.Achievements.Add(achievement);

            //session.Store(goal);
            session.Store(path);

            return new { Achievement = achievement, Goal = goal };
        }

	}
}