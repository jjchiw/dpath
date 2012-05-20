$(document).ready(function () {
    
    $("#generate-token").click(function () {
        var url = "/user/token";

        $.post(url, function (data) {
            $("#token-label").text(data.Token);
        });
    });
});