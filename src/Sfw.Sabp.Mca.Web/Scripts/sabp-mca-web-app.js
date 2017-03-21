if (typeof jQuery != 'undefined') {
    var currentCulture = $("html").prop("lang");

    $(function () {
        $.validator.methods.date = function (value, element) {
            Globalize.culture(currentCulture);
            return this.optional(element) || Globalize.parseDate(value) !== null;
        }
    });

    $(function () {
        $('.date-control').datepicker({
            format: 'dd/mm/yyyy',
            startDate: "-200y",
            endDate: "0",
            autoclose: true,
            language: "en-GB"
        });

        $('.past-date').datepicker({
            format: 'dd/mm/yyyy',
            startDate: "-200y",
            endDate: "0",
            autoclose: true,
            language: "en-GB"
        });
    });

    $('*[data-confirmprompt]').click(function (event) {
        event.preventDefault();

        var form = $(this).closest('form');
        var promptText = $(this).attr('data-confirmprompt');

        bootbox.dialog({
            message: promptText,
            buttons: {
                no: {
                    label: "No"
                },
                yes: {
                    label: "Yes",
                    className: "btn btn-default",
                    callback: function () {
                        form.submit();
                    }
                }
            }
        });
    });

    $('*[data-confirmprompt-url]').click(function (event) {
        event.preventDefault();

        var promptText = $(this).attr('data-confirmprompt-url');

        bootbox.dialog({
            message: promptText,
            buttons: {
                yes: {
                    label: "OK",
                    className: "btn btn-default",
                    callback: function () {
                        window.location = event.currentTarget.href;
                    }
                }
            }
        });
    });

    $('button[class*=btn-submit]').click(function (e) {
        e.preventDefault();

        var button = $(this);
        var form = $(this).closest('form');

        if (form.valid()) {
            button.attr("disabled", true);
            button.closest('form').submit();
        }
        else {
            button.attr("disabled", false);
        }
    });
}