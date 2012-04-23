$(document).ready(function () {
	$("#add-path").click(function () {
		var url = "/"+$("#path-id").val() + "/edit";
		$.post(url, { "name" : $("#path-name").val(), 
								  "description" :  $("#path-description").val() }, function (data) {
			//data is the path object
			$("#goals").toggle();
			$("#path-form").toggle();
			$("#path-name-saved").html(data.name);
			$("#path-id").val(data.id);


		});
	});

	$("#add-goal").click(function () {
		var url = "/"+$("#path-id").val() + "/add-goal";
		$.post(url, { "name": $("#goal-name").val() }, function (data) {

			//data is the path object
			$.get("/Content/js/templates/goal.add.mustache", function (template) {
				var template = Mustache.render(template, data);
				$("#goals-list").prepend(template);
				$("#goal-name").val("");
				$("#goal-name").focus();
			});


		});
	});
});