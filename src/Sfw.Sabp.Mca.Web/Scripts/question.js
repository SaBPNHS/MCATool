
$(document).ready(function () {

    $('button[class*=btn-continue-submit]').click(function (e) {

        event.preventDefault ? event.preventDefault() : event.returnValue = false;

        var button = $(this);
        var form = button.closest('form');
        var controlName = button.attr("name");
        var controlValue = button.attr("value");

        var input = $("<input>").attr("type", "hidden").attr("name", controlName).val(controlValue);
        form.prepend($(input));

        var postUrl = button.attr("post-url");

        button.attr("disabled", true);

        $.ajax({
            url: postUrl,
            type: "POST",
            data: form.serialize(),
            datatype: "json",
            success: function (result) {
                if (result.Redirect != null) {
                    window.location = result.Redirect;
                }
                $("#questionFormDiv").html(result.Html);
            }
        });
    });
});
