$(document).ready(function () {

	var postAchievement = function (goalId, comment, resolution, commentTemplateName) {
		var url = "/" + $("#path-id").val() + "/goal/" + goalId + "/" + resolution;

		$.post(url, { "comment": comment }, function (data) {
			//data is the AchievementView object
			$.get("/Content/js/templates/achievement.add.mustache.js", function (template) {
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
				$(commentTemplateName + goalId).val("");

		});
	};

	$('a[id^="add-astray-link-"]').live("click", function () {
		var goalId = $(this).attr("id").replace("add-astray-link-", "");
		var comment = $("#dynamic-comment-astray-goal-" + goalId).val();

		postAchievement(goalId, comment, "astray", "#dynamic-comment-astray-goal-");

		$("#add-astray-div-" + goalId).addClass("hidden");

		return false;
	});


	$('a[id^="add-on-course-link-"]').live("click", function () {
		var goalId = $(this).attr("id").replace("add-on-course-link-", "");
		var comment = $("#dynamic-comment-on-course-goal-" + goalId).val();

		postAchievement(goalId, comment, "oncourse", "#dynamic-comment-on-course-goal-");

		$("#add-on-course-div-" + goalId).addClass("hidden");

		return false;
	});

	$('a[id^="show-astray-link-"]').live("click", function () {
		var goalId = $(this).attr("id").replace("show-astray-link-", "");

		$('div[id^="add-on-course-div-"].dynamic-form-goal').addClass("hidden");
		$('div[id^="add-astray-div-"].dynamic-form-goal').addClass("hidden");

		$("#add-astray-div-" + goalId).removeClass("hidden");

		$('#dynamic-comment-astray-goal-' + goalId).focus();

		return false;
	});


	$('a[id^="show-on-course-link-"]').live("click", function () {
		var goalId = $(this).attr("id").replace("show-on-course-link-", "");

		$('div[id^="add-on-course-div-"].dynamic-form-goal').addClass("hidden");
		$('div[id^="add-astray-div-"].dynamic-form-goal').addClass("hidden");

		$("#add-on-course-div-" + goalId).removeClass("hidden");

		$('#dynamic-comment-on-course-goal-' + goalId).focus();

		return false;
	});

	$('a[id^="view-goal"]').click(function () {
		var goalId = $(this).attr("id").replace("view-goal-", "");
		var urlPattern = "/" + $("#path-id").val() + "/view-goal/" + goalId;
		var url = urlPattern + "/0";

		$("#view-goal").remove();

		$.get(url, function (data, status) {
			//data is the AchievementView object
			//data is the AchievementView object
			$.get("/Content/js/templates/achievement.view.mustache.js", function (template) {
				var achievement = Mustache.render(template, data);
				$("#container").append(achievement);
				$('#view-goal').modal('show');

				var page = 1;
				var loading = false;
				$("#view-goal").scroll(function () {
					if ($("#view-goal").scrollTop() >= $("#view-goal").height() * page) {
						if (loading == false) {
							loading = true;
							console.log("like loading");
							var urls = urlPattern + "/" + page;
							$.get(urls, function (data, status) {
								$.get("/Content/js/templates/achievement.load.mustache.js", function (template) {
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
			var textAreas = $('textarea[id^="dynamic-comment-on-course-goal-"]:focus');
			if (textAreas.length > 0) {
				var goalId = textAreas[0].id.replace("dynamic-comment-on-course-goal-", "");
				$("#add-on-course-link-" + goalId).click();
			}

			textAreas = $('textarea[id^="dynamic-comment-astray-goal-"]:focus');
			if (textAreas.length > 0) {
				var goalId = textAreas[0].id.replace("dynamic-comment-astray-goal-", "");
				$("#add-astray-link-" + goalId).click();
			}
		}
	});

});