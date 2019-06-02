(function ($, window, document, framework, undefined) {
    $(function () {
        NProgress.configure({
            parent: '#main-modal .modal-body',
            showSpinner: false
        });

        $.ajaxSetup({
            cache: false
        });

        $(document).ajaxStart(function () {
            NProgress.start();
        });

        $(document).ajaxStop(function () {
            NProgress.done();
        });

        $(document).ajaxError(function (event, xhr, settings, thrownError) {
            if (xhr.status === 403 || xhr.status === 401) {
                handleUnauthorizedRequest(xhr);
            } else if (xhr.status === 404) {
                handleNotFoundRequest();
            } else if (xhr.status === 500) {
                handleInternalServerError();
            }
        });

        $('#main-modal.modal').on('hidden.bs.modal', function (e) {
            // remove the bs.modal data attribute from it
            $(this).removeData('bs.modal');
            // and empty the main modal-content element
            $(this)
                .find('.modal-content')
                .html('<div class="modal-body">Loading...</div>');
            //.empty();
        });
    });

    window.loginLocation = '/Account/Login';

    /*----------------------------------  asp-modal-link ---------------------------*/
    window.handleModalLinkLoaded = function (data, status, xhr) {
        prepareForm('#main-modal.modal form');
    };

    window.handleModalLinkFailed = function (xhr, status, error) {
        //....
    };

    /*----------------------------------  asp-modal-form ---------------------------*/
    window.handleModalFormBegin = function (xhr) {
        $('#main-modal a').addClass('disabled');
        $('#main-modal button').attr('disabled', 'disabled');
    };

    window.handleModalFormComplete = function (xhr, status) {
        $('#main-modal a').removeClass('disabled');
        $('#main-modal button').removeAttr('disabled');
    };

    window.handleModalFormSucceeded = function (data, status, xhr) {
        if (xhr.getResponseHeader('Content-Type') === 'text/html; charset=utf-8') {
            prepareForm('#main-modal.modal form');
        } else {
            hideMainModal();
        }
    };

    window.handleModalFormFailed = function (xhr, status, error, formId) {
        if (xhr.status === 400) {
            handleBadRequest(xhr, formId);
        }
    };

    /*---------------------------------- private methods ---------------------------*/

    function handleBadRequest(xhr, formId) {
        var $form = $('#' + formId);
        var validator = $form.data('validator');

        if (!validator) return;

        var errors = $.parseJSON(xhr.responseText);

        // var $summary = $form.find('[data-valmsg-summary=true]');
        // var $ul = $summary.find('ul');
        // $ul.empty();

        // var responseJson = $.parseJSON(xhr.responseText);

        // var validations = Object.keys(responseJson).reduce(function(object, key) {
        //   if (key !== '') {
        //     object[key] = responseJson[key];
        //   } else {
        //     responseJson[key].forEach(function(message) {
        //       $('<li />')
        //         .html(message)
        //         .appendTo($ul);
        //     });
        //   }
        //   return object;
        // }, {});

        // $summary
        //   .removeClass('validation-summary-valid')
        //   .addClass('validation-summary-errors')
        //   .addClass('alert alert-danger');

        validator.showErrors(errors);
    }

    function handleUnauthorizedRequest(xhr) {
        var loginLocation = xhr.getResponseHeader('Location');
        if (loginLocation) {
            window.location = loginLocation;
        } else {
            window.location = window.loginLocation;
        }
    }

    function handleNotFoundRequest() {
        hideMainModal();
    }

    function handleInternalServerError() {
        hideMainModal();
    }

    function hideMainModal() {
        $('#main-modal').modal('hide');
    }

    function prepareForm(selector) {
        handleUnobtrusiveValidation(selector);
        handlePostbackValidation();
        handleInputFocus();
        handleTabValidation();
    }

    function handleUnobtrusiveValidation(selector) {
        $.validator.unobtrusive.parse(selector);
    }

    function handleTabValidation() {
        if (
            $('.tab-content').find('div.tab-pane.active:has(div.is-invalid)')
                .length === 0
        ) {
            $('.tab-content')
                .find('div.tab-pane:hidden:has(div.is-invalid)')
                .each(function (index, tab) {
                    var id = $(tab).attr('id');
                    $('a[href="#' + id + '"]').tab('show');
                });
        }
    }

    function handleInputFocus() {
        $('form')
            .find(
                'input:not([type=hidden]):not([type=checkbox]):not([readonly=readonly]):not([disabled=disabled]):first'
            )
            .focus();
    }

    function handlePostbackValidation() {
        $('form').each(function () {
            $(this)
                .find('div.form-group')
                .each(function () {
                    if ($(this).find('span.field-validation-error').length > 0) {
                        $(this).addClass('is-invalid');
                    }
                });
        });
    }

    // $(document).bind('ajaxSend', function(elm, xhr, s) {
    //   var token = $('[name=__RequestVerificationToken]').val();
    //   if (s.type == 'POST' && typeof token != 'undefined') {
    //     if (s.data.length > 0) {
    //       s.data += '&__RequestVerificationToken=' + encodeURIComponent(token);
    //     } else {
    //       s.data = '__RequestVerificationToken=' + encodeURIComponent(token);
    //     }
    //   }
    // });
    // window.addToken = function(data) {
    //   data.__RequestVerificationToken = $(
    //     'input[name=__RequestVerificationToken]'
    //   ).val();
    //   return data;
    // };
    // $(document).on('click', 'button[value=save-continue]', function() {
    //   var btn = $(this);
    //   btn
    //     .closest('form')
    //     .append('<input type="hidden" name="save-continue" value="true" />');
    // });
})(jQuery, window, document, (framework = window.framework || {}));
