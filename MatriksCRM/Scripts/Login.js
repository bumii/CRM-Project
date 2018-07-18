$('#login-submit').submit(function () {
    var $login_username = $('#inputEmail').val();
    var $login_password = $('#inputPassword').val();

    $("#login_submit").attr("disabled", "disabled");

    $.ajax({
        method: "post",
        url: "/Index/Login",
        data: { email: $rg_email, password: $rg_password }
    }).done(function (res) {
        if (res.HasError) {

            $("#register_submit_btn").removeAttr("disabled");

            $('#register_username').val("");
            $('#register_email').val("");
            $('#register_password').val("");
        }
        else {
            msgChange($('#div-register-msg'), $('#icon-register-msg'), $('#text-register-msg'), "success", "glyphicon-ok", res.Result);

            setTimeout(function () {
                location.reload();
            }, 2000);
        }

    });
});