$(document).ready(function () {
    if ($("#usersTarget").val() != "None") {
        $("#wrapper").show();
        DisplayRoles();
    }
    else {
        $("#wrapper").hide();
    }

    $("#usersTarget").on("change", function () {
        if ($("#usersTarget").val() != "None") {
            $("#wrapper").show();
            DisplayRoles();
        }
        else {
            $("#wrapper").hide();
        }
    });
});