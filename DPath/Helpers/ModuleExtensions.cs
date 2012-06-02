using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using System.Dynamic;
using Nancy.Authentication.Forms;
using DPath.Models;
using DPath.Models.ViewModels;
using Raven.Client;

namespace DPath.Helpers
{
	public static class ModuleExtensions
	{
		public static bool IsLoggedIn(this NancyContext context)
		{
			return !(context == null || context.CurrentUser == null ||
					 string.IsNullOrWhiteSpace(context.CurrentUser.UserName));
		}

		public static string Username(this NancyContext context)
		{
			return (context == null || context.CurrentUser == null || string.IsNullOrWhiteSpace(context.CurrentUser.UserName)) ? string.Empty : context.CurrentUser.UserName;
		}

		public static string UserEmail(this NancyContext context)
		{
			return (context == null || context.CurrentUser == null) ? string.Empty : (context.CurrentUser as User).Email;
		}

		public static string UserGravatarUrl(this NancyContext context)
		{
			return (context == null || context.CurrentUser == null) ? string.Empty : (context.CurrentUser as User).Email.ToGravatarUrl();
		}

		public static dynamic Model(this NancyContext context, string title)
		{
			dynamic model = new ExpandoObject();
			model.Title = title;
			model.IsLoggedIn = context.IsLoggedIn();
			model.UserName = context.Username();
			model.Email = context.UserEmail();
			model.GravatarUrl = context.UserGravatarUrl();
			model.HomeActive = "";
			model.NewPathActive = "";
			model.MyPathsActive = "";
			model.AboutActive = "";
			return model;
		}

		public static PathView ConvertToPathView(this Path path, string userId = "", bool justMyAchievements = false)
		{
			return new PathView
			{
				Id = path.Id,
				Name = path.Name,
				Description = path.Description,
				TotalOncourse = path.Goals.Sum(y => y.Achievements.Where(x => x.Resolution == Resolution.OnCourse).Count()),
				TotalAstray = path.Goals.Sum(y => y.Achievements.Where(x => x.Resolution == Resolution.Astray).Count()),
				Goals = path.Goals.OrderBy(x => x.Order).Select(y => y.ConvertToGoalView()).ToList(),
				DateCreated = path.DateCreated,
				PrettyDate = path.DateCreated.FriendlyParse(),
				LastUpdated = path.LastUpdated,
				PrettyLastUpdatedDate = path.LastUpdated.FriendlyParse(),
				UserName = path.User.UserName,
				TotalUsersInPath = path.Goals.SelectMany(x => x.Achievements).Select(y => y.User.Email).Distinct().Count()
			};
		}

        public static GoalView ConvertToGoalView(this Goal goal, string userId = "", bool justMyAchievements = false)
		{
            List<AchievementView> achievements;
            if(justMyAchievements)
                achievements = goal.Achievements.Where(x => x.User.Id == userId)
                                                .Select(y => y.ConverToAchievementView()).OrderByDescending(x => x.DateCreated).ToList();
            else
                achievements = goal.Achievements.Select(y => y.ConverToAchievementView()).OrderByDescending(x => x.DateCreated).ToList();
			return new GoalView
			{
				Id = goal.Id,
				Name = goal.Name,
                Achievements = achievements,
				TotalUsersInGoal = goal.Achievements.Select(y => y.User.Email).Distinct().Count()
			};
		}

		public static AchievementView ConverToAchievementView(this Achievement achievement, int gravatarSize = 40)
		{
			return new AchievementView
			{
				Id = achievement.Id,
				Comment = achievement.Comment,
				DateCreated = achievement.DateCreated,
				UserName = achievement.User.UserName,
				GravatarUrl = achievement.User.Email.ToGravatarUrl(gravatarSize),
				Resolution = achievement.Resolution.ToString(),
				PrettyDate = achievement.DateCreated.FriendlyParse()
			};
		}

        public static UserView ConverToUserView(this User user)
        {
            return new UserView
            {
                Email = user.Email,
                Token = user.Token,
                UserName = user.UserName
            };
        }
	}
}