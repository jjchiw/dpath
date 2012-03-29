using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using NBrowserID;
using Nancy.Authentication.Forms;
using Raven.Client;
using DPath.Models;
using Newtonsoft.Json;
using System.Text;

namespace DPath.Modules
{
	public class AuthModule : NancyModule
	{
		public AuthModule(IDocumentStore documentStore) : base ("auths")
		{
			Post["/login"] = parameters =>
			{
				var authentication = new BrowserIDAuthentication();
				var verificationResult = authentication.Verify(Request.Form.assertion);
				if (verificationResult.IsVerified)
				{
					string email = verificationResult.Email;
					User user = null;
					using (IDocumentSession session = documentStore.OpenSession())
					{
						user = session.Query<User>()
								.FirstOrDefault(x => x.Email == email);

						if (user == null)
						{
							user = new User
							{
								Email = email,
								UserName = email,
								Guid = Guid.NewGuid()
							};

							session.Store(user);
							session.SaveChanges();
						}

						
					}

					//FormsAuthentication.UserLoggedInResponse(user.Guid, DateTime.Now.AddDays(7));
					var jsonResponseString = JsonConvert.SerializeObject(new { email = email });
					var jsonBytes = Encoding.UTF8.GetBytes(jsonResponseString);

					var response = this.LoginWithoutRedirect(user.Guid, DateTime.Now.AddDays(7));
					response.ContentType = "application/json";
					response.Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length);

					return response;
				}

				return new Response
				{
					ContentType = "application/json",
					Contents = null
				};
			};

			Post["/logout"] = parameters =>
			{
				var jsonResponseString = JsonConvert.SerializeObject("/");
				var jsonBytes = Encoding.UTF8.GetBytes(jsonResponseString);

				var response = this.LogoutWithoutRedirect();
				
				response.ContentType = "application/json";
				response.Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length);

				return response;
			};
		}
	}
}