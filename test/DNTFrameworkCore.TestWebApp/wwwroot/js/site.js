// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.loginLocation = '/Account/Login';

$(function() {
  $('[data-toggle="tooltip"]').tooltip();
  // $(document).on('click', 'button[value=save-continue]', function() {
  //   var btn = $(this);
  //   btn
  //     .closest('form')
  //     .append('<input type="hidden" name="save-continue" value="true" />');
  // });
  NProgress.configure({
    parent: '#main-modal .modal-body',
    showSpinner: false
  });
  $(document).bind('ajaxStart', function() {
    NProgress.start();
  });

  $(document).bind('ajaxStop', function() {
    NProgress.done();
  });

  $('.modal').on('hidden.bs.modal', function(e) {
    // remove the bs.modal data attribute from it
    $(this).removeData('bs.modal');
    // and empty the main modal-content element
    $(this)
      .find('.modal-content')
      .html(
        '<div class="modal-body"><div class="spinner-border text-muted"></div> Loading...</div>'
      );
    //.empty();
  });
});
// • OnBegin – xhr
// • OnComplete – xhr, status
// • OnSuccess – data, status, xhr
// • OnFailure – xhr, status, error
function handleModalFormFailed(xhr, status, error, formId) {
  if (xhr.status === 403 || xhr.status === 401) {
    handleUnauthorizedRequest(xhr);
  } else if (xhr.status === 400) {
    var $form = $('#' + formId);
    var validator = $form.data('validator');

    if (!validator || !$form.valid()) return;

    var errors = $.parseJSON(xhr.responseText);
    validator.showErrors(errors);
  }
}

function handleModalFormBegin(xhr) {
  disableModalButtons();
}

function handleModalFormComplete(xhr, status) {
  enableModalButtons();
}

function handleModalFormLoaded(data, status, xhr) {
  if (xhr.getResponseHeader('Content-Type') === 'text/html; charset=utf-8') {
    initModalForm();
  } else {
    hideMainModal();
  }
}

function handleModalLinkBegin(xhr) {
  disableModalButtons();
}

function handleModalLinkComplete(xhr, status) {
  enableModalButtons();
}

function handleModalLinkLoaded(data, status, xhr) {
  initModalForm();
}

function handleModalLinkFailed(xhr, status, error) {
  if (xhr.status === 403 || xhr.status === 401) {
    handleUnauthorizedRequest(xhr);
  }
  hideMainModal();
}

function handleUnauthorizedRequest(xhr) {
  var loginLocation = xhr.getResponseHeader('Location');
  if (loginLocation) {
    window.location = loginLocation;
  } else {
    window.location = window.loginLocation;
  }
}

function hideMainModal() {
  $('#main-modal').modal('hide');
}

function enableModalButtons() {
  $('#main-modal a').removeClass('disabled');
  $('#main-modal button').removeAttr('disabled');
}

function disableModalButtons() {
  $('#main-modal a').addClass('disabled');
  $('#main-modal button').attr('disabled', 'disabled');
}

function initModalForm() {
  $.validator.unobtrusive.parse('.modal form');
  $('.modal form').each(function() {
    $(this)
      .find('div.form-group')
      .each(function() {
        if ($(this).find('span.field-validation-error').length > 0) {
          $(this).addClass('is-invalid');
        }
      });
  });
  $('.modal form')
    .find('input:not([type=hidden]):not([type=checkbox]):first')
    .focus();
}


// $('.modal').on('shown.bs.modal', function() {
  //   setTimeout(function() {
  //     $.validator.unobtrusive.parse('.modal form');
  //     $('form').each(function() {
  //       $(this)
  //         .find('div.form-group')
  //         .each(function() {
  //           if ($(this).find('span.field-validation-error').length > 0) {
  //             $(this).addClass('is-invalid');
  //           }
  //         });
  //     });
  //     $('form')
  //       .find('input:not([type=hidden]):not([type=checkbox]):first')
  //       .focus();
  //   }, 300);
  // });

  // $.validator.setDefaults({
  //   ignore: '', // for hidden tabs and also textarea's
  //   errorElement: 'div',
  //   errorPlacement: function(error, element) {
  //     error.addClass('invalid-feedback');
  //     element.closest('.form-group').append(error);
  //   },
  //   highlight: function(element, errorClass, validClass) {
  //     if (element.type === 'radio') {
  //       this.findByName(element.name)
  //         .addClass(errorClass)
  //         .removeClass(validClass);
  //     } else {
  //       $(element)
  //         .addClass(errorClass)
  //         .removeClass(validClass);
  //       $(element)
  //         .addClass('is-invalid')
  //         .removeClass('is-valid');
  //       $(element)
  //         .closest('.form-group')
  //         .find('.input-group-text, label')
  //         .removeClass('text-success')
  //         .addClass('text-danger');
  //     }
  //     $(element).trigger('highlited');
  //   },
  //   unhighlight: function(element, errorClass, validClass) {
  //     if (element.type === 'radio') {
  //       this.findByName(element.name)
  //         .removeClass(errorClass)
  //         .addClass(validClass);
  //     } else {
  //       $(element)
  //         .removeClass(errorClass)
  //         .addClass(validClass);
  //       $(element)
  //         .removeClass('is-invalid')
  //         .addClass('is-valid');
  //       $(element)
  //         .closest('.form-group')
  //         .find('.input-group-text, label')
  //         .removeClass('text-danger')
  //         .addClass('text-success');
  //     }
  //     $(element).trigger('unhighlited');
  //   }
  // });