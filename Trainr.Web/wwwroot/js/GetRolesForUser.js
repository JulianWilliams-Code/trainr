function GetRolesForUser(dataUrl, checkBoxName, rolesTarget) {
    var listRoles = $(rolesTarget);
    var usersTarget = $("#usersTarget").val();
    var appendString;

    $.ajax(
        {
            url: dataUrl,
            type: "GET",
            dataType: "json",
            data: { id: usersTarget },
            success: function (data) {
                listRoles.empty();
                $.each(data, function () {
                    appendString = '<input type="checkbox" ';
                    appendString += ' name = "';
                    appendString += checkBoxName;
                    appendString += '" ';
                    appendString += ' value = "';
                    appendString += this.Name;
                    appendString += '" >';
                    appendString += this.Name;
                    appendString += '<br/>'
                    //alert(appendString);
                    listRoles.append(appendString);
                });
            },
            error: function () {
                alert("Data not received");
            }
        });
}