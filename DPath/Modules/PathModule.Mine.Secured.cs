using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Raven.Client;
using DPath.Models;
using DPath.Helpers;
using Nancy.Security;
using DPath.Tasks;

namespace DPath.Modules
{
	public class PathModuleMineSecured : RavenModule
	{
		private Path _path;

		public PathModuleMineSecured(): base("paths")
		{
			this.RequiresAuthentication();

			this.Before.AddItemToEndOfPipeline(ctx =>
			{	
				_path = PathTasks.GetPath(this, ctx.Parameters as DynamicDictionary, RavenSession);
				if (_path.User.Email != ctx.UserEmail())
				{
					RavenSession.Dispose();
					return HttpStatusCode.Unauthorized;
				}
					
			
				return null;
			});

			this.After.AddItemToEndOfPipeline(ctx =>
			{
				if(RavenSession != null)
					RavenSession.Dispose();
			});


			Get["/{id}/edit"] = parameters =>
			{
				var m = Context.Model("Edit");
				m.Path = _path.ConvertToPathView();
				m.NewPathActive = "active";
				return View["Path/Edit", m];
			};

			Post["/{id}/edit"] = parameters =>
			{
				_path.Name = Request.Form.Name;
				_path.Description = Request.Form.Description;

				RavenSession.Store(_path);

				return Response.AsJson(new { name = _path.Name, id = _path.Id, description = _path.Description });
			};

			Post["/{id}/goal/{goalId}/update-name"] = parameters =>
			{
				var goalId = parameters.goalId;
				string newName = Request.Form.Name;
				
				_path.Goals.SingleOrDefault(x => x.Id == goalId).Name = newName;

				RavenSession.Store(_path);

				return new Response { StatusCode = HttpStatusCode.OK };
			};

			Post["/{id}/goal/{goalId}/order"] = parameters =>
			{
				var goalId = parameters.goalId;
				int newOrder = Request.Form.NewOrder;
				int oldOrder = _path.Goals.FirstOrDefault(x => x.Id == goalId).Order;

				var tempNewOrder = newOrder;
				var tempOldOrder = oldOrder;

				var tempOrder = 0;
				if (newOrder < oldOrder)
				{
					tempOrder = tempNewOrder;
					tempNewOrder = tempOldOrder;
					tempOldOrder = tempOrder;
				}

				_path.Goals.Where(x =>
				{
					return x.Order >= tempOldOrder && x.Order <= tempNewOrder && x.Id != goalId;
				})
								.ToList()
								.ForEach(x =>
								{
									if (newOrder < oldOrder)
										x.Order++;
									else
										x.Order--;
								});

				_path.Goals.SingleOrDefault(x => x.Id == goalId).Order = newOrder;

				RavenSession.Store(_path);

				return new Response { StatusCode = HttpStatusCode.OK };
			};

			Post["/{id}/delete"] = parameters =>
			{
				RavenSession.Delete<Path>(_path);
				return HttpStatusCode.OK;
			};

			Post["/{id}/delete-goal"] = parameters =>
			{
				string goalId = Request.Form.goalId;
				var goal = _path.Goals.FirstOrDefault(x => x.Id == goalId);

				if (goal == null)
					return HttpStatusCode.NotFound;

				_path.Goals.Remove(goal);

				return HttpStatusCode.OK;

			};

			Get["/{id}/my-stats"] = parameters =>
			{
				var pathId = String.Format("{0}/{1}", this.ModulePath, parameters.id.Value as string);
				Path path = RavenSession.Load<Path>(pathId);
				if (path == null)
					return new Response { StatusCode = HttpStatusCode.NotFound };


				var m = path.Goals.Select(x => new
				{
					Id = x.Id,
					Name = x.Name,
					TOnCourse = x.Achievements.Where(y => y.Resolution == Resolution.OnCourse && y.User.Id == Context.UserRavenIdString()).Count(),
					TAstray = x.Achievements.Where(y => y.Resolution == Resolution.Astray && y.User.Id == Context.UserRavenIdString()).Count()
				}).ToList();

				m.Add(new
				{
					Id = "0",
					Name = "0",
					TOnCourse = path.Goals.Sum(x => x.Achievements.Where(y => y.Resolution == Resolution.OnCourse && y.User.Id == Context.UserRavenIdString()).Count()),
					TAstray = path.Goals.Sum(x => x.Achievements.Where(y => y.Resolution == Resolution.Astray && y.User.Id == Context.UserRavenIdString()).Count())
				});

				return Response.AsJson(new { m }, HttpStatusCode.OK);
			};
		}
	}
}