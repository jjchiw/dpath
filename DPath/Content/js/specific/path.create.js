$(document).ready(function () {
	$("#add-path").click(function () {
		$.post("/paths/create", { "name": $("#path-name").val(),
			"description": $("#path-description").val()
		}, function (data) {
			//data is the path object
			$("#path-id").val(data.id);
			window.location = "/" + data.id + "/edit";

		});
	});

	$("#add-goal").click(function () {
		var url = "/" + $("#path-id").val() + "/add-goal";
		$(this).addClass("disabled");
		$(this).val("saving");

		$.post(url, { "name": $("#goal-name").val() }, function (data) {
			//data is the path object
			$.get("/Content/js/templates/goal.add.mustache.html", function (template) {
				var template = Mustache.render(template, data);
				$("#goals-list").prepend(template);
				$("#goal-name").val("");
				$("#goal-name").focus();

				$(this).removeClass("disabled");
				$(this).val("add goal");
			});
		});
	});
});