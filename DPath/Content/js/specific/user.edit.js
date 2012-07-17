$(document).ready(function () {
    
    $("#generate-token").click(function () {
        var url = "/user/token";

        $.post(url, function (data) {

			if(typeof data != "object"){
				location = location.protocol + "//" + location.host + "?returnUrl=" + location.pathname;
				return;
			}

            $("#token-label").text(data.Token);
        });
    });
});