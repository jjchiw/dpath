$(document).ready(function () {
	$("#path-delete").click(function () {
		var url = "/" + $("#path-id").val() + "/delete"

		$.post(url, function (data) {
			window.location = "/";
		});

		return false;
	});

});