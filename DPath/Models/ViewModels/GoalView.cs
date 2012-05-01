using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPath.Models.ViewModels
{
	public class GoalView
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public List<AchievementView> Achievements { get; set; }

		public int TotalUsersInGoal { get; set; }
	}
}
