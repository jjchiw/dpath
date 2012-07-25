$(document).ready(function () {

	var markdown = new MarkdownDeep.Markdown();

	var getGoalStats = function (segment) {
		var url = "/" + $("#path-id").val() + "/" + segment;

		$.get(url, function (data) {

			if (typeof data != "object") {
				location = location.protocol + "//" + location.host + "?returnUrl=" + location.pathname;
				return;
			}

			$.each(data.m, function (index, obj) {
				$("#on-course-count-" + obj.Id + " span").html(obj.TOnCourse);
				$("#astray-course-count-" + obj.Id + " span").html(obj.TAstray);
			});

		});

	};

	var postAchievement = function (goalId, comment, resolution, commentTemplateName) {
		var url = "/" + $("#path-id").val() + "/goal/" + goalId + "/" + resolution;

		$.post(url, { "comment": comment }, function (data) {

			if (typeof data != "object") {
				location = location.protocol + "//" + location.host + "?returnUrl=" + location.pathname;
				return;
			}

			//data is the AchievementView object
			$.get("/Content/js/templates/achievement.add.mustache.html", function (template) {
				var achievement = Mustache.render(template, data.AchievementView);
				$("#all-list").prepend(achievement);
				$("#my-list").prepend(achievement);
				$("#on-course-list").prepend(achievement);
				if (resolution == "oncourse") {
					$("#last-oncourse-" + goalId).html(achievement);
					$("#on-course-count-" + goalId + " span").html(data.OnCourseCount);
				} else {
					$("#last-astray-" + goalId).html(achievement);
					$("#astray-count-" + goalId + " span").html(data.AstrayCount);
				}

				$("#achievement-" + data.AchievementView.Id + "-goal-id").val(goalId);

				$("#dynamic-comment-" + data.AchievementView.Id).focus();

			});

			if (comment != "")
				$(commentTemplateName + goalId).val("");

		});
	};

	var postAchievementComment = function (achievementId, comment) {
		var goalId = $("#achievement-" + achievementId + "-goal-id").val();
		var url = "/" + $("#path-id").val() + "/goal/" + goalId + "/achievement/" + achievementId;

		$.post(url, { "comment": comment }, function (data) {

			if (typeof data != "object") {
				location = location.protocol + "//" + location.host + "?returnUrl=" + location.pathname;
				return;
			}


			$("div[name=add-comment-" + achievementId + "]").each(function (index) {
				$(this).html("<p>" + markdown.Transform(comment) + "</p>");
			});
		});
	};

	var showGoalAchievements = function (goalId) {
		var urlPattern = "/" + $("#path-id").val() + "/view-goal/" + goalId;
		var url = urlPattern + "/0";

		//$("#view-goal").remove();

		$.get(url, function (data, status) {
			//data is the AchievementView object
			//data is the AchievementView object
			$.get("/Content/js/templates/achievement.view.mustache.html", function (template) {
				var achievement = Mustache.render(template, data);

				$("#achievements-div").html(achievement);
				var page = 1;
				var loading = false;
				//$("#achievements-div").scroll(function () {
				$(window).scroll(function () {
					//if ($("#achievements-div").scrollTop() >= $("#achievements-div").height() * page) {
					if ($(window).scrollTop() >= $(window).height() * page) {
						if (loading == false) {
							loading = true;
							console.log("like loading");
							var urls = urlPattern + "/" + page;
							$.get(urls, function (data, status) {
								$.get("/Content/js/templates/achievement.load.mustache.html", function (template) {
									var gList = { list: data.m.AllAchievements }
									var list = Mustache.render(template, gList);
									$("#all-list").append(list);

									gList.list = data.m.MyAchievements
									list = Mustache.render(template, gList);
									$("#my-list").append(list);

									gList.list = data.m.OnCourseAchievements
									list = Mustache.render(template, gList);
									$("#on-course-list").append(list);

									//$("#on-course-count-" + goalId).text(data.m.OnCourseAchievements.length);

									gList.list = data.m.AstrayAchievements
									list = Mustache.render(template, gList);
									$("#astray-list").append(list);

									//$("#astray-count-" + goalId).text(data.m.AstrayAchievements.length);
								});

								loading = false;
								page++;
							});

						}
					}
				});

			});
		});
	};

	$('a[id^="add-comment-link-"]').live("click", function () {
		var achievementId = $(this).attr("id").replace("add-comment-link-", "");
		var comment = $("#dynamic-comment-" + achievementId).val();

		postAchievementComment(achievementId, comment);

		return false;
	});

	$('a[id^="dismis-comment-link-"]').live("click", function () {
		var achievementId = $(this).attr("id").replace("dismis-comment-link-", "");

		$("#add-comment-" + achievementId).html("<p></p>");

		return false;
	});

	/*
	Link to add an achievement as astray
	*/
	$('a[id^="add-astray-link-"]').live("click", function () {
		//$('div[id^="add-comment-"]').remove();

		var goalId = $(this).attr("id").replace("add-astray-link-", "");

		postAchievement(goalId, "", "astray", "");

		return false;
	});

	/*
	Link to add an achievement as on course
	*/
	$('a[id^="add-on-course-link-"]').live("click", function () {
		//$('div[id^="add-comment-"]').remove();

		var goalId = $(this).attr("id").replace("add-on-course-link-", "");

		postAchievement(goalId, "", "oncourse", "");

		return false;
	});

	/*
	Link to add an achievement as astray
	*/
	$('a[id^="add-inline-astray-link"]').live("click", function () {
		//$('div[id^="add-comment-"]').remove();

		var goalId = $("#view-goal-id").val();

		postAchievement(goalId, "", "astray", "");

		return false;
	});

	/*
	Link to add an achievement as on course
	*/
	$('a[id^="add-inline-on-course-link"]').live("click", function () {
		//$('div[id^="add-comment-"]').remove();

		var goalId = $("#view-goal-id").val();

		postAchievement(goalId, "", "oncourse", "");

		return false;
	});

	$('a[id^="view-goal"]').click(function () {
		var goalId = $(this).attr("id").replace("view-goal-", "");

		showGoalAchievements(goalId);

		return false;
	});

	$('#show-all-stats').live('click', function () {
		getGoalStats("all-stats");

		$("#span-all-stats").html("all stats");

		if($("#span-my-stats").length > 0)
			$("#span-my-stats").html("<a id='show-my-stats' href='#'>my stats</a></span>");
		

		return false;
	});

	$('#show-my-stats').live('click', function () {
		
		getGoalStats("my-stats");
		
		$("#span-my-stats").html("my stats");

		if($("#span-all-stats").length > 0)
			$("#span-all-stats").html("<a id='show-all-stats' href='#'>all stats</a></span>");

		return false;
	});

	var characters = 255;
	$(".resolution").live("keyup", function () {
		if ($(this).val().length > characters) {
			$(this).val($(this).val().substr(0, characters));
		}
		var remaining = characters - $(this).val().length;
		$("#counter").html(remaining);
	});

	$(document).keypress(function (e) {
		if (e.keyCode == 13) {
			var textAreas = $('textarea[id^="dynamic-comment-"]:focus');
			if (textAreas.length > 0) {
				var textArea = textAreas[0];
				var achievementId = textArea.id.replace("dynamic-comment-", "");
				postAchievementComment(achievementId, textArea.value);
			}
		}
		//ESC
		if (e.keyCode == 27) {
			$('div[id^="add-comment-"]').remove();
		}
	});

	showGoalAchievements(0);
});