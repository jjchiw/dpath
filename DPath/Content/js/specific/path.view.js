$(document).ready(function () {

	var postAchievement = function (goalId, comment, resolution) {
		var url = "/" + $("#path-id").val() + "/goal/" + goalId + "/" + resolution;

		$.post(url, { "comment": comment }, function (data) {
			//data is the AchievementView object
			$.get("/Content/js/templates/achievement.add.mustache", function (template) {
				var achievement = Mustache.render(template, data.AchievementView);
				$("#all-list").prepend(achievement);
				$("#my-list").prepend(achievement);
				$("#on-course-list").prepend(achievement);
				if (resolution == "oncourse") {
					$("#last-oncourse-" + goalId).html(achievement);
					$("#on-course-count-" + goalId).text(data.OnCourseCount);
				} else {
					$("#last-astray-" + goalId).html(achievement);
					$("#astray-count-" + goalId).text(data.AstrayCount);
				}
			});

			if (comment != "")
				$("#comment-" + goalId).val("");

		});
	};

	$('a[id^="add-oncourse"]').live("click", function () {
		var goalId = $(this).attr("id").replace("add-oncourse-", "");
		var comment = $("#comment-" + goalId).val();

		postAchievement(goalId, comment, "oncourse");

		return true;
	});

	$('a[id^="add-astray"]').live("click", function () {
		var goalId = $(this).attr("id").replace("add-astray-", "");
		var comment = $("#comment-" + goalId).val();

		postAchievement(goalId, comment, "astray");

		return true;
	});

	$('a[id^="add-astray-link-"]').live("click", function () {
		var goalId = $(this).attr("id").replace("add-astray-link-", "");
		var comment = "";

		postAchievement(goalId, comment, "astray");

		return true;
	});


	$('a[id^="add-on-course-link-"]').live("click", function () {
		var goalId = $(this).attr("id").replace("add-on-course-link-", "");
		var comment = "";

		postAchievement(goalId, comment, "oncourse");

		return true;
	});

	$('a[id^="view-goal"]').click(function () {
		var goalId = $(this).attr("id").replace("view-goal-", "");
		var urlPattern = "/" + $("#path-id").val() + "/view-goal/" + goalId;
		var url = urlPattern + "/0";

		$("#view-goal").remove();

		$.get(url, function (data, status) {
			//data is the AchievementView object
			//data is the AchievementView object
			console.log(status);
			$.get("/Content/js/templates/achievement.view.mustache", function (template) {
				var achievement = Mustache.render(template, data);
				$("#container").append(achievement);
				$('#view-goal').modal('show');

				var page = 1;
				var loading = false;
				$("#view-goal").scroll(function () {
					console.log($("#view-goal").scrollTop() >= $("#view-goal").height() * page);
					console.log($("#view-goal").height());
					console.log(page);
					if ($("#view-goal").scrollTop() >= $("#view-goal").height() * page) {
						if (loading == false) {
							loading = true;
							console.log("like loading");
							var urls = urlPattern + "/" + page;
							$.get(urls, function (data, status) {
								$.get("/Content/js/templates/achievement.load.mustache", function (template) {
									var gList = { list: data.m.AllAchievements }
									var list = Mustache.render(template, gList);
									$("#all-list").append(list);

									gList.list = data.m.MyAchievements
									list = Mustache.render(template, gList);
									$("#my-list").append(list);

									gList.list = data.m.OnCourseAchievements
									list = Mustache.render(template, gList);
									$("#on-course-list").append(list);

									$("#on-course-count-" + goalId).text(data.m.OnCourseAchievements.length);

									gList.list = data.m.AstrayAchievements
									list = Mustache.render(template, gList);
									$("#astray-list").append(list);

									$("#astray-count-" + goalId).text(data.m.AstrayAchievements.length);
								});


								console.log(data);

								loading = false;
								page++;
							});

						}
					}
				});

			});
		});

		return true;
	});

	var characters = 255;
	$(".resolution").live("keyup", function () {
		if ($(this).val().length > characters) {
			$(this).val($(this).val().substr(0, characters));
		}
		var remaining = characters - $(this).val().length;
		$("#counter").html(remaining);
	});

	//if((($(window).scrollTop()+$(window).height())+250)>=$(document).height()){

});