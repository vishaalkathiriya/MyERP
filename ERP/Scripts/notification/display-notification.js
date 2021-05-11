(function ($) {

    $(function () {
        var conn = $.hubConnection();
        var hub = conn.createHubProxy("notificationHub");

        hub.on("onNewMessage", function (message) {
            toastr.options = {
                "timeOut": "0"
            };
            toastr.success(message.Message + "<br/>From: " + message.From, "<h4 style='color: #fff;'>" + message.Title + "</h4>");
        });
        conn.start(function () {
            hub.invoke("userConnected", window.erpuid, window.erpuname);
        });
    });

})(jQuery);