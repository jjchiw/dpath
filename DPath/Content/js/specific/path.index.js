$(document).ready(function () {
	$("#add-path").click(function () {
		$.post("/paths/create", { "name": $("#path-name").val() }, function (data) {
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
			$("<li/>").html(data.name).appendTo("#goals-list");
			$("#goal-name").val("");


		});
	});
});