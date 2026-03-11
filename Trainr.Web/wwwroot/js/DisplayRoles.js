function DisplayRoles() {
    var inputUrl = "GetCurrentRoles";
    var checkBoxName = "currentRoles";
    var rolesTarget = "#currentRolesTarget";
    GetRolesForUser(inputUrl, checkBoxName, rolesTarget);

    inputUrl = "GetAvailableRoles";
    checkBoxName = "availableRoles";
    rolesTarget = "#availableRolesTarget";
    GetRolesForUser(inputUrl, checkBoxName, rolesTarget);
}
