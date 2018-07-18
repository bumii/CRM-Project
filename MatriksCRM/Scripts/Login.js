$("#login-modal form").submit(function () {
    var $lg_username = $('#login_username').val();
    var $lg_password = $('#login_password').val();
    var $lg_rememberme = false;

    if ($('#login_rememberme').val() == "on") {
        $lg_rememberme = true;
    }

    $("#login_submit_btn").attr("disabled", "disabled");

    $.ajax({
        method: "post",
        url: "/ModalLogin/SignIn",
        data: { login_username: $lg_username, login_password: $lg_password, login_rememberme: $lg_rememberme }
    }).done(function (res) {
        if (res.HasError) {
            msgChange($('#div-login-msg'), $('#icon-login-msg'), $('#text-login-msg'), "error", "glyphicon-remove", res.Result);

            $('#login_username').val("");
            $('#login_password').val("");

            $("#login_submit_btn").removeAttr("disabled");
        }
        else {
            msgChange($('#div-login-msg'), $('#icon-login-msg'), $('#text-login-msg'), "success", "glyphicon-ok", res.Result);

            setTimeout(function () {
                location.reload();
            }, 1500);
        }
    });

    return false;
    break;

});