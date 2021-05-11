(function ($) {

    $(function () {

        var conn = $.hubConnection();
        var hub = conn.createHubProxy("notificationHub");

        hub.on("onConnectUser", function (users) {
            $("#users").html("");
            $.each(users, function (index, user) {
                $("#users").append("<li class='list-group-item' data-id='" + user.UserID + "'>" + user.UserName + "</li>");
            });
        });

        hub.on("onDisconnectUser", function (user) {
            $("#users").find("li[data-id='" + user.UserID + "']").remove();
        });

        //hub.on("onNewMessage", function (message) {
        // $("#messages").append("<li class='list-group-item'><h4>( " + message.From + " ):" + message.Title + "</h4><p>" + message.Message + "</p></li>");
        // });

        conn.start(function () {
            hub.invoke("userConnected", window.erpuid, window.erpuname);
            $("#btnSendMessage").on("click", function (e) {
                var title = $("#title").val(),
                    message = $("#message").val();
                if ($.trim(title) == "" || $.trim(message) == "") {
                    toastr.error("Please enter valid title or message", "Error");
                } else {
                    hub.invoke("sendMessageToUser", {
                        From: window.erpuname,
                        Title: title,
                        Message: message
                    });
                }
                return false;
            });
        });

    });

})(jQuery);