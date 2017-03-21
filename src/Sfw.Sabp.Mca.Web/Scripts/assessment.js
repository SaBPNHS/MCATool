
$(document).ready(function () {
    if ($("#RoleId").val() != "1") {
        $('#decisionmakertext').hide();
    }
});

$('select#RoleId').change(function () {
    // Checking if the selected id is that of advisor (1)
    if ($("#RoleId").val() == "1")
        $('#decisionmakertext').show();
    else {
        $('#decisionmakertext').hide();
    }
});