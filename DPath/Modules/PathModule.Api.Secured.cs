using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DPath.Models;
using Nancy;
using Raven.Client;
using Nancy.OData;
using DPath.Models.ViewModels;
using DPath.Helpers;
using Nancy.ModelBinding;
using DPath.Tasks;

namespace DPath.Modules
{
    public class PathModuleApiSecured :  RavenModule
    {
        User _user;

        public PathModuleApiSecured() : base("api/paths")
        {

            Before.AddItemToEndOfPipeline(ctx =>
            {
                var authString = Request.Headers.Authorization;
                var userAndPassDecoded = new System.Text.ASCIIEncoding().GetString(Convert.FromBase64String(authString.Substring(6)));
                var userAndPass = userAndPassDecoded.Split(':');
                var emailStr = userAndPass[0];
                var tokenStr = userAndPass[1];

                _user = RavenSession.Query<User>().FirstOrDefault(x => x.Email == emailStr && x.Token == tokenStr);

                if (_user != null)
                    return null;

                return HttpStatusCode.Forbidden;
            });

            After.AddItemToEndOfPipeline(ctx =>
            {
                if (RavenSession != null)
                    RavenSession.Dispose();
            });

            Get["/"] = _ =>
            {
                var paths = RavenSession.Query<Path>()
                                    .ToList()
									.Select(x => x.ConvertToPathView())
									.OrderByDescending(x => x.DateCreated)
									.ToList();
                
                return Response.AsOData<PathView>(paths.AsQueryable(), HttpStatusCode.OK);
            };

            Post["/user/validate"] = _ =>
            {
                var userViewParams = this.Bind<UserView>();

                var user = RavenSession.Query<User>().FirstOrDefault(x => x.Email == userViewParams.Email && x.Token == userViewParams.Token);

                if (user == null)
                    return HttpStatusCode.NotFound;

                return Response.AsJson<UserView>(user.ConverToUserView());
            };

            Post["/my-paths"] = _ =>
            {
                var results = GetPathsSubscribedUser();

                return Response.AsJson<List<PathView>>(results);
            };

            Post["/{id}/goal/{goalId}/{resolution}"] = parameters =>
            {
                var result = PathTasks.AddResolution(this, parameters, RavenSession, _user, Request.Form.comment);

                if (result == null)
                    return HttpStatusCode.NotFound;

                return Response.AsJson<AchievementView>((result.Achievement as Achievement).ConverToAchievementView());
            };
        }

        private List<PathView> GetPathsSubscribedUser()
        {
            var pathsWithCustomer = RavenSession.Advanced.LuceneQuery<Path>()
                                                         .Where(string.Format("SubscribedUsers:({0})", _user.Id));

            var results = new List<PathView>(); // Prepare our results list
            foreach (var path in pathsWithCustomer)
            {
                // For each user, load its associated alias based on that user Id
                results.Add(path.ConvertToPathView(_user.Id, true));
            }
            return results;
        }
    }
}