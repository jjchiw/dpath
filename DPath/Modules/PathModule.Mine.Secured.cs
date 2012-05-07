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
	public class PathModuleMineSecured : NancyModule
	{
		private IDocumentStore _documentStore;
		private IDocumentSession _session;
		private Path _path;

		public PathModuleMineSecured(IDocumentStore documentStore): base("paths")
		{
			this.RequiresAuthentication();

			_documentStore = documentStore;

			this.Before.AddItemToEndOfPipeline(ctx =>
			{
				_session = _documentStore.OpenSession();
				
				_path = PathTasks.GetPath(this, ctx.Parameters as DynamicDictionary, _session);
				if (_path.User.Email != ctx.UserEmail())
				{
					_session.Dispose();
					return HttpStatusCode.Unauthorized;
				}
					
			
				return null;
			});

			this.After.AddItemToEndOfPipeline(ctx =>
			{
				if(_session != null)
					_session.Dispose();
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

				_session.Store(_path);
				_session.SaveChanges();

				return Response.AsJson(new { name = _path.Name, id = _path.Id, description = _path.Description });
			};

			Post["/{id}/goal/{goalId}/update-name"] = parameters =>
			{
				var goalId = parameters.goalId;
				string newName = Request.Form.Name;
				
				_path.Goals.SingleOrDefault(x => x.Id == goalId).Name = newName;

				_session.Store(_path);
				_session.SaveChanges();


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

				_session.Store(_path);
				_session.SaveChanges();


				return new Response { StatusCode = HttpStatusCode.OK };
			};

			Post["/{id}/delete"] = parameters =>
			{
				_session.Delete<Path>(_path);
				_session.SaveChanges();

				return HttpStatusCode.OK;
			};

			Post["/{id}/delete-goal"] = parameters =>
			{
				string goalId = Request.Form.goalId;
				var goal = _path.Goals.FirstOrDefault(x => x.Id == goalId);

				if (goal == null)
					return HttpStatusCode.NotFound;

				_path.Goals.Remove(goal);

				_session.SaveChanges();

				return HttpStatusCode.OK;

			};
		}
	}
}