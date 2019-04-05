// <![CDATA[
(function($) {
  $.bootstrapToast = function(options) {
    var defaults = {
      color: 'text-info',
      title: '',
      body: ''
    };

    options = $.extend(defaults, options);

    var toastContainer = '#toastContainer';
    var html =
      '<div class="toast" data-autohide="false" data-animation="true" id="toastContainer"><div class="toast-header">' +
      '<strong class="mr-auto ' +
      options.color +
      ' ">' +
      options.title +
      '</strong>' +
      '<button type="button" class="ml-2 mb-1 close" data-dismiss="toast">&times;</button>' +
      '</div> <div class="toast-body">' +
      options.body +
      '</div></div>';

    $(toastContainer).toast('hide');

    $(toastContainer).remove();
    $(html).appendTo('body');
    $(toastContainer).toast('show');
  };
})(jQuery);
// ]]>
