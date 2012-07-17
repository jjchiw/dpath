function setSession(val) {
	if (navigator.id) {
		navigator.id.sessions = val ? val : [];
	}
}

// a handler that is passed an assertion after the user logs in via the
// browserid dialog
function gotVerifiedEmail(assertion) {
	// got an assertion, now send it up to the server for verification
	if (assertion !== null) {
		$.ajax({
			type: 'POST',
			url: '/auths/login',
			data: { assertion: assertion },
			success: function (res, status, xhr) {
				if (res != null) {
					if (location.search.indexOf("?returnUrl=") > -1) {
						var returnUrl = location.search.replace("?returnUrl=", "")
						location = location.protocol + "//" + location.host + returnUrl

						return;
					}
					location.reload();
					//loggedIn(res.email);
				}
			},
			error: function (res, status, xhr) {
				alert("login failure" + res);
			}
		});
	}
}

function loggedIn(email) {
	setSession(email);
	$('#login').toggleClass().removeClass('show').addClass('hide');
	$('#loggedinview').removeClass('hide').addClass('show');
	setDisplayStyle(email);
}

function setDisplayStyle(email, username) {
	var iurl = 'http://www.gravatar.com/avatar/' + Crypto.MD5($.trim(email).toLowerCase()) + "?s=20";
	$("#user-gravatar").attr("src", iurl);
	$("#username").text(username);
}

function logout() {
	setSession();
	$('#login').removeClass('hide').addClass('show');
	$('#loggedinview').removeClass('show').addClass('hide');
	$('#gravatar').html('');
}